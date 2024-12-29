# Navigate to the project directory
Set-Location -Path "C:/Project/Habitica/HomeAssistant"

# Run dotnet build
Write-Host "Running dotnet build..."
dotnet build

# Build the Docker image
Write-Host "Building Docker image..."
docker build -t newmoonnotifier:latest .

# Stop and remove the running container if it exists
$containerId = docker ps -aq --filter "name=newmoonnotifier"
if ($containerId) {
    Write-Host "Stopping and removing existing container..."
    docker stop $containerId
    docker rm $containerId
}

# Run the Docker container
Write-Host "Starting new Docker container..."
docker run -d --name newmoonnotifier newmoonnotifier:latest

Write-Host "Build and deployment completed successfully."