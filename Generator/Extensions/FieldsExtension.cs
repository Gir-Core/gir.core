using System.Collections.Generic;
using System.Linq;
using System.Text;
using GirLoader.Output.Model;

namespace Generator
{
    internal static class FieldsExtension
    {
        public static string WriteNativeDelegates(this IEnumerable<Field> fields, Namespace currentNamespace)
        {
            var builder = new StringBuilder();

            foreach (Field field in fields)
            {
                if (field.Callback is { })
                    builder.AppendLine(field.Callback.WriteNative(currentNamespace));
            }

            return builder.ToString();
        }

        public static string WriteNative(this IEnumerable<Field> fields, Namespace currentNamespace)
        {
            var builder = new StringBuilder();

            foreach (Field field in fields)
                builder.AppendLine(field.WriteNative(currentNamespace));

            return builder.ToString();
        }

        public static string WriteUnionStructFields(this IEnumerable<Field> fields, Namespace currentNamespace)
        {
            var builder = new StringBuilder();

            foreach (Field field in fields)
            {
                builder.AppendLine("//[FieldOffset(0)] TODO Enable offset");
                builder.AppendLine(field.WriteNative(currentNamespace));
            }

            return builder.ToString();
        }

        public static string WriteClassStructFields(this IEnumerable<Field> fields, string className, Namespace currentNamespace)
        {
            var list = fields.ToArray();
            if (list.Length == 0)
                return "";

            var builder = new StringBuilder();
            builder.AppendLine(WriteFirstNativeClassStructField(list[0], className));

            foreach (var field in list[1..])
                builder.AppendLine(field.WriteNative(currentNamespace));

            return builder.ToString();
        }

        private static string WriteFirstNativeClassStructField(Field field, string className)
        {
            var builder = new StringBuilder();
            builder.Append(field.WriteNativeSummary());
            builder.AppendLine($"    public {className}.Native.ClassStruct {field.Name};");

            return builder.ToString();
        }
    }
}
