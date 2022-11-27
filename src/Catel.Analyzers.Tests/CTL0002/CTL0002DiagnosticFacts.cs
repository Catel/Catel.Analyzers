namespace Catel.Analyzers.Tests
{
    using Catel.Analyzers;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    public class CTL0002DiagnosticFacts
    {
        private static readonly MethodsAnalyzer Analyzer = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CTL0002_UseRaisePropertyChangedWithNameOf);

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
    using Catel.Data;

    namespace MyWpfApp
    {
        public class MyModel : ObservableObject
        {
            public string FirstName { get; private set; }

            public void ManualRaise()
            {
                RaisePropertyChanged(nameof(FirstName));
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
    using Catel.Data;

    namespace MyWpfApp
    {
        public class MyModel : ObservableObject
        {
            public string FirstName { get; private set; }

            public void ManualRaise()
            {
                RaisePropertyChanged(() => FirstName);
            }
        }
    }";

            Solution.Verify<MethodsAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
        }
    }
}
