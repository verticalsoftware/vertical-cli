dotnet clean
dotnet build
dotnet test --no-restore --no-build

# Copy source generator assembly
Copy-Item -Path .\src\Vertical.Cli.SourceGenerator\bin\Debug\netstandard2.0\Vertical.Cli.SourceGenerator.dll .\src\Vertical.Cli\bin\Debug

# Pack
dotnet pack .\src\Vertical.Cli\Vertical.Cli.csproj -c Debug --no-restore --include-symbols -p:VersionSuffix=dev -o .\src\Vertical.Cli\package

# Push
Remove-Item $env:USERPROFILE\.nuget\packages\vertical-cli -Recurse -ErrorAction SilentlyContinue
dotnet nuget push .\src\Vertical.Cli\package\vertical-cli.1.0.0-dev.nupkg -s $env:USERPROFILE\.nuget\packages