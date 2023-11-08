using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Vital.Models.Responses; 

public class ErrorResponse
{
    public string Type { get; }
    public int Status { get; }
    public string? TraceId { get; }
    public string Error { get; }

    public ErrorResponse(string type, int status, string? traceId, string error)
    {
        Type = type;
        Status = status;
        TraceId = traceId;
        Error = error;
    }

    public ErrorResponse(int status, string error)
    {
        Type = "";
        Status = status;
        TraceId = Activity.Current?.Id;
        Error = error;
    }

    public ErrorResponse(string type, int status, string error)
    {
        Type = type;
        Status = status;
        TraceId = Activity.Current?.Id;
        Error = error;
    }

    public ErrorResponse(ModelStateDictionary modelState, int status)
    {
        Type = "";
        Status = status;
        TraceId = Activity.Current?.Id;
        // Select first error from first property
        Error = modelState?.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage ?? "";
    }
}
