# Parameters for resource group and location
param (
    [string]$resourceGroup,
    [string]$location
)

# create resource group if it doesn't exist
Write-Host "Checking if resource group $resourceGroup exists..."
$resourceGroupExists = (az group exists --name $resourceGroup) -eq "true"

if (-not $resourceGroupExists) {
    Write-Host "Resource group $resourceGroup does not exist. Creating..."
    az group create --name $resourceGroup --location $location
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to create resource group $resourceGroup. Exiting."
        exit $LASTEXITCODE
    }
} else {
    Write-Host "Resource group $resourceGroup already exists. Proceeding..."
}

# Set project variables
$solutionDirectory = "../WeatherApp"
$prefix = "weatherapp3"
$outputDirectory = "./publish"
$bicepFile = "main.bicep"

# List of function apps in your solution
$functionAppProjects = @(
    "StartJobFunction",
    "FunctionProcessWeatherImage",
    "GenerateImageFunction",
    "ExposeBlobFunction"
)

# Delete the output directory if it exists
if (Test-Path $outputDirectory) {
    Remove-Item -Recurse -Force $outputDirectory
}

# Create output directory
New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null

# Deploy the Bicep file to set up infrastructure
Write-Host "Deploying infrastructure with Bicep file..."
az deployment group create --resource-group $resourceGroup --template-file $bicepFile --parameters location=$location

# Check if Bicep deployment was successful
if ($LASTEXITCODE -ne 0) {
    Write-Host "Bicep deployment failed. Exiting."
    exit $LASTEXITCODE
}

# Publish the function apps
foreach ($project in $functionAppProjects) {
    Write-Host "Publishing function app: $project"
    
    # Construct the project path and .csproj file path
    $projectPath = Join-Path $solutionDirectory $project
    $csprojFile = "$project.csproj"

    # Check if .csproj file exists
    $fullCsprojPath = Join-Path $projectPath $csprojFile
    Write-Host "Expected .csproj path for ${project}: $fullCsprojPath"
    if (!(Test-Path $fullCsprojPath)) {
        Write-Host "Project file not found: $fullCsprojPath. Skipping project."
        continue
    }

    # Publish the project
    dotnet publish $fullCsprojPath -c Release -o "$outputDirectory/$project"
    
    # Check if publish was successful
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to publish $project. Exiting."
        exit $LASTEXITCODE
    }

    # Verify the published directory exists
    $publishedDir = "$outputDirectory/$project"
    if (!(Test-Path $publishedDir)) {
        Write-Host "Publish output not found for $project at $publishedDir. Skipping zipping."
        continue
    }

    # Create a zip package for deployment
    Write-Host "Creating zip package for: $project"
    Compress-Archive -Path "$publishedDir\*" -DestinationPath "$outputDirectory/$project.zip" -Force

    # Verify that the zip file was created
    if (!(Test-Path "$outputDirectory/$project.zip")) {
        Write-Host "Failed to create zip package for $project. Skipping deployment."
        continue
    }
}

# Deploy each function app to Azure
foreach ($project in $functionAppProjects) {
    $functionAppName = "$prefix$project"
    Write-Host "Deploying function app: $functionAppName"
    
    # Check if the zip file exists before attempting deployment
    $zipPath = "$outputDirectory/$project.zip"
    if (!(Test-Path $zipPath)) {
        Write-Host "Deployment zip not found for $project at $zipPath. Skipping deployment."
        continue
    }

    # Deploy using the zip package
    az functionapp deployment source config-zip --name $functionAppName --resource-group $resourceGroup --src $zipPath

    # Check if the deployment was successful
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to deploy $functionAppName. Exiting."
        exit $LASTEXITCODE
    }
}

Write-Host "All function apps published and deployed successfully!"