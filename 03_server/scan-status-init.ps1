<#
.SYNOPSIS
    Scan all controllers for [FromBody] entities that have a Status property
    but missing Status=1 initialization.
.DESCRIPTION
    Static code analysis script that detects potential comment/content visibility bugs:
    1. Finds all entity types with a `public int Status` property
    2. Finds all controller methods accepting those entities via [FromBody]
    3. Checks if the method body contains `.Status = 1`
    4. Reports violations
.NOTES
    Can be integrated into CI/CD as a pre-merge gate.
#>
param(
    [string]$ServerDir = "e:\kmccXM\03_server"
)

$ErrorActionPreference = "Stop"
$violations = @()

# Step 1: Find all entity types with a Status property
Write-Host "[Scanner] Scanning for entities with Status property..." -ForegroundColor Cyan
$entityFiles = Get-ChildItem -Path $ServerDir -Recurse -Filter "*.cs" | Where-Object {
    $_.FullName -match "Models[\\/]Entities" -or $_.Name -match "Entities\.cs$"
}

$entitiesWithStatus = @{}
foreach ($file in $entityFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    # Match: class ClassName ... { ... public int Status ... }
    $classMatches = [regex]::Matches($content, 'class\s+(\w+)\s*(?:\{|:|\s)')
    foreach ($cm in $classMatches) {
        $className = $cm.Groups[1].Value
        # Check if this class has a Status property
        if ($content -match "class\s+$className[\s\S]*?public\s+int\s+Status\b") {
            if (-not $entitiesWithStatus.ContainsKey($className)) {
                $entitiesWithStatus[$className] = $file.FullName
                Write-Host "  Found: $className (has Status property)" -ForegroundColor Gray
            }
        }
    }
}
Write-Host "[Scanner] Found $($entitiesWithStatus.Count) entities with Status property." -ForegroundColor Cyan

# Step 2: Find all controller methods with [FromBody] of those entity types
Write-Host "`n[Scanner] Scanning controllers for [FromBody] parameters..." -ForegroundColor Cyan
$controllerFiles = Get-ChildItem -Path $ServerDir -Recurse -Filter "*Controller.cs"

foreach ($file in $controllerFiles) {
    $lines = Get-Content -Path $file.FullName
    $content = Get-Content -Path $file.FullName -Raw

    # Find all [FromBody] Type paramName patterns
    $bodyParamMatches = [regex]::Matches($content, '\[FromBody\]\s+(\w+)\s+(\w+)')
    foreach ($bm in $bodyParamMatches) {
        $paramType = $bm.Groups[1].Value
        $paramName = $bm.Groups[2].Value

        # Skip if the type doesn't have a Status property
        if (-not $entitiesWithStatus.ContainsKey($paramType)) { continue }

        # Find the line number of this [FromBody] occurrence
        $lineIdx = 0
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match "\[FromBody\]\s+$paramType\s+$paramName") {
                $lineIdx = $i
                break
            }
        }

        # Extract the method body (from the [FromBody] line to the next closing brace at same indent level)
        $methodBody = ""
        $braceCount = 0
        $started = $false
        for ($i = $lineIdx; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            if ($line -match '\{') { $braceCount += ([regex]::Matches($line, '\{')).Count; $started = $true }
            if ($line -match '\}') { $braceCount -= ([regex]::Matches($line, '\}')).Count }
            $methodBody += $line + "`n"
            if ($started -and $braceCount -le 0) { break }
        }

        # Check if the method body contains Status = 1 initialization
        $hasStatusInit = $false
        if ($methodBody -match "$paramName\.Status\s*=\s*1") {
            $hasStatusInit = $true
        }
        # Also check patterns like "xxx.Status = 1" where xxx might be set from paramName
        if ($methodBody -match "\.Status\s*=\s*1") {
            $hasStatusInit = $true
        }

        if (-not $hasStatusInit) {
            $relPath = $file.FullName.Replace($ServerDir + "\", "")
            $violations += [PSCustomObject]@{
                File = $relPath
                Line = $lineIdx + 1
                Entity = $paramType
                Param = $paramName
                Issue = "Missing '$paramName.Status = 1' initialization"
            }
            Write-Host "  [VIOLATION] ${relPath}:$($lineIdx + 1) - $paramType $paramName missing Status=1" -ForegroundColor Red
        }
    }
}

# Step 3: Report
Write-Host "`n========================================" -ForegroundColor Cyan
if ($violations.Count -eq 0) {
    Write-Host " [PASS] No violations found. All [FromBody] entities with Status property are properly initialized." -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    exit 0
} else {
    Write-Host " [FAIL] Found $($violations.Count) violation(s):" -ForegroundColor Red
    $violations | Format-Table -AutoSize
    Write-Host "========================================" -ForegroundColor Cyan
    exit 1
}
