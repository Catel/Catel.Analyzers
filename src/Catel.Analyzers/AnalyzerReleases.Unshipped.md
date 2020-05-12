; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
CTL0001 | Catel.Analyzers.MVVM | Error | Use InvokeTaskAsync(async () => ...) instead of InvokeAsync(async () => ...) when invoking tasks using the IDispatcherService