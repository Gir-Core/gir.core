using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Sf = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Sk = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using St = Microsoft.CodeAnalysis.SyntaxToken;

namespace GirLoader.Helper
{
    public class String
    {
        private readonly string _text;

        public String(string text)
        {
            _text = text;
        }

        public String ToCamelCase()
        {
            var pascalCase = ToPascal(_text);

            return _text switch
            {
                { Length: 0 } => new String(""),
                { Length: 1 } => new String(pascalCase.ToLower()),
                _ => new String(char.ToLower(pascalCase[0]) + pascalCase[1..])
            };
        }

        public String ToPascalCase()
        {
            return _text switch
            {
                { Length: 0 } => new String(""),
                { Length: 1 } => new String(_text.ToUpper()),
                _ => new String(ToPascal(_text))
            };
        }

        private static string ToPascal(string str)
        {
            var words = str.Replace("_", "-").Split("-");
            var builder = new StringBuilder();
            foreach (var word in words.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                builder
                    .Append(char.ToUpper(word[0]))
                    .Append(word[1..]);
            }

            return builder.ToString();
        }

        public String EscapeIdentifier()
        {
            var escaped = FixFirstCharIfNumber(_text);
            escaped = FixIfIdentifierIsKeyword(escaped);

            return new String(escaped);
        }

        private static string FixIfIdentifierIsKeyword(string identifier)
        {
            St token = Sf.ParseToken(identifier);
            if (token.Kind() != Sk.IdentifierToken)
                identifier = "@" + identifier;

            return identifier;
        }

        private static string FixFirstCharIfNumber(string identifier)
        {
            var firstChar = identifier[0];

            if (char.IsNumber(firstChar))
            {
                // Capitalise Second Char
                if (identifier.Length > 1)
                    identifier = CapitaliseSecondChar(identifier);

                var number = (int) char.GetNumericValue(firstChar);
                return number switch
                {
                    0 => ReplaceFirstChar("Zero", identifier),
                    1 => ReplaceFirstChar("One", identifier),
                    2 => ReplaceFirstChar("Two", identifier),
                    3 => ReplaceFirstChar("Three", identifier),
                    4 => ReplaceFirstChar("Four", identifier),
                    5 => ReplaceFirstChar("Five", identifier),
                    6 => ReplaceFirstChar("Six", identifier),
                    7 => ReplaceFirstChar("Seven", identifier),
                    8 => ReplaceFirstChar("Eight", identifier),
                    9 => ReplaceFirstChar("Nine", identifier),
                    _ => throw new Exception("Can't fix identifier " + identifier)
                };
            }

            return identifier;
        }

        private static string ReplaceFirstChar(string prefix, string str)
            => prefix + str[1..];

        private static string CapitaliseSecondChar(string identifier)
            => $"{identifier[0]}{char.ToUpper(identifier[1])}{identifier?[2..]}";
        
        public string Get()
            => _text;

        public override string ToString()
        {
            return _text;
        }
    }
}
