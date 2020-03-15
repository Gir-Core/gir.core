using System;

namespace Gir
{
    public class GTypeResolver : TypeResolver
    {
        private readonly AliasResolver resolver;

        public GTypeResolver(AliasResolver resolver)
        {
            this.resolver = resolver;
        }

        public string GetType(IType type, bool isParameter)
        {
            var ret = type switch
            {
                { Type : {}} => GetReturn(type.Type, isParameter),
                { Array: {Type:{}}} => GetReturn(type.Array.Type, isParameter),
                _ => throw new NotSupportedException("Type is missing supported Type information")
            };

            if(type.Array is {} && ret != "IntPtr")
            {
                if(ret == "string")
                {
                    return "ref IntPtr";
                }
                else
                {
                    ret = ret + "[]"; 

                    if(isParameter)
                        ret = GetMarshal(type.Array) + " " + ret;
                }
            }

            return ret;    
        }

        private string GetMarshal(GArray array) => $"[MarshalAs(UnmanagedType.LPArray, SizeParamIndex={array.Length})]";

        private string GetReturn(GType gtype, bool isParameter)
        {
            if (gtype.Type.In("va_list", "GType", "gpointer", "gconstpointer"))
                return "IntPtr";

            var typeName = gtype.Name;
            var cType = gtype.Type;

            if (resolver != null && gtype.Type is {} && resolver.TryGet(gtype.Type, out string resolvedType))
                typeName = resolvedType;

            var isPointer = gtype.Type?.EndsWith("*") ?? false;
            bool isPrimitive;
            (typeName, isPrimitive) = ResolveGType(typeName, cType);

            if(typeName == "string" && isParameter)
                return typeName; //string stays string for parameter values, they are marshalled automatically
            else if(isPointer && isPrimitive)
                return "ref " + typeName;
            else if(isPointer && !isPrimitive)
                return "IntPtr";
            else
                return typeName;
        }

        private static (string Type, bool IsPrimitive) ResolveGType(string typeName, string cType) => (typeName, cType) switch
        {
            ("none", _) => ("void", true),
            ("gboolean", _) => ("bool", true),
            ("gint16", _) => ("short", true),
            ("guint16", _) => ("ushort", true),
            ("gfloat", _) => ("float", true),
            ("utf8", _) => ("string", false),
            ("filename", _) => ("string", false),
            ("Callback", _) => ("Delegate", false), // Signature of a callback is determined by the context in which it is used

            ("Value", "JSCValue*") => ("IntPtr", false),
            ("Value", _) => ("GObject.Value", true),

            var t when t.typeName.In("gdouble", "long double") => ("double", true),
            var t when t.typeName.In("gint","gint32") => ("int", true),
            var t when t.typeName.In("guint", "guint32", "GLib.Quark", "GQuark", "gunichar") => ("uint", true),
            var t when t.typeName.In("guint8", "gint8", "gchar") => ("byte", true),
            var t when t.typeName.In("glong", "gssize", "gint64") => ("long", true),
            var t when t.typeName.In("gsize", "guint64", "gulong", "xlib.Window") => ("ulong", true),

            var t when t.typeName.In("TokenValue", "IConv", "GType") => throw new NotSupportedException($"{typeName} is not supported"),
            var t when t.typeName.StartsWith("Atk.") => throw new NotSupportedException($"{typeName} is not supported"),

            _ => (typeName, false)
        };
    }


}
