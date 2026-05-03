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

        private static readonly HashSet<string> HostTypeNames = new()
        {
            "IHost",
            "Microsoft.Extensions.Hosting.IHost",
        };

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

            var hostFieldNames = GetHostFieldNames(classDeclaration);
            if (hostFieldNames.Count == 0)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            foreach (var hostFieldName in hostFieldNames)
            {
                if (!IsStopAsyncCalledOnField(classDeclaration, hostFieldName))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.CTL0014_CallStopAsyncOnHost,
                            classDeclaration.Identifier.GetLocation(),
                            hostFieldName));
                }
            }
        }

        private static List<string> GetHostFieldNames(ClassDeclarationSyntax classDeclaration)
        {
            var hostFieldNames = new List<string>();

            foreach (var member in classDeclaration.Members)
            {
                if (member is not FieldDeclarationSyntax field)
                {
                    continue;
                }

                var typeName = field.Declaration.Type.ToString();
                if (!HostTypeNames.Contains(typeName))
                {
                    continue;
                }

                foreach (var variable in field.Declaration.Variables)
                {
                    hostFieldNames.Add(variable.Identifier.Text);
                }
            }

            return hostFieldNames;
        }

        private static bool IsStopAsyncCalledOnField(ClassDeclarationSyntax classDeclaration, string fieldName)
        {
            // Check regular member access: _host.StopAsync()
            var invocations = classDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
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
                    var conditionalAccess = invocation.Parent as ConditionalAccessExpressionSyntax;
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
