param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("dev", "stage")]
    [string]$Environment,

    [Parameter(Mandatory = $true)]
    [ValidateSet("operations-api", "recruitment-api", "operations-web", "recruitment-web", "all")]
    [string]$App,

    [string]$ResourceGroup = "moehe-devtest-qc",

    [string]$Configuration
)

$ErrorActionPreference = "Stop"

# ==============================
# Resolve build configuration
# ==============================

if (-not $Configuration) {
    if ($Environment -eq "dev") {
        $Configuration = "Debug"
    }
    else {
        $Configuration = "Release"
    }
}

Write-Host "Environment: $Environment"
Write-Host "Target App:   $App"
Write-Host "Config:       $Configuration"
Write-Host "RG:           $ResourceGroup"
Write-Host ""

# ==============================
# App Service mapping
# ==============================

$apps = @{
    "operations-api" = @{
        "dev" = @{
            appService = "App-Tawtheef-Operations-Api-Dev-01"
            project    = "src/Tawtheef.APIs/Operations.API/Operations.API.csproj"
            type       = "dotnet"
        }
        "stage" = @{
            appService = "App-Tawtheef-Operations-Api-Stg-QC"
            project    = "src/Tawtheef.APIs/Operations.API/Operations.API.csproj"
            type       = "dotnet"
        }
    }

    "recruitment-api" = @{
        "dev" = @{
            appService = "App-Tawtheef-Recruitment-Api-Dev-01"
            project    = "src/Tawtheef.APIs/Recruitment.API/Recruitment.API.csproj"
            type       = "dotnet"
        }
        "stage" = @{
            appService = "App-Tawtheef-Recruitment-Api-Stg-QC"
            project    = "src/Tawtheef.APIs/Recruitment.API/Recruitment.API.csproj"
            type       = "dotnet"
        }
    }

    "operations-web" = @{
        "dev" = @{
            appService = "App-Tawtheef-Operations-Web-Dev-01"
            project    = "src/Tawtheef.Webs/Operations.Web"
            type       = "frontend"
        }
        "stage" = @{
            appService = "App-Tawtheef-Operations-Web-Stg-QC"
            project    = "src/Tawtheef.Webs/Operations.Web"
            type       = "frontend"
        }
    }

    "recruitment-web" = @{
        "dev" = @{
            appService = "App-Tawtheef-Recruitment-Web-Dev-01"
            project    = "src/Tawtheef.Webs/Recruitment.Web"
            type       = "frontend"
        }
        "stage" = @{
            appService = "App-Tawtheef-Recruitment-Web-Stg-QC"
            project    = "src/Tawtheef.Webs/Recruitment.Web"
            type       = "frontend"
        }
    }
}

# ==============================
# Helpers
# ==============================

function Assert-AzureLogin {
    Write-Host "Checking Azure login..."
    az account show | Out-Null
}

function New-CleanDirectory {
    param([string]$Path)

    if (Test-Path $Path) {
        Remove-Item $Path -Recurse -Force
    }

    New-Item -ItemType Directory -Path $Path | Out-Null
}

function Deploy-ZipToAppService {
    param(
        [string]$AppServiceName,
        [string]$ZipPath
    )

    Write-Host "Deploying ZIP to App Service: $AppServiceName"

    az webapp deploy `
        --resource-group $ResourceGroup `
        --name $AppServiceName `
        --src-path $ZipPath `
        --type zip

    Write-Host "Restarting App Service: $AppServiceName"

    az webapp restart `
        --resource-group $ResourceGroup `
        --name $AppServiceName
}

function Deploy-DotNetApp {
    param(
        [string]$Key,
        [hashtable]$Target
    )

    $appServiceName = $Target.appService
    $projectPath = $Target.project

    $publishDir = ".deploy/$Key/$Environment/publish"
    $zipPath = ".deploy/$Key-$Environment.zip"

    Write-Host ""
    Write-Host "========================================"
    Write-Host "Deploying .NET app: $Key"
    Write-Host "App Service: $appServiceName"
    Write-Host "Project: $projectPath"
    Write-Host "========================================"

    New-CleanDirectory $publishDir

    dotnet restore $projectPath

    dotnet publish $projectPath `
        --configuration $Configuration `
        --output $publishDir

    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }

    Compress-Archive `
        -Path "$publishDir/*" `
        -DestinationPath $zipPath `
        -Force

    Deploy-ZipToAppService `
        -AppServiceName $appServiceName `
        -ZipPath $zipPath
}

function Deploy-FrontendApp {
    param(
        [string]$Key,
        [hashtable]$Target
    )

    $appServiceName = $Target.appService
    $projectPath = $Target.project

    $publishDir = ".deploy/$Key/$Environment/publish"
    $zipPath = ".deploy/$Key-$Environment.zip"

    Write-Host ""
    Write-Host "========================================"
    Write-Host "Deploying frontend app: $Key"
    Write-Host "App Service: $appServiceName"
    Write-Host "Project: $projectPath"
    Write-Host "========================================"

    New-CleanDirectory $publishDir

    Push-Location $projectPath

    npm ci

    if ($Environment -eq "dev") {
        npm run build:pre-stage
    }
    else {
        npm run build:stage
    }

    Pop-Location

    # Adjust this if your frontend output folder is different.
    # Common options: dist, build, .next
    $frontendDist = Join-Path $projectPath "dist"

    if (-not (Test-Path $frontendDist)) {
        throw "Frontend build output folder not found: $frontendDist. Update `$frontendDist in the script."
    }

    Copy-Item "$frontendDist/*" $publishDir -Recurse -Force

    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }

    Compress-Archive `
        -Path "$publishDir/*" `
        -DestinationPath $zipPath `
        -Force

    Deploy-ZipToAppService `
        -AppServiceName $appServiceName `
        -ZipPath $zipPath
}

function Deploy-One {
    param([string]$Key)

    $target = $apps[$Key][$Environment]

    if ($target.type -eq "dotnet") {
        Deploy-DotNetApp -Key $Key -Target $target
    }
    elseif ($target.type -eq "frontend") {
        Deploy-FrontendApp -Key $Key -Target $target
    }
    else {
        throw "Unknown app type: $($target.type)"
    }
}

# ==============================
# Main
# ==============================

Assert-AzureLogin

if ($App -eq "all") {
    Deploy-One "operations-api"
    Deploy-One "recruitment-api"
    Deploy-One "operations-web"
    Deploy-One "recruitment-web"
}
else {
    Deploy-One $App
}

Write-Host ""
Write-Host "Deployment completed successfully."