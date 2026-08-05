[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Stop"

# ── 端口自动释放 ──────────────────────────────────────────────
# Docker Compose 映射到宿主机的端口
$mappedPorts = @(53517, 3306, 6379, 5672, 15672)

# 本地开发时各微服务监听的端口（启动 Docker 前应停止本地服务避免混淆）
$devPorts = @(53211, 53215, 53216, 53221, 53222, 53224, 53225, 53523, 53517, 53514)

function Release-Port {
    param(int $Port, [bool]$Force = $false)
    $connections = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    foreach ($conn in $connections) {
        $pid = $conn.OwningProcess
        if ($pid -eq 0 -or $pid -eq $PID) { continue }
        $proc = Get-Process -Id $pid -ErrorAction SilentlyContinue
        if (-not $proc) { continue }

        $procName = $proc.ProcessName
        $isDotnet = $procName -match "dotnet|ApiGateway|User\.Service|Merchant\.Service|Order\.Service|Social\.Service|Wallet\.Service|Coupon\.Service|Campus\.Service|Delivery\.Service"

        if ($Force -or $isDotnet) {
            Write-Host "  Port $Port occupied by $procName($pid) — auto-releasing..." -ForegroundColor Yellow
            Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
            Start-Sleep -Milliseconds 500
        } else {
            Write-Warning "Port $Port occupied by $procName($pid). Cannot auto-release (non-dotnet process). Please stop it manually."
            exit 1
        }
    }
}

Write-Host "Releasing host ports for Docker Compose ..." -ForegroundColor Cyan
foreach ($port in $mappedPorts) {
    Release-Port -Port $port -Force $true
}

Write-Host "Stopping local development services (if running) ..." -ForegroundColor Cyan
foreach ($port in $devPorts) {
    Release-Port -Port $port -Force $false
}

# ── 清理旧容器 ────────────────────────────────────────────────
$containers = @(
    "campus_apigateway",
    "campus_user_service", "campus_merchant_service", "campus_order_service", "campus_delivery_service",
    "campus_campus_service", "campus_social_service", "campus_wallet_service", "campus_coupon_service",
    "campus_mysql", "campus_redis", "campus_rabbitmq"
)

Write-Host "Stopping existing campus containers ..." -ForegroundColor Cyan
foreach ($c in $containers) {
    docker stop $c 2>$null | Out-Null
    docker rm $c 2>$null | Out-Null
}

# ── 启动 Docker Compose ───────────────────────────────────────
Write-Host "Starting all services with Docker Compose ..." -ForegroundColor Cyan
docker-compose up -d

Write-Host "Waiting for services to initialize ..." -ForegroundColor Cyan
Start-Sleep -Seconds 8

# ── 健康检查 ──────────────────────────────────────────────────
Write-Host "Health check ..." -ForegroundColor Cyan
$retry = 0
$maxRetry = 5
$healthy = $false
while ($retry -lt $maxRetry) {
    try {
        $resp = Invoke-RestMethod -Uri "http://localhost:53517/api/v1/merchant/list?page=1&pageSize=1" -TimeoutSec 3
        if ($resp.code -eq 0) {
            $healthy = $true
            break
        }
    } catch {
        $retry++
        Write-Host "  Retry $retry/$maxRetry ..." -ForegroundColor Yellow
        Start-Sleep -Seconds 3
    }
}

if ($healthy) {
    Write-Host ""
    Write-Host "Campus platform is running. API Gateway: http://localhost:53517" -ForegroundColor Green
    Write-Host "  MySQL:      localhost:3306" -ForegroundColor Gray
    Write-Host "  Redis:      localhost:6379" -ForegroundColor Gray
    Write-Host "  RabbitMQ:   localhost:15672 (guest/guest)" -ForegroundColor Gray
} else {
    Write-Warning "ApiGateway did not respond within expected time. Check logs: ./docker-logs.ps1 all"
}
