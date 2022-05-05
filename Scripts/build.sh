cd ..
rm -rf Publish
dotnet build Lambda.csproj
dotnet publish -o Publish
cd Publish
zip Publish.zip *