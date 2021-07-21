using System.Text;
using GirLoader.Output.Model;

namespace Generator
{
    public static class ElementExtension
    {
        public static string WriteNativeSummary(this Symbol symbol)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"/// <summary>");
            builder.AppendLine($"/// Name: {symbol.OriginalName}.");
            builder.AppendLine($"/// </summary>");
            return builder.ToString();
        }
    }
}
