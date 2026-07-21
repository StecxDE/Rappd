using Microsoft.AspNetCore.Http;
using Rappd.CQRS;

namespace Rappd.Api;

public static class ResponseExtensions
{
    public static IResult ToOkResult(this Response response, IResult unsuccessfulResult)
        => response.IsSuccess ? Results.Ok() : unsuccessfulResult;
    public static IResult ToOkResult<TData>(this Response<TData> response, IResult unsuccessfulResult)
        => response.IsSuccess ? Results.Ok(response.Result) : unsuccessfulResult;
}
