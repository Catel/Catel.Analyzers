namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CTL0001Analyzer : IAnalyzer
    {
        public void Handle(SyntaxNodeAnalysisContext context)
        {
            var memberSymbol = context.ContainingSymbol as ISymbol;
            if (memberSymbol is null || memberSymbol.Kind == SymbolKind.Method)
            {
                return;
            }

            //if (context.SemanticModel. == KnownSymbols.IDispatcherService)
            //{

            //}

            //context.SemanticModel.

            // TODO: Look for usage of IDispatcherService and report in if required
            //if (!context.IsExcludedFromAnalysis() &&
            //    context.ContainingSymbol is ISymbol memberSymbol &&
            //    memberSymbol.Kind == SymbolKind.Method)
            //{
            //    context.ReportDiagnostic(
            //        Diagnostic.Create(
            //            Descriptors.CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks,
            //            keyword.GetLocation(),
            //            memberSymbol.ToDisplayString(),
            //            memberSymbol.ContainingType.ToDisplayString()));
            //}
        }
    }
}
