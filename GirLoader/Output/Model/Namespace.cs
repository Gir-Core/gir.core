using System.Collections.Generic;
using GirLoader.Helper;

namespace GirLoader.Output.Model
{
    public class Namespace : TypeReferenceProvider
    {
        #region Properties

        public string IdentifierPrefixes { get; }
        public string SymbolPrefixes { get; }
        
        public NamespaceName NativeName => Name with { Value = Name.Value + ".Native" };
        public NamespaceName Name { get; }
        public string Version { get; }
        public string? SharedLibrary { get; }

        private readonly List<Alias> _aliases = new();
        public IEnumerable<Alias> Aliases => _aliases;

        private readonly List<Callback> _callbacks = new();
        public IEnumerable<Callback> Callbacks => _callbacks;

        private readonly List<Class> _classes = new();
        public IEnumerable<Class> Classes => _classes;

        private readonly List<Enumeration> _enumerations = new();
        public IEnumerable<Enumeration> Enumerations => _enumerations;

        private readonly List<Enumeration> _bitfields = new();
        public IEnumerable<Enumeration> Bitfields => _bitfields;

        private readonly List<Interface> _interfaces = new();
        public IEnumerable<Interface> Interfaces => _interfaces;

        private readonly List<Record> _records = new();
        public IEnumerable<Record> Records => _records;

        private readonly List<Method> _functions = new();
        public IEnumerable<Method> Functions => _functions;

        private readonly List<Union> _unions = new();
        public IEnumerable<Union> Unions => _unions;

        private readonly List<Constant> _constants = new();
        public IEnumerable<Constant> Constants => _constants;

        #endregion

        public Namespace(string name, string version, string? sharedLibrary, string identifierPrefixes, string symbolPrefixes)
        {
            Name = new NamespaceName(name);
            Version = version;
            SharedLibrary = sharedLibrary;
            IdentifierPrefixes = identifierPrefixes;
            SymbolPrefixes = symbolPrefixes;
        }

        internal void AddAlias(Alias alias)
            => _aliases.Add(alias);

        internal void AddCallback(Callback callback)
            => _callbacks.Add(callback);

        internal void AddClass(Class @class)
            => _classes.Add(@class);

        internal void AddEnumeration(Enumeration enumeration)
            => _enumerations.Add(enumeration);

        internal void AddBitfield(Enumeration enumeration)
            => _bitfields.Add(enumeration);

        internal void AddInterface(Interface @interface)
            => _interfaces.Add(@interface);

        internal void AddRecord(Record @record)
            => _records.Add(@record);

        public void RemoveRecord(Record @record)
            => _records.Remove(@record);

        internal void AddFunction(Method method)
            => _functions.Add(method);

        internal void AddUnion(Union union)
            => _unions.Add(union);

        internal void AddConstant(Constant constant)
            => _constants.Add(constant);

        public IEnumerable<TypeReference> GetTypeReferences()
        {
            return IEnumerables.Concat(
                Aliases.GetTypeReferences(),
                Callbacks.GetTypeReferences(),
                Classes.GetTypeReferences(),
                Enumerations.GetTypeReferences(),
                Bitfields.GetTypeReferences(),
                Interfaces.GetTypeReferences(),
                Records.GetTypeReferences(),
                Functions.GetTypeReferences(),
                Unions.GetTypeReferences(),
                Constants.GetTypeReferences()
            );
        }

        public string ToCanonicalName() => $"{Name}-{Version}";

        internal void Strip()
        {
            Classes.Strip();
            Interfaces.Strip();

            _aliases.RemoveAll(Remove);
            _callbacks.RemoveAll(Remove);
            _classes.RemoveAll(Remove);
            _enumerations.RemoveAll(Remove);
            _bitfields.RemoveAll(Remove);
            _interfaces.RemoveAll(Remove);
            _records.RemoveAll(Remove);
            _functions.RemoveAll(Remove);
            _unions.RemoveAll(Remove);
            _constants.RemoveAll(Remove);
        }

        private bool Remove(Alias alias)
        {
            var result = alias.GetIsResolved();

            if (!result)
                Log.Information($"{nameof(Alias)} {alias.Name}: Removed because parts of it could not be completely resolvled");

            return !result;
        }

        private bool Remove(Symbol symbol)
        {
            var result = symbol.GetIsResolved();

            if (!result)
                Log.Information($"{symbol.GetType().Name} {symbol.OriginalName}: Removed because parts of it could not be completely resolvled");

            return !result;
        }

        private bool Remove(Type type)
        {
            var result = type.GetIsResolved();

            if (!result)
                Log.Information($"{type.GetType().Name} {type.Repository?.Namespace.Name}.{type.OriginalName}: Removed because parts of it could not be completely resolvled");

            return !result;
        }

        public override string ToString()
            => Name;
    }
}
