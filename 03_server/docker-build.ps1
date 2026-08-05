[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Stop"

$services = @(
    @{ Dockerfile = "ApiGateway/Dockerfile"; Image = "campus/apigateway" },
    @{ Dockerfile = "Services/Campus.Service/Dockerfile"; Image = "campus/campus.service" },
    @{ Dockerfile = "Services/Coupon.Service/Dockerfile"; Image = "campus/coupon.service" },
    @{ Dockerfile = "Services/Delivery.Service/Dockerfile"; Image = "campus/delivery.service" },
    @{ Dockerfile = "Services/Merchant.Service/Dockerfile"; Image = "campus/merchant.service" },
    @{ Dockerfile = "Services/Order.Service/Dockerfile"; Image = "campus/order.service" },
    @{ Dockerfile = "Services/Social.Service/Dockerfile"; Image = "campus/social.service" },
    @{ Dockerfile = "Services/User.Service/Dockerfile"; Image = "campus/user.service" },
    @{ Dockerfile = "Services/Wallet.Service/Dockerfile"; Image = "campus/wallet.service" }
)

foreach ($svc in $services) {
    Write-Host "Building $($svc.Image) from $($svc.Dockerfile) ..." -ForegroundColor Cyan
    docker build -t $svc.Image -f $svc.Dockerfile .
    if ($LASTEXITCODE -ne 0) { throw "Build failed for $($svc.Image)" }
}

Write-Host "All images built successfully." -ForegroundColor Green
