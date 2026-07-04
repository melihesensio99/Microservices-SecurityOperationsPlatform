using Microsoft.AspNetCore.Http;

namespace SecurityPlatform.BuildingBlocks.Results;

public static class HttpResultExtensions
{
    public static IResult ToOkHttpResult<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? global::Microsoft.AspNetCore.Http.Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    public static IResult ToCreatedHttpResult<T>(this Result<T> result, Func<T, string> locationFactory)
    {
        return result.IsSuccess
            ? global::Microsoft.AspNetCore.Http.Results.Created(locationFactory(result.Value!), result.Value)
            : result.Error.ToHttpResult();
    }

    public static IResult ToHttpResult(this Error error)
    {
        return global::Microsoft.AspNetCore.Http.Results.Json(
            new { error = error.Message, code = error.Code },
            statusCode: error.StatusCode);
    }
}
