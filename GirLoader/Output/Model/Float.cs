namespace GirLoader.Output.Model
{
    public class Float : PrimitiveValueType
    {
        public Float(string ctype) : base(new CType(ctype), new SymbolName("float")) { }
    }
}
