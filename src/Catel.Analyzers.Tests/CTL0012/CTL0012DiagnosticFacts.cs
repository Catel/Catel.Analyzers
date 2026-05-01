namespace Catel.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    internal class CTL0012DiagnosticFacts
    {
        public class Reports_Diagnostic
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = 
                ExpectedDiagnostic.Create(Descriptors.CTL0012_ConcreteTypesShouldGoFirstInViewModelConstructor.Id,
                    Descriptors.CTL0012_ConcreteTypesShouldGoFirstInViewModelConstructor.MessageFormat.ToString());

            [TestCase]
            public void InvalidCode_InterfaceBeforeConcrete_InViewModelConstructor()
            {
                var before = @"
namespace MyWpfApp
{
    public interface IMyService { }
    public class MyModel { }

    public class MyViewModel : Catel.MVVM.ViewModelBase
    {
        public MyViewModel(IMyService service, MyModel ↓model)
        {
        }
    }
}";

                Solution.Verify<ConstructorsAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Does_Not_Report_Diagnostic
        {
            [TestCase]
            public void ValidCode_ModelBase_InterfaceBeforeConcrete_NotReported()
            {
                var before = @"
namespace MyWpfApp
{
    using Catel.Data;

    public interface IMyService { }
    public class MyModel { }

    public class MyDataModel : Catel.Data.ModelBase
    {
        public MyDataModel(IMyService service, MyModel model)
        {
        }
    }
}";

                Solution.Verify<ConstructorsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0012_ConcreteTypesShouldGoFirstInViewModelConstructor, before));
            }

            [TestCase]
            public void ValidCode_ConcreteBeforeInterface_InViewModelConstructor()
            {
                var before = @"
namespace MyWpfApp
{
    public interface IMyService { }
    public class MyModel { }

    public class MyViewModel : Catel.MVVM.ViewModelBase
    {
        public MyViewModel(MyModel model, IMyService service)
        {
        }
    }
}";

                Solution.Verify<ConstructorsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CTL0012_ConcreteTypesShouldGoFirstInViewModelConstructor, before));
            }
        }
    }
}
