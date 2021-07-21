using System.Collections.Generic;
using System.Linq;
using GirLoader.Helper;

namespace GirLoader.Output.Model
{
    public class Record : Type
    {
        private readonly List<Method> _methods;
        private readonly List<Method> _functions;
        private readonly List<Method> _constructors;
        private readonly List<Field> _fields;

        public bool IsClassStruct => GLibClassStructFor is { };

        public Method? GetTypeFunction { get; }
        public IEnumerable<Field> Fields => _fields;
        public bool Disguised { get; }
        public IEnumerable<Method> Methods => _methods;
        public IEnumerable<Method> Constructors => _constructors;
        public IEnumerable<Method> Functions => _functions;
        public TypeReference? GLibClassStructFor { get; }

        public Record(Repository repository, CType? cType, SymbolName originalName, SymbolName symbolName, TypeReference? gLibClassStructFor, IEnumerable<Method> methods, IEnumerable<Method> functions, Method? getTypeFunction, IEnumerable<Field> fields, bool disguised, IEnumerable<Method> constructors) : base(repository, cType, originalName, symbolName)
        {
            GLibClassStructFor = gLibClassStructFor;
            GetTypeFunction = getTypeFunction;
            Disguised = disguised;

            this._constructors = constructors.ToList();
            this._methods = methods.ToList();
            this._functions = functions.ToList();
            this._fields = fields.ToList();
        }

        public override IEnumerable<TypeReference> GetTypeReferences()
        {
            var symbolReferences = IEnumerables.Concat(
                Constructors.GetTypeReferences(),
                Fields.GetTypeReferences(),
                Methods.GetTypeReferences(),
                Functions.GetTypeReferences()
            );

            if (GetTypeFunction is { })
                symbolReferences = symbolReferences.Concat(GetTypeFunction.GetTypeReferences());

            if (GLibClassStructFor is { })
                symbolReferences = symbolReferences.Append(GLibClassStructFor);

            return symbolReferences;
        }

        public override bool GetIsResolved()
        {
            if (!(GetTypeFunction?.GetIsResolved() ?? true))
                return false;

            return Methods.AllResolved()
                   && Functions.AllResolved()
                   && Constructors.AllResolved()
                   && Fields.AllResolved();
        }

        internal override void Strip()
        {
            //Fields are not cleaned as those are needed
            //to represent the native structure of the object / class

            _methods.RemoveAll(Remove);
            _functions.RemoveAll(Remove);
            _constructors.RemoveAll(Remove);
        }

        private bool Remove(Symbol symbol)
        {
            var result = symbol.GetIsResolved();

            if (!result)
                Log.Information($"Record {Repository?.Namespace.Name}.{OriginalName}: Stripping symbol {symbol.OriginalName}");

            return !result;
        }
        
        internal override bool Matches(TypeReference typeReference)
        {
            if (typeReference.CTypeReference is not null  && typeReference.CTypeReference.CType != "gpointer")
                return typeReference.CTypeReference.CType == CType;

            if (typeReference.SymbolNameReference is not null)
            {
                var nameMatches = typeReference.SymbolNameReference.SymbolName == OriginalName;
                var namespaceMatches = typeReference.SymbolNameReference.NamespaceName == Repository.Namespace.Name;
                var namespaceMissing = typeReference.SymbolNameReference.NamespaceName == null;

                return nameMatches && (namespaceMatches || namespaceMissing);
            }
            
            return false;
        }
    }
}
