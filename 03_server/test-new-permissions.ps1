$baseUrl = "http://localhost:53517"
$headers = @{ "Content-Type" = "application/json" }
$pass = 0
$fail = 0

# Login User A (student)
$body = @{ phone = "13800000001" } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/send-code" -Method Post -Body $body -Headers $headers
$tokenA = (Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/login-by-code" -Method Post -Body (@{ phone = "13800000001"; code = $r.data.code } | ConvertTo-Json) -Headers $headers).data.token
$h = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $tokenA" }

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " New Permission Fix Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Test 1: Non-merchant tries merchant order list (expect 403)
Write-Host "`n[1] Student tries merchant order list (expect 403)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/merchant/list?page=1&pageSize=20" -Method Get -Headers $h
if ($r.code -eq 403) { Write-Host "  [PASS] Blocked: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] code=$($r.code)" -ForegroundColor Red; $fail++ }

# Test 2: Non-merchant tries merchant stats (expect 403)
Write-Host "`n[2] Student tries merchant stats (expect 403)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/merchant-stats?merchantId=1" -Method Get -Headers $h
if ($r.code -eq 403) { Write-Host "  [PASS] Blocked: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] code=$($r.code)" -ForegroundColor Red; $fail++ }

# Test 3: Non-merchant tries merchant stats endpoint (expect 403)
Write-Host "`n[3] Student tries merchant/stats (expect 403)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/merchant/stats" -Method Get -Headers $h
if ($r.code -eq 403) { Write-Host "  [PASS] Blocked: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] code=$($r.code)" -ForegroundColor Red; $fail++ }

# Test 4: User comments on own order (expect SUCCESS)
Write-Host "`n[4] User comments on own order (expect SUCCESS)..." -ForegroundColor Yellow
$orderBody = @{ merchantId = 1; remark = "comment test"; items = @(@{ productId = 1; productName = "test"; price = 5.0; quantity = 1 }) } | ConvertTo-Json -Depth 5
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/create" -Method Post -Body $orderBody -Headers $h
$orderId = $r.data.id
Invoke-RestMethod -Uri "$baseUrl/api/v1/order/$orderId/pay" -Method Post -Headers $h | Out-Null
$commentBody = @{ orderId = $orderId; targetType = 1; targetId = 1; rating = 5; content = "good" } | ConvertTo-Json -Depth 5
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/comment/submit" -Method Post -Body $commentBody -Headers $h
if ($r.code -eq 0) { Write-Host "  [PASS] Comment succeeded: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] code=$($r.code) msg=$($r.message)" -ForegroundColor Red; $fail++ }

# Test 5: User B comments on User A's order (expect 403)
Write-Host "`n[5] User B comments on User A's order (expect 403)..." -ForegroundColor Yellow
$body = @{ phone = "13800000002" } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/send-code" -Method Post -Body $body -Headers $headers
$tokenB = (Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/login-by-code" -Method Post -Body (@{ phone = "13800000002"; code = $r.data.code } | ConvertTo-Json) -Headers $headers).data.token
$h2 = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $tokenB" }
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/comment/submit" -Method Post -Body $commentBody -Headers $h2
if ($r.code -eq 403) { Write-Host "  [PASS] Cross-user comment blocked: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] code=$($r.code) msg=$($r.message)" -ForegroundColor Red; $fail++ }

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " Results: $pass passed, $fail failed" -ForegroundColor $(if ($fail -eq 0) { "Green" } else { "Red" })
Write-Host "========================================" -ForegroundColor Cyan
