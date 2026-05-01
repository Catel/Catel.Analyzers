namespace Catel.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    internal class CTL0013DiagnosticFacts
    {
        public class Reports_Diagnostic
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic =
                ExpectedDiagnostic.Create(Descriptors.CTL0013_UseFeaturedViewModelBase.Id,
                    Descriptors.CTL0013_UseFeaturedViewModelBase.MessageFormat.ToString());

            [TestCase]
            public void InvalidCode_ViewModelBase_WithModelAttribute()
            {
                var before = @"
namespace MyWpfApp
{
    using Catel.MVVM;

    public class MyModel { }

    public class ↓MyViewModel : Catel.MVVM.ViewModelBase
    {
        [Model]
        public MyModel MyModel { get; set; }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }

            [TestCase]
            public void InvalidCode_ViewModelBase_WithViewModelToModelAttribute()
            {
                var before = @"
namespace MyWpfApp
{
    using Catel.MVVM;

    public class MyModel { }

    public class ↓MyViewModel : Catel.MVVM.ViewModelBase
    {
        [Model]
        public MyModel MyModel { get; set; }

        [ViewModelToModel(nameof(MyModel))]
        public string FirstName { get; set; }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Does_Not_Report_Diagnostic
        {
            [TestCase]
            public void ValidCode_ViewModelBase_NoAttributes()
            {
                var before = @"
namespace MyWpfApp
{
    public class MyViewModel : Catel.MVVM.ViewModelBase
    {
        public string FirstName { get; set; }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0013_UseFeaturedViewModelBase, before));
            }

            [TestCase]
            public void ValidCode_FeaturedViewModelBase_WithModelAttribute()
            {
                var before = @"
namespace MyWpfApp
{
    using Catel.MVVM;

    public class MyModel { }

    public class MyViewModel : Catel.MVVM.FeaturedViewModelBase
    {
        [Model]
        public MyModel MyModel { get; set; }
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0013_UseFeaturedViewModelBase, before));
            }
        }
    }
}
