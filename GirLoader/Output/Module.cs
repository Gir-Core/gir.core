using StrongInject;

namespace GirLoader.Output
{
    [Register(typeof(Loader))]
    [Register(typeof(RepositoryLoader))]
    [Register(typeof(RepositoryResolver))]
    [Register(typeof(Model.NamespaceFactory))]
    [Register(typeof(Model.TypeReferenceFactory))]
    [Register(typeof(Model.IncludeFactory))]
    [Register(typeof(Model.ClassFactory))]
    [Register(typeof(Model.AliasFactory))]
    [Register(typeof(Model.ReturnValueFactory))]
    [Register(typeof(Model.SingleParameterFactory))]
    [Register(typeof(Model.InstanceParameterFactory))]
    [Register(typeof(Model.ParameterListFactory))]
    [Register(typeof(Model.CallbackFactory))]
    [Register(typeof(Model.EnumerationFactory))]
    [Register(typeof(Model.InterfaceFactory))]
    [Register(typeof(Model.RecordFactory))]
    [Register(typeof(Model.MethodFactory))]
    [Register(typeof(Model.TransferFactory))]
    [Register(typeof(Model.MemberFactory))]
    [Register(typeof(Model.PropertyFactory))]
    [Register(typeof(Model.FieldFactory))]
    [Register(typeof(Model.SignalFactory))]
    [Register(typeof(Model.ConstantFactory))]
    [Register(typeof(Model.UnionFactory))]
    [Register(typeof(Model.RepositoryFactory))]
    internal class Module { }
}
