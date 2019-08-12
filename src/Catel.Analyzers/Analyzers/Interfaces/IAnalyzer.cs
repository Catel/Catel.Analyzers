namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis.Diagnostics;

    internal interface IAnalyzer
    {
        void HandleOperation(OperationAnalysisContext context);

        void HandleSymbol(SymbolAnalysisContext context);

        void HandleSyntaxNode(SyntaxNodeAnalysisContext context);
    }
}
