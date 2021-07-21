namespace GirLoader.Output.Model
{
    internal class ReturnValueFactory
    {
        private readonly TypeReferenceFactory _typeReferenceFactory;
        private readonly TransferFactory _transferFactory;

        public ReturnValueFactory(TypeReferenceFactory typeReferenceFactory, TransferFactory transferFactory)
        {
            _typeReferenceFactory = typeReferenceFactory;
            _transferFactory = transferFactory;
        }

        public ReturnValue Create(Input.Model.ReturnValue returnValue)
        {
            return new ReturnValue(
                typeReference: _typeReferenceFactory.Create(returnValue),
                transfer: _transferFactory.FromText(returnValue.TransferOwnership),
                nullable: returnValue.Nullable
            );
        }

        public ReturnValue Create(string ctype, Transfer transfer, bool nullable)
        {
            return new ReturnValue(
                typeReference: _typeReferenceFactory.CreateResolveable(ctype, ctype),
                transfer: transfer,
                nullable: nullable
            );
        }
    }
}
