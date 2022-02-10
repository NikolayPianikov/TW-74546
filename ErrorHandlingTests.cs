namespace Lombiq.Tests.UI.Samples.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ErrorHandlingTests
    {
        private readonly ITestOutputHelper _output;

        public ErrorHandlingTests(ITestOutputHelper output)
        {
            _output = output;
            Console.WriteLine("##teamcity[testMetadata testName='Lombiq.Tests.UI.Samples' name='aaa' type='int' value='10']");
        }
        
        [Theory, Chrome]
        public Task ServerSideErrorOnLoadedPageShouldHaltTest(Browser browser)
        {
            TeamCityMetadataReporter.ReportInt(_output, "Lombiq.Tests.UI.Samples: Lombiq.Tests.UI.Samples.Tests.ErrorHandlingTests.ServerSideErrorOnLoadedPageShouldHaltTest(browser: Chrome)", "Val1", 99);
            return Task.FromResult(1);
        }

        [Theory, Chrome]
        public Task ClientSideErrorOnLoadedPageShouldHaltTest(Browser browser)
        {
            TeamCityMetadataReporter.ReportInt(_output, "Lombiq.Tests.UI.Samples: Lombiq.Tests.UI.Samples.Tests.ErrorHandlingTests.ClientSideErrorOnLoadedPageShouldHaltTest(browser: Chrome)", "Val2", 33);
            return Task.FromResult(1);
        }
    }
    
    public enum Browser
    {
        Chrome,
        Edge,
        Firefox,
        InternetExplorer,
    }
    
    public sealed class ChromeAttribute : BrowserAttributeBase
    {
        protected override Browser Browser => Browser.Chrome;
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class BrowserAttributeBase : DataAttribute
    {
        protected abstract Browser Browser { get; }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new[] { Browser as object };
        }
    }
    
    public static class TeamCityMetadataReporter
    {
        public static void ReportInt(ITestOutputHelper output, string testName, string name, int number) =>
            ReportNumber(output, testName, name, number.ToString(CultureInfo.InvariantCulture));

        public static void ReportNumber(ITestOutputHelper output, string testName, string name, string number) =>
            Report(output, testName, name, "number", number);

        public static void Report(ITestOutputHelper output, string testName, string name, string type, string value) =>
            // Starting with a line break is sometimes necessary not to mix up these messages in the build output.
            output.WriteLine(
                Environment.NewLine +
                $"##teamcity[testMetadata testName='{Escape(testName)}' name='{Escape(name)}' type='{type}' value='{Escape(value)}']");

        // TeamCity needs forward slashes to replacing backslashes if the platform uses that.
        private static string PreparePath(string artifactPath) => artifactPath.Replace(Path.DirectorySeparatorChar, '/');

        // Escaping values for TeamCity, see:
        // https://www.jetbrains.com/help/teamcity/service-messages.html#Escaped+values.
        private static string Escape(string value) => value
            .Replace("|", "||", StringComparison.Ordinal)
            .Replace("'", "|'", StringComparison.Ordinal)
            .Replace("\n", "n", StringComparison.Ordinal)
            .Replace("\r", "|r", StringComparison.Ordinal)
            .Replace(@"\uNNNN", "|0xNNNN", StringComparison.Ordinal)
            .Replace("[", "|[", StringComparison.Ordinal)
            .Replace("]", "|]", StringComparison.Ordinal);
    }
}