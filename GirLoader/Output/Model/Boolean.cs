namespace GirLoader.Output.Model
{
    public class Boolean : PrimitiveValueType
    {
        public Boolean(string ctype) : base(new CType(ctype), new SymbolName("bool")) { }
    }
}
