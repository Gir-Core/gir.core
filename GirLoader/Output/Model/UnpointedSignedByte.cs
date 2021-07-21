namespace GirLoader.Output.Model
{
    public class UnpointedSignedByte : PrimitiveValueType
    {
        public UnpointedSignedByte(string ctype) : base(new CType(ctype), new SymbolName("sbyte")) { }

        internal override bool Matches(TypeReference typeReference)
        {
            if (typeReference.CTypeReference?.IsPointer == true)
                return false;
            
            return base.Matches(typeReference);
        }
    }
}
