# PowerShell script to generate JavaScript gRPC-Web client code

Write-Host "Generating JavaScript gRPC-Web client code..." -ForegroundColor Green

# Create the generated directory if it doesn't exist
$generatedDir = "client\generated"
if (!(Test-Path $generatedDir)) {
    New-Item -ItemType Directory -Path $generatedDir -Force
    Write-Host "Created directory: $generatedDir" -ForegroundColor Yellow
}

# Check if protoc is available
try {
    $protocVersion = & protoc --version 2>$null
    Write-Host "Found protoc: $protocVersion" -ForegroundColor Green
} catch {
    Write-Host "Error: protoc not found. Please install Protocol Buffers compiler." -ForegroundColor Red
    Write-Host "Download from: https://github.com/protocolbuffers/protobuf/releases" -ForegroundColor Yellow
    exit 1
}

# Generate JavaScript client code
Write-Host "Generating client code..." -ForegroundColor Yellow

$protocArgs = @(
    "-I=GrpcRealtimePush\Protos",
    "--js_out=import_style=commonjs:client\generated",
    "--grpc-web_out=import_style=commonjs,mode=grpcwebtext:client\generated",
    "GrpcRealtimePush\Protos\chat.proto"
)

try {
    & protoc @protocArgs
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "JavaScript gRPC-Web client code generated successfully!" -ForegroundColor Green
        Write-Host "Files created in client\generated\:" -ForegroundColor Yellow
        Get-ChildItem "client\generated\" | Format-Table Name, Length, LastWriteTime
    } else {
        throw "protoc command failed with exit code $LASTEXITCODE"
    }
} catch {
    Write-Host "Error generating client code: $_" -ForegroundColor Red
    Write-Host "Please ensure:" -ForegroundColor Yellow
    Write-Host "1. protoc is installed and in PATH" -ForegroundColor Yellow
    Write-Host "2. grpc-web plugin is installed" -ForegroundColor Yellow
    Write-Host "3. Run: npm install -g grpc-web" -ForegroundColor Yellow
    exit 1
}

Write-Host "Client code generation completed!" -ForegroundColor Green