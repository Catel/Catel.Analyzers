namespace Catel.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ArgumentsAnalyzer : DiagnosticAnalyzerBase
    {
        private static readonly OperationKind[] TriggerOperations = new[]
        {
            OperationKind.Invocation
        };

        private static readonly SymbolKind[] TriggerSymbols = new[]
        {
            SymbolKind.Local,
            SymbolKind.Method,
        };

        private static readonly SyntaxKind[] TriggerSyntaxNodes = new[]
        {
            SyntaxKind.InvocationExpression,
            SyntaxKind.ExpressionStatement,
        };

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CL0008_DoUseThrowIfNullForArgumentCheck);

        protected override OperationKind[] GetTriggerOperations()
        {
            return TriggerOperations;
        }

        protected override SymbolKind[] GetTriggerSymbols()
        {
            return TriggerSymbols;
        }

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return TriggerSyntaxNodes;
        }

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            return true;
        }
    }
}
