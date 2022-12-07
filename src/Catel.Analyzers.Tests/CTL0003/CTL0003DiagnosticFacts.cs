namespace Catel.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    internal class CTL0003DiagnosticFacts
    {
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CTL0003_FixOnPropertyChangedMethodToMatchSomeProperty);
        public class Reports_Diagnostic
        {
            [TestCase]
            public void Invalid_Code_01()
            {
                var before = @"
namespace TestApp1
{
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;

    public class DummyTestViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly ITypeFactory _typeFactory;
        private readonly IMessageService _messageService;
        private readonly string _initialSelectedConditionName;

        private readonly ObservableCollection<IDummaryConditions> _conditions;

        public DummyTestViewModel(string name, IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory, IMessageService messageService)
        {
            ArgumentNullException.ThrowIfNull(uiVisualizerService);
            ArgumentNullException.ThrowIfNull(typeFactory);
            ArgumentNullException.ThrowIfNull(messageService);

            _initialSelectedConditionName = name;
            _uiVisualizerService = uiVisualizerService;
            _typeFactory = typeFactory;
            _messageService = messageService;

            _conditions = new ObservableCollection<IDummaryConditions>();

            CreateConditionAsync = new TaskCommand(OnCreateConditionAsync);
        }

        public string ConditionFilter { get; set; }
        public IDummaryConditions SelectedCondition { get; set; }
        public CollectionViewSource ConditionsViewSource { get; set; }

        public TaskCommand CreateConditionAsync { get; }

        private async Task OnCreateConditionAsync()
        {
            var viewModel = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<DialogViewModel>();

            if ((await _uiVisualizerService.ShowDialogAsync(viewModel)).DialogResult == true)
            {
                _conditions.Add(condition);

                SelectedCondition = condition;
            }
        }

        protected override Task InitializeAsync()
        {
            ConditionsViewSource = new CollectionViewSource
            {
                Source = _conditions,
                View =
                {
                    Filter = FilterCondition
                }
            };

            if (!string.IsNullOrWhiteSpace(_initialSelectedConditionName))
            {
                SelectedCondition = _conditions.FirstOrDefault(x => Equals(x.Name, _initialSelectedConditionName));
            }

            return base.InitializeAsync();
        }

        protected override async Task CloseAsync()
        {
            await base.CloseAsync();
        }

        ↓private void OnConditionFilterChanged()
        {
            ConditionsViewSource?.View?.Refresh();
        }

        private bool FilterCondition(object obj)
        {
            if (obj is not IDummaryConditions condition)
            {
                return false;
            }

            var conditionFilter = ConditionFilter;
            if (string.IsNullOrWhiteSpace(conditionFilter))
            {
                return true;
            }

            return condition.Name.IndexOfIgnoreCase(conditionFilter) >= 0;
        }
    }
}";

                Solution.Verify<MethodsAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }
    }
}
