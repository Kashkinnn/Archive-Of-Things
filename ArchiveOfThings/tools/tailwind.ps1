param([switch]$Watch)

$root = Split-Path $PSScriptRoot -Parent
$exe = Join-Path $PSScriptRoot "tailwindcss.exe"

if (-not (Test-Path $exe)) {
    Invoke-WebRequest "https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-windows-x64.exe" -OutFile $exe
}

Push-Location $root
try {
    $cli = @("-i", "Styles/app.input.css", "-o", "wwwroot/app.css")
    if ($Watch) { $cli += "--watch" } else { $cli += "--minify" }
    & $exe @cli
}
finally {
    Pop-Location
}
