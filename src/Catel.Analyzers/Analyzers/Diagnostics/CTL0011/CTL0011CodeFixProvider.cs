namespace Catel.Analyzers
{
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
    using Microsoft.CodeAnalysis.Editing;
    using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CTL0011CodeFixProvider))]
    internal class CTL0011CodeFixProvider : CodeFixProvider
    {
        public const string Title = "Replace with Log.ErrorAndCreateException";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CTL0011_ProvideCatelLogOnThrowingException.Id);

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

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            context.RegisterCodeFix(
              CodeAction.Create(Title, cancellationToken =>
              FixAsync(context.Document, diagnosticNode, cancellationToken), equivalenceKey: Title), context.Diagnostics);
        }

        private static async Task<Document> FixAsync(Document document, SyntaxNode diagnosticNode, CancellationToken cancellationToken)
        {
            if (diagnosticNode is not ThrowStatementSyntax throwStatement)
            {
                return document;
            }

            var exceptionCreationSyntax = throwStatement.ChildNodes().FirstOrDefault(x => x is ObjectCreationExpressionSyntax) as ObjectCreationExpressionSyntax;
            if (exceptionCreationSyntax is null)
            {
                return document;
            }

            var arguments = exceptionCreationSyntax.ArgumentList;
            var logErrorExpression = SF.InvocationExpression(SF.ParseExpression($"Log.ErrorAndCreateException<{exceptionCreationSyntax.Type}>"));

            if (arguments is not null)
            {
                if (arguments.Arguments.Count <= 1)
                {
                    logErrorExpression = logErrorExpression.WithArgumentList(arguments);
                }
                else
                {
                    var logErrorMethodArguments = SF.ArgumentList();

                    // Assume 'string message' is last string parameter in Exception ctor aguments;
                    var messageArgument = arguments.Arguments.LastOrDefault(x => x.Expression.IsKind(SyntaxKind.InterpolatedStringExpression)
                    || x.Expression.IsKind(SyntaxKind.StringLiteralExpression));
                    if (messageArgument is null)
                    {
                        // just wrap as callback
                        var callbackFunctor =
                            SF.Argument(SF.SimpleLambdaExpression(SF.Parameter(SF.ParseToken("_")), exceptionCreationSyntax));

                        logErrorMethodArguments = logErrorMethodArguments.AddArguments(callbackFunctor);
                    }
                    else
                    {
                        // callback + message parameter: Log.ErrorAndCreateException(message => createExpFunc, message)
                        var exceptionCreationArguments = arguments.Arguments.ToList();

                        // Replace with lambda parameter
                        var messageArgumentIndex = exceptionCreationArguments.IndexOf(messageArgument);
                        if (messageArgumentIndex != -1)
                        {
                            exceptionCreationArguments[messageArgumentIndex] = SF.Argument(SF.ParseExpression("message"));

                            var callbackRightPart = SF.ObjectCreationExpression(exceptionCreationSyntax.Type)
                                .WithArgumentList(exceptionCreationArguments.ToArgumentList());

                            // Generate new argument lists and expression
                            logErrorMethodArguments = logErrorMethodArguments.AddArguments(
                                SF.Argument(SF.SimpleLambdaExpression(SF.Parameter(SF.ParseToken("message")), callbackRightPart)),
                                SF.Argument(SF.ParseExpression($"{messageArgument.ToFullString()}")));
                        }
                    }

                    logErrorExpression = SF.InvocationExpression(SF.ParseExpression($"Log.ErrorAndCreateException"))
                        .WithArgumentList(logErrorMethodArguments);
                }
            }

            var documentEditor = await DocumentEditor.CreateAsync(document, cancellationToken);
            documentEditor.ReplaceNode(exceptionCreationSyntax, logErrorExpression);

            document = documentEditor.GetChangedDocument();
            return document;
        }
    }
}
