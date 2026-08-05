$baseUrl = "http://localhost:53517"
$headers = @{ "Content-Type" = "application/json" }
$pass = 0
$fail = 0

# Login test user
$body = @{ phone = "13800000001" } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/send-code" -Method Post -Body $body -Headers $headers
$token = (Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/login-by-code" -Method Post -Body (@{ phone = "13800000001"; code = $r.data.code } | ConvertTo-Json) -Headers $headers).data.token
$h = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $token" }

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Comment Status Verification Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Step 1: Create a test post
Write-Host "`n[1] Create test post..." -ForegroundColor Yellow
$postBody = @{ title = "comment-test-post"; content = "for testing comment status"; categoryId = 1 } | ConvertTo-Json -Depth 3
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/social/post/create" -Method Post -Body $postBody -Headers $h
$postId = $r.data.id
Write-Host "  Post created: id=$postId, status=$($r.data.status)" -ForegroundColor Green

# Step 2: Add a comment to the post
Write-Host "`n[2] Add comment to post..." -ForegroundColor Yellow
$commentBody = @{ content = "test comment 1" } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/social/post/$postId/comment" -Method Post -Body $commentBody -Headers $h
$commentId = $r.data.id
$commentStatus = $r.data.status
Write-Host "  Comment created: id=$commentId, status=$commentStatus" -ForegroundColor Green
if ($commentStatus -eq 1) {
    Write-Host "  [PASS] Status initialized to 1" -ForegroundColor Green
    $pass++
} else {
    Write-Host "  [FAIL] Status is $commentStatus, expected 1" -ForegroundColor Red
    $fail++
}

# Step 3: Add a second comment (simulate client sending status=0)
Write-Host "`n[3] Add comment with status=0 in payload (expect server overrides to 1)..." -ForegroundColor Yellow
$badCommentBody = @{ content = "test comment 2"; status = 0 } | ConvertTo-Json
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/social/post/$postId/comment" -Method Post -Body $badCommentBody -Headers $h
$comment2Status = $r.data.status
Write-Host "  Comment created: id=$($r.data.id), status=$comment2Status"
if ($comment2Status -eq 1) {
    Write-Host "  [PASS] Server overrode status to 1" -ForegroundColor Green
    $pass++
} else {
    Write-Host "  [FAIL] Status is $comment2Status, expected 1" -ForegroundColor Red
    $fail++
}

# Step 4: Fetch comments and verify both are visible
Write-Host "`n[4] Fetch comments (expect 2 visible)..." -ForegroundColor Yellow
$r = Invoke-RestMethod -Uri "$baseUrl/api/v1/social/post/$postId/comments?page=1&pageSize=20" -Method Get -Headers $h
$total = $r.data.total
$list = $r.data.list
Write-Host "  Total comments: $total"
foreach ($c in $list) {
    Write-Host "    - id=$($c.id), status=$($c.status), content=$($c.content)"
}
if ($total -eq 2) {
    Write-Host "  [PASS] Both comments visible" -ForegroundColor Green
    $pass++
} else {
    Write-Host "  [FAIL] Expected 2 comments, got $total" -ForegroundColor Red
    $fail++
}

# Step 5: Verify each comment has status=1 in the response
Write-Host "`n[5] Verify all comments have status=1..." -ForegroundColor Yellow
$allStatus1 = $true
foreach ($c in $list) {
    if ($c.status -ne 1) { $allStatus1 = $false; Write-Host "  Comment $($c.id) has status=$($c.status)" -ForegroundColor Red }
}
if ($allStatus1) {
    Write-Host "  [PASS] All comments have status=1" -ForegroundColor Green
    $pass++
} else {
    Write-Host "  [FAIL] Some comments have status != 1" -ForegroundColor Red
    $fail++
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " Results: $pass passed, $fail failed" -ForegroundColor $(if ($fail -eq 0) { "Green" } else { "Red" })
Write-Host "========================================" -ForegroundColor Cyan
