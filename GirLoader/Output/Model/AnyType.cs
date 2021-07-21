namespace GirLoader.Output.Model
{
    public interface TransferableAnyType : AnyType
    {
        Transfer Transfer { get; }
    }
    
    public interface AnyType
    {
        TypeReference TypeReference { get; }
    }
}
