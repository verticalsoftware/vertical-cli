nugetPath="$HOME/.nuget/packages"
installPath="$nugetPath/vertical-cli"
version=$1
packagePath="$installPath/$version"

if [ -d "$packagePath" ]; then
    echo "Remove existing installation from $packagePath"
fi

dotnet clean
dotnet restore --force
dotnet build src/Vertical.Cli.SourceGenerator -o ./lib
dotnet pack src/Vertical.Cli --no-restore -o ./pack -c Debug /p:Version=$version --include-symbols
dotnet nuget push "./pack/vertical-cli.$version.nupkg" -s $nugetPath

rm -rf ./pack
rm -rf ./lib
