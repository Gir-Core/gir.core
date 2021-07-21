namespace GirLoader.Output.Model
{
    public abstract class Type : Symbol
    {
        public Repository? Repository { get; }
        public Metadata Metadata { get; } = new();

        /// <summary>
        /// Name of the symbol in the c world
        /// </summary>
        public CType? CType { get; }

        protected internal Type(CType ctype, SymbolName name) : this(ctype, name, name)
        {
        }

        protected internal Type(CType? ctype, SymbolName originalName, SymbolName symbolName) : this(null, ctype, originalName, symbolName)
        {
        }

        protected internal Type(Repository? repository, CType? cType, SymbolName orignalName, SymbolName symbolName) : base(orignalName, symbolName)
        {
            Repository = repository;
            CType = cType;
        }

        internal virtual void Strip() { }

        public string GetMetadataString(string key)
            => Metadata[key]?.ToString() ?? "";

        internal abstract bool Matches(TypeReference typeReference);
    }
}
