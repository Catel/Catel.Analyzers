namespace Catel.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    internal class CTL0008CodeFixProviderFacts
    {
        public class CanFix
        {
            private static readonly CodeFixProvider Fixer = new CTL0008CodeFixProvider();

            [TestCase]
            public void InvalidCode_Default()
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
                var after = @"
namespace ConsoleApp1
{
    using Catel;

    internal class Program
    {
        public Program(object arg)
        {
            ArgumentNullException.ThrowIfNull(arg);
        }
    }
}";

                Solution.Verify<ArgumentsAnalyzer>(analyzer => RoslynAssert.CodeFix(analyzer, Fixer, before, after));
            }

            [TestCase]
            public void InvalidCode_CS0103()
            {
                var before = @"
namespace ConsoleApp1
{
    internal class Program
    {
        public Program(object arg)
        {
            ↓ArgumentNullException.ThrowIfNull(arg);
        }
    }
}";
                var after = @"
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

                Solution.Verify<ArgumentsAnalyzer>(analyzer => RoslynAssert.CodeFix(Fixer, ExpectedDiagnostic.Create("CS0103"), before, after));
            }
        }
    }
}
