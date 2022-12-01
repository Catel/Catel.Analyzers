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

                var allMetadata = transitiveMetadataReferences.Append(debugMetadataReferences)
                    .Append(systemMetadataReferences);

                Settings.Default = Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.WarningsAndErrors)
                    .WithMetadataReferences(refs => refs.Concat(allMetadata));
            }
        }
    }
}
