param location string = resourceGroup().location

var prefix = 'weatherapp1'
var serverFarmName = '${prefix}sf'
var storageAccountName = '${prefix}sta'

var startJobFunctionName = '${prefix}StartJobFunction'
var processWeatherImageFunctionName = '${prefix}FunctionProcessWeatherImage'
var generateImageFunctionName = '${prefix}GenerateImageFunction'
var exposeBlobFunctionName = '${prefix}ExposeBlobFunction'

resource serverFarm 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: serverFarmName
  location: location
  tags: resourceGroup().tags
  sku: {
    tier: 'Consumption'
    name: 'Y1'
  }
  kind: 'elastic'
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  tags: resourceGroup().tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    accessTier: 'Hot'
    publicNetworkAccess: 'Enabled'
  }
}

// StartJob Function (HTTP Trigger)
resource startJobFunction 'Microsoft.Web/sites@2021-03-01' = {
  name: startJobFunctionName
  location: location
  tags: resourceGroup().tags
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    enabled: true
    serverFarmId: serverFarm.id
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      http20Enabled: true
    }
    clientAffinityEnabled: false
    httpsOnly: true
    containerSize: 1536
    redundancyMode: 'None'
  }
}

resource startJobFunctionConfig 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${startJobFunctionName}/appsettings'
  properties: {
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet-isolated'
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED: '1'
    WEBSITE_RUN_FROM_PACKAGE: '1'
    AzureWebJobsStorage: storageAccountConnectionString
    JobQueueName: 'jobstartqueue'
  }
}

// ProcessWeatherImage Function (Queue Trigger)
resource processWeatherImageFunction 'Microsoft.Web/sites@2021-03-01' = {
  name: processWeatherImageFunctionName
  location: location
  tags: resourceGroup().tags
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    enabled: true
    serverFarmId: serverFarm.id
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      http20Enabled: true
    }
    clientAffinityEnabled: false
    httpsOnly: true
    containerSize: 1536
    redundancyMode: 'None'
  }
}

resource processWeatherImageFunctionConfig 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${processWeatherImageFunctionName}/appsettings'
  properties: {
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet-isolated'
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED: '1'
    WEBSITE_RUN_FROM_PACKAGE: '1'
    AzureWebJobsStorage: storageAccountConnectionString
    StationQueueName: 'stationqueue' 
  }
}

// generateImage Function (Queue Trigger)
resource generateImageFunction 'Microsoft.Web/sites@2021-03-01' = {
  name: generateImageFunctionName
  location: location
  tags: resourceGroup().tags
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    enabled: true
    serverFarmId: serverFarm.id
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      http20Enabled: true
    }
    clientAffinityEnabled: false
    httpsOnly: true
    containerSize: 1536
    redundancyMode: 'None'
  }
}

resource generateImageFunctionConfig 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${generateImageFunctionName}/appsettings'
  properties: {
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet-isolated'
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED: '1'
    WEBSITE_RUN_FROM_PACKAGE: '1'
    AzureWebJobsStorage: storageAccountConnectionString
    BlobStorageConnectionString: storageAccountConnectionString
    StationQueueName: 'stationqueue'
    BlobContainerName: 'weatherimages'
  }
}

// ExposeBlob Function (HTTP Trigger)
resource exposeBlobFunction 'Microsoft.Web/sites@2021-03-01' = {
  name: exposeBlobFunctionName
  location: location
  tags: resourceGroup().tags
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    enabled: true
    serverFarmId: serverFarm.id
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      http20Enabled: true
    }
    clientAffinityEnabled: false
    httpsOnly: true
    containerSize: 1536
    redundancyMode: 'None'
  }
}

resource exposeBlobFunctionConfig 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${exposeBlobFunctionName}/appsettings'
  properties: {
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet-isolated'
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED: '1'
    WEBSITE_RUN_FROM_PACKAGE: '1'
    AzureWebJobsStorage: storageAccountConnectionString
    BlobStorageConnection: storageAccountConnectionString
    BlobContainerName: 'weatherimages'
  }
}
