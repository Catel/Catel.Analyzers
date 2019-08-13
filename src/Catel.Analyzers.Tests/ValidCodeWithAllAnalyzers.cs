namespace Catel.Analyzers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class ValidCodeWithAllAnalyzers
    {
        private static readonly IReadOnlyList<DiagnosticAnalyzer> AllAnalyzers = typeof(KnownSymbols)
            .Assembly.GetTypes()
            .Where(x => typeof(DiagnosticAnalyzer).IsAssignableFrom(x) && !x.IsAbstract)
            .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
            .ToArray();

        //private static readonly Solution AnalyzerProjectSolution = CodeFactory.CreateSolution(
        //    new FileInfo(AssemblyDirectoryHelper.Resolve(@"..\..\..\..\src\Catel.Analyzers.Tests\Catel.Analyzers.Tests.csproj")),
        //    AllAnalyzers);

        [Test]
        public void NotEmpty()
        {
            CollectionAssert.IsNotEmpty(AllAnalyzers);
            Assert.Pass($"Count: {AllAnalyzers.Count}");
        }

        //[Explicit, TestCaseSource(nameof(AllAnalyzers))]
        //public void AnalyzerProject(DiagnosticAnalyzer analyzer)
        //{
        //    RoslynAssert.Valid(analyzer, AnalyzerProjectSolution);
        //}
    }
}
