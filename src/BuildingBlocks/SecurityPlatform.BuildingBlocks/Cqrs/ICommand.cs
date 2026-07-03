using MediatR;

namespace SecurityPlatform.BuildingBlocks.Cqrs;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand
{
}
