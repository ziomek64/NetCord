﻿using Microsoft.AspNetCore.Routing.Patterns;

namespace NetCord.Hosting.AspNetCore;

internal static class RoutePatternHelper
{
    public static RoutePattern ParseLiteral(string pattern)
    {
        var trimmedPattern = TrimPrefix(pattern);

        var routePattern = RoutePatternFactory.Pattern(pattern, trimmedPattern.Length == 0 ? [] : trimmedPattern.Split('/').Select(p => RoutePatternFactory.Segment(RoutePatternFactory.LiteralPart(p))));
        return routePattern;
    }

    private static string TrimPrefix(string routePattern)
    {
        if (routePattern.StartsWith("~/", StringComparison.Ordinal))
            return routePattern.Substring(2);
        else if (routePattern.StartsWith('/'))
            return routePattern.Substring(1);
        else if (routePattern.StartsWith('~'))
            throw new RoutePatternException(routePattern, "The route template cannot start with a '~' character unless followed by a '/'.");

        return routePattern;
    }
}
