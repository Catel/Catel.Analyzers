namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal abstract class AnalyzerBase : DiagnosticAnalyzer
    {
        private readonly Dictionary<string, IAnalyzer> _analyzers = new Dictionary<string, IAnalyzer>();

        public AnalyzerBase()
        {

        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            foreach (var diagnosticDescriptor in SupportedDiagnostics)
            {
                _analyzers[diagnosticDescriptor.Id] = ResolveAnalyzer(diagnosticDescriptor);
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(
                c => Handle(c),
                SyntaxKind.FieldDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.ClassDeclaration);
        }

        protected abstract bool ShouldHandle(SyntaxNodeAnalysisContext context);

        protected virtual IAnalyzer ResolveAnalyzer(DiagnosticDescriptor descriptor)
        {
            var typeName = $"Catel.Analyzers.{descriptor.Id}Analyzer";
            var type = Type.GetType(typeName);

            return (IAnalyzer)Activator.CreateInstance(type);
        }

        private void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            if (!ShouldHandle(context))
            {
                return;
            }

            foreach (var analyzer in _analyzers)
            {
                analyzer.Value.Handle(context);
            }
        }
    }
}
