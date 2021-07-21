namespace GirLoader.Output.Model
{
    public class Byte : PrimitiveValueType
    {
        public Byte(string ctype) : base(new CType(ctype), new SymbolName("byte")) { }
    }
}
