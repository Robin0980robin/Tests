using Microsoft.AspNetCore.Routing;

namespace Api.Application.Common;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
