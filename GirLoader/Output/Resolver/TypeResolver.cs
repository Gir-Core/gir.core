using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GirLoader.Output.Resolver
{
    internal partial class TypeResolver
    {
        private readonly Dictionary<Model.Repository, TypeCache> _data = new();
        private readonly TypeCache _globalTypes = new GlobalTypeCache();

        public bool Resolve(Model.TypeReference typeReference, Model.Repository repository, [MaybeNullWhen(false)] out Model.Type type)
        {
            if (_globalTypes.TryResolve(typeReference, out type))
                return true;

            if (TryResolveAlias(typeReference, repository, out type))
                return true;

            //Prefer current namespace to resolve types before trying other ones
            if(_data.TryGetValue(repository, out var typeCache))
                if (typeCache.TryResolve(typeReference, out type))
                    return true;
            
            foreach (var cache in _data.Values)
            {
                if (cache.TryResolve(typeReference, out type))
                    return true;
            }

            return false;
        }

        private bool TryResolveAlias(Model.TypeReference typeReference, Model.Repository repository, [MaybeNullWhen(false)] out Model.Type type)
        {
            type = null;

            type = RecursiveResolveAlias(repository, typeReference);

            return type is { };
        }

        private Model.Type? RecursiveResolveAlias(Model.Repository repository, Model.TypeReference typeReference)
        {
            var directResult = repository.Namespace.Aliases.FirstOrDefault(x => x.Matches(typeReference));

            if (directResult is { })
                return directResult.TypeReference.GetResolvedType();

            foreach (var parent in repository.Dependencies)
            {
                var parentResult = RecursiveResolveAlias(parent, typeReference);
                if (parentResult is { })
                    return parentResult;
            }

            return null;
        }

        public void AddRepository(Model.Repository repository)
        {
            if (_data.ContainsKey(repository))
                throw new Exception($"The repository contains an already known repository {repository}.");
            
            _data.Add(repository, new RepositoryTypeCache(repository));
        }
    }
}
