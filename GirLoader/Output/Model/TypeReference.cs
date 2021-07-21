using System;

namespace GirLoader.Output.Model
{
    public abstract class TypeReference : Resolveable
    {
        #region Properties
        public CTypeReference? CTypeReference { get; }
        public SymbolNameReference? SymbolNameReference { get; }
        public abstract Type? ResolvedType { get; }
        
        #endregion

        public TypeReference(SymbolNameReference? symbolNameReference, CTypeReference? ctypeReference)
        {
            CTypeReference = ctypeReference;
            SymbolNameReference = symbolNameReference;
        }
        
        public Type GetResolvedType()
        {
            if (ResolvedType is null)
                throw new InvalidOperationException($"The {GetType().Namespace} for {SymbolNameReference} has not been resolved.");

            return ResolvedType;
        }

        public override string ToString()
        {
            return $"CType: {CTypeReference}, OriginalName: {SymbolNameReference}";
        }

        public bool GetIsResolved()
            => ResolvedType is { };
    }
}
