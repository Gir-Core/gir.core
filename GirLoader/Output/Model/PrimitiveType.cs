using System.Collections.Generic;
using System.Linq;

namespace GirLoader.Output.Model
{
    public abstract class PrimitiveType : Type
    {
        protected PrimitiveType(CType ctype, SymbolName originalName, SymbolName symbolName) : base(ctype, originalName, symbolName)
        {
        }
        
        protected PrimitiveType(CType ctype, SymbolName symbolName) : base(ctype, symbolName)
        {
        }
        
        public override IEnumerable<TypeReference> GetTypeReferences() 
            => Enumerable.Empty<TypeReference>();

        public override bool GetIsResolved() 
            => true;
    }
}
