using System.Diagnostics.CodeAnalysis;

namespace GirLoader.Output.Model
{
    public record SymbolName(string Value)
    {
        public SymbolName(Helper.String helper) : this(helper.Get())
        {
        }
        
        [return: NotNullIfNotNull("name")]
        public static implicit operator string?(SymbolName? name)
            => name?.Value;

        public override string ToString()
            => Value;
    }
}
