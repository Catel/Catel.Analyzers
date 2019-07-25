﻿namespace Catel.Analyzers.Tests
{
    using System;
    using System.Collections.Generic;
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

        private static readonly Solution AnalyzerProjectSolution = CodeFactory.CreateSolution(
            ProjectFile.Find("Catel.Analyzers.csproj"),
            AllAnalyzers,
            RoslynAssert.MetadataReferences);

        //private static readonly Solution ValidCodeProjectSln = CodeFactory.CreateSolution(
        //    ProjectFile.Find("ValidCode.csproj"),
        //    AllAnalyzers,
        //    RoslynAssert.MetadataReferences);

        //[SetUp]
        //public void Setup()
        //{
        //    // The cache will be enabled when running in VS.
        //    // It speeds up the tests and makes them more realistic
        //    Cache<SyntaxTree, SemanticModel>.Begin();
        //}

        //[TearDown]
        //public void TearDown()
        //{
        //    Cache<SyntaxTree, SemanticModel>.End();
        //}

        [Test]
        public void NotEmpty()
        {
            CollectionAssert.IsNotEmpty(AllAnalyzers);
            Assert.Pass($"Count: {AllAnalyzers.Count}");
        }

        //[TestCaseSource(nameof(AllAnalyzers))]
        //public void ValidCodeProject(DiagnosticAnalyzer analyzer)
        //{
        //    RoslynAssert.Valid(analyzer, ValidCodeProjectSln);
        //}

        [TestCaseSource(nameof(AllAnalyzers))]
        public void AnalyzerProject(DiagnosticAnalyzer analyzer)
        {
            RoslynAssert.Valid(analyzer, AnalyzerProjectSolution);
        }
    }
}