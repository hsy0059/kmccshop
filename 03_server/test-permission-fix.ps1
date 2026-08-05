$baseUrl = "http://localhost:53517"
$headers = @{ "Content-Type" = "application/json" }
$pass = 0
$fail = 0

function Send-Code($phone) {
    $body = @{ phone = $phone } | ConvertTo-Json
    $r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/send-code" -Method Post -Body $body -Headers $headers
    return $r.data.code
}

function Login-ByCode($phone, $code) {
    $body = @{ phone = $phone; code = $code } | ConvertTo-Json
    $r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/login-by-code" -Method Post -Body $body -Headers $headers
    return $r.data.token
}

function Create-Order($token) {
    $body = @{
        merchantId = 1
        remark = "权限测试订单"
        items = @(
            @{ productId = 1; productName = "测试商品"; price = 10.0; quantity = 2 }
        )
    } | ConvertTo-Json -Depth 5
    $h = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $token" }
    $r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/create" -Method Post -Body $body -Headers $h
    return $r.data
}

function Cancel-Order($token, $orderId) {
    $h = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $token" }
    $r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/$orderId/cancel" -Method Post -Headers $h
    return $r
}

function Pay-Order($token, $orderId) {
    $h = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $token" }
    $r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/$orderId/pay" -Method Post -Headers $h
    return $r
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Order Permission Verification Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Step 1: Create/Login User A
Write-Host "`n[1] Creating test User A (13800000001)..." -ForegroundColor Yellow
$codeA = Send-Code "13800000001"
$tokenA = Login-ByCode "13800000001" $codeA
Write-Host "  User A token obtained: $($tokenA.Substring(0,20))..." -ForegroundColor Green

# Step 2: Create/Login User B
Write-Host "`n[2] Creating test User B (13800000002)..." -ForegroundColor Yellow
$codeB = Send-Code "13800000002"
$tokenB = Login-ByCode "13800000002" $codeB
Write-Host "  User B token obtained: $($tokenB.Substring(0,20))..." -ForegroundColor Green

# Step 3: User A creates Order #1 (for cancel test - own order)
Write-Host "`n[3] User A creates Order #1..." -ForegroundColor Yellow
$order1 = Create-Order $tokenA
Write-Host "  Order created: ID=$($order1.id), No=$($order1.orderNo), Status=$($order1.status)" -ForegroundColor Green

# Step 4: User A cancels own order -> should SUCCEED
Write-Host "`n[4] User A cancels own Order #1 (expect SUCCESS)..." -ForegroundColor Yellow
$r4 = Cancel-Order $tokenA $order1.id
if ($r4.code -eq 0) {
    Write-Host "  [PASS] Cancel own order succeeded: $($r4.message)" -ForegroundColor Green
    $pass++
} else {
    Write-Host "  [FAIL] Cancel own order failed: code=$($r4.code) msg=$($r4.message)" -ForegroundColor Red
    $fail++
}

# Step 5: User A creates Order #2 (for cross-user cancel test)
Write-Host "`n[5] User A creates Order #2..." -ForegroundColor Yellow
$order2 = Create-Order $tokenA
Write-Host "  Order created: ID=$($order2.id), No=$($order2.orderNo)" -ForegroundColor Green

# Step 6: User B tries to cancel User A's order -> should FAIL with 403
Write-Host "`n[6] User B cancels User A's Order #2 (expect 403 FORBIDDEN)..." -ForegroundColor Yellow
$r6 = Cancel-Order $tokenB $order2.id
if ($r6.code -eq 403) {
    Write-Host "  [PASS] Cross-user cancel blocked: $($r6.message)" -ForegroundColor Green
    $pass++
} else {
    Write-Host "  [FAIL] Cross-user cancel NOT blocked: code=$($r6.code) msg=$($r6.message)" -ForegroundColor Red
    $fail++
}

# Step 7: User A pays own order -> should SUCCEED
Write-Host "`n[7] User A pays own Order #2 (expect SUCCESS)..." -ForegroundColor Yellow
$r7 = Pay-Order $tokenA $order2.id
if ($r7.code -eq 0) {
    Write-Host "  [PASS] Pay own order succeeded: $($r7.message)" -ForegroundColor Green
    $pass++
} else {
    Write-Host "  [FAIL] Pay own order failed: code=$($r7.code) msg=$($r7.message)" -ForegroundColor Red
    $fail++
}

# Step 8: User A creates Order #3 (for cross-user pay test)
Write-Host "`n[8] User A creates Order #3..." -ForegroundColor Yellow
$order3 = Create-Order $tokenA
Write-Host "  Order created: ID=$($order3.id), No=$($order3.orderNo)" -ForegroundColor Green

# Step 9: User B tries to pay User A's order -> should FAIL with 403
Write-Host "`n[9] User B pays User A's Order #3 (expect 403 FORBIDDEN)..." -ForegroundColor Yellow
$r9 = Pay-Order $tokenB $order3.id
if ($r9.code -eq 403) {
    Write-Host "  [PASS] Cross-user pay blocked: $($r9.message)" -ForegroundColor Green
    $pass++
} else {
    Write-Host "  [FAIL] Cross-user pay NOT blocked: code=$($r9.code) msg=$($r9.message)" -ForegroundColor Red
    $fail++
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " Test Results: $pass passed, $fail failed" -ForegroundColor $(if ($fail -eq 0) { "Green" } else { "Red" })
Write-Host "========================================" -ForegroundColor Cyan
