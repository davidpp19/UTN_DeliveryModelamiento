# Script de Despliegue de RayoExpres en Microsoft Azure
# Requisitos: Tener instalado Azure CLI (https://aka.ms/installazurecli) y haber ejecutado 'az login'

$resourceGroup = "RayoExpres-RG-3"
# ¡IMPORTANTE!: Tu suscripción tiene políticas estrictas de región. 
# Si 'centralus' y 'eastus' fallaron, intenta con: 'brazilsouth', 'eastus2', o 'westus2'.
$location = "brazilsouth"
$dbServerName = "rayoexpres-db-" + (Get-Random -Maximum 10000)
$dbAdminUser = "rayoadmin"
$dbAdminPassword = "SecurePassword123!" # CÁMBIALO
$dbName = "RayoExpresDB"

$storageAccountName = "rayoexprestorage" + (Get-Random -Maximum 10000)
$appServicePlan = "RayoExpres-Plan"
$apiAppName = "rayoexpres-api-" + (Get-Random -Maximum 10000)
$mvcAppName = "rayoexpres-mvc-" + (Get-Random -Maximum 10000)

Write-Host "0. Registrando proveedores de Azure (para evitar el error SubscriptionNotFound)..."
az provider register --namespace Microsoft.Storage
az provider register --namespace Microsoft.Web
az provider register --namespace Microsoft.DBforPostgreSQL

Write-Host "1. Creando Resource Group..."
az group create --name $resourceGroup --location $location

Write-Host "2. Creando Servidor PostgreSQL Flexible..."
az postgres flexible-server create `
  --resource-group $resourceGroup `
  --name $dbServerName `
  --location $location `
  --admin-user $dbAdminUser `
  --admin-password $dbAdminPassword `
  --sku-name Standard_B1ms `
  --tier Burstable `
  --public-access 0.0.0.0

Write-Host "2.1 Creando la Base de Datos dentro del servidor..."
az postgres flexible-server db create `
  --resource-group $resourceGroup `
  --server-name $dbServerName `
  --name $dbName

$connectionString = "Server=$dbServerName.postgres.database.azure.com;Database=$dbName;Port=5432;User Id=$dbAdminUser;Password=$dbAdminPassword;Ssl Mode=Require;"

Write-Host "3. Creando Azure Blob Storage..."
az storage account create `
  --name $storageAccountName `
  --resource-group $resourceGroup `
  --location $location `
  --sku Standard_LRS `
  --allow-blob-public-access true

$blobStorageConnectionString = az storage account show-connection-string --name $storageAccountName --resource-group $resourceGroup --query connectionString -o tsv

Write-Host "4. Creando App Service Plan..."
az appservice plan create `
  --name $appServicePlan `
  --resource-group $resourceGroup `
  --location $location `
  --sku B1 `
  --is-linux

Write-Host "5. Creando Web App para API..."
# En Windows, para evitar el bug de PowerShell con el símbolo |, usamos cmd.exe directamente
cmd.exe /c "az webapp create --resource-group $resourceGroup --plan $appServicePlan --name $apiAppName --runtime `"DOTNETCORE|9.0`""

Write-Host "6. Creando Web App para MVC..."
cmd.exe /c "az webapp create --resource-group $resourceGroup --plan $appServicePlan --name $mvcAppName --runtime `"DOTNETCORE|9.0`""

Write-Host "7. Configurando Variables de Entorno y Arranque en API..."
$apiSettings = @(
    @{ name = "ConnectionStrings__DefaultConnection"; value = $connectionString; slotSetting = $false },
    @{ name = "Jwt__Key"; value = "UnaClaveLargaYSeguraParaProduccion12345!"; slotSetting = $false },
    @{ name = "Jwt__Issuer"; value = "RayoExpresAPI"; slotSetting = $false },
    @{ name = "Jwt__Audience"; value = "RayoExpresClient"; slotSetting = $false },
    @{ name = "AllowedOrigins"; value = "https://$mvcAppName.azurewebsites.net"; slotSetting = $false },
    @{ name = "RunSeeder"; value = "true"; slotSetting = $false },
    @{ name = "WEBSITE_RUN_FROM_PACKAGE"; value = "1"; slotSetting = $false },
    @{ name = "WEBSITES_CONTAINER_START_TIME_LIMIT"; value = "1800"; slotSetting = $false }
)
$apiSettings | ConvertTo-Json -Depth 10 | Out-File "api_settings.json" -Encoding utf8
az webapp config appsettings set --resource-group $resourceGroup --name $apiAppName --settings "@api_settings.json"

az webapp config set --resource-group $resourceGroup --name $apiAppName --startup-file "dotnet Delivery.API.dll"

Write-Host "8. Configurando Variables de Entorno y Arranque en MVC..."
$mvcSettings = @(
    @{ name = "ApiUrl"; value = "https://$apiAppName.azurewebsites.net/"; slotSetting = $false },
    @{ name = "BlobStorageConnectionString"; value = $blobStorageConnectionString; slotSetting = $false },
    @{ name = "WEBSITE_RUN_FROM_PACKAGE"; value = "1"; slotSetting = $false }
)
$mvcSettings | ConvertTo-Json -Depth 10 | Out-File "mvc_settings.json" -Encoding utf8
az webapp config appsettings set --resource-group $resourceGroup --name $mvcAppName --settings "@mvc_settings.json"

az webapp config set --resource-group $resourceGroup --name $mvcAppName --startup-file "dotnet Delivery.MVC.dll"

# Importante: Activar ARR Affinity (Sticky Sessions) para que las sesiones en memoria del MVC funcionen si el plan se escala.
az webapp update --resource-group $resourceGroup --name $mvcAppName --set clientAffinityEnabled=true

Write-Host "9. Compilando y Publicando Código (Esto requiere estar en la carpeta raíz del proyecto)..."

# Limpiar publicaciones anteriores
Remove-Item -Recurse -Force ./publish_api -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ./publish_mvc -ErrorAction SilentlyContinue
Remove-Item -Force api.zip -ErrorAction SilentlyContinue
Remove-Item -Force mvc.zip -ErrorAction SilentlyContinue

# Cargar librería para comprimir de forma segura en Windows
Add-Type -AssemblyName System.IO.Compression.FileSystem

$apiZipPath = Join-Path (Get-Location) "api.zip"
$mvcZipPath = Join-Path (Get-Location) "mvc.zip"

Write-Host "Publicando API..."
dotnet publish Delivery.API/Delivery.API.csproj -c Release -o ./publish_api
[System.IO.Compression.ZipFile]::CreateFromDirectory((Join-Path (Get-Location) "publish_api"), $apiZipPath)
az webapp deployment source config-zip --resource-group $resourceGroup --name $apiAppName --src $apiZipPath

Write-Host "Publicando MVC..."
dotnet publish Delivery.MVC/Delivery.MVC.csproj -c Release -o ./publish_mvc
[System.IO.Compression.ZipFile]::CreateFromDirectory((Join-Path (Get-Location) "publish_mvc"), $mvcZipPath)
az webapp deployment source config-zip --resource-group $resourceGroup --name $mvcAppName --src $mvcZipPath

Write-Host "¡Despliegue Finalizado!"
Write-Host "URL API: https://$apiAppName.azurewebsites.net"
Write-Host "URL MVC: https://$mvcAppName.azurewebsites.net"
