$files = Get-ChildItem -Path C:\Users\t-a.jaber\RiderProjects\Tawtheef\src -Filter *.cs -Recurse
$pattern = "\b(SendCommandAsync|SendQueryAsync)\s*<((?>[^<>]+|<(?<c>)|>(?<-c>))*(?(c)(?!)))>\s*\("
$patternNonGeneric = "\b(SendCommandAsync|SendQueryAsync)\s*\("

foreach ($file in $files) {
    if ($file.FullName -match "MigrateToMediatr.ps1") { continue }
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    $content = [regex]::Replace($content, $pattern, "Send(")
    $content = [regex]::Replace($content, $patternNonGeneric, "Send(")
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8
    }
}
