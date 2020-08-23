using System;
using System.Reflection;
using System.Collections.Generic;

namespace GObject
{
    public partial class Object
    {
        // Type Dictionary for mapping C#'s System.Type
        // and GLib GTypes (currently Sys.Type, although
        // this might change)
        internal static class TypeDictionary
        {
            // Dual dictionaries for looking up types and gtypes
            private static Dictionary<Type, Sys.Type> typedict;
            private static Dictionary<Sys.Type, Type> gtypedict;

            static TypeDictionary()
            {
                // Initialise Dictionaries
                typedict = new Dictionary<Type, Sys.Type>();
                gtypedict = new Dictionary<Sys.Type, Type>();

                // Add GObject and GInitiallyUnowned
                Add(typeof(Object), Object.GetGType());
                Add(typeof(InitiallyUnowned), InitiallyUnowned.GetGType());
            }

            // Add to type dictionary
            internal static void Add(Type type, Sys.Type gtype)
            {
                if (typedict.ContainsKey(type) ||
                    gtypedict.ContainsKey(gtype))
                    return;

                typedict.Add(type, gtype);
                gtypedict.Add(gtype, type);
            }

            // Get System.Type from GType
            internal static Type Get(Sys.Type gtype)
            {
                // Check Type Dictionary
                if (gtypedict.TryGetValue(gtype, out var type))
                    return type;

                // It is quite unlikely that we need to perform a lookup
                // by gtype of a type we haven't created ourselves. Therefore,
                // this shouldn't be too prohibitively expensive.

                // TODO: Revise Generator to automatically implement
                // Use Wrapper Attribute for mapping GType name to C# Type
                // Use GetGType function for reverse mapping.

                // Search all System.Type which contain a [Wrapper(TypeName)]
                // for the corresponding type.

                // Possible Idea: Autogenerate a 'RegisterTypes.cs' file that
                // on startup will add every type to the type dictionary?

                // Quick Path: Find the first 'Word' in the type and lookup
                // assemblies by that name. Do we hardcode references to 'Pango',
                // 'Gtk', etc?

                // foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                // {
                //     assembly.GetType()
                // }

                // Search through unloaded assemblies?

                // TODO: For now, we'll just look through the type's inheritance chain
                // and find the first already registered type (e.g. GtkWidget). Effectively,
                // the lowest-common-denominator of functionality will be exposed.
                while (!Contains(gtype))
                {
                    ulong parent = Sys.Methods.type_parent(gtype);
                    if (parent == 0)
                        throw new Exception("Could not get Type from GType");

                    // TODO: One-way registration?
                    
                    gtype = new Sys.Type(parent);
                }

                return gtypedict[gtype];
            }

            // Get GType from System.Type
            internal static Sys.Type Get(Type type)
            {
                // Check Type Dictionary
                if (typedict.TryGetValue(type, out var gtype))
                    return gtype;

                // If we are looking up a type that is not yet in
                // the type dictionary, we are most likely registering
                // a new type. Therefore, we should register the type
                // and parent types recursively now to avoid having to
                // do this in the future.
                
                // Retrieve the GType accordingly
                if (IsSubclass(type))
                {
                    // We are a subclass
                    // RegisterNativeType will recursively add this
                    // and all parent types to the type dictionary
                    RegisterNativeType(type);
                    return typedict[type];
                }
                
                // We are a wrapper, so register types recursively
                Console.WriteLine("Registering Recursively");
                Type baseType = type;
                while (!Contains(baseType))
                {
                    Console.WriteLine(baseType.Name);
                    var methodInfo = GetGTypeMethodInfo(baseType)!;
                    ulong typeid = (Sys.Type)methodInfo.Invoke(null, null);
                    gtype = new Sys.Type(typeid);
                    
                    // Add to typedict for future use
                    Add(baseType, gtype);
                    Console.WriteLine($"Adding {baseType.Name}");

                    baseType = baseType.BaseType;
                }

                // Return gtype for *this* type
                return typedict[type];
            }
            
            // Contains functions
            internal static bool Contains(Type type) => typedict.ContainsKey(type);
            internal static bool Contains(Sys.Type gtype) => gtypedict.ContainsKey(gtype);

            // Determines whether the type is a managed subclass,
            // as opposed to wrapping an existing type.
            internal static bool IsSubclass(Type type)
                => type != typeof(Object) &&
                type != typeof(InitiallyUnowned) &&
                GetGTypeMethodInfo(type) is null;

            // Returns the MethodInfo for the 'GetGType()' function
            // if the type in question implements it (i.e. a wrapper)
            private static MethodInfo? GetGTypeMethodInfo(Type type)
            {
                const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
                return type.GetMethod(nameof(GetGType), flags);
            }
        }
    }
}
