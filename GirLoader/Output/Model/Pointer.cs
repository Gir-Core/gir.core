namespace GirLoader.Output.Model
{
    public class Pointer : PrimitiveType
    {
        public Pointer(string ctype) : base(new CType(ctype), new SymbolName("IntPtr")) { }
        
        internal override bool Matches(TypeReference typeReference)
        {
            if (typeReference.CTypeReference is null)
                return false;
            
            return typeReference.CTypeReference.CType == CType;
        }
    }
}
