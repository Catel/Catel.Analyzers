namespace Catel.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal static class KnownSymbols
    {
        internal static readonly DispatcherServiceType IDispatcherService = new DispatcherServiceType();

        private static QualifiedType Create(string qualifiedName, string alias = null)
        {
            return new QualifiedType(qualifiedName, alias);
        }
    }

    internal class DispatcherServiceType : QualifiedType
    {
        internal readonly QualifiedMethod InvokeAsync;

        public DispatcherServiceType()
            : base("Catel.Services.IDispatcherService")
        {
            InvokeAsync = new QualifiedMethod(this, "InvokeAsync");
        }
    }
}
