namespace Catel.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CTL0008CodeFixProvider))]
    internal class CTL0008CodeFixProvider : CodeFixProvider
    {
        private const string NameDoesNotExistInCurrentContextId = "CS0103";
        public const string Title_CS0008 = "Replace with ArgumentNullException.ThrowIfNull";
        public const string Title_CS0103 = "Add using System";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CL0008_DoUseThrowIfNullForArgumentCheck.Id, NameDoesNotExistInCurrentContextId);
        public override FixAllProvider? GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnosticNode = await context.FindSyntaxNodeAsync().ConfigureAwait(false);
            if (diagnosticNode is null)
            {
                return;
            }

            if (diagnosticNode is IdentifierNameSyntax identifierNameSyntax)
            {
                if (identifierNameSyntax.Identifier.Text != "ArgumentNullException")
                {
                    return;
                }

                // Allow user to fix missed System namespace after first CodeFix
                context.RegisterCodeFix(
                    CodeAction.Create(Title_CS0103,
                    cancellationToken => FixNamespaceAsync(context.Document, identifierNameSyntax, cancellationToken),
                    equivalenceKey: Title_CS0103), context.Diagnostics);
            }

            if (diagnosticNode is not InvocationExpressionSyntax invocationSyntax)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            context.RegisterCodeFix(
              CodeAction.Create(Title_CS0008,
              cancellationToken => FixAsync(context.Document, invocationSyntax, cancellationToken),
              equivalenceKey: Title_CS0008), context.Diagnostics);
        }

        /// <summary>
        /// Code action changes Argument.IsNotNull expression to ArgumentNullException.ThrowIfNull invocation
        /// </summary>
        /// <param name="document"></param>
        /// <param name="invocationExpressionSyntax"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<Document> FixAsync(Document document, InvocationExpressionSyntax invocationExpressionSyntax, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            var argumentNames = GetArgumentNames(invocationExpressionSyntax);
            if (!argumentNames.Any())
            {
                return document;
            }

            var isNotNullParameter = argumentNames.FirstOrDefault();
            if (string.IsNullOrEmpty(isNotNullParameter))
            {
                return document;
            }

            var parameters = new SeparatedSyntaxList<ArgumentSyntax>().AddRange(
                new ArgumentSyntax[]
                {
                    SF.Argument(SF.IdentifierName(isNotNullParameter))
                });

            var throwIfNullInvocation = SF.InvocationExpression(SF.IdentifierName("ArgumentNullException.ThrowIfNull"))
                .WithArgumentList(SF.ArgumentList().WithArguments(parameters));

            var updatedRoot = root.ReplaceNode(invocationExpressionSyntax, throwIfNullInvocation);

            return document.WithSyntaxRoot(updatedRoot);
        }

        private static async Task<Document> FixNamespaceAsync(Document document, IdentifierNameSyntax identifierNameSyntax, CancellationToken cancellationToken)
        {
            var containingNamespace = identifierNameSyntax.FirstAncestor<NamespaceDeclarationSyntax>();
            if (containingNamespace is null)
            {
                return document;
            }

            if (containingNamespace.Usings.Any(x => string.Equals("System", x.Name.ToFullString())))
            {
                return document;
            }

            var updatedNamespace = containingNamespace.AddUsings(SF.UsingDirective(SF.ParseName("System")));

            if (cancellationToken.IsCancellationRequested)
            {
                return document;
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            return document.WithSyntaxRoot(root.ReplaceNode(containingNamespace, updatedNamespace));
        }

        private static string[] GetArgumentNames(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var argumentList = invocationExpressionSyntax.ChildNodes().FirstOrDefault(x => x.IsKind(SyntaxKind.ArgumentList)) as ArgumentListSyntax;
            if (argumentList is null)
            {
                return Array.Empty<string>();
            }

            var argumentNames = argumentList.Arguments.Select(x =>
            {
                if (x.DescendantNodes().FirstOrDefault(x => x.IsKind(SyntaxKind.IdentifierName)) is IdentifierNameSyntax identifierNameSyntax)
                {
                    return identifierNameSyntax.Identifier.ValueText;
                }

                return string.Empty;
            }).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return argumentNames;
        }
    }
}
