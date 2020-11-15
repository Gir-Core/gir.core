using System;
using System.Collections.Generic;
using System.Linq;
using Gir;

namespace Generator
{
    public class ResolvedType : IEquatable<ResolvedType>
    {
        #region Properties

        public string Type { get; }
        public string Attribute { get; }
        public bool IsRef { get; }

        #endregion

        #region Constructors

        public ResolvedType(string type, bool isRef = false, string attribute = "")
        {
            Type = type;
            Attribute = attribute;
            IsRef = isRef;
        }

        #endregion

        #region Methods

        public override string ToString() => GetTypeString();

        public string GetTypeString() => Attribute + (IsRef ? "ref " : string.Empty) + Type;
        public string GetFieldString() => Attribute + (IsRef ? "IntPtr" : Type);

        #endregion

        #region IEquatable<ResolvedType> Implementation

        public bool Equals(ResolvedType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && IsRef == other.IsRef;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ResolvedType) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, IsRef);
        }

        #endregion
    }

    internal class MyType
    {
        #region Properties

        public string? ArrayLengthParameter { get; set; }
        public bool IsArray { get; set; }
        public string Type { get; set; }
        public bool IsPointer { get; set; }
        public bool IsValueType { get; set; }
        public bool IsParameter { get; set; }

        #endregion

        #region Constructors

        public MyType(string type)
        {
            Type = type;
        }

        #endregion
    }

    public class TypeResolver
    {
        #region Fields

        private readonly AliasResolver _aliasResolver;

        #endregion

        #region Constructors

        public TypeResolver(AliasResolver resolver)
        {
            _aliasResolver = resolver;
        }

        #endregion

        #region Methods

        public ResolvedType Resolve(IType typeInfo) => typeInfo switch
        {
            GField f when f.Callback is { } c => ResolveCallback(c),
            { Array: { CType: { } n } } when n.EndsWith("**") => new ResolvedType("IntPtr", true),
            { Type: { } gtype } => GetTypeName(ConvertGType(gtype, typeInfo is GParameter)),
            { Array: { Length: { } length, Type: { CType: { } } gtype } } => GetTypeName(ResolveArrayType(gtype, typeInfo is GParameter, length)),
            { Array: { Length: { } length, Type: { Name: "utf8" } name } } => GetTypeName(StringArray(length, typeInfo is GParameter)),
            { Array: { Length: "1", Type: {Name: "guint8"}}} => new ResolvedType("byte[]"),
            { Array: { } } => new ResolvedType("IntPtr"),
            _ => throw new NotSupportedException("Type is missing supported Type information")
        };

        private ResolvedType ResolveCallback(GCallback callback)
        {
            ResolvedType returntype = Resolve(callback.ReturnValue ?? throw new Exception("Missing return for callback"));

            List<ResolvedType> parameters = callback.Parameters?.AllParameters
                                                        .Select(Resolve).ToList() ?? new ();
            parameters.Add(returntype);
            
            var parametersString = string.Join(", ", parameters.Select(x => x.ToString()));
            return new ResolvedType($"unsafe delegate*<{parametersString}>");
        }

        private MyType StringArray(string length, bool isParameter) => new MyType("byte")
        {
            IsArray = true,
            ArrayLengthParameter = length,
            IsPointer = true,
            IsValueType = false,
            IsParameter = isParameter
        };

        public ResolvedType GetTypeString(GType type)
            => GetTypeName(ConvertGType(type, true));

        private MyType ResolveArrayType(GType arrayType, bool isParameter, string? length)
        {
            MyType? type = ConvertGType(arrayType, isParameter);
            type.IsArray = true;
            type.ArrayLengthParameter = length;

            return type;
        }

        private MyType ConvertGType(GType gtype, bool isParameter)
        {
            var ctype = gtype.CType;
            if (ctype is null)
            {
                Console.WriteLine($"GType is missing CType. Assuming {gtype.Name} as CType");
                ctype = gtype.Name ?? throw new Exception($"GType {gtype.Name} is missing CType");
            }

            if (_aliasResolver.TryGetForCType(ctype, out var resolvedCType, out var resolvedName))
                ctype = resolvedCType;

            MyType? result = ResolveCType(ctype);
            result.IsParameter = isParameter;

            if (!result.IsValueType && gtype.Name is { })
            {
                result.Type = resolvedName ?? gtype.Name;
            }

            return result;
        }

        private ResolvedType GetTypeName(MyType type)
            => type switch
            {
                { Type: "gpointer" } => new ResolvedType("IntPtr"),
                { IsArray: false, Type: "void", IsPointer: true } => new ResolvedType("IntPtr"),
                { IsArray: false, Type: "byte", IsPointer: true, IsParameter: true } => new ResolvedType("string"),  //string in parameters are marshalled automatically
                { IsArray: false, Type: "byte", IsPointer: true, IsParameter: false } => new ResolvedType("IntPtr"),
                { IsArray: true, Type: "byte", IsPointer: true, IsParameter: true, ArrayLengthParameter: { } l } => new ResolvedType("string[]", attribute: GetMarshal(l)),
                { IsArray: false, IsPointer: true, IsValueType: true } => new ResolvedType(type.Type, true),
                { IsArray: false, IsPointer: true, IsValueType: false } => new ResolvedType("IntPtr"),
                { IsArray: true, Type: "byte", IsPointer: true } => new ResolvedType("IntPtr", true), //string array
                { IsArray: true, IsValueType: false, IsParameter: true, ArrayLengthParameter: { } l } => new ResolvedType("IntPtr[]", attribute: GetMarshal(l)),
                { IsArray: true, IsValueType: true, IsParameter: true, ArrayLengthParameter: { } l } => new ResolvedType(type.Type + "[]", attribute: GetMarshal(l)),
                { IsArray: true, IsValueType: true, ArrayLengthParameter: { } } => new ResolvedType(type.Type + "[]"),
                { IsArray: true, IsValueType: true, ArrayLengthParameter: null } => new ResolvedType("IntPtr"),
                _ => new ResolvedType(type.Type)
            };

        private string GetMarshal(string arrayLength)
            => $"[MarshalAs(UnmanagedType.LPArray, SizeParamIndex={arrayLength})]";

        private MyType ResolveCType(string cType)
        {
            var isPointer = cType.EndsWith("*");
            cType = cType.Replace("*", "").Replace("const ", "").Replace("volatile ", "");

            MyType? result = cType switch
            {
                "void" => ValueType("void"),
                "gboolean" => ValueType("bool"),
                "gfloat" => Float(),
                "float" => Float(),

                //"GCallback" => ReferenceType("Delegate"), // Signature of a callback is determined by the context in which it is used               

                "gconstpointer" => IntPtr(),
                "va_list" => IntPtr(),
                "gpointer" => IntPtr(),
                "GType" => IntPtr(),
                "tm" => IntPtr(),
                var t when t.StartsWith("Atk") => IntPtr(),
                var t when t.StartsWith("Cogl") => IntPtr(),

                "GValue" => Value(),
                //"GError" => Error(),
                //"GVariantType" => VariantType(),

                "guint16" => UShort(),
                "gushort" => UShort(),

                "gint16" => Short(),
                "gshort" => Short(),

                "double" => Double(),
                "gdouble" => Double(),
                "long double" => Double(),

                "cairo_format_t" => Int(),//Workaround
                "int" => Int(),
                "gint" => Int(),
                "gint32" => Int(),
                "pid_t" => Int(),

                "unsigned int" => UInt(), //Workaround
                "unsigned" => UInt(),//Workaround
                "guint" => UInt(),
                "guint32" => UInt(),
                "GQuark" => UInt(),
                "gunichar" => UInt(),
                "uid_t" => UInt(),

                "guchar" => Byte(),
                "gchar" => Byte(),
                "char" => Byte(),
                "guint8" => Byte(),
                "gint8" => Byte(),

                "glong" => Long(),
                "gssize" => Long(),
                "gint64" => Long(),
                "goffset" => Long(),
                "time_t" => Long(),

                "gsize" => ULong(),
                "guint64" => ULong(),
                "gulong" => ULong(),
                "Window" => ULong(),

                _ => ReferenceType(cType)
            };
            result.IsPointer = isPointer;

            return result;
        }

        private MyType String() => ReferenceType("string");
        private MyType IntPtr() => ValueType("IntPtr");
        private MyType Value() => ValueType("GObject.Value");
        private MyType UShort() => ValueType("ushort");
        private MyType Short() => ValueType("short");
        private MyType Double() => ValueType("double");
        private MyType Int() => ValueType("int");
        private MyType UInt() => ValueType("uint");
        private MyType Byte() => ValueType("byte");
        private MyType Long() => ValueType("long");
        private MyType ULong() => ValueType("ulong");
        private MyType Float() => ValueType("float");
        private MyType Error() => ValueType("Error");
        private MyType VariantType() => ValueType("VariantType");

        private MyType ValueType(string str) => new MyType(str) { IsValueType = true };
        private MyType ReferenceType(string str) => new MyType(str);

        #endregion
    }
}
