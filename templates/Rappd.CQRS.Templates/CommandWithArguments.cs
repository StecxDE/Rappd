﻿using Rappd.CQRS;

namespace $rootnamespace$;

internal record $safeitemrootname$ : Command<$safeitemrootname$, $argumenttype$>;
internal record $safeitemrootname$Handler : $safeitemrootname$.Handler
{
    public override async Task<Result> HandleAsync(CancellationToken cancellationToken)
    {
        // Do something and return
    }
}
