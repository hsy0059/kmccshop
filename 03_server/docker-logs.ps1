[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$service = $args[0]
if (-not $service) {
    Write-Host "Usage: ./docker-logs.ps1 <service-name|all> [-f]" -ForegroundColor Yellow
    Write-Host "Example: ./docker-logs.ps1 apigateway -f" -ForegroundColor Yellow
    exit 1
}

if ($service -eq "all") {
    docker-compose logs $args[1..($args.Length - 1)]
} else {
    docker-compose logs $args
}
