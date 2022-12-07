namespace Catel.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SemanticModelExtensions
    {
        public static bool TryGetContainingTypeInfo(this SemanticModel semanticModel, SyntaxNode syntaxNode, CancellationToken cancellationToken, [NotNullWhen(true)] out TypeInfo? typeInfo)
        {
            typeInfo = null;

            var containerClassSyntax = syntaxNode.FirstAncestor<ClassDeclarationSyntax>();
            if (containerClassSyntax is null)
            {
                return false;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            var declaredClassSymbol = semanticModel.GetDeclaredSymbol(containerClassSyntax, cancellationToken);
            if (declaredClassSymbol is ITypeSymbol containerClassSymbol)
            {
                typeInfo = new TypeInfo(containerClassSyntax, containerClassSymbol);
                return true;
            }

            return false;
        }
    }
}
