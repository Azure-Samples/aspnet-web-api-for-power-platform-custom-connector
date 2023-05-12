targetScope = 'subscription'

param name string
param location string = 'Korea Central'

param apiManagementPublisherName string = 'Ask Me Anything Bot'
param apiManagementPublisherEmail string = 'apim@contoso.com'
@secure()
param appServiceKey string
param gitHubUsername string
param gitHubRepository string

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'rg-${name}'
  location: location
}

module cogsvc './provision-cognitiveServices.bicep' = {
  name: 'CognitiveServices'
  scope: rg
  params: {
    name: name
    location: location
  }
}

module appsvc './provision-appService.bicep' = {
  name: 'AppService'
  scope: rg
  params: {
    name: name
    location: location
    aoaiApiKey: cogsvc.outputs.aoaiApiKey
    aoaiApiEndpoint: cogsvc.outputs.aoaiApiEndpoint
    aoaiApiDeploymentId: cogsvc.outputs.aoaiApiDeploymentId
    appsvcAuthKey: appServiceKey
    gitHubUsername: gitHubUsername
    gitHubRepository: gitHubRepository
  }
}
    
module apim './provision-apiManagement.bicep' = {
  name: 'ApiManagement'
  scope: rg
  params: {
    name: name
    location: location
    apiManagementPublisherName: apiManagementPublisherName
    apiManagementPublisherEmail: apiManagementPublisherEmail
    appsvcAuthKey: appsvc.outputs.authKey
    apiManagementPolicyFormat: 'xml-link'
    apiManagementPolicyValue: 'https://raw.githubusercontent.com/Azure-Samples/aspnet-web-api-for-power-platform-custom-connector/main/infra/apim-policy-global.xml'
  }
}

module apis './provision-apiManagementApi.bicep' = {
  name: 'ApiManagementApi'
  scope: rg
  dependsOn: [
    apim
  ]
  params: {
    name: name
    location: location
    apiManagementApiName: 'GitHubIssueSummary'
    apiManagementApiDisplayName: 'GitHubIssueSummary'
    apiManagementApiDescription: 'GitHubIssueSummary'
    apiManagementApiServiceUrl: 'https://${appsvc.outputs.name}.azurewebsites.net'
    apiManagementApiPath: ''
    apiManagementApiFormat: 'openapi-link'
    apiManagementApiValue: 'https://raw.githubusercontent.com/Azure-Samples/aspnet-web-api-for-power-platform-custom-connector/main/infra/openapi.yaml'
    apiManagementApiSubscriptionRequired: true
    apiManagementApiPolicyFormat: 'xml-link'
    apiManagementApiPolicyValue: 'https://raw.githubusercontent.com/Azure-Samples/aspnet-web-api-for-power-platform-custom-connector/main/infra/apim-policy-githubissuesummary.xml'
  }
}
