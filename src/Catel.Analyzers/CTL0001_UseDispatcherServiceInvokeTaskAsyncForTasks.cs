namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class CTL0001_UseDispatcherServiceInvokeTaskAsyncForTasks
    {
        public const string DiagnosticId = "CTL0001";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Use InvokeTaskAsync instead when invoking tasks using the IDispatcherService.",
            messageFormat: "Use InvokeTaskAsync instead.",
            category: AnalyzerCategory.MVVM,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Use InvokeTaskAsync instead when invoking tasks using the IDispatcherService.",
            helpLinkUri: HelpLink.ForId(DiagnosticId));
    }
}
