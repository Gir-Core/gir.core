using System.Collections.Generic;
using System.Linq;

namespace GirLoader.Output.Model
{
    public class Member : Symbol
    {
        public string Value { get; }

        public Member(SymbolName originalName, SymbolName symbolName, string value) : base(originalName, symbolName)
        {
            Value = value;
        }

        public override IEnumerable<TypeReference> GetTypeReferences()
            => Enumerable.Empty<TypeReference>();

        public override bool GetIsResolved()
            => true;
    }
}
