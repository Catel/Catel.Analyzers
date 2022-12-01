namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal abstract class DiagnosticAnalyzerBase : DiagnosticAnalyzer
    {
        private readonly Dictionary<string, IDiagnostic> _analyzers = new();

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

            var operations = GetTriggerOperations();
            if (operations.Any())
            {
                context.RegisterOperationAction(c => HandleOperationAction(c), operations);
            }

            var symbols = GetTriggerSymbols();
            if (symbols.Any())
            {
                context.RegisterSymbolAction(c => HandleSymbolAction(c), symbols);
            }

            var syntaxes = GetTriggerSyntaxNodes();
            if (syntaxes.Any())
            {
                context.RegisterSyntaxNodeAction(c => HandleSyntaxNodeAction(c), syntaxes);
            }
        }


        protected virtual OperationKind[] GetTriggerOperations()
        {
            return new[]
            {
                OperationKind.AnonymousFunction,
                OperationKind.Await,
                OperationKind.Block,
                OperationKind.ExpressionStatement,
                OperationKind.Invocation,
            };
        }

        protected virtual SymbolKind[] GetTriggerSymbols()
        {
            return new[]
            {
                SymbolKind.Method,
            };
        }

        protected virtual SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return new SyntaxKind[]
            {
                SyntaxKind.FieldDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.InvocationExpression,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.ClassDeclaration,
            };
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

        protected virtual IDiagnostic ResolveAnalyzer(DiagnosticDescriptor descriptor)
        {
            var typeName = $"Catel.Analyzers.{descriptor.Id}Diagnostic";
            var type = Type.GetType(typeName);
            if (type is null)
            {
                throw new Exception($"Cannot create diagnostic from '{typeName}'");
            }

            var analyzer = Activator.CreateInstance(type) as IDiagnostic;
            if (analyzer is null)
            {
                throw new Exception($"Cannot create diagnostic from '{typeName}'");
            }

            return analyzer;
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
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

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
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

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
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                analyzer.Value.HandleSyntaxNode(context);
            }
        }
    }
}
