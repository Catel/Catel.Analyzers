namespace Catel.Analyzers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal static class MethodConvention
    {
        public static bool TryMatchToConvention(IMethodSymbol symbol, string prefix, string suffix, [NotNullWhen(true)] out string? originalName)
        {
            if (!symbol.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !symbol.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                originalName = null;
                return false;
            }

            originalName = symbol.Name.ReplaceFirst(prefix, string.Empty).ReplaceLast(suffix, string.Empty);
            return true;
        }
    }
}
