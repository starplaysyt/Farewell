using Farewell.Abstractions.Infrastructure;
using Farewell.Debug.Domain.Entities.Company;
using FastEndpoints;

namespace Farewell.Debug.Presentation.ASP.Endpoints.TestingDataEndpoints;

public class TestEndpointRequest
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}

public class TestEndpointResponse
{
    public string Result { get; set; } = string.Empty;
}

public class TestEndpoint : EndpointWithoutRequest<TestEndpointResponse>
{
    public IQueryableRepository<WorkerEntity> Repository { get; set; }
    
    public override void Configure()
    {
        Post("api/test");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Repository.AddAsync(new WorkerEntity()
            {
                Identifier = Guid.NewGuid().ToString(),
                FirstName = "John",
                LastName = "Doe",
            }, ct);
        await Repository.SaveChangesAsync(ct);
        await Send.OkAsync(new TestEndpointResponse() { Result = "OK" }, ct);
    }
}