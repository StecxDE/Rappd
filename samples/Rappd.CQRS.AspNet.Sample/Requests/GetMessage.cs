namespace Rappd.CQRS.AspNet.Sample.Requests;

public record GetMessage : Query<GetMessage, string>;
public record GetMessageHandler(IHttpContextAccessor ContextAccessor) : GetMessage.Handler
{
    public override async Task<Result<string>> HandleAsync(CancellationToken cancellationToken)
    {
        if (ContextAccessor.HttpContext?.Session is ISession session)
        {
            await session.LoadAsync();
            return session.GetString("Message") ?? string.Empty;
        }
        else
            return Results.Error;
    }
}