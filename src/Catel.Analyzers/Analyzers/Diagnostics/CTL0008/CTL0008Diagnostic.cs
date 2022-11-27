namespace Catel.Analyzers
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CTL0008Diagnostic : DiagnosticBase
    {
        public const string Id = "CTL0008";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            if (node is not InvocationExpressionSyntax invocationExpression)
            {
                return;
            }

            HandleSyntaxNode(invocationExpression, context, context.CancellationToken);
        }

        private void HandleSyntaxNode(InvocationExpressionSyntax syntaxNode, SyntaxNodeAnalysisContext context, CancellationToken cancellationToken)
        {
            if (syntaxNode.Expression is not MemberAccessExpressionSyntax inner)
            {
                return;
            }

            var isCatelIsNotNull = string.Equals(inner.Name.Identifier.ValueText, "IsNotNull", StringComparison.OrdinalIgnoreCase);
            if (!isCatelIsNotNull)
            {
                return;
            }

            var isCatelArgument = inner.Expression is IdentifierNameSyntax identifierName
                && string.Equals(identifierName.Identifier.ValueText, "Argument", StringComparison.OrdinalIgnoreCase);

            if (!isCatelArgument)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0008_DoUseThrowIfNullForArgumentCheck, syntaxNode.GetLocation()));
        }
    }
}
