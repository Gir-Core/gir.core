﻿using System.Collections.Generic;
using System.Linq;
using Repository.Analysis;

namespace Repository.Model
{
    public class Class : Symbol
    {
        public bool IsFundamental { get; }
        public string CType { get; }
        public Method GetTypeFunction { get; }
        public IEnumerable<SymbolReference> Implements { get; }
        
        public IEnumerable<Method> Methods { get; }
        public IEnumerable<Method> Functions { get; }
        public SymbolReference? Parent { get; }
        public IEnumerable<Property> Properties { get; }
        public IEnumerable<Field> Fields { get; private set; }
        public IEnumerable<Signal> Signals { get; }
        public IEnumerable<Method> Constructors { get; }

        public Class(Namespace @namespace, string name, string managedName, string ctype, SymbolReference? parent, IEnumerable<SymbolReference> implements, IEnumerable<Method> methods, IEnumerable<Method> functions, Method getTypeFunction, IEnumerable<Property> properties, IEnumerable<Field> fields, IEnumerable<Signal> signals, IEnumerable<Method> constructors, bool isFundamental) : base(@namespace, name, managedName)
        {
            Parent = parent;
            Implements = implements;
            Methods = methods;
            Functions = functions;
            GetTypeFunction = getTypeFunction;
            Properties = properties;
            Fields = fields;
            Signals = signals;
            Constructors = constructors;
            IsFundamental = isFundamental;
            CType = ctype;
        }

        public override IEnumerable<SymbolReference> GetSymbolReferences()
        {
            var symbolReferences = IEnumerables.Concat(
                Implements,
                GetTypeFunction.GetSymbolReferences(),
                Constructors.GetSymbolReferences(),
                Methods.GetSymbolReferences(),
                Functions.GetSymbolReferences(),
                Properties.GetSymbolReferences(),
                Fields.GetSymbolReferences(),
                Signals.GetSymbolReferences()
            );

            if (Parent is { })
                symbolReferences = symbolReferences.Append(Parent);

            return symbolReferences;
        }

        public void ClearFields()
        {
            Fields = Enumerable.Empty<Field>();
        }
    }
}
