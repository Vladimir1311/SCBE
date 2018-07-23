#
# UpdateProduction.ps1
#
cd ..
$migrationListOutput = dotnet ef migrations list --configuration Release
$migrationName = $migrationListOutput.Split()[-1]
echo "Applying migration $migrationName"
dotnet ef database update $migrationName --configuration Release
echo "END PROGRAM"
Read-Host
