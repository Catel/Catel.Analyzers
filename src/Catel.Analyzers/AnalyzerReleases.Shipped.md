; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 7.0.0

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
CTL0012 | Catel.Analyzers.MVVM | Warning | Concrete types should go first in a view model constructor since they are most likely used for model injection
CTL0013 | Catel.Analyzers.MVVM | Warning | Use FeaturedViewModelBase instead of ViewModelBase

## Release 1.6.0

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
CTL0003 | Catel.Analyzers.Core | Warning | Fix method name to match some property raising NotifyPropertyChanged event


## Release 1.5.0

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
CTL0001 | Catel.Analyzers.MVVM | Error | Use InvokeTaskAsync(async () => ...) instead of InvokeAsync(async () => ...) when invoking tasks using the IDispatcherService
CTL0002 | Catel.Analyzers.Core | Warning | Use RaisePropertyChanged(nameof(MyProperty)) instead of RaisePropertyChanged(() => MyProperty)
CTL0008 | Catel.Analyzers.Core | Warning | Use ArgumentNullException.ThrowIfNull for argument check
CTL0011 | Catel.Analyzers.Core | Warning | Provide log on throwing exception
