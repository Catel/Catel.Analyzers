namespace Catel.Analyzers.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using TestHelper;
    using Catel.Analyzers;
    using NUnit.Framework;

    [TestFixture]
    public class CTL0001AnalyzerUnitTests : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [Test]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Test]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.Services;

    namespace MyWpfApp
    {
        public class MyViewModel : ViewModelBase
        {
            private readonly IDispatcherService _dispatcherService;

            public MyViewModel(IDispatcherService dispatcherService)
            {
                _dispatcherService = dispatcherService;
            }

            protected override async Task InitializeAsync()
            {
                await _dispatcherService.InvokeAsync(async () => { });
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "CatelAnalyzers",
                Message = string.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new CatelAnalyzersCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MethodsAnalyzer();
        }
    }
}
