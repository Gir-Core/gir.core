using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GirLoader.Output.Resolver
{
    internal partial class TypeResolver
    {
        private class TypeCache
        {
            private readonly HashSet<Model.Type> _types = new();

            public void Add(Model.Type type)
            {
                _types.Add(type);
            }

            public bool TryResolve(Model.TypeReference typeReference, [MaybeNullWhen(false)] out Model.Type type)
            {
                type = _types.FirstOrDefault(type => type.Matches(typeReference));
                return type is not null;
            }
        }
    }
}
