using System.Collections.Generic;

namespace GirLoader.Output.Resolver
{
    internal partial class TypeResolver
    {
        private class RepositoryTypeCache : TypeCache
        {
            private readonly Model.Repository _repository;

            public RepositoryTypeCache(Model.Repository repository)
            {
                _repository = repository;
                Add(repository.Namespace.Classes);
                Add(repository.Namespace.Interfaces);
                Add(repository.Namespace.Callbacks);
                Add(repository.Namespace.Enumerations);
                Add(repository.Namespace.Bitfields);
                Add(repository.Namespace.Records);
                Add(repository.Namespace.Unions);
            }

            private void Add(IEnumerable<Model.Type> types)
            {
                foreach(var type in types)
                    Add(type);
            }
            
            public override string ToString()
            {
                return $"{nameof(TypeCache)} for: {_repository.Namespace.Name}";
            }
        }
    }
}
