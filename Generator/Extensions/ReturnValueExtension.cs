using GirLoader.Output.Model;

namespace Generator
{
    internal static class ReturnValueExtension
    {
        public static bool IsVoid(this ReturnValue returnValue)
            => returnValue.TypeReference.GetResolvedType().Name == "void";

        public static string WriteNative(this ReturnValue returnValue, Namespace currentNamespace)
        {
            return returnValue.WriteType(Target.Native, currentNamespace);
        }

        public static string WriteManaged(this ReturnValue returnValue, Namespace currentNamespace)
        {
            return returnValue.WriteType(Target.Managed, currentNamespace);
        }
    }
}
