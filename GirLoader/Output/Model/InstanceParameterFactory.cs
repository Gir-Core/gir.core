using System;
using GirLoader.Input.Model;

namespace GirLoader.Output.Model
{
    internal class InstanceParameterFactory
    {
        private readonly TypeReferenceFactory _typeReferenceFactory;
        private readonly TransferFactory _transferFactory;

        public InstanceParameterFactory(TypeReferenceFactory typeReferenceFactory, TransferFactory transferFactory)
        {
            _typeReferenceFactory = typeReferenceFactory;
            _transferFactory = transferFactory;
        }

        public InstanceParameter Create(InstanceParameterInfo parameterInfo)
        {
            if (parameterInfo.Name is null)
                throw new Exception("Argument name is null");

            return new InstanceParameter(
                originalName: new SymbolName(parameterInfo.Name),
                symbolName: new SymbolName(new Helper.String(parameterInfo.Name).ToCamelCase().EscapeIdentifier()),
                typeReference: _typeReferenceFactory.Create(parameterInfo),
                direction: DirectionFactory.Create(parameterInfo.Direction),
                transfer: _transferFactory.FromText(parameterInfo.TransferOwnership),
                nullable: parameterInfo.Nullable,
                callerAllocates: parameterInfo.CallerAllocates
            );
        }
    }
}
