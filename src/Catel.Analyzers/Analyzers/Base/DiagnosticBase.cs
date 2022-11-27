namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.CodeAnalysis;
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
