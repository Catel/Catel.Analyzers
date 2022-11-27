namespace Catel.Analyzers
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
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
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CL0011_ProvideCatelLogOnThrowingException.Id);

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
                logErrorExpression = logErrorExpression.WithArgumentList(arguments);
            }

            var documentEditor = await DocumentEditor.CreateAsync(document, cancellationToken);
            documentEditor.ReplaceNode(exceptionCreationSyntax, logErrorExpression);

            document = documentEditor.GetChangedDocument();

            var sm = await document.GetSemanticModelAsync(cancellationToken);
            if (sm is null)
            {
                return document;
            }

            var classSpan = throwStatement.FirstAncestor<ClassDeclarationSyntax>()?.FullSpan ?? default;

            var classDeclaration = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))?.FindNode(classSpan) as ClassDeclarationSyntax;
            if (classDeclaration is null)
            {
                return document;
            }

            var updatedDocument = await EnsureLogStaticFieldForClassOnRootAsync(classDeclaration, sm, document, cancellationToken).ConfigureAwait(false);

            return updatedDocument;
        }

        private static async Task<Document> EnsureLogStaticFieldForClassOnRootAsync(
            ClassDeclarationSyntax classDeclaration,
            SemanticModel semantic,
            Document document,
            CancellationToken cancellationToken)
        {
            var fieldDeclaraions = classDeclaration.ChildNodes().Where(x => x.IsKind(SyntaxKind.FieldDeclaration)).ToList();
            var classSymbol = semantic.GetDeclaredSymbol(classDeclaration, cancellationToken);
            if (classSymbol is not INamedTypeSymbol typeSymbol)
            {
                return document;
            }

            if (typeSymbol.TryFindFirstMember<IFieldSymbol>(field =>
            {
                return field.IsStatic && field.Type.Name == "ILog" && field.Name == "Log";
            }, out var logStaticField))
            {
                return document;
            }

            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            // Add namespace
            var root = editor.GetChangedRoot();
            var namespaceDeclaration = root.DescendantNodes().FirstOrDefault(x => x is NamespaceDeclarationSyntax) as NamespaceDeclarationSyntax;

            NamespaceDeclarationSyntax? updatedNamespace = null;
            // Note: we expect using are part of NamespaceDeclaration, not a root
            if (namespaceDeclaration is null)
            {
                return document;
            }

            if (!namespaceDeclaration.Usings.Any(x => string.Equals("Catel.Logging", x.Name.ToFullString())))
            {
                updatedNamespace = namespaceDeclaration.AddUsings(
                    SF.UsingDirective(SF.ParseName("Catel.Logging")).NormalizeWhitespace(elasticTrivia: true).WithTrailingElasticLineFeed());
            }

            if (updatedNamespace is not null)
            {
                editor.ReplaceNode(namespaceDeclaration, updatedNamespace);
            }

            // Replacing namespace node can change root
            // In that way we can't locate class node to insert Log static field in same batch
            // Recreate editor for further changes
            editor = await DocumentEditor.CreateAsync(editor.GetChangedDocument()).ConfigureAwait(false);

            if (cancellationToken.IsCancellationRequested)
            {
                return document;
            }

            // Create Log member
            var logStaticFieldSyntax = SF.FieldDeclaration(
                SF.VariableDeclaration(SF.ParseTypeName("ILog"),
                SF.SeparatedList(new[]
                {
                    SF.VariableDeclarator(SF.Identifier("Log"))
                    .WithInitializer(SF.EqualsValueClause(SF.InvocationExpression(SF.IdentifierName("LogManager.GetCurrentClassLogger"))))
                })))
                .AddModifiers(
                SF.Token(SyntaxKind.PrivateKeyword), SF.Token(SyntaxKind.StaticKeyword), SF.Token(SyntaxKind.ReadOnlyKeyword));

            var equalClassDeclarationSyntax = editor.OriginalRoot.DescendantNodes()
                .FirstOrDefault(syntax =>
                {
                    if (syntax is ClassDeclarationSyntax syntaxClass)
                    {
                        return string.Equals(syntaxClass.Identifier.ValueText, classDeclaration.Identifier.ValueText);
                    }

                    return false;
                });

            editor.InsertMembers(equalClassDeclarationSyntax, 0, new[]
            {
                logStaticFieldSyntax
            });

            var changedDocument = editor.GetChangedDocument();

            return changedDocument;
        }
    }
}
