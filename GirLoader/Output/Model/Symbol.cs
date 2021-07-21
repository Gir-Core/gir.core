using System.Collections.Generic;

namespace GirLoader.Output.Model
{
    //TODO: Improve hierarchy
    // - Symbol does not need an "OriginalName" as "PrimitiveTypes" do not need a "OriginalName"
    // - An "OriginalName" is needed only for elements which can be renamed. This appies mostly to complex types like Class / Interface / Record / Union
    // - There are other elements like Parameters / Methods which can be renamed, too
    //
    // Possible Hierarchy for Types:
    // - remove Symbol from hierarchy, integrate properties into Type
    // Class -> ComplexType -> Type -> S̶y̶m̶b̶o̶l̶ 
    // Integer -> PrimitiveType -> Type -> S̶y̶m̶b̶o̶l̶
    //
    // For other elements like methods
    // - Keep Symbol which has a OriginalName
    // Method -> Symbol
    // Parameter -> Symbol
    // ReturnValue (has no hierarchy as it has no representation / is no symbol)
    public abstract class Symbol : TypeReferenceProvider, Resolveable
    {
        public SymbolName OriginalName { get; }
        public SymbolName Name { get; set; }

        protected Symbol(SymbolName name)
        {
            OriginalName = name;
            Name = name;
        }
        
        protected Symbol(SymbolName originalName, SymbolName name)
        {
            OriginalName = originalName;
            Name = name;
        }

        public abstract IEnumerable<TypeReference> GetTypeReferences();
        public abstract bool GetIsResolved();

        public override string ToString()
            => Name;
    }
}
