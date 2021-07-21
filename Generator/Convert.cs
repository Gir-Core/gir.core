using System;
using GirLoader.Output.Model;
using String = GirLoader.Output.Model.String;
using Type = GirLoader.Output.Model.Type;

namespace Generator
{
    internal static class Convert
    {
        internal static string ManagedToNative(TransferableAnyType transferable, string fromParam, Namespace currentNamespace)
        {
            Transfer transfer = transferable.Transfer;
            Type type = transferable.TypeReference.ResolvedType
                        ?? throw new NullReferenceException($"Error: Conversion of '{fromParam}' to {nameof(Target.Native)} failed - {transferable.GetType().Name} does not have a type");

            var qualifiedNativeType = type.Write(Target.Native, currentNamespace);
            var qualifiedManagedType = type.Write(Target.Managed, currentNamespace);

            return transferable.TypeReference switch
            {
                // String Array Handling
                ArrayTypeReference { Length: null, ResolvedType: String } when transfer == Transfer.None => $"new GLib.Native.StringArrayNullTerminatedSafeHandle({fromParam}).DangerousGetHandle()",
                
                // String Handling
                ResolveableTypeReference { ResolvedType: String } when (transfer == Transfer.None) && (transferable is ReturnValue) => $"GLib.Native.StringHelper.StringToHGlobalUTF8({fromParam})",
                ResolveableTypeReference { ResolvedType: String } => fromParam,
                
                //Record Array Conversions 
                ArrayTypeReference { ResolvedType: Record, TypeReference: { CTypeReference: { IsPointer: true}}} => $"{fromParam}.Select(x => x.Handle.DangerousGetHandle()).ToArray()",
                ArrayTypeReference { ResolvedType: Record r, TypeReference: { CTypeReference: { IsPointer: false}}} => $"({r.Repository.Namespace}.Native.{r.GetMetadataString("StructRefName")}[]) default!; //TODO: Fixme",
                ArrayTypeReference { ResolvedType: Record r, TypeReference: { CTypeReference: null}} => $"({r.Repository.Namespace}.Native.{r.GetMetadataString("StructRefName")}[]) default!; //TODO: Fixme",

                //Record Conversions
                ResolveableTypeReference { ResolvedType: Record, CTypeReference: { IsPointer: true }} => $"{fromParam}.Handle",
                ResolveableTypeReference { ResolvedType: Record r, CTypeReference: { IsPointer: false }} => $"({r.Repository.Namespace}.Native.{r.GetMetadataString("StructRefName")}) default!; //TODO: Fixme",

                // Class Array Conversions
                ArrayTypeReference { ResolvedType: Class, TypeReference: { CTypeReference: {IsPointer: true}}} => throw new NotImplementedException($"Can't create delegate for argument {fromParam}"),
                ArrayTypeReference { ResolvedType: Class} => $"{fromParam}.Select(cls => cls.Handle).ToArray()",
                
                // Class Conversions
                ResolveableTypeReference { ResolvedType: Class {IsFundamental: true}, CTypeReference: {IsPointer: true}} => $"{qualifiedManagedType}.To({fromParam})",
                ResolveableTypeReference { ResolvedType: Class, CTypeReference: {IsPointer: true}} => $"{fromParam}.Handle",

                // Interface Array Conversions
                ArrayTypeReference { ResolvedType: Interface } => $"{fromParam}.Select(iface => (iface as GObject.Object).Handle).ToArray()",
                
                // Interface Array Conversions
                ResolveableTypeReference {ResolvedType: Interface} => $"({fromParam} as GObject.Object).Handle",

                // Other -> Try a brute-force cast
                ArrayTypeReference { } => $"({qualifiedNativeType}[]){fromParam}",
                _ => $"({qualifiedNativeType}){fromParam}"
            };
        }

        internal static string NativeToManaged(TransferableAnyType transferable, string fromParam, Namespace currentNamespace, bool useSafeHandle = true)
        {
            Transfer transfer = transferable.Transfer;
            Type type = transferable.TypeReference.ResolvedType
                        ?? throw new NullReferenceException($"Error: Conversion of '{fromParam}' to {nameof(Target.Managed)} failed - {transferable.GetType().Name} does not have a type");

            var qualifiedType = type.Write(Target.Managed, currentNamespace);

            var bla = transferable.TypeReference switch
            {
                _ => ""
            };
            
            return transferable.TypeReference switch
            {
                // String Array Handling
                ArrayTypeReference { Length: null, ResolvedType: String } when transfer == Transfer.None => $"GLib.Native.StringHelper.ToStringArrayUtf8({fromParam})",
                
                // String Handling
                {ResolvedType: String} when (transfer == Transfer.None) && (transferable is ReturnValue) => $"GLib.Native.StringHelper.ToStringUtf8({fromParam})",
                
                // Record Conversions (safe handles)
                ArrayTypeReference { ResolvedType: Record r, TypeReference: { CTypeReference: {IsPointer: true}}} when useSafeHandle => $"{fromParam}.Select(x => new {r.Write(Target.Managed, currentNamespace)}(x)).ToArray()",
                ResolveableTypeReference {ResolvedType: Record r, CTypeReference: {IsPointer: true}} when useSafeHandle => $"new {r.Write(Target.Managed, currentNamespace)}({fromParam})",

                // Record Conversions (raw pointers)
                ArrayTypeReference { ResolvedType: Record r, TypeReference: { CTypeReference: {IsPointer: true}}} when !useSafeHandle => $"{fromParam}.Select(x => new {r.Write(Target.Managed, currentNamespace)}(new {SafeHandleFromRecord(r)}(x))).ToArray()",
                ResolveableTypeReference {ResolvedType: Record r, CTypeReference: {IsPointer: true}} when !useSafeHandle => $"new {r.Write(Target.Managed, currentNamespace)}(new {SafeHandleFromRecord(r)}({fromParam}))",
                
                //Record Conversions without pointers are not working yet
                ArrayTypeReference { ResolvedType: Record r, TypeReference: { CTypeReference: {IsPointer: false}}} => $"({r.Write(Target.Managed, currentNamespace)}[]) default!; //TODO: Fixme",
                ResolveableTypeReference {ResolvedType: Record r, CTypeReference: {IsPointer: false}} when !useSafeHandle => $"({r.Write(Target.Managed, currentNamespace)}) default!; //TODO: Fixme",
                
                // Class Conversions
                ArrayTypeReference {ResolvedType: Class, CTypeReference: {IsPointer: true}} => throw new NotImplementedException($"Can't create delegate for argument '{fromParam}'"),
                ResolveableTypeReference {ResolvedType: Class {IsFundamental: true}, CTypeReference: {IsPointer: true}} => $"{qualifiedType}.From({fromParam})",
                ResolveableTypeReference {ResolvedType: Class, CTypeReference: {IsPointer: true}} => $"GObject.Native.ObjectWrapper.WrapHandle<{qualifiedType}>({fromParam}, {transfer.IsOwnedRef().ToString().ToLower()})",

                // Misc
                ResolveableTypeReference {ResolvedType: Interface} => $"GObject.Native.ObjectWrapper.WrapHandle<{qualifiedType}>({fromParam}, {transfer.IsOwnedRef().ToString().ToLower()})",
                ResolveableTypeReference {ResolvedType: Union} => $"",

                // Other -> Try a brute-force cast
                ArrayTypeReference { } => $"({qualifiedType}[]){fromParam}",
                _ => $"({qualifiedType}){fromParam}"
            };
        }

        private static string SafeHandleFromRecord(Record r)
        {
            var type = r.GetMetadataString("SafeHandleRefName");
            var nspace = $"{r.Repository.Namespace}.Native";
            return nspace + "." + type;
        }
    }
}
