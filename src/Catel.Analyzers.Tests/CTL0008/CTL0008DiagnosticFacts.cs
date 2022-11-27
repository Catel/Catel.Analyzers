namespace Catel.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    internal class CTL0008DiagnosticFacts
    {
        public class Reports_Diagnostic
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0008_DoUseThrowIfNullForArgumentCheck);

            [TestCase]
            public void InvalidCode_ArgumentCheck_In_Ctor()
            {
                var before = @"
namespace ConsoleApp1
{
    using Catel;

    internal class Program
    {
        public Program(object arg)
        {
            ↓Argument.IsNotNull(() => arg);
        }
    }
}";

                Solution.Verify<ArgumentsAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }

            [TestCase]
            public void InvalidCode_ArgumentCheck_In_Method()
            {
                var before = @"
namespace ConsoleApp1
{
    using Catel;

    internal class Program
    {
        private static void Main(string[] args)
        {
            ↓Argument.IsNotNull(() => args);

            if (args.Length > 0)
            {
                Console.WriteLine(""Hello, World!"");
            }
        }
    }
}";

                Solution.Verify<ArgumentsAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Reports_NoDiagnostic
        {
            [TestCase]
            public void ValidCode_ArgumentNullException_ThrowIfNull()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {
        public Program(object arg)
        {
            ArgumentNullException.ThrowIfNull(arg);
        }
    }
}";
                Solution.Verify<ArgumentsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0008_DoUseThrowIfNullForArgumentCheck, before));
            }

            [TestCase]
            public void ValidCode_NoArgumentCheck()
            {
                var before = @"
namespace ConsoleApp1
{
    using Catel;

    internal class Program
    {
        public Program(object arg)
        {

        }
    }
}";
                Solution.Verify<ArgumentsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0008_DoUseThrowIfNullForArgumentCheck, before));
            }
        }
    }
}
