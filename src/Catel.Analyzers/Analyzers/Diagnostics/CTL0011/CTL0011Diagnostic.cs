namespace Catel.Analyzers
{
    using System;
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
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

            var containingClass = syntax.FirstAncestor<ClassDeclarationSyntax>();
            if (containingClass is null)
            {
                return;
            }

            // Don't report diagnostic if LogManager not used for this class
            if (!IsCatelLogStaticFieldPresentsInClass(containingClass, context.SemanticModel, context.CancellationToken))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CTL0011_ProvideCatelLogOnThrowingException, context.Node.GetLocation()));
        }

        public static bool IsCatelLogStaticFieldPresentsInClass(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var fieldDeclaraions = classDeclaration.ChildNodes().Where(x => x.IsKind(SyntaxKind.FieldDeclaration))
                .Cast<FieldDeclarationSyntax>().ToList();

            if (fieldDeclaraions.Any(node =>
            {
                if (semanticModel.TryGetSymbol(node, cancellationToken, out var fieldSymbol))
                {
                    var symbolQualifiedTypeName = fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var isSameType = string.Equals(symbolQualifiedTypeName, KnownSymbols.Catel_Core.Log.FullName, StringComparison.OrdinalIgnoreCase);
                    if (!isSameType)
                    {
                        isSameType = string.Equals(symbolQualifiedTypeName, $"global::{KnownSymbols.Catel_Core.Log.FullName}", StringComparison.OrdinalIgnoreCase);
                    }

                    return isSameType;
                }

                return false;
            }))
            {
                return true;
            }

            return false;
        }
    }
}
