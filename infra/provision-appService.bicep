param name string
param location string = resourceGroup().location

@secure()
param aoaiApiKey string
param aoaiApiEndpoint string
param aoaiApiDeploymentId string
@secure()
param appsvcAuthKey string
param gitHubUsername string
param gitHubRepository string

module wrkspc './logAnalyticsWorkspace.bicep' = {
  name: 'LogAnalyticsWorkspace_AppService'
  params: {
    name: '${name}-api'
    location: location
  }
}

module appins './applicationInsights.bicep' = {
  name: 'ApplicationInsights_AppService'
  params: {
    name: '${name}-api'
    location: location
    workspaceId: wrkspc.outputs.id
  }
}

module asplan './appServicePlan.bicep' = {
  name: 'AppServicePlan_AppService'
  params: {
    name: '${name}-api'
    location: location
  }
}

module appsvc './appService.bicep' = {
  name: 'AppService_AppService'
  params: {
    name: '${name}-api'
    location: location
    appInsightsInstrumentationKey: appins.outputs.instrumentationKey
    appInsightsConnectionString: appins.outputs.connectionString
    appServicePlanId: asplan.outputs.id
    aoaiApiKey: aoaiApiKey
    aoaiApiEndpoint: aoaiApiEndpoint
    aoaiApiDeploymentId: aoaiApiDeploymentId
    appsvcAuthKey: appsvcAuthKey
    gitHubUsername: gitHubUsername
    gitHubRepository: gitHubRepository
  }
}

output id string = appsvc.outputs.id
output name string = appsvc.outputs.name
output authKey string = appsvcAuthKey
