namespace Catel.Analyzers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class CSharpSyntaxNodeExtensions
    {
        public static string? GetIdentifier(this SyntaxNode syntaxNode)
        {
            if (syntaxNode is null)
            {
                return null;
            }

            var identifierSyntax = syntaxNode.ChildNodes().Where(node => node is IdentifierNameSyntax name).FirstOrDefault() as IdentifierNameSyntax;

            return identifierSyntax?.Identifier.ValueText;
        }
    }
}
