using System;
using System.Collections.Generic;
using System.Linq;
using GirLoader.Output.Resolver;

namespace GirLoader.Output
{
    internal class RepositoryResolver
    {
        private readonly TypeResolver _typeResolver = new();
        private readonly HashSet<Model.Repository> _knownRepositories = new();

        /// <summary>
        /// Loads the given repository and all its dependencies
        /// </summary>
        public void Add(Model.Repository repository)
        {
            if (!_knownRepositories.Add(repository))
                return; //Ignore known repositories

            _typeResolver.AddRepository(repository);

            foreach (var depdenentRepository in repository.Includes.Select(x => x.GetResolvedRepository()))
                Add(depdenentRepository);
        }

        /// <summary>
        /// Resolves all loaded repositories
        /// </summary>
        public void Resolve()
        {
            var dependencyResolver = new Helper.DependencyResolver<Model.Repository>();
            var orderedRepositories = dependencyResolver.ResolveOrdered(_knownRepositories).Cast<Model.Repository>();

            foreach (var repository in orderedRepositories)
                ResolveTypeReferences(repository);
        }

        private void ResolveTypeReferences(Model.Repository repository)
        {
            foreach (var reference in repository.Namespace.GetTypeReferences())
                ResolveTypeReference(reference, repository);

            Log.Debug($"Resolved type references for repository {repository.Namespace.Name}");
        }

        private void ResolveTypeReference(Model.TypeReference reference, Model.Repository repository)
        {
            if (reference is Model.ArrayTypeReference arrayTypeReference)
            {
                // Array type references are not resolved directly. Only their type get's resolved
                // because arrays are no type themself. They only provide structure.
                ResolveTypeReference(arrayTypeReference.TypeReference, repository);
            }
            else if(reference is Model.ResolveableTypeReference resolveableTypeReference)
            {
                if (_typeResolver.Resolve(resolveableTypeReference, repository,  out var type))
                    resolveableTypeReference.ResolveAs(type);
                else
                    Log.Verbose($"Could not resolve type reference {reference}");   
            }
            else
            {
                throw new Exception($"Unknown {nameof(Model.TypeReference)} {reference.GetType().Name}");
            }
        }
    }
}
