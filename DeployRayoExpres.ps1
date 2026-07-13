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
  --database-name $dbName

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

Write-Host "7. Configurando Variables de Entorno en API..."
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name $apiAppName `
  --settings `
    ConnectionStrings__DefaultConnection=$connectionString `
    Jwt__Key="UnaClaveLargaYSeguraParaProduccion12345!" `
    Jwt__Issuer="RayoExpresAPI" `
    Jwt__Audience="RayoExpresClient" `
    AllowedOrigins="https://$mvcAppName.azurewebsites.net" `
    RunSeeder="true"

Write-Host "8. Configurando Variables de Entorno en MVC..."
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name $mvcAppName `
  --settings `
    ApiUrl="https://$apiAppName.azurewebsites.net/" `
    BlobStorageConnectionString=$blobStorageConnectionString

# Importante: Activar ARR Affinity (Sticky Sessions) para que las sesiones en memoria del MVC funcionen si el plan se escala.
az webapp update --resource-group $resourceGroup --name $mvcAppName --set clientAffinityEnabled=true

Write-Host "9. Compilando y Publicando Código (Esto requiere estar en la carpeta raíz del proyecto)..."

# Publicando API
dotnet publish Delivery.API/Delivery.API.csproj -c Release -o ./publish_api
Compress-Archive -Path ./publish_api/* -DestinationPath api.zip -Force
az webapp deploy --resource-group $resourceGroup --name $apiAppName --src-path api.zip --type zip

# Publicando MVC
dotnet publish Delivery.MVC/Delivery.MVC.csproj -c Release -o ./publish_mvc
Compress-Archive -Path ./publish_mvc/* -DestinationPath mvc.zip -Force
az webapp deploy --resource-group $resourceGroup --name $mvcAppName --src-path mvc.zip --type zip

Write-Host "¡Despliegue Finalizado!"
Write-Host "URL API: https://$apiAppName.azurewebsites.net"
Write-Host "URL MVC: https://$mvcAppName.azurewebsites.net"
