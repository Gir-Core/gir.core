namespace GirLoader.Output.Model
{
    public class Void : PrimitiveType
    {
        public Void(string ctype) : base(new CType(ctype), new SymbolName("void")) { }
        
        internal override bool Matches(TypeReference typeReference)
        {
            return typeReference.CTypeReference?.CType == CType;
        }
    }
}
