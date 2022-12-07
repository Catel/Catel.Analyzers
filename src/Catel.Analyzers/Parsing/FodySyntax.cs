namespace Catel.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class FodySyntax
    {
        public static bool IsPropertyExposedWithFodyAttribute(PropertyDeclarationSyntax property, INamedTypeSymbol attributeTypeToMatch, string propertyName, SemanticModel model, CancellationToken cancellationToken)
        {
            var propertySymbol = model.GetDeclaredSymbol(property, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            if (propertySymbol is null)
            {
                return false;
            }

            var exposeAttributesList = propertySymbol.GetAttributes().Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeTypeToMatch)).ToList();

            foreach (var exposeAttribute in exposeAttributesList)
            {
                if (exposeAttribute.ConstructorArguments.Any())
                {
                    var constructorArgValue = exposeAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();

                    if (string.Equals(constructorArgValue, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                if (exposeAttribute.NamedArguments.Any())
                {
                    var argument = exposeAttribute.NamedArguments.FirstOrDefault(arg => string.Equals(arg.Key, "propertyName", StringComparison.OrdinalIgnoreCase));

                    var propertyIsExposed = !argument.Equals(default(KeyValuePair<string, TypedConstant>)) && string.Equals(argument.Value.Value?.ToString() ?? string.Empty, propertyName, StringComparison.OrdinalIgnoreCase);
                    if (propertyIsExposed)
                    {
                        return false;
                    }
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            return false;
        }
    }
}
