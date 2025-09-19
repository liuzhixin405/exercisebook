# 测试Swagger API
Write-Host "Testing Swagger API..." -ForegroundColor Green

# 等待应用启动
Start-Sleep -Seconds 3

# 测试Swagger JSON端点
try {
    $swaggerJson = Invoke-RestMethod -Uri "http://localhost:5000/swagger/v1/swagger.json" -Method Get
    Write-Host "✅ Swagger JSON endpoint is working!" -ForegroundColor Green
    Write-Host "API Title: $($swaggerJson.info.title)" -ForegroundColor Cyan
    Write-Host "API Version: $($swaggerJson.info.version)" -ForegroundColor Cyan
    Write-Host "Number of endpoints: $($swaggerJson.paths.Count)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Swagger JSON endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# 测试Swagger UI
try {
    $swaggerUI = Invoke-WebRequest -Uri "http://localhost:5000/" -Method Get
    if ($swaggerUI.StatusCode -eq 200) {
        Write-Host "✅ Swagger UI is accessible!" -ForegroundColor Green
    }
} catch {
    Write-Host "❌ Swagger UI failed: $($_.Exception.Message)" -ForegroundColor Red
}

# 测试API端点
try {
    $users = Invoke-RestMethod -Uri "http://localhost:5000/api/users" -Method Get
    Write-Host "✅ Users API endpoint is working!" -ForegroundColor Green
    Write-Host "Number of users: $($users.Count)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Users API endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 Swagger integration test completed!" -ForegroundColor Yellow
Write-Host "You can now access:" -ForegroundColor White
Write-Host "  - Swagger UI: http://localhost:5000/" -ForegroundColor White
Write-Host "  - Swagger JSON: http://localhost:5000/swagger/v1/swagger.json" -ForegroundColor White
Write-Host "  - Users API: http://localhost:5000/api/users" -ForegroundColor White
