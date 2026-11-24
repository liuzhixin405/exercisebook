# 启动 coldhub 的全部进程，每个进程一个独立的控制台窗口（关窗口即停止）。
# 用法（PowerShell）：  .\run-all.ps1        停止：  .\stop-all.ps1
# 需要：本机 Redis/Valkey 在 127.0.0.1:6379；已执行过 dotnet build ColdHub.slnx。
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

function Start-Host($dir, $dll, $title, $envVars) {
    $dllPath = Join-Path $root "src\$dir\bin\Debug\net10.0\$dll.dll"
    if (-not (Test-Path $dllPath)) { throw "没有 $dllPath，请先运行 dotnet build ColdHub.slnx" }

    # 子进程继承当前进程的环境变量：设好再启动，启动完立刻还原
    foreach ($key in $envVars.Keys) { Set-Item -Path "Env:$key" -Value $envVars[$key] }
    try {
        Start-Process -FilePath 'cmd.exe' -WorkingDirectory (Join-Path $root "src\$dir") `
            -ArgumentList '/k', "title $title && dotnet bin\Debug\net10.0\$dll.dll"
    }
    finally {
        foreach ($key in $envVars.Keys) { Remove-Item -Path "Env:$key" -ErrorAction SilentlyContinue }
    }
}

function Wait-Port($port, $seconds = 30) {
    $deadline = (Get-Date).AddSeconds($seconds)
    while ((Get-Date) -lt $deadline) {
        if (Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue) { return }
        Start-Sleep -Milliseconds 300
    }
    throw "端口 $port 在 $seconds 秒内没有起来，看对应窗口里的报错"
}

# 先检查 Redis，免得起了一堆进程才发现连不上
if (-not (Test-NetConnection 127.0.0.1 -Port 6379 -InformationLevel Quiet -WarningAction SilentlyContinue)) {
    throw '连不上 127.0.0.1:6379，请先启动本机的 Redis/Valkey'
}

Start-Host 'Admin' 'Admin' 'ColdHub Admin :5310' @{ ASPNETCORE_URLS = 'http://localhost:5310' }
Wait-Port 5310

Start-Host 'Source' 'Source' 'ColdHub Source warehouse-a :5321' @{
    ASPNETCORE_URLS = 'http://localhost:5321'; Source__Name = 'warehouse-a'; Source__Connector = 'sine' }
Start-Host 'Source' 'Source' 'ColdHub Source warehouse-b :5322' @{
    ASPNETCORE_URLS = 'http://localhost:5322'; Source__Name = 'warehouse-b'; Source__Connector = 'random' }
Start-Host 'Recorder' 'Recorder' 'ColdHub Recorder :5330' @{ ASPNETCORE_URLS = 'http://localhost:5330' }
Start-Host 'Hub' 'Hub' 'ColdHub Hub :5410' @{ ASPNETCORE_URLS = 'http://localhost:5410' }
Start-Host 'Console' 'Console' 'ColdHub Console :5340' @{ ASPNETCORE_URLS = 'http://localhost:5340' }

foreach ($port in 5321, 5322, 5330, 5410, 5340) { Wait-Port $port }

Write-Host ''
Write-Host '全部已启动：' -ForegroundColor Green
Write-Host '  后台管理   http://localhost:5340   （alice / alice-demo-pass 可改；bob / bob-demo-pass 只读）'
Write-Host '  实时订阅   cd src\Client; dotnet run -- ws://localhost:5410/ws demo-client-key Reading:warehouse-a:freezer-1'
Write-Host '  停止       .\stop-all.ps1'
