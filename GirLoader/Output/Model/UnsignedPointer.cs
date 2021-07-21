namespace GirLoader.Output.Model
{
    public class UnsignedPointer : PrimitiveType
    {
        public UnsignedPointer(string ctype) : base(new CType(ctype), new SymbolName("UIntPtr")) { }
        
        internal override bool Matches(TypeReference typeReference)
        {
            if (typeReference.CTypeReference is null)
                return false;
            
            return typeReference.CTypeReference.CType == CType;
        }
    }
}
