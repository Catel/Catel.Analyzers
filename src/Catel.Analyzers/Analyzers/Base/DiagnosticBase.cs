namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis.Diagnostics;

    public abstract class DiagnosticBase : IDiagnostic
    {
        public virtual void HandleOperation(OperationAnalysisContext context)
        {
        }

        public virtual void HandleSymbol(SymbolAnalysisContext context)
        {
        }

        public virtual void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
        }
    }
}
