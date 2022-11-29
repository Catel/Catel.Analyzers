namespace Catel.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ExceptionsAnalyzer : DiagnosticAnalyzerBase
    {
        private static readonly SyntaxKind[] TriggerSyntaxNodes = new[]
        {
            SyntaxKind.ThrowStatement,
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CTL0011_ProvideCatelLogOnThrowingException);

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return TriggerSyntaxNodes;
        }

        protected override SymbolKind[] GetTriggerSymbols()
        {
            return Array.Empty<SymbolKind>();
        }

        protected override OperationKind[] GetTriggerOperations()
        {
            return Array.Empty<OperationKind>();
        }

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            return true;
        }
    }
}
