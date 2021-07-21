namespace GirLoader.Output.Model
{
    public interface Parameter : TransferableAnyType
    {
        SymbolName Name { get; }
        Direction Direction { get; }
        bool Nullable { get; }
        bool CallerAllocates { get; }
    }
}
