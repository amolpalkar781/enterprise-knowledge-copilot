$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($env:OPENAI_API_KEY)) {
    throw 'Set OPENAI_API_KEY in this PowerShell session before creating the vector store.'
}

$headers = @{ Authorization = "Bearer $env:OPENAI_API_KEY" }
$body = @{ name = 'Enterprise Knowledge Copilot' } | ConvertTo-Json
$response = Invoke-RestMethod `
    -Method Post `
    -Uri 'https://api.openai.com/v1/vector_stores' `
    -Headers $headers `
    -ContentType 'application/json' `
    -Body $body

Write-Output "Vector store ID: $($response.id)"
Write-Output 'Copy this ID into OPENAI_VECTOR_STORE_ID in the API terminal.'
