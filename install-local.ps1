$nugetPath = "$env:USERPROFILE\.nuget\packages"
$installPath = "$nugetPath\vertical-cli"
$packagePath = "$installPath\$1"

if (Test-Path $packagePath) {
    Write-Host "Removing existing installation from $packagePath"
    Remove-Item -Recurse $packagePath
}

if (Test-Path .\lib) {
    Remove-Item -Recurse .\lib
}

if (Test-Path .\pack) {    
    Remove-Item -Recurse .\pack
}

dotnet clean
dotnet restore --force
dotnet build src\Vertical.Cli.SourceGenerator -o .\lib
dotnet pack src\Vertical.Cli --no-restore -o .\pack -c Debug /p:Version=$1
dotnet nuget push ".\pack\vertical-cli.$1.nupkg" -s $nugetPath

Remove-Item -Recurse .\pack
Remove-Item -Recurse .\lib