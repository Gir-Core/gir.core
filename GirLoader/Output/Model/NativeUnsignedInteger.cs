namespace GirLoader.Output.Model
{
    public class NativeUnsignedInteger : PrimitiveValueType
    {
        public NativeUnsignedInteger(string ctype) : base(new CType(ctype), new SymbolName("nuint")) { }
    }
}
