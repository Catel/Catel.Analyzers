namespace Catel.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CTL0001Analyzer : AnalyzerBase
    {
        internal const string DispatcherServiceTypeName = "Catel.MVVM.IDispatcherService";
        internal const string InvokeAsyncMethodName = "InvokeAsync";

        //public override void HandleOperation(OperationAnalysisContext context)
        //{
        //    var operation = context.Operation;
        //    if (operation is null)
        //    {
        //        return;
        //    }

        //    var expression = 

        //    foreach (var childOperation in operation.Children)
        //    {
        //        var semanticModel = childOperation.SemanticModel;
        //    }
        //}

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var methodSymbol = context.ContainingSymbol as IMethodSymbol;
            if (methodSymbol is null)
            {
                return;
            }

            var expression = context.Node as InvocationExpressionSyntax;
            if (expression is null)
            {
                return;
            }


            var typeInfo = context.SemanticModel.GetTypeInfo(expression);


            //if (!expression.TryGetTarget(KnownSymbols.IDispatcherService.InvokeAsync, context.SemanticModel, context.CancellationToken, out _))
            //{
            //    return;
            //}
        }

        //public void Handle(SyntaxNodeAnalysisContext context)
        //{


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
