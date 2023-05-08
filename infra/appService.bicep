param name string
param location string = resourceGroup().location

param appServicePlanId string

@secure()
param appInsightsInstrumentationKey string
@secure()
param appInsightsConnectionString string

@secure()
param aoaiApiKey string
param aoaiApiEndpoint string
param aoaiApiDeploymentId string

@secure()
param appsvcAuthKey string
param gitHubUsername string
param gitHubRepository string

var asplan = {
  id: appServicePlanId
}
  
var appInsights = {
  instrumentationKey: appInsightsInstrumentationKey
  connectionString: appInsightsConnectionString
}

var aoai = {
  apiKey: aoaiApiKey
  endpoint: aoaiApiEndpoint
  deploymentId: aoaiApiDeploymentId
}

var apiApp = {
  name: 'appsvc-${name}'
  location: location
  authKey: appsvcAuthKey
  github: {
    username: gitHubUsername
    repository: gitHubRepository
  }
}

resource appsvc 'Microsoft.Web/sites@2022-03-01' = {
  name: apiApp.name
  location: apiApp.location
  kind: 'app'
  properties: {
    serverFarmId: asplan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      appSettings: [
        // Common Settings
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.instrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.connectionString
        }
        // API Auth
        {
          name: 'Auth__ApiKey'
          value: apiApp.authKey
        }
        // OpenAPI
        {
          name: 'OpenApi__Title'
          value: 'GitHub Issues Summary'
        }
        {
          name: 'OpenApi__Version'
          value: 'v1'
        }
        {
          name: 'OpenApi__Server'
          value: 'https://${apiApp.name}.azurewebsites.net'
        }
        {
          name: 'OpenApi__IncludeOnDeployment'
          value: 'true'
        }
        // GitHub
        {
          name: 'GitHub__Agent'
          value: 'Issue Summary Bot'
        }
        {
          name: 'GitHub__User'
          value: apiApp.github.username
        }
        {
          name: 'GitHub__Repository'
          value: apiApp.github.repository
        }
        // Azure OpenAI Service
        {
          name: 'AOAI__ApiKey'
          value: aoai.apiKey
        }
        {
          name: 'AOAI__Endpoint'
          value: aoai.endpoint
        }
        {
          name: 'AOAI__DeploymentId'
          value: aoai.deploymentId
        }
      ]
    }
  }
}

var policies = [
  {
    name: 'scm'
    allow: false
  }
  {
    name: 'ftp'
    allow: false
  }
]

resource appsvcPolicies 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2022-03-01' = [for policy in policies: {
  name: '${appsvc.name}/${policy.name}'
  location: apiApp.location
  properties: {
    allow: policy.allow
  }
}]

output id string = appsvc.id
output name string = appsvc.name
