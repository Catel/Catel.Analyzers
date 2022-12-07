namespace Catel.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CTL0001Diagnostic : DiagnosticBase
    {
        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var methodSymbol = context.ContainingSymbol as IMethodSymbol;
            if (methodSymbol is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var expression = context.Node as InvocationExpressionSyntax;
            if (expression is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var validTarget = expression.ArgumentList is { }
                && expression.TryGetMethodName(out var name) && name == KnownSymbols.Catel_MVVM.IDispatcherService.InvokeAsync.Name;

            if (!validTarget || expression.ArgumentList is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
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
