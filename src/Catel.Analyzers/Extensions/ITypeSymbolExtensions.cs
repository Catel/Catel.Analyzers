﻿namespace Catel.Analyzers
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class ITypeSymbolExtensions
    {
        public static bool InheritsFrom(this ITypeSymbol symbol, ITypeSymbol type, int maxDepth = 8)
        {
            return InheritsFrom(symbol, s => SymbolEqualityComparer.IncludeNullability.Equals(s, type), maxDepth);
        }

        public static bool InheritsFrom(this ITypeSymbol symbol, string shortName, int maxDepth = 8)
        {
            return InheritsFrom(symbol, s => string.Equals(s.Name, shortName), maxDepth);
        }

        public static bool InheritsFrom(this ITypeSymbol symbol, Func<ITypeSymbol, bool> selector, int maxDepth = 8)
        {
            var baseType = symbol.BaseType;
            var depth = 1;

            while (baseType is null == false && depth <= maxDepth)
            {
                if (selector(baseType))
                {
                    return true;
                }

                baseType = baseType.BaseType;
                depth++;
            }

            return false;
        }

        public static ClassDeclarationSyntax? GetClassDeclarationSyntax(this ITypeSymbol typeSymbol, CancellationToken cancellationToken)
        {
            foreach (var linkedSyntax in typeSymbol.DeclaringSyntaxReferences)
            {
                var declaringSyntax = linkedSyntax.GetSyntax(cancellationToken);
                if (declaringSyntax.IsKind(SyntaxKind.ClassDeclaration))
                {
                    return declaringSyntax as ClassDeclarationSyntax;
                }
            }

            return null;
        }
    }
}
