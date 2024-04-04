rm -rf Code/Nuget
mkdir Code/Nuget
rm -rf Code/Build
mkdir Code/Build
dotnet publish Code/ImageColourSwap.Lambda.csproj -o Code/Nuget
dotnet build Code/ImageColourSwap.Lambda.csproj -o Code/Build -c Release
dotnet pack --no-restore Code/ImageColourSwap.Lambda.csproj /p:OutputPath=Nuget -o Code/Nuget
cd Code/Build
zip Lambda.zip *
cd ..