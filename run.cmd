dotnet build

SET TEAMCITY_VERSION=2022
dotnet test "bin\Debug\netcoreapp3.1\Lombiq.Tests.UI.Samples.dll" /logger:"logger://teamcity" /TestAdapterPath:"tools\vstest15" /logger:"console;verbosity=normal"