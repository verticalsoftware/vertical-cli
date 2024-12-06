nugetPath="$HOME/.nuget/packages"
installPath="$nugetPath/vertical-cli"
version=$1
packagePath="$installPath/$version"

if [ -d "$packagePath" ]; then
    echo "Remove existing installation from $packagePath"
fi

dotnet clean
dotnet restore --force
dotnet build src/source-generator -o ./analyzers
dotnet build src/analyzers -o ./analyzers
dotnet pack src/lib --no-restore -o ./pack -c Debug /p:Version=$version --include-symbols
dotnet nuget push "./pack/vertical-cli.$version.nupkg" -s $nugetPath

rm -rf ./pack
rm -rf ./analyzers