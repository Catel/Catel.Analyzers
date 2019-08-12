namespace Catel.Analyzers.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using Catel.Analyzers;
    using NUnit.Framework;
    using Gu.Roslyn.Asserts;

    [TestFixture]
    public class CTL0001AnalyzerUnitTests
    {
        private static readonly MethodsAnalyzer Analyzer = new MethodsAnalyzer();

        //No diagnostics expected to show up
        [Test]
        public void TestMethod1()
        {
            var before = @"";

            RoslynAssert.Valid(Analyzer, before);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Test]
        public void TestMethod2()
        {
            var before = @"
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

            RoslynAssert.Valid(Analyzer, before);

    //        var fixtest = @"
    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Text;
    //using System.Threading.Tasks;
    //using System.Diagnostics;

    //namespace ConsoleApplication1
    //{
    //    class TYPENAME
    //    {   
    //    }
    //}";
            //VerifyCSharpFix(before, fixtest);
        }
    }
}
