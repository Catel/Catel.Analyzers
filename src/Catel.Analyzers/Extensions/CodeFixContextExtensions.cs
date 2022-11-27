namespace Catel.Analyzers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;

    internal static class CodeFixContextExtensions
    {
        /// <summary>
        /// Finds syntax node in document pointed by diagnostic.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<SyntaxNode?> FindSyntaxNodeAsync(this CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return null;
            }

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var diagnosticToken = root.FindNode(diagnosticSpan);

            return diagnosticToken;
        }
    }
}
