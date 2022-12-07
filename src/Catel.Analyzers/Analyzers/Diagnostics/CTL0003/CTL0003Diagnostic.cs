namespace Catel.Analyzers
{
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CTL0003Diagnostic : DiagnosticBase
    {
        private const string CatelBaseClassLookupName = "ObservableObject";

        public const string Id = "CTL0003";

        public override void HandleSymbol(SymbolAnalysisContext context)
        {
            if (context.Symbol is not IMethodSymbol methodSymbol)
            {
                return;
            }

            if (!methodSymbol.Locations.Any())
            {
                return;
            }

            if (methodSymbol.DeclaredAccessibility != Accessibility.Private)
            {
                return;
            }

            if (!MethodConvention.TryMatchToConvention(methodSymbol, "On", "Changed", out var abstractName))
            {
                return;
            }

            var methodOwner = methodSymbol.ContainingType;
            if (!methodOwner.InheritsFrom(CatelBaseClassLookupName))
            {
                return;
            }

            if (methodSymbol.HasCallersInTypeAsync(context.CancellationToken))
            {
                return;
            }

            // If after all check method is still potential INotifyPropertyChanged handler
            // We should check 
            // 1. Corresponding property declared explicitly
            // 2. Corresponding property [Exposed] from Model
            if (methodOwner.TryFindProperty(abstractName, out _))
            {
                return;
            }

            // Check if it's possible to expose type
            var exposeAttributeType = context.Compilation.GetTypeByMetadataName(KnownSymbols.Catel_Fody.ExposeAttribute.FullName);
            if (exposeAttributeType is null)
            {
                return;
            }

            var containerClassSyntax = methodOwner.GetClassDeclarationSyntax(context.CancellationToken);
            if (containerClassSyntax is null)
            {
                return;
            }

            var exposedMarkedDeclarations = from descendantNode in containerClassSyntax.DescendantNodes(
                                                    x => x.IsKind(SyntaxKind.AttributeList)
                                                    || x.IsKind(SyntaxKind.PropertyDeclaration)
                                                    || x.IsKind(SyntaxKind.ClassDeclaration))
                                            where descendantNode is AttributeSyntax && IsSameAttribute(descendantNode)
                                            select descendantNode.FirstAncestor<PropertyDeclarationSyntax>();

            var semanticModel = context.Compilation.GetSemanticModel(containerClassSyntax.SyntaxTree);
            foreach (var property in exposedMarkedDeclarations)
            {
                if (FodySyntax.IsPropertyExposedWithFodyAttribute(property, exposeAttributeType, abstractName, semanticModel, context.CancellationToken))
                {
                    return;
                }
            }

            var declarationLocation = methodSymbol.Locations[0];

            var diagnostic = Diagnostic.Create(Descriptors.CTL0003_FixOnPropertyChangedMethodToMatchSomeProperty, declarationLocation, methodSymbol.Name, abstractName);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsSameAttribute(SyntaxNode syntaxNode)
        {
            var identifier = syntaxNode.GetIdentifier();
            if (string.IsNullOrEmpty(identifier))
            {
                return false;
            }

            return KnownSymbols.Catel_Fody.ExposeAttribute.FullName.EndsWith($"{identifier}Attribute");
        }
    }
}
