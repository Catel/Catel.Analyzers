namespace Catel.Analyzers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CTL0011Diagnostic : DiagnosticBase
    {
        public const string Id = "CTL0011";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ThrowStatementSyntax syntax)
            {
                return;
            }

            // Followed by object creation pattern
            var objectCreationSyntax = syntax.ChildNodes().FirstOrDefault(s => s.IsKind(SyntaxKind.ObjectCreationExpression));
            if (objectCreationSyntax is null)
            {
                return;
            }

            var objectCreationArgumentsSyntax = objectCreationSyntax.ChildNodes().FirstOrDefault(s => s.IsKind(SyntaxKind.ArgumentList));
            if (objectCreationArgumentsSyntax is null || (objectCreationArgumentsSyntax is ArgumentListSyntax argumentList && argumentList.Arguments.Count == 0))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0011_ProvideCatelLogOnThrowingException, context.Node.GetLocation()));
        }
    }
}
