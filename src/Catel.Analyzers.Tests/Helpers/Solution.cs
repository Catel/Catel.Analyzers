namespace Catel.Analyzers.Tests
{
    using System;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal static class Solution
    {
        internal static void Verify<TAnalyzer>(Action<TAnalyzer> assertAction)
            where TAnalyzer : DiagnosticAnalyzerBase
        {
            var analyzer = Activator.CreateInstance<TAnalyzer>();
            assertAction.Invoke(analyzer);
        }

        [SetUpFixture]
        internal static class Bootstrap
        {
            [OneTimeSetUp]
            public static void GlobalSetup()
            {
                var systemMetadataReferences = MetadataReferences.CreateFromAssembly(typeof(object).Assembly)
                    .WithAliases(new[] { "global", "mscorlib" });
                var debugMetadataReferences = MetadataReferences.CreateFromAssembly(typeof(System.Diagnostics.Debug).Assembly)
                    .WithAliases(new[] { "global", "System" });
                var transitiveMetadataReferences = MetadataReferences.Transitive(typeof(ValidCodeWithAllAnalyzers).Assembly);
                var catelMetadataReferences = MetadataReferences.CreateFromAssembly(typeof(Logging.Log).Assembly);

                var allMetadata = transitiveMetadataReferences.Append(debugMetadataReferences)
                    .Append(systemMetadataReferences)
                    .Append(catelMetadataReferences);

                Settings.Default = Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.WarningsAndErrors)
                    .WithMetadataReferences(refs => refs.Concat(allMetadata));

                var catelFodyMetadataReference = BinaryReference.Compile(
@"namespace Catel.Fody
{
    using System;

    /// <summary>
    /// The expose attribute.
    /// </summary>
    /// <seealso cref=""System.Attribute"" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExposeAttribute : Attribute
    {
        public ExposeAttribute(string propertyName)
            : this(propertyName, string.Empty)
        {
        }

        public ExposeAttribute(string propertyName, string propertyNameOnModel)
        {
        }
    }
}");

                Settings.Default = Settings.Default.WithMetadataReferences(refs => refs.Concat(new[] { catelFodyMetadataReference }));
            }
        }
    }
}
