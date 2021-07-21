using System.Collections.Generic;

namespace GirLoader.Output.Model
{
    public class SingleParameter : Symbol, Parameter
    {
        public TypeReference TypeReference { get; }
        public Direction Direction { get; }
        public Transfer Transfer { get; }
        public bool Nullable { get; }
        public bool CallerAllocates { get; }
        public int? ClosureIndex { get; }
        public int? DestroyIndex { get; }
        public Scope CallbackScope { get; }

        public SingleParameter(SymbolName originalName, SymbolName symbolName, TypeReference typeReference, Direction direction, Transfer transfer, bool nullable, bool callerAllocates, int? closureIndex, int? destroyIndex, Scope scope) : base(originalName, symbolName)
        {
            TypeReference = typeReference;
            Direction = direction;
            Transfer = transfer;
            Nullable = nullable;
            CallerAllocates = callerAllocates;
            ClosureIndex = closureIndex;
            DestroyIndex = destroyIndex;
            CallbackScope = scope;
        }

        public override IEnumerable<TypeReference> GetTypeReferences()
        {
            yield return TypeReference;
        }

        public override bool GetIsResolved()
            => TypeReference.GetIsResolved();
    }
}
