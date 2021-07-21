using System.Collections.Generic;
using GirLoader.Helper;

namespace GirLoader.Output.Model
{
    public class Callback : Type
    {
        public ReturnValue ReturnValue { get; }
        public ParameterList ParameterList { get; }

        public Callback(Repository repository, CType? ctype, SymbolName originalName, SymbolName symbolName, ReturnValue returnValue, ParameterList parameterList) : base(repository, ctype, originalName, symbolName)
        {
            ReturnValue = returnValue;
            ParameterList = parameterList;
        }

        public override IEnumerable<TypeReference> GetTypeReferences()
        {
            return IEnumerables.Concat(
                ReturnValue.GetTypeReferences(),
                ParameterList.GetSymbolReferences()
            );
        }

        public override bool GetIsResolved()
            => ReturnValue.GetIsResolved() && ParameterList.GetIsResolved();
        
        internal override bool Matches(TypeReference typeReference)
        {
            if (typeReference.CTypeReference is null)
                return false;
            
            return typeReference.CTypeReference.CType == CType;
        }
    }
}
