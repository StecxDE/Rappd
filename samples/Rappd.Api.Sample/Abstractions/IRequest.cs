using Rappd.Data;

namespace Rappd.Api.Sample.Abstractions;

[BaseInterface(nameof(Type))]
public interface IRequest
{
    string Type { get; }
}

[SubInterface<IRequest>(TYPE)]
public interface IHelloRequest : IRequest
{   
    const string TYPE = "hello";
    string Hello { get; }
}

[SubInterface<IRequest>(TYPE)]
public interface IWorldRequest : IRequest
{
    const string TYPE = "world";
    string World { get; }
}