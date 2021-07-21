namespace GirLoader.Output.Model
{
    public class Double : PrimitiveValueType
    {
        public Double(string ctype) : base(new CType(ctype), new SymbolName("double")) { }
    }
}
