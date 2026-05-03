namespace Catel.Analyzers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CTL0014Diagnostic : DiagnosticBase
    {
        public const string Id = "CTL0014";

        private const string IHostFullName = "Microsoft.Extensions.Hosting.IHost";

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

            var hostFields = GetHostFields(classDeclaration, context.SemanticModel, context.CancellationToken);
            if (hostFields.Count == 0)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            foreach (var (fieldName, fieldLocation) in hostFields)
            {
                if (!IsStopAsyncCalledOnField(classDeclaration, fieldName))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.CTL0014_CallStopAsyncOnHost,
                            fieldLocation,
                            fieldName));
                }
            }
        }

        private static List<(string FieldName, Location FieldLocation)> GetHostFields(
            ClassDeclarationSyntax classDeclaration,
            SemanticModel semanticModel,
            System.Threading.CancellationToken cancellationToken)
        {
            var hostFields = new List<(string, Location)>();

            foreach (var member in classDeclaration.Members)
            {
                if (member is not FieldDeclarationSyntax field)
                {
                    continue;
                }

                if (!IsIHostType(field.Declaration.Type, semanticModel, cancellationToken))
                {
                    continue;
                }

                foreach (var variable in field.Declaration.Variables)
                {
                    hostFields.Add((variable.Identifier.Text, field.Declaration.Type.GetLocation()));
                }
            }

            return hostFields;
        }

        private static bool IsIHostType(TypeSyntax typeSyntax, SemanticModel semanticModel, System.Threading.CancellationToken cancellationToken)
        {
            // Try semantic model first for accurate type resolution
            var typeInfo = semanticModel.GetTypeInfo(typeSyntax, cancellationToken);
            if (typeInfo.Type is INamedTypeSymbol typeSymbol)
            {
                var fullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                return string.Equals(fullName, $"global::{IHostFullName}", System.StringComparison.Ordinal);
            }

            // Fall back to syntax-based check when semantic model cannot resolve the type
            var typeName = typeSyntax.ToString();
            return typeName == "IHost" || typeName == IHostFullName;
        }

        private static bool IsStopAsyncCalledOnField(ClassDeclarationSyntax classDeclaration, string fieldName)
        {
            var invocations = classDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                // Check regular member access: _host.StopAsync()
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                    memberAccess.Name.Identifier.Text == "StopAsync" &&
                    memberAccess.Expression is IdentifierNameSyntax identifier &&
                    identifier.Identifier.Text == fieldName)
                {
                    return true;
                }

                // Check conditional access: _host?.StopAsync()
                if (invocation.Expression is MemberBindingExpressionSyntax memberBinding &&
                    memberBinding.Name.Identifier.Text == "StopAsync")
                {
                    var conditionalAccess = invocation.Ancestors().OfType<ConditionalAccessExpressionSyntax>().FirstOrDefault();
                    if (conditionalAccess?.Expression is IdentifierNameSyntax conditionalIdentifier &&
                        conditionalIdentifier.Identifier.Text == fieldName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
