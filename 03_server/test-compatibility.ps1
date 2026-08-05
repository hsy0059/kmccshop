$baseUrl = "http://localhost:53517"
$headers = @{ "Content-Type" = "application/json" }
$pass = 0
$fail = 0

# Login User A
$body = @{ phone = "13800000001" } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/send-code" -Method Post -Body $body -Headers $headers
$codeA = $r.data.code
$body = @{ phone = "13800000001"; code = $codeA } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/login-by-code" -Method Post -Body $body -Headers $headers
$tokenA = $r.data.token
$h = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $tokenA" }

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Compatibility Verification Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Test 1: User A views own feedback list (should succeed)
Write-Host "`n[1] User A views feedback list (expect SUCCESS)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/user/feedback/list?page=1&pageSize=20" -Method Get -Headers $h
if ($r.code -eq 0) { Write-Host "  [PASS] Feedback list accessible" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] Feedback list blocked: code=$($r.code) msg=$($r.message)" -ForegroundColor Red; $fail++ }

# Test 2: User A views own order detail (should succeed)
Write-Host "`n[2] User A views order detail #4 (expect SUCCESS)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/4" -Method Get -Headers $h
if ($r.code -eq 0) { Write-Host "  [PASS] Order detail accessible" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] Order detail blocked: code=$($r.code) msg=$($r.message)" -ForegroundColor Red; $fail++ }

# Test 3: User A views my-orders (should succeed)
Write-Host "`n[3] User A views my-orders (expect SUCCESS)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/my-orders?page=1&pageSize=20" -Method Get -Headers $h
if ($r.code -eq 0) { Write-Host "  [PASS] My-orders accessible" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] My-orders blocked: code=$($r.code)" -ForegroundColor Red; $fail++ }

# Test 4: User A tries admin order list (should be blocked)
Write-Host "`n[4] User A tries admin order list (expect 403)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/list?page=1&pageSize=20" -Method Get -Headers $h
if ($r.code -eq 403) { Write-Host "  [PASS] Admin order list blocked: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] Admin list NOT blocked: code=$($r.code)" -ForegroundColor Red; $fail++ }

# Test 5: User A refunds own paid Order #5 (should succeed)
Write-Host "`n[5] User A refunds own Order #5 (expect SUCCESS)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/5/refund" -Method Post -Headers $h
if ($r.code -eq 0) { Write-Host "  [PASS] Refund succeeded: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] Refund failed: code=$($r.code) msg=$($r.message)" -ForegroundColor Red; $fail++ }

# Test 6: User B refunds User A's Order #6 (should be blocked 403)
Write-Host "`n[6] User B refunds User A's Order #6 (expect 403)..." -ForegroundColor Yellow
$body = @{ phone = "13800000002" } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/send-code" -Method Post -Body $body -Headers $headers
$codeB = $r.data.code
$body = @{ phone = "13800000002"; code = $codeB } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/login-by-code" -Method Post -Body $body -Headers $headers
$tokenB = $r.data.token
$h2 = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $tokenB" }
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/6/refund" -Method Post -Headers $h2
if ($r.code -eq 403) { Write-Host "  [PASS] Cross-user refund blocked: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] Cross-user refund NOT blocked: code=$($r.code)" -ForegroundColor Red; $fail++ }

# Test 7: Duplicate refund on Order #5 (should be blocked 400)
Write-Host "`n[7] User A duplicate refund Order #5 (expect 400)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/order/5/refund" -Method Post -Headers $h
if ($r.code -eq 400) { Write-Host "  [PASS] Duplicate refund blocked: $($r.message)" -ForegroundColor Green; $pass++ } else { Write-Host "  [FAIL] Duplicate refund NOT blocked: code=$($r.code)" -ForegroundColor Red; $fail++ }

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " Results: $pass passed, $fail failed" -ForegroundColor $(if ($fail -eq 0) { "Green" } else { "Red" })
Write-Host "========================================" -ForegroundColor Cyan
