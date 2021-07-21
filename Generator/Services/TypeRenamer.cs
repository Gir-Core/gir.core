using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GirLoader;
using GirLoader.Output.Model;

namespace Generator.Services
{
    internal class TypeRenamer
    {
        public void FixClassNameClashes(IEnumerable<Namespace> namespaces)
        {
            // TODO: Make a decision on member name priority
            // Priority: Class Name > Method Name > Property Name
            foreach (var cls in namespaces.SelectMany(x => x.Classes))
            {
                FixClassMethods(cls);
                FixClassProperties(cls);
                FixMethodsAndProperties(cls);
            }
        }

        private static void FixMethodsAndProperties(Class cls)
        {
            // TODO: This seems very inefficient - Optimise?
            foreach (var method in cls.Methods)
                foreach (var prop in cls.Properties)
                    if (method.Name == prop.Name)
                        prop.Name = new SymbolName(prop.Name + "Prop");
        }

        private static void FixClassMethods(Class cls)
        {
            foreach (var method in cls.Methods)
            {
                if (method.Name == cls.Name)
                    method.Name = new SymbolName(method.Name + "Func");
            }
        }

        private static void FixClassProperties(Class cls)
        {
            foreach (var prop in cls.Properties)
            {
                if (prop.Name == cls.Name)
                {
                    prop.Name = prop.Name with { Value = prop.Name + "Prop" };
                }
            }
        }

        public void SetMetadata(IEnumerable<Namespace> namespaces)
        {
            foreach (Namespace ns in namespaces)
            {
                SetRecordMetadata(ns.Records);
                SetRecordFieldsCallbackMetadata(ns.Records);
                SetUnionMetadata(ns.Unions);
                SetCallbacksMetadata(ns.Callbacks);
            }

            Log.Information("Metadata set.");
        }

        private void SetRecordFieldsCallbackMetadata(IEnumerable<Record> records)
        {
            foreach (var record in records)
            {
                foreach (var callback in record.Fields.Select(x => x.Callback))
                {
                    if (callback is { })
                        SetCallbackMetadata(callback);
                }
            }
        }

        private void SetCallbacksMetadata(IEnumerable<Callback> callbacks)
        {
            foreach (var callback in callbacks)
                SetCallbackMetadata(callback);
        }

        private void SetCallbackMetadata(Callback callback)
        {
            callback.Metadata["ManagedName"] = callback.Name;
            callback.Name = new SymbolName(callback.Name + "Callback");
        }

        private void SetUnionMetadata(IEnumerable<Union> unions)
        {
            foreach (var union in unions)
                SetUnionMetadata(union);
        }

        private void SetUnionMetadata(Union union)
        {
            union.Metadata["Name"] = union.Name;
            union.Metadata["StructName"] = "Struct";

            union.Name = new SymbolName($"{union.Name}.Struct");
        }

        private void SetRecordMetadata(IEnumerable<Record> records)
        {
            foreach (var record in records)
            {
                if (record.GLibClassStructFor is { })
                    SetClassStructMetadata(record);
                else
                    SetRecordMetadata(record);
            }
        }

        private void SetClassStructMetadata(Record record)
        {
            Debug.Assert(record.GLibClassStructFor is not null);

            var className = record.GLibClassStructFor.GetResolvedType().Name;
            record.Metadata["Name"] = className;
            record.Metadata["StructName"] = "Class";
            record.Metadata["StructRefName"] = $"{className}.Class";
            record.Metadata["SafeHandleName"] = "Handle";
            record.Metadata["SafeHandleRefName"] = $"{className}.Handle";

            record.Name = new SymbolName($"{className}.Class");
        }

        private void SetRecordMetadata(Record record)
        {
            record.Metadata["Name"] = record.Name;
            record.Metadata["StructName"] = "Struct";
            record.Metadata["StructRefName"] = $"{record.Name}.Struct";
            record.Metadata["SafeHandleName"] = "Handle";
            record.Metadata["SafeHandleRefName"] = $"{record.Name}.Handle";

            record.Name = new SymbolName($"{record.Name}");
        }
    }
}
