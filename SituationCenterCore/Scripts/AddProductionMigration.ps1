#
# AddProductionMigration.ps1
#
cd ..
Remove-Item .\Migrations -Force -Recurse
$migrationName = Read-Host 'Write name for new production Migration'
dotnet ef migrations add $migrationName -o Data/Migrations/Production --configuration Release
echo "END PROGRAM"
Read-Host
