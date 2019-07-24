namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal interface IAnalyzer
    {
        void Handle(SyntaxNodeAnalysisContext context);
    }
}
