namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.CodeAnalysis.Diagnostics;

    public abstract class AnalyzerBase : IAnalyzer
    {
        public virtual void HandleOperation(OperationAnalysisContext context)
        {
            //throw new NotImplementedException();
        }

        public virtual void HandleSymbol(SymbolAnalysisContext context)
        {
            //throw new NotImplementedException();
        }

        public virtual void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
