using Rappd.CQRS;

namespace $rootnamespace$;

internal record $safeitemrootname$ : Query<$safeitemrootname$, $argumenttype$, $resulttype$>;
internal record $safeitemrootname$Handler : $safeitemrootname$.Handler
{
    public override async Task<Result<$resulttype$>> HandleAsync(CancellationToken cancellationToken)
    {
        // Do something and return
    }
}
