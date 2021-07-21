using System;

namespace GirLoader.Output.Model
{
    internal class SingleParameterFactory
    {
        private readonly TypeReferenceFactory _typeReferenceFactory;
        private readonly TransferFactory _transferFactory;

        public SingleParameterFactory(TypeReferenceFactory typeReferenceFactory, TransferFactory transferFactory)
        {
            _typeReferenceFactory = typeReferenceFactory;
            _transferFactory = transferFactory;
        }

        public SingleParameter Create(Input.Model.Parameter parameter)
        {
            if (parameter.VarArgs is { })
                throw new VarArgsNotSupportedException("Arguments containing variadic paramters are not supported.");

            Scope callbackScope = parameter.Scope switch
            {
                "async" => Scope.Async,
                "notified" => Scope.Notified,
                _ => Scope.Call,
            };

            if (parameter.Name is null)
                throw new Exception("Argument name is null");

            return new SingleParameter(
                originalName: new SymbolName(parameter.Name),
                symbolName: new SymbolName(new Helper.String(parameter.Name).ToCamelCase().EscapeIdentifier()),
                typeReference: _typeReferenceFactory.Create(parameter),
                direction: DirectionFactory.Create(parameter.Direction),
                transfer: _transferFactory.FromText(parameter.TransferOwnership),
                nullable: parameter.Nullable,
                callerAllocates: parameter.CallerAllocates,
                closureIndex: parameter.Closure == -1 ? null : parameter.Closure,
                destroyIndex: parameter.Destroy == -1 ? null : parameter.Destroy,
                scope: callbackScope
            );
        }

        public class VarArgsNotSupportedException : Exception
        {
            public VarArgsNotSupportedException(string message) : base(message) { }
        }
    }
}
