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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CL0011_ProvideCatelLogOnThrowingException);

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return new[]
            {
                SyntaxKind.ThrowStatement,
            };
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
