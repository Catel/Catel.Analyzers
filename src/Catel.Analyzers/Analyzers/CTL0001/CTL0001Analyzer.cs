namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CTL0001Analyzer : AnalyzerBase
    {
        internal const string DispatcherServiceTypeName = "Catel.MVVM.IDispatcherService";
        internal const string InvokeAsyncMethodName = "InvokeAsync";

        public override void HandleOperation(OperationAnalysisContext context)
        {
            var operation = context.Operation;
            if (operation is null)
            {
                return;
            }

            foreach (var childOperation in operation.Children)
            {
                var semanticModel = childOperation.SemanticModel;
            }
        }

        //public void Handle(SyntaxNodeAnalysisContext context)
        //{
        //    var methodSymbol = context.ContainingSymbol as IMethodSymbol;
        //    if (methodSymbol is null)
        //    {
        //        return;
        //    }

        //    var dispatcherServiceType = context.Compilation.GetTypeByMetadataName(DispatcherServiceTypeName);

        //    methodSymbol.

        //    //RegularBodyMethodSymbol

        //    //foreach (var body in memberSymbol.Bodies)



        //    //if (context.SemanticModel. == KnownSymbols.IDispatcherService)
        //    //{

        //    //}

        //    //context.SemanticModel.

        //    // TODO: Look for usage of IDispatcherService and report in if required
        //    //if (!context.IsExcludedFromAnalysis() &&
        //    //    context.ContainingSymbol is ISymbol memberSymbol &&
        //    //    memberSymbol.Kind == SymbolKind.Method)
        //    //{
        //    //    context.ReportDiagnostic(
        //    //        Diagnostic.Create(
        //    //            Descriptors.CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks,
        //    //            keyword.GetLocation(),
        //    //            memberSymbol.ToDisplayString(),
        //    //            memberSymbol.ContainingType.ToDisplayString()));
        //    //}
        //}
    }
}
