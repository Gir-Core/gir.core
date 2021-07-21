using System.Collections.Generic;
using GirLoader.Helper;

namespace GirLoader.Output.Model
{
    public class Signal : Symbol
    {
        public ReturnValue ReturnValue { get; }
        public ParameterList ParameterList { get; }

        public Signal(SymbolName originalName, SymbolName symbolName, ReturnValue returnValue, ParameterList parameterList) : base(originalName, symbolName)
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
    }
}
