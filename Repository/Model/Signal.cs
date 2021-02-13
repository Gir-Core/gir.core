﻿using System.Collections.Generic;
using System.Linq;
using Repository.Analysis;

namespace Repository.Model
{
    public class Signal : Symbol
    {
        public ReturnValue ReturnValue { get; }
        public IEnumerable<Argument> Arguments { get; }

        public Signal(string nativeName, string managedName, ReturnValue returnValue, IEnumerable<Argument> arguments) : base(nativeName, managedName)
        {
            ReturnValue = returnValue;
            Arguments = arguments;
        }

        public override string ToString()
            => ManagedName;

        public override IEnumerable<ISymbolReference> GetSymbolReferences()
        {
            return IEnumerables.Concat(
                ReturnValue.GetSymbolReferences(),
                Arguments.SelectMany(x => x.GetSymbolReferences())
            );
        }
    }
}