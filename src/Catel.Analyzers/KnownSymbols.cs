namespace Catel.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal static class KnownSymbols
    {
        internal static class Catel_Core
        {
            internal static readonly ObservableObjectType ObservableObject = new();
            internal static readonly LogType Log = new();
        }

        internal static class Catel_MVVM
        {
            internal static readonly DispatcherServiceType IDispatcherService = new();
        }

        internal static class Catel_Fody
        {
            internal static readonly ExposeAttributeType ExposeAttribute = new();
        }
    }

    internal class ObservableObjectType : QualifiedType
    {
        internal readonly QualifiedMethod RaisePropertyChanged_ExpressionBased;

        public ObservableObjectType()
            : base("Catel.Data.ObservableObject")
        {
            RaisePropertyChanged_ExpressionBased = new QualifiedMethod(this, "RaisePropertyChanged");
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

    internal class LogType : QualifiedType
    {
        public LogType()
            : base("Catel.Logging.ILog")
        {

        }
    }

    internal class ExposeAttributeType : QualifiedType
    {
        public ExposeAttributeType() 
            : base("Catel.Fody.ExposeAttribute")
        {
        }
    }
}
