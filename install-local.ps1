$nugetPath = "$env:USERPROFILE\.nuget\packages"
$installPath = "$nugetPath\vertical-cli"

if (Test-Path $installPath) {
    Write-Host "Removing existing installation from $installPath"
    Remove-Item -Recurse $installPath
}

dotnet restore
dotnet build src\Vertical.Cli.SourceGenerator -o .\lib
dotnet pack src\Vertical.Cli --no-restore --no-build -o .\pack -c Debug
dotnet nuget push .\pack\vertical-cli.1.0.0.nupkg -s $nugetPath

Remove-Item -Recurse .\pack