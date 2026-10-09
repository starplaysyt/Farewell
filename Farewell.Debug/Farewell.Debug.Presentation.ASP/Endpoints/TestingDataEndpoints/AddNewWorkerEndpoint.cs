using System.Net;
using Farewell.Abstractions.CQRS;
using Farewell.Debug.Application.Commands;
using FastEndpoints;

namespace Farewell.Debug.Presentation.ASP.Endpoints.TestingDataEndpoints;

public class AddNewWorkerRequest
{
    public string Name { get; set; }
    public string Surname { get; set; }
}

public class AddNewWorkerEndpoint : Endpoint<AddNewWorkerRequest>
{
    public Mediator Mediator { get; set; }
    
    public override void Configure()
    {
        Post("/api/workers/add");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddNewWorkerRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.Name) || string.IsNullOrEmpty(req.Surname))
            await Send.ResponseAsync(null, (int)HttpStatusCode.BadRequest, ct);
        
        var result = await Mediator.HandleAsync(new AddNewWorkerCommand(req.Name, req.Surname), ct);
        await Send.ResponseAsync(result.Message, result.Code, ct);
    }
}