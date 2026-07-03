using MediatR;

namespace SecurityPlatform.BuildingBlocks.Cqrs;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
