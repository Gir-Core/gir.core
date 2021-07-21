namespace GirLoader.Output.Model
{
    public class UnsignedShort : PrimitiveValueType
    {
        public UnsignedShort(string ctype) : base(new CType(ctype), new SymbolName("ushort")) { }
    }
}
