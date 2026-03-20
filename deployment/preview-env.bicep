@description('PR ID for naming')
param prId string

@description('Location for all resources')
param location string = resourceGroup().location

var appServicePlanName = 'asp-pr-${prId}'
var appServiceName = 'app-pr-${prId}'

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  kind: 'linux'
  properties: {
    reserved: true  // ← FIXED: Now in properties
  }
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
}

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: appServiceName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNET|10.0'
    }
  }
}

output appUrl string = 'https://${appServiceName}.azurewebsites.net'
