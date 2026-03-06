$files = Get-ChildItem -Path C:\Users\t-a.jaber\RiderProjects\Tawtheef\src -Filter *.cs -Recurse

foreach ($file in $files) {
    if ($file.FullName -match "MigrateToMediatr.ps1") { continue }
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    $content = $content -replace "using Cortex\.Mediator\.Commands;", "using MediatR;"
    $content = $content -replace "using Cortex\.Mediator\.Queries;", "using MediatR;"
    $content = $content -replace "using Cortex\.Mediator;", "using MediatR;"
    
    $content = $content -replace "\bICommand<", "IRequest<"
    $content = $content -replace "\bICommand\b", "IRequest"
    $content = $content -replace "\bIQuery<", "IRequest<"
    $content = $content -replace "\bICommandHandler<", "IRequestHandler<"
    $content = $content -replace "\bIQueryHandler<", "IRequestHandler<"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8
    }
}

$csprojs = Get-ChildItem -Path C:\Users\t-a.jaber\RiderProjects\Tawtheef\src -Filter *.csproj -Recurse
foreach ($csproj in $csprojs) {
    $content = Get-Content $csproj.FullName -Raw
    if ($content -match "Cortex\.Mediator") {
        # remove any version
        $content = $content -replace '<PackageReference Include="Cortex\.Mediator"(.*?)/>', '<PackageReference Include="MediatR" Version="12.5.0" />'
        Set-Content -Path $csproj.FullName -Value $content -Encoding UTF8
    }
}
