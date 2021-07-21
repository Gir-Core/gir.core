namespace GirLoader.Output.Model
{
    public class Short : PrimitiveValueType
    {
        public Short(string ctype) : base(new CType(ctype), new SymbolName("short")) { }
    }
}
