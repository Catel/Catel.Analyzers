namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

        public static ArgumentListSyntax ToArgumentList(this List<ArgumentSyntax> list)
        {
            if (list is null)
            {
                return SF.ArgumentList();
            }

            return SF.ArgumentList(SF.SeparatedList(list));
        }

        public static bool IsPrivate(this SyntaxNode syntaxNode)
        {
            var accessModifierNode = syntaxNode.ChildTokens().FirstOrDefault(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword));

            // Check agains "None" kind empty token
            if (accessModifierNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword))
            {
                return true;
            }

            // Skip non-private
            return false;
        }
    }
}
