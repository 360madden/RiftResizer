# Version: 0.1.0
# Total Characters: 6122
# Purpose: Generate the missing .NET solution and project files for RiftResizer, then wire project references so the repo becomes buildable.

param(
    [string]$SolutionName = "RiftResizer",
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "[RiftResizer] $Message"
}

function Ensure-Command {
    param([string]$Name)

    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Required command not found: $Name"
    }
}

function Ensure-Directory {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path | Out-Null
    }
}

function Ensure-Project {
    param(
        [string]$Path,
        [string]$Template,
        [string]$Framework
    )

    if (Test-Path -LiteralPath $Path) {
        if (-not $Force) {
            Write-Step "Project already exists: $Path"
            return
        }

        Remove-Item -LiteralPath $Path -Force
    }

    $projectDirectory = Split-Path -Parent $Path
    Ensure-Directory $projectDirectory

    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($Path)
    dotnet new $Template --framework $Framework --name $projectName --output $projectDirectory | Out-Null

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Failed to generate project: $Path"
    }

    Write-Step "Generated project: $Path"
}

function Replace-FileContents {
    param(
        [string]$Path,
        [string]$Contents
    )

    [System.IO.File]::WriteAllText($Path, $Contents, [System.Text.Encoding]::UTF8)
    Write-Step "Wrote file: $Path"
}

Ensure-Command dotnet

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $repoRoot

$solutionPath = Join-Path $repoRoot "$SolutionName.sln"
$coreProject = Join-Path $repoRoot "src/RiftResizer.Core/RiftResizer.Core.csproj"
$layoutsProject = Join-Path $repoRoot "src/RiftResizer.Layouts/RiftResizer.Layouts.csproj"
$windowingProject = Join-Path $repoRoot "src/RiftResizer.Windowing/RiftResizer.Windowing.csproj"
$profilesProject = Join-Path $repoRoot "src/RiftResizer.Profiles/RiftResizer.Profiles.csproj"
$appProject = Join-Path $repoRoot "src/RiftResizer.App/RiftResizer.App.csproj"

if ((Test-Path -LiteralPath $solutionPath) -and $Force) {
    Remove-Item -LiteralPath $solutionPath -Force
}

if (-not (Test-Path -LiteralPath $solutionPath)) {
    dotnet new sln --name $SolutionName | Out-Null
    Write-Step "Created solution: $solutionPath"
} else {
    Write-Step "Solution already exists: $solutionPath"
}

Ensure-Project -Path $coreProject -Template classlib -Framework net9.0
Ensure-Project -Path $layoutsProject -Template classlib -Framework net9.0
Ensure-Project -Path $windowingProject -Template classlib -Framework net9.0-windows
Ensure-Project -Path $profilesProject -Template classlib -Framework net9.0
Ensure-Project -Path $appProject -Template console -Framework net9.0-windows

$appCsproj = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0-windows</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""..\RiftResizer.Core\RiftResizer.Core.csproj"" />
    <ProjectReference Include=""..\RiftResizer.Layouts\RiftResizer.Layouts.csproj"" />
    <ProjectReference Include=""..\RiftResizer.Windowing\RiftResizer.Windowing.csproj"" />
    <ProjectReference Include=""..\RiftResizer.Profiles\RiftResizer.Profiles.csproj"" />
  </ItemGroup>
</Project>
"@

$coreCsproj = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
"@

$layoutsCsproj = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""..\RiftResizer.Core\RiftResizer.Core.csproj"" />
  </ItemGroup>
</Project>
"@

$windowingCsproj = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0-windows</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""..\RiftResizer.Core\RiftResizer.Core.csproj"" />
  </ItemGroup>
</Project>
"@

$profilesCsproj = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""..\RiftResizer.Core\RiftResizer.Core.csproj"" />
  </ItemGroup>
</Project>
"@

Replace-FileContents -Path $coreProject -Contents $coreCsproj
Replace-FileContents -Path $layoutsProject -Contents $layoutsCsproj
Replace-FileContents -Path $windowingProject -Contents $windowingCsproj
Replace-FileContents -Path $profilesProject -Contents $profilesCsproj
Replace-FileContents -Path $appProject -Contents $appCsproj

$projects = @($coreProject, $layoutsProject, $windowingProject, $profilesProject, $appProject)
foreach ($project in $projects) {
    dotnet sln $solutionPath add $project | Out-Null
}

Write-Step "Bootstrap complete."
Write-Step "Next: dotnet build $solutionPath"

# End of script.
