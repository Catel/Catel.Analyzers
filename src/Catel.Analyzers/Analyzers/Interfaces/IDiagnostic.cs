namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis.Diagnostics;

    internal interface IDiagnostic
    {
        void HandleOperation(OperationAnalysisContext context);

        void HandleSymbol(SymbolAnalysisContext context);

        void HandleSyntaxNode(SyntaxNodeAnalysisContext context);
    }
}
