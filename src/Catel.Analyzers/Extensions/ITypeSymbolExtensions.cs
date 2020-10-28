namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class ITypeSymbolExtensions
    {
        public static bool InheritsFrom(this ITypeSymbol symbol, ITypeSymbol type, int maxDepth = 8)
        {
            var baseType = symbol.BaseType;
            var depth = 1;

            while (baseType != null && depth <= maxDepth)
            {
                if (SymbolEqualityComparer.IncludeNullability.Equals(baseType, type))
                {
                    return true;
                }

                baseType = baseType.BaseType;
                depth++;
            }

            return false;
        }

        public static bool InheritsFrom(this ITypeSymbol symbol, string shortName, int maxDepth = 8)
        {
            var baseType = symbol.BaseType;
            var depth = 1;

            while (baseType != null && depth <= maxDepth)
            {
                if (string.Equals(baseType.Name, shortName))
                {
                    return true;
                }

                baseType = baseType.BaseType;
                depth++;
            }

            return false;
        }
    }
}
