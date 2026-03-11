$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "==========================================" -ForegroundColor Magenta
Write-Host "   Rancher Cluster Reset" -ForegroundColor Magenta
Write-Host "==========================================" -ForegroundColor Magenta
Write-Host ""
Write-Host "[1/4] Stopping all containers..." -ForegroundColor Cyan
docker-compose down
Write-Host "      Done." -ForegroundColor Green

Write-Host ""
Write-Host "[2/4] Removing Rancher data volume..." -ForegroundColor Cyan
docker volume rm deliverly_rancher_data
Write-Host "      Done." -ForegroundColor Green

Write-Host ""
Write-Host "[4/4] Starting everything back up..." -ForegroundColor Cyan
docker-compose up -d
Write-Host "      Done." -ForegroundColor Green

Write-Host ""
Write-Host "==========================================" -ForegroundColor Magenta
Write-Host "   Rancher reset complete!" -ForegroundColor Green
Write-Host "   Wait ~2 minutes for k3s to initialize" -ForegroundColor Yellow
Write-Host "   Then run: npm run deploy" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Magenta
Write-Host ""
