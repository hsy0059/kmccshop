[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Stop"

Write-Host "Stopping campus platform containers ..." -ForegroundColor Cyan
docker-compose down

Write-Host "All campus services stopped." -ForegroundColor Green
