namespace GirLoader.Output.Model
{
    public class UnsignedInteger : PrimitiveValueType
    {
        public UnsignedInteger(string ctype) : base(new CType(ctype), new SymbolName("uint")) { }
    }
}
