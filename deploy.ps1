param(
    [Parameter(Mandatory = $true)]
    [string]$App
)

$ErrorActionPreference = "Stop"

$TarFile = "${App}_tmp.tar"
$K8sManifest = "./apps/$App/kubernetes.yaml"

Write-Host "Step 1: Building Docker image for '$App'..." -ForegroundColor Cyan
docker-compose build $App

Write-Host "Step 2: Transferring image via temporary file..." -ForegroundColor Cyan
docker save -o $TarFile "${App}:latest"

docker cp $TarFile deliverly_rancher:/tmp/$TarFile

docker exec deliverly_rancher ctr -n k8s.io images import /tmp/$TarFile

Remove-Item $TarFile
docker exec deliverly_rancher rm /tmp/$TarFile

Write-Host "Step 3: Applying Kubernetes manifests..." -ForegroundColor Cyan
Get-Content $K8sManifest | docker exec -i deliverly_rancher kubectl apply -f -

Write-Host "Step 4: Restarting Pods in Kubernetes..." -ForegroundColor Cyan
docker exec -i deliverly_rancher kubectl rollout restart deployment/$App -n deliverly
docker exec -i deliverly_rancher kubectl rollout status deployment/$App -n deliverly

Write-Host "Step 5: Pruning unused images from containerd..." -ForegroundColor Cyan
docker exec deliverly_rancher bash -c "echo 'runtime-endpoint: unix:///run/k3s/containerd/containerd.sock' > /etc/crictl.yaml && crictl rmi --prune"

Write-Host "Deployment of '$App' completed successfully!" -ForegroundColor Green
Write-Host "Current Pod status:" -ForegroundColor Yellow
docker exec -i deliverly_rancher kubectl get pods -n deliverly
