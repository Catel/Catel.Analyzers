namespace Catel.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CTL0001Analyzer : AnalyzerBase
    {
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

            if (!expression.TryGetTarget(KnownSymbols.IDispatcherService.InvokeAsync, context.SemanticModel, context.CancellationToken, out _))
            {
                return;
            }

            // TODO: Optimize performance by different check for async

            // Check if async () is being used
            var firstArgument = expression.ArgumentList.Arguments.FirstOrDefault();
            if (firstArgument is null || !firstArgument.ToString().StartsWith("async"))
            {
                return;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    Descriptors.CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks,
                    expression.GetLocation()));
        }
    }
}
