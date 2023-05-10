namespace Rappd.CQRS.AspNet.Sample.Requests;

public record SetMessage : Command<SetMessage, string>;
public record SetMessageHandler(IHttpContextAccessor ContextAccessor) : SetMessage.Handler
{
    public override async Task<Result> HandleAsync(CancellationToken cancellationToken)
    {
        if (ContextAccessor.HttpContext?.Session is ISession session)
        {
            await session.LoadAsync();
            session.SetString("Message", Arguments);
            await session.CommitAsync();
            return Results.Ok;
        }
        else
            return Results.Error;
    }
}