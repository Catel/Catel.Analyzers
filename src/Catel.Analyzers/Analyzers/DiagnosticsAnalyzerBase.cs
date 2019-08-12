namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal abstract class DiagnosticAnalyzerBase : DiagnosticAnalyzer
    {
        private readonly Dictionary<string, IAnalyzer> _analyzers = new Dictionary<string, IAnalyzer>();

        protected DiagnosticAnalyzerBase()
        {
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            foreach (var diagnosticDescriptor in SupportedDiagnostics)
            {
                _analyzers[diagnosticDescriptor.Id] = ResolveAnalyzer(diagnosticDescriptor);
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterOperationAction(
                c => HandleOperationAction(c),
                OperationKind.AnonymousFunction,
                OperationKind.Await,
                OperationKind.Block,
                OperationKind.ExpressionStatement,
                OperationKind.Invocation);
            context.RegisterSymbolAction(
                c => HandleSymbolAction(c),
                SymbolKind.Method);
            context.RegisterSyntaxNodeAction(
                c => HandleSyntaxNodeAction(c),
                SyntaxKind.FieldDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.InvocationExpression,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.ClassDeclaration);
            //context.RegisterCompilationStartAction(c => HandleCompilationStartAction(c));
        }

        protected virtual bool ShouldHandleOperation(OperationAnalysisContext context)
        {
            return false;
        }

        protected virtual bool ShouldHandleSymbol(SymbolAnalysisContext context)
        {
            return false;
        }

        protected virtual bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            return false;
        }

        protected virtual IAnalyzer ResolveAnalyzer(DiagnosticDescriptor descriptor)
        {
            var typeName = $"Catel.Analyzers.{descriptor.Id}Analyzer";
            var type = Type.GetType(typeName);

            return (IAnalyzer)Activator.CreateInstance(type);
        }

        private void HandleOperationAction(OperationAnalysisContext context)
        {
            //if (context.IsExcludedFromAnalysis())
            //{
            //    return;
            //}

            if (!ShouldHandleOperation(context))
            {
                return;
            }

            foreach (var analyzer in _analyzers)
            {
                analyzer.Value.HandleOperation(context);
            }
        }

        private void HandleSymbolAction(SymbolAnalysisContext context)
        {
            //if (context.IsExcludedFromAnalysis())
            //{
            //    return;
            //}

            if (!ShouldHandleSymbol(context))
            {
                return;
            }

            foreach (var analyzer in _analyzers)
            {
                analyzer.Value.HandleSymbol(context);
            }
        }

        private void HandleSyntaxNodeAction(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            if (!ShouldHandleSyntaxNode(context))
            {
                return;
            }

            foreach (var analyzer in _analyzers)
            {
                analyzer.Value.HandleSyntaxNode(context);
            }
        }
    }
}
