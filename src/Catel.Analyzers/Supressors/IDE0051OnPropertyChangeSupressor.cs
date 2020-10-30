namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IDE0051OnPropertyChangeSupressor : DiagnosticSuppressor
    {
        private static readonly string CatelBaseClassLookupName = "ObservableObject";
        private static readonly string CatelFodyAttributeLookupName = "Expose";

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(
            new SuppressionDescriptor("CTLS0001", "IDE0051", "Supress IDE0051 on methods used for automatic property change callback generation")
        );

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            try
            {
                // Get expose attribute instance and check Catel.Fody assembly
                var exposeAttributeType = context.Compilation.GetTypeByMetadataName("Catel.Fody.ExposeAttribute");

                if (exposeAttributeType is null)
                {
                    return;
                }

                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var supresssionDescriptor = SupportedSuppressions.FirstOrDefault();

                foreach (var diagnostic in context.ReportedDiagnostics)
                {
                    var diagnosticSourceSyntaxTree = diagnostic.Location.SourceTree;

                    if (diagnosticSourceSyntaxTree is null || !diagnostic.Location.IsInSource)
                    {
                        continue;
                    }

                    var rootSyntax = diagnosticSourceSyntaxTree.GetRoot(context.CancellationToken);

                    var targetSyntax = rootSyntax.FindNode(diagnostic.Location.SourceSpan, false, true);

                    // Look is class weaved by Fody
                    var containerClassSyntax = targetSyntax.FirstAncestor<ClassDeclarationSyntax>();

                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var rootSemantic = context.GetSemanticModel(diagnosticSourceSyntaxTree);
                    var containerClassSymbol = (rootSemantic.GetDeclaredSymbol(containerClassSyntax, context.CancellationToken) as ITypeSymbol);

                    if (containerClassSymbol is null)
                    {
                        // Wrong class declaration
                        continue;
                    }

                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!containerClassSymbol.InheritsFrom(CatelBaseClassLookupName))
                    {
                        continue;
                    }

                    var targetSymbol = rootSemantic.GetDeclaredSymbol(targetSyntax, context.CancellationToken);

                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var accessModifierNode = targetSyntax.ChildTokens().FirstOrDefault(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword));

                    // Check agains "None" kine empty token
                    if (!accessModifierNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword))
                    {
                        // Skip non-private
                        continue;
                    }

                    if (!(targetSymbol is IMethodSymbol method) || !method.Name.StartsWith("On") || !method.Name.EndsWith("Changed"))
                    {
                        continue;
                    }

                    var propertyName = method.Name.ReplaceFirst("On", string.Empty).ReplaceLast("Changed", string.Empty);

                    // Search for property
                    // Step 1: Search by property name through class properties
                    // Step 2: Try to check Expose attributes:
                    //  - arguments passed to ctor
                    //  - named arguments
                    // This call is more expensive than queries on syntax tree

                    if (containerClassSymbol.TryFindProperty(propertyName, out var weavedPropertySymbol))
                    {
                        context.ReportSuppression(Suppression.Create(supresssionDescriptor, diagnostic));
                        return;
                    }

                    var exposedMarkedDeclarations = from descendantNode in containerClassSyntax.DescendantNodes(x => x is ClassDeclarationSyntax || x is PropertyDeclarationSyntax || x is AttributeListSyntax)
                                                    where descendantNode is AttributeSyntax && string.Equals(descendantNode.GetIdentifier(), CatelFodyAttributeLookupName)
                                                    select descendantNode.FirstAncestor<PropertyDeclarationSyntax>();

                    if (IsAnyExposedPropertyDeclarationMatch(exposedMarkedDeclarations, exposeAttributeType, propertyName, rootSemantic, context.CancellationToken))
                    {
                        context.ReportSuppression(Suppression.Create(supresssionDescriptor, diagnostic));
                    }

                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsAnyExposedPropertyDeclarationMatch(IEnumerable<PropertyDeclarationSyntax> propertyDeclarations, INamedTypeSymbol attributeTypeToMatch, string propertyName, SemanticModel model, CancellationToken cancellationToken)
        {
            foreach (var property in propertyDeclarations.Distinct())
            {
                var propertySymbol = model.GetDeclaredSymbol(property, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                var exposeAttributesList = propertySymbol.GetAttributes().Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeTypeToMatch)).ToList();

                foreach (var exposeAttribute in exposeAttributesList)
                {
                    if (exposeAttribute.ConstructorArguments.Any())
                    {
                        var constructorArgValue = exposeAttribute.ConstructorArguments.FirstOrDefault().Value.ToString();

                        if (string.Equals(constructorArgValue, propertyName))
                        {
                            return true;
                        }
                    }

                    if (exposeAttribute.NamedArguments.Any())
                    {
                        var argument = exposeAttribute.NamedArguments.FirstOrDefault(arg => string.Equals(arg.Key, "propertyName"));

                        var propertyIsExposed = argument.Equals(default(KeyValuePair<string, TypedConstant>)) ? false : string.Equals(argument.Value.Value.ToString(), propertyName);

                        if (propertyIsExposed)
                        {
                            return true;
                        }
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
