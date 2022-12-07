namespace Catel.Analyzers
{
    using System;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CTL0003Diagnostic : DiagnosticBase
    {
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

            if (methodSymbol.HasCallersInTypeAsync(context.CancellationToken))
            {
                return;
            }

            // If after all check method is still potential INotifyPropertyChanged handler
            // We should check 
            // 1. Corresponding property declared explicitly
            // 2. Corresponding property [Exposed] from Model
            var classType = methodSymbol.ContainingType;
            if (classType.TryFindProperty(abstractName, out _))
            {
                return;
            }

            // Check if it's possible to expose type
            var exposeAttributeType = context.Compilation.GetTypeByMetadataName(KnownSymbols.Catel_Fody.ExposeAttribute.FullName);
            if (exposeAttributeType is null)
            {
                return;
            }

            var containerClassSyntax = classType.GetClassDeclarationSyntax(context.CancellationToken);
            if (containerClassSyntax is null)
            {
                return;
            }

            var exposedMarkedDeclarations = from descendantNode in containerClassSyntax.DescendantNodes(x => x is ClassDeclarationSyntax || x is PropertyDeclarationSyntax || x is AttributeListSyntax)
                                            where descendantNode is AttributeSyntax && string.Equals(descendantNode.GetIdentifier(), KnownSymbols.Catel_Fody.ExposeAttribute.FullName, StringComparison.OrdinalIgnoreCase)
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
    }
}
