namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class Descriptors
    {
        internal static readonly DiagnosticDescriptor CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks = Create(
            id: "CTL0001",
            title: "Use InvokeTaskAsync(async () => ...) instead of InvokeAsync(async () => ...) when invoking tasks using the IDispatcherService",
            messageFormat: "Use InvokeTaskAsync instead",
            category: AnalyzerCategory.MVVM,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Use InvokeTaskAsync instead when invoking tasks using the IDispatcherService.");

        internal static readonly DiagnosticDescriptor CTL0002_UseRaisePropertyChangedWithNameOf = Create(
            id: "CTL0002",
            title: "Use RaisePropertyChanged(nameof(MyProperty)) instead of RaisePropertyChanged(() => MyProperty)",
            messageFormat: "Use RaisePropertyChanged(nameof(PropertyName)) instead",
            category: AnalyzerCategory.Core,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Use RaisePropertyChanged(nameof(MyProperty)) instead of RaisePropertyChanged(() => MyProperty) to improve performance and decrease allocations.");

        internal static readonly DiagnosticDescriptor CTL0003_FixOnPropertyChangedMethodToMatchSomeProperty = Create(
            id: CTL0003Diagnostic.Id,
            title: "Fix method name to match some property raising NotifyPropertyChanged event",
            messageFormat: "Fix method name '{0}' to match some property raising NotifyPropertyChanged event or add '{1}' property",
            category: AnalyzerCategory.Core,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The following method matching Catel.Fody NotifyProperyChanged handler naming convention but class missing corresponding property. This message indicates potential error.");

        internal static readonly DiagnosticDescriptor CTL0008_DoUseThrowIfNullForArgumentCheck = Create(
            id: CTL0008Diagnostic.Id,
            title: "Use ArgumentNullException.ThrowIfNull for argument check",
            messageFormat: "Use ArgumentNullException.ThrowIfNull for argument check",
            category: AnalyzerCategory.Core,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: @"Starting from .NET 6 use new ArgumentNullException.ThrowIfNull method instead of Catel.Argument.");

        internal static readonly DiagnosticDescriptor CTL0011_ProvideCatelLogOnThrowingException = Create(
            id: CTL0011Diagnostic.Id,
            title: "Provide log on throwing exception",
            messageFormat: "Use Log.ErrorAndCreationException for throwing Exception to provide log information",
            category: AnalyzerCategory.Core,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Use Log.ErrorAndCreationException for throwing Exception to provide log information.");

        internal static readonly DiagnosticDescriptor CTL0012_ConcreteTypesShouldGoFirstInViewModelConstructor = Create(
            id: CTL0012Diagnostic.Id,
            title: "Concrete types should go first",
            messageFormat: "Concrete types should go first in a view model constructor since they are most likely used for model injection",
            category: AnalyzerCategory.MVVM,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Concrete types should go first in a view model constructor since they are most likely used for model injection.");

        internal static readonly DiagnosticDescriptor CTL0013_UseFeaturedViewModelBase = Create(
            id: CTL0013Diagnostic.Id,
            title: "Use FeaturedViewModelBase instead of ViewModelBase",
            messageFormat: "Use FeaturedViewModelBase instead of ViewModelBase for view models that use advanced features",
            category: AnalyzerCategory.MVVM,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Use FeaturedViewModelBase instead of ViewModelBase for view models that use advanced features.");

        internal static readonly DiagnosticDescriptor CTL0014_CallStopAsyncOnHost = Create(
            id: CTL0014Diagnostic.Id,
            title: "Call StopAsync on IHost",
            messageFormat: "Call StopAsync on IHost field '{0}' to ensure the host is properly stopped",
            category: AnalyzerCategory.Core,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "When a class contains an IHost field, StopAsync should be called on it to ensure the host is properly stopped and resources are released.");

        /// <summary>
        /// Create a DiagnosticDescriptor, which provides description about a <see cref="T:Microsoft.CodeAnalysis.Diagnostic" />.
        /// NOTE: For localizable <paramref name="title" />, <paramref name="description" /> and/or <paramref name="messageFormat" />,
        /// use constructor overload <see cref="M:Microsoft.CodeAnalysis.DiagnosticDescriptor.#ctor(System.String,Microsoft.CodeAnalysis.LocalizableString,Microsoft.CodeAnalysis.LocalizableString,System.String,Microsoft.CodeAnalysis.DiagnosticSeverity,System.Boolean,Microsoft.CodeAnalysis.LocalizableString,System.String,System.String[])" />.
        /// </summary>
        /// <param name="id">A unique identifier for the diagnostic. For example, code analysis diagnostic ID "CA1001".</param>
        /// <param name="title">A short title describing the diagnostic. For example, for CA1001: "Types that own disposable fields should be disposable".</param>
        /// <param name="messageFormat">A format message string, which can be passed as the first argument to <see cref="M:System.String.Format(System.String,System.Object[])" /> when creating the diagnostic message with this descriptor.
        /// For example, for CA1001: "Implement IDisposable on '{0}' because it creates members of the following IDisposable types: '{1}'.</param>
        /// <param name="category">The category of the diagnostic (like Design, Naming etc.). For example, for CA1001: "Microsoft.Design".</param>
        /// <param name="defaultSeverity">Default severity of the diagnostic.</param>
        /// <param name="isEnabledByDefault">True if the diagnostic is enabled by default.</param>
        /// <param name="description">An optional longer description of the diagnostic.</param>
        /// <param name="customTags">Optional custom tags for the diagnostic. See <see cref="T:Microsoft.CodeAnalysis.WellKnownDiagnosticTags" /> for some well known tags.</param>
        private static DiagnosticDescriptor Create(
          string id,
          string title,
          string messageFormat,
          string category,
          DiagnosticSeverity defaultSeverity,
          bool isEnabledByDefault,
          string description = "",
          params string[] customTags)
        {
            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                category: category,
                defaultSeverity: defaultSeverity,
                isEnabledByDefault: isEnabledByDefault,
                description: description,
                helpLinkUri: HelpLink.ForId(id),
                customTags: customTags);
        }
    }
}
