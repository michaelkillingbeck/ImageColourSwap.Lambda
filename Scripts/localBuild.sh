rm -rf Code/Nuget
mkdir Code/Nuget
rm -rf Code/Build
mkdir Code/Build
dotnet publish Code/Lambda.csproj -o Code/Nuget
dotnet build Code/Lambda.csproj -o Code/Build -c Release
dotnet pack --no-restore Code/Lambda.csproj /p:OutputPath=Nuget -o Code/Nuget
cd Code/Build
zip Lambda.zip *
cd ..