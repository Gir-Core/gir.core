namespace GirLoader.Output.Model
{
    public class CTypeReference
    {
        public CType CType { get; }
        public bool IsPointer { get; }
        public bool IsConst { get; }
        public bool IsVolatile { get; }

        public CTypeReference(string cTypeReference)
        {
            IsPointer = cTypeReference.Contains("*") || cTypeReference == "gpointer";
            IsConst = cTypeReference.Contains("const ") || cTypeReference.Contains(" const");
            IsVolatile = cTypeReference.Contains("volatile ");

            cTypeReference = cTypeReference
                .Replace("*", "")
                .Replace("const ", "")
                .Replace(" const", "")
                .Replace("volatile ", "");

            CType = new CType(cTypeReference);
        }

        public override string ToString()
            => CType;
    }
}
