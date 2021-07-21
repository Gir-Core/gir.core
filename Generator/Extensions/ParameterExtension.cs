using System;
using System.Text;
using GirLoader.Output.Model;
using Type = GirLoader.Output.Model.Type;

namespace Generator
{
    internal static class ParameterExtension
    {
        internal static string Write(this Parameter parameter, Target target, Namespace currentNamespace, bool useSafeHandle = true)
        {
            var type = GetFullType(parameter, target, currentNamespace, useSafeHandle);

            var builder = new StringBuilder();
            builder.Append(type);
            builder.Append(' ');
            builder.Append(parameter.Name);

            return builder.ToString();
        }

        private static string GetFullType(this Parameter parameter, Target target, Namespace currentNamespace, bool useSafeHandle)
        {
            // FIXME: SafeHandles and ref *do not* work together when marshalling
            // apparently. For any Record-type parameters, we do not generate a direction
            // attribute for the native method. Find a more reliable way of doing this?
            var useDirection = !(parameter.TypeReference.ResolvedType is Record && target == Target.Native);

            var direction = useDirection ? GetDirection(parameter) : string.Empty;
            var type = GetType(parameter, target, currentNamespace, useSafeHandle);

            return $"{direction}{type}";
        }

        private static string GetDirection(Parameter parameter)
        {
            return parameter switch
            {
                // Arrays are automatically marshalled correctly. They don't need any direction
                { Direction: Direction.Ref, TypeReference: ArrayTypeReference } => "",

                { Direction: Direction.Ref } => "ref ",
                { Direction: Direction.Out, CallerAllocates: true } => "ref ",
                { Direction: Direction.Out } => "out ",
                _ => ""
            };
        }

        private static string GetType(this Parameter parameter, Target target, Namespace currentNamespace, bool useSafeHandle)
        {
            Type type = parameter.TypeReference.GetResolvedType();

            // TODO: Do this check here?
            // We cannot have a void-type parameter, so use IntPtr instead
            if (type.Name == "void")
                return "IntPtr";

            // Do nullability checks (actual logic in `TypeExtension.WriteType()`)
            return (target, parameter, symbol: type) switch
            {
                (Target.Managed, _, _) => Nullable(parameter, target, currentNamespace, useSafeHandle),

                //IntPtr can't be nullable they can be "nulled" via IntPtr.Zero
                (Target.Native, _, { Name: { Value: "IntPtr" } }) => NotNullable(parameter, target, currentNamespace, useSafeHandle),

                //Native arrays can not be nullable
                (Target.Native, { TypeReference: ArrayTypeReference }, _) => NotNullable(parameter, target, currentNamespace, useSafeHandle),

                //Classes are represented as IntPtr and should not be nullable
                (Target.Native, _, Class) => NotNullable(parameter, target, currentNamespace, useSafeHandle),

                //Records are represented as SafeHandles and are not nullable
                (Target.Native, _, Record) => NotNullable(parameter, target, currentNamespace, useSafeHandle),

                //Pointer to primitive value types are not nullable
                (Target.Native, { TypeReference: { CTypeReference:{ IsPointer: true }} }, PrimitiveValueType) => NotNullable(parameter, target, currentNamespace, useSafeHandle),

                (Target.Native, _, _) => Nullable(parameter, target, currentNamespace, useSafeHandle),

                _ => throw new Exception($"Unknown {nameof(Target)}")
            };
        }

        private static string NotNullable(Parameter parameter, Target target, Namespace currentNamespace, bool useSafeHandle)
            => parameter.WriteType(target, currentNamespace, useSafeHandle);

        private static string Nullable(Parameter parameter, Target target, Namespace currentNamespace, bool useSafeHandle)
            => parameter.WriteType(target, currentNamespace, useSafeHandle) + GetNullable(parameter);

        private static string GetNullable(Parameter singleParameter)
            => singleParameter.Nullable ? "?" : string.Empty;
    }
}
