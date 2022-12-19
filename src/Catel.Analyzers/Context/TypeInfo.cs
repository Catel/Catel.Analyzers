namespace Catel.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Context-like class simplifying passing multiple values
    /// </summary>
    internal class TypeInfo
    {
        public TypeInfo(ClassDeclarationSyntax syntax, ITypeSymbol declaredSymbol)
        {
            Syntax = syntax;
            DeclaredSymbol = declaredSymbol;
        }

        public ClassDeclarationSyntax Syntax { get; }

        public ITypeSymbol DeclaredSymbol { get; }
    }
}
