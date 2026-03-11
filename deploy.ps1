param(
    [Parameter(Mandatory = $true)]
    [string]$App
)

$ErrorActionPreference = "Stop"

$TarFile = "${App}_tmp.tar"
$K8sManifest = "./apps/$App/kubernetes.yaml"

Write-Host ""
Write-Host "==========================================" -ForegroundColor Magenta
Write-Host "   Deploying $App" -ForegroundColor Magenta
Write-Host "==========================================" -ForegroundColor Magenta

Write-Host ""
Write-Host "[1/5] Building Docker image..." -ForegroundColor Cyan
docker-compose build $App
Write-Host "      Done." -ForegroundColor Green

Write-Host ""
Write-Host "[2/5] Transferring image to Rancher..." -ForegroundColor Cyan
docker save -o $TarFile "${App}:latest"
docker cp $TarFile deliverly_rancher:/tmp/$TarFile
docker exec deliverly_rancher ctr -n k8s.io images import /tmp/$TarFile
Remove-Item $TarFile
docker exec deliverly_rancher rm /tmp/$TarFile
Write-Host "      Done." -ForegroundColor Green

Write-Host ""
Write-Host "[3/5] Applying Kubernetes manifests..." -ForegroundColor Cyan
Get-Content $K8sManifest | docker exec -i deliverly_rancher kubectl apply -f -
Write-Host "      Done." -ForegroundColor Green

Write-Host ""
Write-Host "[4/5] Restarting Pods..." -ForegroundColor Cyan
docker exec -i deliverly_rancher kubectl rollout restart deployment/$App -n deliverly
docker exec -i deliverly_rancher kubectl rollout status deployment/$App -n deliverly
Write-Host "      Done." -ForegroundColor Green

Write-Host ""
Write-Host "[5/5] Pruning unused images..." -ForegroundColor Cyan
docker exec deliverly_rancher bash -c "echo 'runtime-endpoint: unix:///run/k3s/containerd/containerd.sock' > /etc/crictl.yaml && crictl rmi --prune"
Write-Host "      Done." -ForegroundColor Green

Write-Host ""
Write-Host "==========================================" -ForegroundColor Magenta
Write-Host "   $App deployed successfully!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Magenta
Write-Host ""
Write-Host "Current Pod status:" -ForegroundColor Yellow
docker exec -i deliverly_rancher kubectl get pods -n deliverly
Write-Host ""
