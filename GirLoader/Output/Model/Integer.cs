namespace GirLoader.Output.Model
{
    public class Integer : PrimitiveValueType
    {
        public Integer(string ctype) : base(new CType(ctype), new SymbolName("int")) { }
    }
}
