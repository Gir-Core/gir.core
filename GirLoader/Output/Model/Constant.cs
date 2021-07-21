using System.Collections.Generic;

namespace GirLoader.Output.Model
{
    public class Constant : Symbol
    {
        public string Value { get; }
        public TypeReference TypeReference { get; }

        public Constant(SymbolName originalName, SymbolName symbolName, TypeReference typeReference, string value) : base(originalName, symbolName)
        {
            TypeReference = typeReference;
            Value = value;
        }

        public override IEnumerable<TypeReference> GetTypeReferences()
        {
            yield return TypeReference;
        }

        public override bool GetIsResolved()
            => TypeReference.GetIsResolved();
    }
}
