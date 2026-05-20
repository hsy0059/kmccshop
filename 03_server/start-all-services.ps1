Write-Host "========================================" -ForegroundColor Cyan
Write-Host " 校园生活服务平台 - 启动所有后端服务" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$services = @(
    @{Name="User.Service"; Path="Services\User.Service"; Port=53222},
    @{Name="Merchant.Service"; Path="Services\Merchant.Service"; Port=53523},
    @{Name="Order.Service"; Path="Services\Order.Service"; Port=53216},
    @{Name="Delivery.Service"; Path="Services\Delivery.Service"; Port=53221},
    @{Name="Campus.Service"; Path="Services\Campus.Service"; Port=53211},
    @{Name="Social.Service"; Path="Services\Social.Service"; Port=53215},
    @{Name="Wallet.Service"; Path="Services\Wallet.Service"; Port=53224},
    @{Name="Coupon.Service"; Path="Services\Coupon.Service"; Port=53225},
    @{Name="ApiGateway"; Path="ApiGateway"; Port=53517}
)

foreach ($svc in $services) {
    Write-Host "Starting $($svc.Name) on port $($svc.Port)..." -ForegroundColor Green
    Start-Process dotnet -ArgumentList "run --project $($svc.Path) -- $($svc.Port)" -WindowStyle Minimized
    Start-Sleep -Seconds 2
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " All services launched!" -ForegroundColor Green
Write-Host " Gateway: http://localhost:53517" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan