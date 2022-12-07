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
            ImmutableArray.Create(Descriptors.CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks,
                                  Descriptors.CTL0002_UseRaisePropertyChangedWithNameOf,
                                  Descriptors.CTL0003_FixOnPropertyChangedMethodToMatchSomeProperty);

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            return context.ContainingSymbol is not ISymbol memberSymbol || memberSymbol.Kind != SymbolKind.Method;
        }

        protected override bool ShouldHandleSymbol(SymbolAnalysisContext context)
        {
            return context.Symbol.Kind == SymbolKind.Method;
        }
    }
}
