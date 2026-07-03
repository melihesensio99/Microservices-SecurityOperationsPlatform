using SecurityCore.Api.Features.Incidents.AddNote;
using SecurityCore.Api.Features.Incidents.Create;
using SecurityCore.Api.Features.Incidents.GetById;
using SecurityCore.Api.Features.Incidents.GetList;
using SecurityCore.Api.Features.Incidents.GetNotes;

namespace SecurityCore.Api.Features.Incidents;

public static class IncidentEndpoints
{
    public static IEndpointRouteBuilder MapIncidentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/incidents")
            .WithTags("Incidents");

        group
            .MapCreateIncidentEndpoint()
            .MapAddIncidentNoteEndpoint()
            .MapGetIncidentsEndpoint()
            .MapGetIncidentByIdEndpoint()
            .MapGetIncidentNotesEndpoint();

        return app;
    }
}
