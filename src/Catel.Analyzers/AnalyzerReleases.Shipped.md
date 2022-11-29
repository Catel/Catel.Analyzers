; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md


## Release 1.5.0

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
CTL0001 | Catel.Analyzers.MVVM | Error | Use InvokeTaskAsync(async () => ...) instead of InvokeAsync(async () => ...) when invoking tasks using the IDispatcherService
CTL0002 | Catel.Analyzers.Core | Warning | Use RaisePropertyChanged(nameof(MyProperty)) instead of RaisePropertyChanged(() => MyProperty)
CTL0008 | Catel.Analyzers.Core | Warning | Use ArgumentNullException.ThrowIfNull for argument check
CTL0011 | Catel.Analyzers.Core | Warning | Provide log on throwing exception
