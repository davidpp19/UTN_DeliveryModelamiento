# Script de Despliegue de RayoExpres en Microsoft Azure
# Requisitos: Tener instalado Azure CLI (https://aka.ms/installazurecli) y haber ejecutado 'az login'

# ============================================================
# CONFIGURACION FIJA - Nombres fijos para reutilizar recursos
# ============================================================
$resourceGroup     = "RayoExpres-RG-7"
$location          = "brazilsouth"
$dbServerName      = "rayoexpres-db-v7"
$dbAdminUser       = "rayoadmin"
$dbAdminPassword   = "SecurePassword123!"
$dbName            = "RayoExpresDB"
$storageAccountName = "rayoexpresstoragev7"
$appServicePlan    = "RayoExpres-Plan"
$apiAppName        = "rayoexpres-api-v7"
$mvcAppName        = "rayoexpres-mvc-v7"
# ============================================================

Write-Host "0. Registrando proveedores de Azure..."
az provider register --namespace Microsoft.Storage --wait | Out-Null
az provider register --namespace Microsoft.Web --wait | Out-Null
az provider register --namespace Microsoft.DBforPostgreSQL --wait | Out-Null

Write-Host "1. Creando/Verificando Resource Group..."
az group create --name $resourceGroup --location $location | Out-Null
Write-Host "   Resource Group: OK"

# ============================================================
# LIMPIEZA: Eliminar apps viejas con nombres distintos al final
# ============================================================
Write-Host "1.5 Limpiando apps antiguas del plan para liberar recursos..."
$allApps = az webapp list --resource-group $resourceGroup --query "[].name" -o tsv
foreach ($app in $allApps) {
    $app = $app.Trim()
    if ($app -ne $apiAppName -and $app -ne $mvcAppName -and $app -ne "") {
        Write-Host "   Eliminando app antigua: $app"
        az webapp delete --resource-group $resourceGroup --name $app --keep-empty-plan | Out-Null
    }
}
Write-Host "   Limpieza completada."

Write-Host "2. Creando Servidor PostgreSQL Flexible..."
$dbExists = az postgres flexible-server show --resource-group $resourceGroup --name $dbServerName 2>$null
if ($dbExists) {
    Write-Host "   Servidor de base de datos ya existe, reutilizando..."
} else {
    az postgres flexible-server create `
      --resource-group $resourceGroup `
      --name $dbServerName `
      --location $location `
      --admin-user $dbAdminUser `
      --admin-password $dbAdminPassword `
      --sku-name Standard_B1ms `
      --tier Burstable `
      --public-access 0.0.0.0

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR CRITICO: Fallo al crear el servidor PostgreSQL. Puede ser por limites de cuota."
        Write-Host "Cancelando despliegue para evitar errores en cadena..."
        exit 1
    }
}

Write-Host "2.1 Creando la Base de Datos..."
az postgres flexible-server db create `
  --resource-group $resourceGroup `
  --server-name $dbServerName `
  --name $dbName 2>$null | Out-Null
Write-Host "   Base de datos: OK"

$connectionString = "Server=$dbServerName.postgres.database.azure.com;Database=$dbName;Port=5432;User Id=$dbAdminUser;Password=$dbAdminPassword;Ssl Mode=Require;"

Write-Host "3. Creando Azure Blob Storage..."
$storageExists = az storage account show --name $storageAccountName --resource-group $resourceGroup 2>$null
if ($storageExists) {
    Write-Host "   Storage account ya existe, reutilizando..."
} else {
    az storage account create `
      --name $storageAccountName `
      --resource-group $resourceGroup `
      --location $location `
      --sku Standard_LRS `
      --allow-blob-public-access true
}
$blobStorageConnectionString = az storage account show-connection-string --name $storageAccountName --resource-group $resourceGroup --query connectionString -o tsv
Write-Host "   Blob Storage: OK"

Write-Host "4. Creando App Service Plan..."
$planExists = az appservice plan show --name $appServicePlan --resource-group $resourceGroup 2>$null
if ($planExists) {
    Write-Host "   App Service Plan ya existe, reutilizando..."
} else {
    az appservice plan create `
      --name $appServicePlan `
      --resource-group $resourceGroup `
      --location $location `
      --sku B1 `
      --is-linux
}
Write-Host "   App Service Plan: OK"

Write-Host "5. Creando/Verificando Web App para API..."
$apiExists = az webapp show --resource-group $resourceGroup --name $apiAppName 2>$null
if ($apiExists) {
    Write-Host "   Web App API ya existe, reutilizando..."
} else {
    cmd.exe /c "az webapp create --resource-group $resourceGroup --plan $appServicePlan --name $apiAppName --runtime `"DOTNETCORE|9.0`""
}
Write-Host "   Web App API: OK"

Write-Host "6. Creando/Verificando Web App para MVC..."
$mvcExists = az webapp show --resource-group $resourceGroup --name $mvcAppName 2>$null
if ($mvcExists) {
    Write-Host "   Web App MVC ya existe, reutilizando..."
} else {
    cmd.exe /c "az webapp create --resource-group $resourceGroup --plan $appServicePlan --name $mvcAppName --runtime `"DOTNETCORE|9.0`""
}
Write-Host "   Web App MVC: OK"

# ============================================================
# CONFIGURAR VARIABLES DE ENTORNO usando el metodo directo
# ============================================================
Write-Host "7. Configurando Variables de Entorno en API..."
az webapp config appsettings set `
    --resource-group $resourceGroup `
    --name $apiAppName `
    --settings `
    "ConnectionStrings__DefaultConnection=$connectionString" `
    "Jwt__Key=UnaClaveLargaYSeguraParaProduccion12345!" `
    "Jwt__Issuer=RayoExpresAPI" `
    "Jwt__Audience=RayoExpresClient" `
    "AllowedOrigins=https://$mvcAppName.azurewebsites.net" `
    "RunSeeder=true" `
    "WEBSITE_RUN_FROM_PACKAGE=1" `
    "WEBSITES_CONTAINER_START_TIME_LIMIT=1800" | Out-Null

az webapp config set `
    --resource-group $resourceGroup `
    --name $apiAppName `
    --startup-file "dotnet Delivery.API.dll" | Out-Null
Write-Host "   Config API: OK"

Write-Host "8. Configurando Variables de Entorno en MVC..."
az webapp config appsettings set `
    --resource-group $resourceGroup `
    --name $mvcAppName `
    --settings `
    "ApiUrl=https://$apiAppName.azurewebsites.net/" `
    "BlobStorageConnectionString=$blobStorageConnectionString" `
    "WEBSITE_RUN_FROM_PACKAGE=1" | Out-Null

az webapp config set `
    --resource-group $resourceGroup `
    --name $mvcAppName `
    --startup-file "dotnet Delivery.MVC.dll" | Out-Null

az webapp update `
    --resource-group $resourceGroup `
    --name $mvcAppName `
    --set clientAffinityEnabled=true | Out-Null
Write-Host "   Config MVC: OK"

# ============================================================
# COMPILAR Y DESPLEGAR
# ============================================================
Write-Host "9. Compilando y Publicando Codigo..."

Remove-Item -Recurse -Force ./publish_api -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ./publish_mvc -ErrorAction SilentlyContinue
Remove-Item -Force api.zip -ErrorAction SilentlyContinue
Remove-Item -Force mvc.zip -ErrorAction SilentlyContinue

Add-Type -AssemblyName System.IO.Compression.FileSystem
$apiZipPath = Join-Path (Get-Location) "api.zip"
$mvcZipPath = Join-Path (Get-Location) "mvc.zip"

Write-Host "Publicando API..."
dotnet publish Delivery.API/Delivery.API.csproj -c Release -o ./publish_api
[System.IO.Compression.ZipFile]::CreateFromDirectory((Join-Path (Get-Location) "publish_api"), $apiZipPath)
az webapp deploy --resource-group $resourceGroup --name $apiAppName --src-path $apiZipPath --type zip --timeout 1800000

Write-Host "Publicando MVC..."
dotnet publish Delivery.MVC/Delivery.MVC.csproj -c Release -o ./publish_mvc
[System.IO.Compression.ZipFile]::CreateFromDirectory((Join-Path (Get-Location) "publish_mvc"), $mvcZipPath)
az webapp deploy --resource-group $resourceGroup --name $mvcAppName --src-path $mvcZipPath --type zip --timeout 1800000

Write-Host ""
Write-Host "=========================================="
Write-Host "   DESPLIEGUE FINALIZADO"
Write-Host "=========================================="
Write-Host "URL API: https://$apiAppName.azurewebsites.net"
Write-Host "URL MVC: https://$mvcAppName.azurewebsites.net"
Write-Host "=========================================="
