namespace Catel.Analyzers.Tests
{
    using Catel.Analyzers;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    public class CTL0001DiagnosticFacts
    {
        private static readonly MethodsAnalyzer Analyzer = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks);

        [Test]
        public void Valid_NoCode()
        {
            var before = @"";

            RoslynAssert.Valid(Analyzer, before);
        }

        [Test]
        public void Valid_Code_01()
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
                await _dispatcherService.InvokeTaskAsync(async () => { });
            }
        }
    }";

            Solution.Verify<MethodsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks, before));
        }

        [Test]
        public void Valid_Code_02()
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
                await _dispatcherService.InvokeAsync(() => { });
            }
        }
    }";

            Solution.Verify<MethodsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks, before));
        }

        [Test]
        public void Invalid_Code_01()
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
                await ↓_dispatcherService.InvokeAsync(async () => { });
            }
        }
    }";

            Solution.Verify<MethodsAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
        }

        [Test]
        public void Invalid_Code_02()
        {
            var before = @"
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Catel.Services;

    namespace MyWpfApp
    {
        public class MyViewModel : ViewModelBase
        {
            public IDispatcherService DispatcherService { get; private set; }

            public MyViewModel(IDispatcherService dispatcherService)
            {
                DispatcherService = dispatcherService;
            }

            protected async Task MyMethod(object project)
            {
                Argument.IsNotNull(() => project);

                await ↓DispatcherService.InvokeAsync(async () =>
                {
                    // some code here
                });
            }
        }
    }";

            Solution.Verify<MethodsAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
        }
    }
}
