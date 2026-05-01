namespace Catel.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

#pragma warning disable RS1038 // Compiler extensions should be implemented in assemblies with compiler-provided references
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning restore RS1038 // Compiler extensions should be implemented in assemblies with compiler-provided references
    internal class ConstructorsAnalyzer : DiagnosticAnalyzerBase
    {
        private static readonly SyntaxKind[] TriggerSyntaxNodes = new[]
        {
            SyntaxKind.ConstructorDeclaration,
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CTL0012_ConcreteTypesShouldGoFirstInViewModelConstructor);

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return TriggerSyntaxNodes;
        }

        protected override SymbolKind[] GetTriggerSymbols()
        {
            return Array.Empty<SymbolKind>();
        }

        protected override OperationKind[] GetTriggerOperations()
        {
            return Array.Empty<OperationKind>();
        }

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            return true;
        }
    }
}
