namespace Catel.Analyzers
{
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CTL0012Diagnostic : DiagnosticBase
    {
        public const string Id = "CTL0012";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = context.Node as ConstructorDeclarationSyntax;
            if (constructorDeclaration is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // TryGetSymbol resolves a ConstructorDeclarationSyntax to an IMethodSymbol
            if (!context.SemanticModel.TryGetSymbol<IMethodSymbol>(constructorDeclaration, context.CancellationToken, out var constructorSymbol))
            {
                return;
            }

            var containingType = constructorSymbol.ContainingType;
            if (!containingType.InheritsFrom("ViewModelBase"))
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Inspect parameters: find if any interface parameter appears before a concrete type parameter
            var parameters = constructorSymbol.Parameters;
            if (parameters.Length < 2)
            {
                return;
            }

            Location? concreteTypeLocation = null;

            foreach (var parameter in parameters.Reverse())
            {
                var paramType = parameter.Type;

                if (paramType.TypeKind == TypeKind.Class || paramType.TypeKind == TypeKind.Struct)
                {
                    concreteTypeLocation = parameter.Locations.First();
                }
                else if (paramType.TypeKind == TypeKind.Interface && 
                         concreteTypeLocation is not null)
                {
                    // An interface parameter appears before a concrete type parameter
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.CTL0012_ConcreteTypesShouldGoFirstInViewModelConstructor,
                            concreteTypeLocation));
                    return;
                }
            }
        }
    }
}
