namespace GirLoader.Output.Model
{
    public class GPointer : Pointer
    {
        public GPointer() : base("gpointer") { }
        
        internal override bool Matches(TypeReference typeReference)
        {
            if (typeReference.CTypeReference is null)
                return false;

            return typeReference.CTypeReference.CType == CType && typeReference.SymbolNameReference.SymbolName == "gpointer";
        }
    }
}
