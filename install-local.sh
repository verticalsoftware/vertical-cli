rm -rf pack
dotnet build -c Release src/generator -o analyzers
dotnet pack -c Release src/lib --version-suffix $1 -o pack --include-symbols
dotnet nuget push pack/* --source "$HOME/.nuget/packages"