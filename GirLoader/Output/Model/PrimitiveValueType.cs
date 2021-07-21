#nullable enable
namespace GirLoader.Output.Model
{
    public abstract class PrimitiveValueType : PrimitiveType
    {
        protected PrimitiveValueType(CType ctype, SymbolName symbolName) : base(ctype, symbolName)
        {
        }

        internal override bool Matches(TypeReference typeReference)
        {
            if (typeReference.CTypeReference is not null && typeReference.CTypeReference.CType != "gpointer")
                return typeReference.CTypeReference.CType == CType;

            if (typeReference.SymbolNameReference.SymbolName.Value is not null)
                return typeReference.SymbolNameReference.SymbolName.Value == CType.Value;
            
            return false;
        }
    }
}
