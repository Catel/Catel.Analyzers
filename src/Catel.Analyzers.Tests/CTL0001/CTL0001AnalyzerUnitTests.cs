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

            RoslynAssert.Valid(Analyzer, before);
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

            RoslynAssert.Valid(Analyzer, before);
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

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
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

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
        }
    }
}
