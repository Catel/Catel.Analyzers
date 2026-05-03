namespace Catel.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    internal class CTL0014DiagnosticFacts
    {
        public class Reports_Diagnostic
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic =
                ExpectedDiagnostic.Create(Descriptors.CTL0014_CallStopAsyncOnHost.Id,
                    Descriptors.CTL0014_CallStopAsyncOnHost.MessageFormat.ToString());

            [TestCase]
            public void InvalidCode_IHost_NoStopAsync()
            {
                var before = @"
namespace MyApp
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public partial class App
    {
        private readonly ↓IHost _host;

        public App()
        {
            _host = new HostBuilder().Build();
        }

        protected async void OnStartup()
        {
            await _host.StartAsync();
        }

        protected async void OnExit()
        {
            // StopAsync is not called
        }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }

            [TestCase]
            public void InvalidCode_IHost_OnlyStartAsync_Called()
            {
                var before = @"
namespace MyApp
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public partial class App
    {
        private readonly ↓IHost _host;

        public App()
        {
            _host = new HostBuilder().Build();
        }

        protected async void OnStartup()
        {
            await _host.StartAsync();
        }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Does_Not_Report_Diagnostic
        {
            [TestCase]
            public void ValidCode_IHost_StopAsync_Called()
            {
                var before = @"
namespace MyApp
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public partial class App
    {
        private readonly IHost _host;

        public App()
        {
            _host = new HostBuilder().Build();
        }

        protected async void OnStartup()
        {
            await _host.StartAsync();
        }

        protected async void OnExit()
        {
            await _host.StopAsync();
        }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0014_CallStopAsyncOnHost, before));
            }

            [TestCase]
            public void ValidCode_IHost_StopAsync_CalledWithUsing()
            {
                var before = @"
namespace MyApp
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public partial class App
    {
        private readonly IHost _host;

        public App()
        {
            _host = new HostBuilder().Build();
        }

        protected async void OnStartup()
        {
            await _host.StartAsync();
        }

        protected async void OnExit()
        {
            using (_host)
            {
                await _host.StopAsync();
            }
        }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0014_CallStopAsyncOnHost, before));
            }

            [TestCase]
            public void ValidCode_IHost_ConditionalStopAsync_Called()
            {
                var before = @"
namespace MyApp
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public partial class App
    {
        private readonly IHost _host;

        public App()
        {
            _host = new HostBuilder().Build();
        }

        protected async void OnExit()
        {
            await _host?.StopAsync();
        }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0014_CallStopAsyncOnHost, before));
            }

            [TestCase]
            public void ValidCode_NoIHostField()
            {
                var before = @"
namespace MyApp
{
    public partial class App
    {
        public App()
        {
        }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0014_CallStopAsyncOnHost, before));
            }
        }
    }
}
