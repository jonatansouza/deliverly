$ErrorActionPreference = "Stop"

Write-Host "Step 1: Building Docker image locally..." -ForegroundColor Cyan
docker-compose build deliverly-front    

Write-Host "Step 2: Transferring image via temporary file..." -ForegroundColor Cyan
docker save -o frontend_tmp.tar deliverly-front:latest

docker cp frontend_tmp.tar deliverly_rancher:/tmp/frontend_tmp.tar

docker exec deliverly_rancher ctr -n k8s.io images import /tmp/frontend_tmp.tar

Remove-Item frontend_tmp.tar
docker exec deliverly_rancher rm /tmp/frontend_tmp.tar

Write-Host "Step 3: Restarting Pods in Kubernetes..." -ForegroundColor Cyan
docker exec -i deliverly_rancher kubectl rollout restart deployment/deliverly-front -n deliverly

Write-Host "Deployment completed successfully!" -ForegroundColor Green
Write-Host "Current Pod status:" -ForegroundColor Yellow
docker exec -i deliverly_rancher kubectl get pods -n deliverly

# Apply Kubernetes manifests
Get-Content kubernetes.yaml | docker exec -i deliverly_rancher kubectl apply -f -