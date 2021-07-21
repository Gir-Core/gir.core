namespace GirLoader.Output.Model
{
    public class ResolveableTypeReference : TypeReference
    {
        private Type? _resolvedType;
        public override Type? ResolvedType => _resolvedType;

        public ResolveableTypeReference(SymbolNameReference? symbolNameReference, CTypeReference? ctype)
            : base(symbolNameReference, ctype)
        {
        }

        public void ResolveAs(Type type)
        {
            _resolvedType = type;
        }
    }
}
