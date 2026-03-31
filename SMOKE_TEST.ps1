# Version: 0.1.0
# Total Characters: 3555
# Purpose: Bootstrap, build, and run a basic validation sequence for RiftResizer on Windows.

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "[RiftResizer Smoke] $Message"
}

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $repoRoot

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet SDK is required but was not found in PATH."
}

if (-not (Test-Path -LiteralPath ".\RiftResizer.sln")) {
    Write-Step "Solution file not found. Running bootstrap.ps1 first."
    & .\bootstrap.ps1
}

Write-Step "Building solution."
dotnet build .\RiftResizer.sln

Write-Step "Listing displays."
dotnet run --project .\src\RiftResizer.App\RiftResizer.App.csproj -- displays

Write-Step "Scanning for likely RIFT windows."
dotnet run --project .\src\RiftResizer.App\RiftResizer.App.csproj -- scan

Write-Step "Generating example 5-window layout on the primary display."
dotnet run --project .\src\RiftResizer.App\RiftResizer.App.csproj -- generate 5

Write-Step "Smoke test completed."
Write-Step "If scan did not detect RIFT windows, the next debugging step is to inspect window titles and process names on the target machine."

# End of script.
