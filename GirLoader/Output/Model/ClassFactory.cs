using System;

namespace GirLoader.Output.Model
{
    internal class ClassFactory
    {
        private readonly TypeReferenceFactory _typeReferenceFactory;
        private readonly MethodFactory _methodFactory;
        private readonly PropertyFactory _propertyFactory;
        private readonly FieldFactory _fieldFactory;
        private readonly SignalFactory _signalFactory;

        public ClassFactory(TypeReferenceFactory typeReferenceFactory, MethodFactory methodFactory, PropertyFactory propertyFactory, FieldFactory fieldFactory, SignalFactory signalFactory)
        {
            _typeReferenceFactory = typeReferenceFactory;
            _methodFactory = methodFactory;
            _propertyFactory = propertyFactory;
            _fieldFactory = fieldFactory;
            _signalFactory = signalFactory;
        }

        public Class Create(Input.Model.Class cls, Repository repository)
        {
            if (cls.Name is null)
                throw new Exception("Class is missing data");

            if (cls.GetTypeFunction is null)
                throw new Exception($"Class {cls.Name} is missing a get type function");

            CType? cTypeName = null;
            if (cls.Type is { })
                cTypeName = new CType(cls.Type);

            return new Class(
                repository: repository,
                originalName: new SymbolName(cls.Name),
                symbolName: new SymbolName(cls.Name),
                cType: cTypeName,
                parent: CreateParentTypeReference(cls.Parent, repository.Namespace),
                implements: _typeReferenceFactory.Create(cls.Implements),
                methods: _methodFactory.Create(cls.Methods),
                functions: _methodFactory.Create(cls.Functions),
                getTypeFunction: _methodFactory.CreateGetTypeMethod(cls.GetTypeFunction),
                properties: _propertyFactory.Create(cls.Properties),
                fields: _fieldFactory.Create(cls.Fields, repository),
                signals: _signalFactory.Create(cls.Signals),
                constructors: _methodFactory.Create(cls.Constructors),
                isFundamental: cls.Fundamental
            );
        }

        private TypeReference? CreateParentTypeReference(string? parentName, Namespace @namespace)
        {
            if (parentName is { })
                return CreateTypeReference(parentName, @namespace);

            return null;
        }

        private TypeReference CreateTypeReference(string name, Namespace @namespace)
        {
            var ctype = !name.Contains(".") ? @namespace.IdentifierPrefixes + name : null;

            return _typeReferenceFactory.CreateResolveable(name, ctype);
        }
    }
}
