using GirLoader.Output.Model;

namespace Generator
{
    internal static class ConstantExtension
    {
        public static string WriteManaged(this Constant constant)
        {
            var value = GetValue(constant);

            var referencedTypeName = constant.TypeReference.GetResolvedType().Name;
            return $"public static {referencedTypeName} {constant.Name} = {value};\r\n";
        }

        private static string GetValue(Constant constant)
        {
            var resolvedType = constant.TypeReference.GetResolvedType();

            if (resolvedType.Name.Value.EndsWith("Flags"))
                return $"({resolvedType.Name}) {constant.Value}";

            if (resolvedType is String)
                return "\"" + constant.Value + "\"";

            return constant.Value;
        }
    }
}
