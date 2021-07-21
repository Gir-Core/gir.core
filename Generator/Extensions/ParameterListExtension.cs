using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GirLoader;
using GirLoader.Output.Model;
using Array = System.Array;
using String = GirLoader.Output.Model.String;

namespace Generator
{
    internal static class ParameterListExtension
    {
        public static IEnumerable<SingleParameter> GetManagedParameters(this ParameterList parameterList)
        {
            // Exclude "userData" parameters
            SingleParameter[] args = parameterList.SingleParameters.ToArray();//.Where(x => x.ClosureIndex == null);

            var exclude = new List<SingleParameter>();
            foreach (var arg in args)
            {
                if (arg.ClosureIndex.HasValue && arg.ClosureIndex != 0)
                    exclude.Add(args[arg.ClosureIndex.Value]);
            }

            return args.Except(exclude);
        }
        public static string WriteManaged(this ParameterList parameterList, Namespace currentNamespace)
        {
            IEnumerable<string> paramArray = parameterList
                .GetManagedParameters()
                .Select(x => x.Write(Target.Managed, currentNamespace));

            return string.Join(
                separator: ", ",
                values: paramArray
            );
        }

        public static string WriteNative(this ParameterList parameterList, Namespace currentNamespace, bool useSafeHandle = true)
        {
            var offset = parameterList.InstanceParameter is { } ? 1 : 0;
            var parameters = new List<string>();
            foreach (var parameter in parameterList.GetParameters())
            {
                var attribute = GetAttribute(parameter, Target.Native, offset);
                parameters.Add(attribute + parameter.Write(Target.Native, currentNamespace, useSafeHandle));
            }

            return string.Join(", ", parameters);
        }

        private static string GetAttribute(Parameter parameter, Target target, int offset)
        {
            if (target == Target.Managed)
                return string.Empty;

            return parameter switch
            {
                // Simple array with fixed size length
                { TypeReference: ArrayTypeReference { Length: { } l }  } => $"[MarshalAs(UnmanagedType.LPArray, SizeParamIndex={l + offset})]",

                // Array without length and no transfer of type string. We assume null terminated array, which should
                // be marshaled as a SafeHandle: We do not need an attribute.
                { TypeReference: ArrayTypeReference { Length: null }, Transfer: Transfer.None, TypeReference: { ResolvedType: String } } => string.Empty,

                // Marshal as a UTF-8 encoded string
                { TypeReference: { ResolvedType: Utf8String } } => "[MarshalAs(UnmanagedType.LPUTF8Str)] ",

                // Marshal as a null-terminated array of ANSI characters
                // TODO: This is likely incorrect:
                //  - GObject introspection specifies that Windows should use
                //    UTF-8 and Unix should use ANSI. Does using ANSI for
                //    everything cause problems here?
                { TypeReference: { ResolvedType: PlatformString } } => "[MarshalAs(UnmanagedType.LPStr)] ",

                String => throw new NotSupportedException($"Unknown {nameof(String)} type - cannot create attribute"),

                _ => string.Empty
            };
        }

        public static string WriteSignalArgsProperties(this ParameterList parameterList, Namespace currentNamespace)
        {
            var builder = new StringBuilder();

            var index = 0;
            foreach (var argument in parameterList.GetParameters())
            {
                index += 1;
                var type = argument.WriteType(Target.Native, currentNamespace);
                var name = new GirLoader.Helper.String(argument.Name).ToPascalCase();

                builder.AppendLine($"//TODO: public {type} {name} => Args[{index}].Extract<{type}>();");
                builder.AppendLine($"public string {name} => \"\";");
            }

            return builder.ToString();
        }
    }
}
