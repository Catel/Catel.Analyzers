namespace Catel.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class MethodsAnalyzer : DiagnosticAnalyzerBase
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptors.CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks);

        protected override bool ShouldHandleOperation(OperationAnalysisContext context)
        {
            return true;
        }

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var memberSymbol = context.ContainingSymbol as ISymbol;
            if (memberSymbol is null || memberSymbol.Kind != SymbolKind.Method)
            {
                return false;
            }

            return true;
        }
    }
}
