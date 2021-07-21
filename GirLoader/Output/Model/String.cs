namespace GirLoader.Output.Model
{
    /// <summary>
    /// Unicode string using UTF-8 encoding
    /// </summary>
    public class Utf8String : String
    {
        public Utf8String() : base("utf8") { }
    }

    /// <summary>
    /// A platform native string. This should be utf-8 on Windows and
    /// a zero terminated guint8 array on Unix.
    /// TODO: We currently use null terminated ASCII on both platforms, which may be problematic.
    /// </summary>
    public class PlatformString : String
    {
        public PlatformString() : base("filename") { }
    }

    public abstract class String : PrimitiveType
    {
        protected String(string originalName) : base(new CType("gchar*"), new SymbolName(originalName), new SymbolName("string"))
        {
        }
        
        internal override bool Matches(TypeReference typeReference)
        {
            return typeReference.SymbolNameReference.SymbolName == OriginalName;
        }
    }
}
