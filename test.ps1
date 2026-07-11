$body = @{
    Email = "admin@rayoexpres.com"
    Password = "Admin123*"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri 'https://localhost:7278/api/Auth/login' -Method Post -ContentType 'application/json' -Body $body
$token = $response.token

Write-Host "Token: $token"

try {
    $authHeader = @{ Authorization = "Bearer $token" }
    $res2 = Invoke-RestMethod -Uri 'https://localhost:7278/api/AdminAprobaciones/repartidores/pendientes' -Method Get -Headers $authHeader
    $res2 | ConvertTo-Json
} catch {
    Write-Host "Error: $($_.Exception.Message)"
}
