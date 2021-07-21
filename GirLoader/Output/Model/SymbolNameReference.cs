namespace GirLoader.Output.Model
{
    public class SymbolNameReference
    {
        public SymbolName SymbolName { get; }
        public NamespaceName? NamespaceName { get; }

        public SymbolNameReference(SymbolName symbolName) : this(symbolName, null)
        {
        }

        public SymbolNameReference(SymbolName symbolName, NamespaceName namespaceName)
        {
            SymbolName = symbolName;
            NamespaceName = namespaceName;
        }
    }
}
