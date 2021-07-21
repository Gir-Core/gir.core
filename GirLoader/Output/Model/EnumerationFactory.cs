using System;
using System.Linq;

namespace GirLoader.Output.Model
{
    internal class EnumerationFactory
    {
        private readonly MemberFactory _memberFactory;

        public EnumerationFactory(MemberFactory memberFactory)
        {
            _memberFactory = memberFactory;
        }

        public Enumeration Create(Input.Model.Enum @enum, Repository repository, bool hasFlags)
        {
            if (@enum.Name is null)
                throw new Exception("Enum has no name");

            if (@enum.Type is null)
                throw new Exception("Enum is missing a type");

            return new Enumeration(
                repository: repository,
                originalName: new SymbolName(@enum.Name),
                symbolName: new SymbolName(@enum.Name),
                hasFlags: hasFlags,
                members: @enum.Members.Select(x => _memberFactory.Create(x)).ToList(),
                cType: new CType(@enum.Type)
            );
        }
    }
}
