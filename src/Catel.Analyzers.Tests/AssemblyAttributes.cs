﻿using Gu.Roslyn.Asserts;

[assembly: MetadataReference(typeof(object), new[] { "global", "mscorlib" })]
[assembly: MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "System" })]
[assembly: TransitiveMetadataReferences(typeof(Catel.Analyzers.Tests.ValidCodeWithAllAnalyzers))]
[assembly: TransitiveMetadataReferences(
    //typeof(System.Linq.Enumerable),
    //typeof(System.Net.WebClient),
    //typeof(System.Drawing.Bitmap),
    //typeof(System.Data.Common.DbConnection),
    //typeof(System.Xml.Serialization.XmlSerializer),
    //typeof(System.Runtime.Serialization.DataContractSerializer),
    //typeof(System.Windows.Media.Brush),
    //typeof(System.Windows.Controls.Control),
    //typeof(System.Windows.Media.Matrix),
    //typeof(System.Xaml.XamlLanguage),
    //typeof(System.Collections.Immutable.ImmutableArray),
    typeof(Catel.CoreModule),
    typeof(Catel.MVVMModule),
    typeof(Catel.Services.IDispatcherService),
    //typeof(Gu.Roslyn.AnalyzerExtensions.Cache),
    //typeof(Gu.Roslyn.CodeFixExtensions.CodeStyle),
    typeof(NUnit.Framework.Assert),
    typeof(RoslynAssert))]
