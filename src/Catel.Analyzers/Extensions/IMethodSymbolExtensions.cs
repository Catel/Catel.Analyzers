namespace Catel.Analyzers
{
    using System;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class IMethodSymbolExtensions
    {
        /// <summary>
        /// Find callers inside declaring type
        /// </summary>
        /// <param name="methodSymbol"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static bool HasCallersInTypeAsync(this IMethodSymbol methodSymbol, CancellationToken cancellationToken)
        {
            var classDeclaration = methodSymbol.ContainingType.GetClassDeclarationSyntax(cancellationToken);
            if (classDeclaration is null)
            {
                return false;
            }

            InvocationExpressionSyntax? node = null;
            try
            {
                var methodName = methodSymbol.Name;
                node = classDeclaration.DescendantNodes().Cast<InvocationExpressionSyntax>()
                    .FirstOrDefault(x => string.Equals(((MemberAccessExpressionSyntax)x.Expression).Name.ToString(), methodName));
            }
            catch (Exception)
            {
                // Swallow the exception of type cast. 
                // Could be avoided by a better filtering on above linq.
                return false;
            }

            return node is not null;
        }
    }
}
