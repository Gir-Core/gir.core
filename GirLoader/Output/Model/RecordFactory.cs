using System;

namespace GirLoader.Output.Model
{
    internal class RecordFactory
    {
        private readonly TypeReferenceFactory _typeReferenceFactory;
        private readonly MethodFactory _methodFactory;
        private readonly FieldFactory _fieldFactory;

        public RecordFactory(TypeReferenceFactory typeReferenceFactory, MethodFactory methodFactory, FieldFactory fieldFactory)
        {
            _typeReferenceFactory = typeReferenceFactory;
            _methodFactory = methodFactory;
            _fieldFactory = fieldFactory;
        }

        public Record Create(Input.Model.Record @record, Repository repository)
        {
            if (@record.Name is null)
                throw new Exception("Record is missing a name");

            Method? getTypeFunction = @record.GetTypeFunction switch
            {
                { } f => _methodFactory.CreateGetTypeMethod(f),
                _ => null
            };

            CType? cTypeName = null;
            if (@record.CType is { })
                cTypeName = new CType(@record.CType);

            return new Record(
                repository: repository,
                cType: cTypeName,
                originalName: new SymbolName(@record.Name),
                symbolName: new SymbolName(@record.Name),
                gLibClassStructFor: GetGLibClassStructFor(@record.GLibIsGTypeStructFor, repository.Namespace),
                methods: _methodFactory.Create(@record.Methods),
                functions: _methodFactory.Create(@record.Functions),
                getTypeFunction: getTypeFunction,
                fields: _fieldFactory.Create(@record.Fields, repository),
                disguised: @record.Disguised,
                constructors: _methodFactory.Create(@record.Constructors)
            );
        }

        private TypeReference? GetGLibClassStructFor(string? classStructForName, Namespace @namespace)
        {
            TypeReference? getGLibClassStructFor = null;

            if (classStructForName is { })
            {
                //We can generate the CType automatically because the class struct
                //of a class must be part of the repository of the class itself.
                var ctype = @namespace.IdentifierPrefixes + classStructForName;
                getGLibClassStructFor = _typeReferenceFactory.CreateResolveable(classStructForName, ctype);
            }

            return getGLibClassStructFor;
        }
    }
}
