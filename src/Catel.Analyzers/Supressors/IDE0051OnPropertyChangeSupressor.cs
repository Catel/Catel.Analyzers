namespace Catel.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IDE0051OnPropertyChangeSupressor : DiagnosticSuppressor
    {
        private const string CatelBaseClassLookupName = "ObservableObject";

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(
            new SuppressionDescriptor("CTLS0001", "IDE0051", "Supress IDE0051 on methods used for automatic property change callback generation")
        );

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            try
            {
                // Get expose attribute instance and check Catel.Fody assembly
                var exposeAttributeType = context.Compilation.GetTypeByMetadataName(KnownSymbols.Catel_Fody.ExposeAttribute.FullName);
                if (exposeAttributeType is null)
                {
                    return;
                }

                var supresssionDescriptor = SupportedSuppressions.FirstOrDefault();
                if (supresssionDescriptor is null)
                {
                    return;
                }

                foreach (var diagnostic in context.ReportedDiagnostics)
                {
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var diagnosticSourceSyntaxTree = diagnostic.Location.SourceTree;

                    if (diagnosticSourceSyntaxTree is null || !diagnostic.Location.IsInSource)
                    {
                        continue;
                    }

                    var rootSyntax = diagnosticSourceSyntaxTree.GetRoot(context.CancellationToken);

                    var targetSyntax = rootSyntax.FindNode(diagnostic.Location.SourceSpan, false, true);

                    var rootSemantic = context.GetSemanticModel(diagnosticSourceSyntaxTree);

                    // Look is class weaved by Fody
                    if (!rootSemantic.TryGetContainingTypeInfo(targetSyntax, context.CancellationToken, out var containingType))
                    {
                        continue;
                    }

                    var containerClassSymbol = containingType.DeclaredSymbol;
                    if (!containerClassSymbol.InheritsFrom(CatelBaseClassLookupName))
                    {
                        continue;
                    }

                    var targetSymbol = rootSemantic.GetDeclaredSymbol(targetSyntax, context.CancellationToken);
                    if (targetSymbol is not IMethodSymbol method)
                    {
                        continue;
                    }

                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!targetSyntax.IsPrivate())
                    {
                        continue;
                    }

                    if (!MethodConvention.TryMatchToConvention(method, "On", "Changed", out var propertyName))
                    {
                        continue;
                    }

                    // Search for property
                    // Step 1: Search by property name through class properties
                    // Step 2: Try to check Expose attributes:
                    //  - arguments passed to ctor
                    //  - named arguments
                    // This call is more expensive than queries on syntax tree

                    if (containerClassSymbol.TryFindProperty(propertyName, out var weavedPropertySymbol))
                    {
                        context.ReportSuppression(Suppression.Create(supresssionDescriptor, diagnostic));
                        continue;
                    }

                    var containerClassSyntax = containingType.Syntax;

                    var exposedMarkedDeclarations = from descendantNode in containerClassSyntax.DescendantNodes(
                                                    x => x.IsKind(SyntaxKind.AttributeList) 
                                                    || x.IsKind(SyntaxKind.PropertyDeclaration) 
                                                    || x.IsKind(SyntaxKind.ClassDeclaration))
                                                    where descendantNode is AttributeSyntax && IsSameAttribute(descendantNode)
                                                    select descendantNode.FirstAncestor<PropertyDeclarationSyntax>();

                    foreach (var property in exposedMarkedDeclarations)
                    {
                        if (FodySyntax.IsPropertyExposedWithFodyAttribute(property, exposeAttributeType, propertyName, rootSemantic, context.CancellationToken))
                        {
                            context.ReportSuppression(Suppression.Create(supresssionDescriptor, diagnostic));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
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
