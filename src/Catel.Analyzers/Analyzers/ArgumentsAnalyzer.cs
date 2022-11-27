namespace Catel.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ArgumentsAnalyzer : DiagnosticAnalyzerBase
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CL0008_DoUseThrowIfNullForArgumentCheck);

        protected override OperationKind[] GetTriggerOperations()
        {
            return new[]
             {
                OperationKind.Invocation,
            };
        }

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return new[]
            {
                SyntaxKind.InvocationExpression,
                SyntaxKind.ExpressionStatement,
            };
        }

        protected override SymbolKind[] GetTriggerSymbols()
        {
            return new[]
            {
                SymbolKind.Local,
                SymbolKind.Method,
            };
        }

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            return true;
        }
    }
}
