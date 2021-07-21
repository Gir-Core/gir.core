using GirLoader.Output.Model;

namespace Generator
{
    internal static class CallbackExtension
    {
        public static string WriteNative(this Callback callback, Namespace currentNamespace)
        {
            var returnType = callback.ReturnValue.WriteNative(currentNamespace);
            var arguments = callback.ParameterList.WriteNative(currentNamespace);
            return $"public delegate {returnType} {callback.Name}({arguments});";
        }
    }
}
