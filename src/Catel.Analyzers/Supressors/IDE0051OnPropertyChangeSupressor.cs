namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IDE0051OnPropertyChangeSupressor : DiagnosticSuppressor
    {
        private static readonly string FodyBaseClassLookupName = "ObservableObject";
        private static readonly string FodyAttributeLookupName = "Expose";

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(
            new SuppressionDescriptor("CTLS0001", "IDE0051", "Supress IDE0051 on methods used for Fody INotifyPropertyChanged interception")
        );

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            try
            {
                // Check fody assembly
                var fodyType = context.Compilation.GetTypeByMetadataName("PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute");

                if (fodyType is null)
                {
                    return;
                }

                // Get expose attribute instance
                var fodyExposeType = context.Compilation.GetTypeByMetadataName("Catel.Fody.ExposeAttribute");

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

                    var rootSemantic = context.GetSemanticModel(diagnosticSourceSyntaxTree);
                    var containerClassSymbol = (rootSemantic.GetDeclaredSymbol(containerClassSyntax, default) as ITypeSymbol);

                    if (containerClassSymbol is null)
                    {
                        // Wrong class declaration
                        continue;
                    }

                    if (!containerClassSymbol.InheritsFrom(FodyBaseClassLookupName))
                    {
                        continue;
                    }

                    var targetSymbol = rootSemantic.GetDeclaredSymbol(targetSyntax, context.CancellationToken);

                    var accessModifierNode = targetSyntax.ChildTokens().FirstOrDefault(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword));

                    if (!accessModifierNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword))
                    {
                        // Skip non-private
                        continue;
                    }

                    if (targetSymbol is IMethodSymbol method && method.Name.StartsWith("On") && method.Name.EndsWith("Changed"))
                    {
                        var propertyName = method.Name.ReplaceFirst("On", "").ReplaceLast("Changed", "");

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

                        if (fodyExposeType is null)
                        {
                            continue;
                        }

                        var exposedMarkedDeclarations = from descendantNode in containerClassSyntax.DescendantNodes(x => x is ClassDeclarationSyntax || x is PropertyDeclarationSyntax || x is AttributeListSyntax)
                                                        where descendantNode is AttributeSyntax && string.Equals(GetIdentifier(descendantNode as AttributeSyntax), FodyAttributeLookupName)
                                                        select descendantNode.FirstAncestor<PropertyDeclarationSyntax>();

                        foreach (var property in exposedMarkedDeclarations.Distinct())
                        {
                            var propertySymbol = rootSemantic.GetDeclaredSymbol(property, default);

                            var exposeAttributesList = propertySymbol.GetAttributes().Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, fodyExposeType)).ToList();

                            foreach (var exposeAttribute in exposeAttributesList)
                            {
                                if (exposeAttribute.ConstructorArguments.Any())
                                {
                                    var constructorArgValue = exposeAttribute.ConstructorArguments.FirstOrDefault().Value.ToString();

                                    if (string.Equals(constructorArgValue, propertyName))
                                    {
                                        context.ReportSuppression(Suppression.Create(supresssionDescriptor, diagnostic));
                                        return;
                                    }
                                }

                                if (exposeAttribute.NamedArguments.Any())
                                {
                                    var argument = exposeAttribute.NamedArguments.FirstOrDefault(arg => string.Equals(arg.Key, "propertyName"));

                                    var propertyIsExposed = argument.Equals(default(KeyValuePair<string, TypedConstant>)) ? false : string.Equals(argument.Value.Value.ToString(), propertyName);

                                    if (propertyIsExposed)
                                    {
                                        context.ReportSuppression(Suppression.Create(supresssionDescriptor, diagnostic));
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetIdentifier(AttributeSyntax syntaxNode)
        {
            if (syntaxNode is null)
            {
                return null;
            }

            var identifierSyntax = syntaxNode.ChildNodes().Where(node => node is IdentifierNameSyntax name).FirstOrDefault() as IdentifierNameSyntax;

            return identifierSyntax.Identifier.ValueText;
        }
    }
}
