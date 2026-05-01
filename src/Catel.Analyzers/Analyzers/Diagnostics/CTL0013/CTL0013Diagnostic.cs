namespace Catel.Analyzers
{
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CTL0013Diagnostic : DiagnosticBase
    {
        public const string Id = "CTL0013";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ClassDeclarationSyntax classDeclaration)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (!context.SemanticModel.TryGetSymbol(classDeclaration, context.CancellationToken, out INamedTypeSymbol? classSymbol))
            {
                return;
            }

            // Must derive from ViewModelBase
            if (!classSymbol.InheritsFrom(KnownSymbols.Catel_MVVM.ViewModelBase.Type))
            {
                return;
            }

            // Skip if already deriving from FeaturedViewModelBase
            if (classSymbol.InheritsFrom(KnownSymbols.Catel_MVVM.FeaturedViewModelBase.Type))
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Check all members (properties/fields) for ModelAttribute or ViewModelToModelAttribute
            var hasAdvancedAttribute = classSymbol.GetMembers()
                .SelectMany(m => m.GetAttributes())
                .Any(attr =>
                {
                    var attrClass = attr.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    return string.Equals(attrClass, $"global::{KnownSymbols.Catel_MVVM.ModelAttribute.FullName}") ||
                           string.Equals(attrClass, $"global::{KnownSymbols.Catel_MVVM.ViewModelToModelAttribute.FullName}");
                });

            if (!hasAdvancedAttribute)
            {
                return;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    Descriptors.CTL0013_UseFeaturedViewModelBase,
                    classDeclaration.Identifier.GetLocation()));
        }
    }
}
