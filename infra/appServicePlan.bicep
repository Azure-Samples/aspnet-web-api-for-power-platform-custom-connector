param name string
param location string = resourceGroup().location

var hostingPlan = {
  name: 'asplan-${name}'
  location: location
}

resource asplan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: hostingPlan.name
  location: hostingPlan.location
  kind: 'app'
  sku: {
    name: 'S1'
    tier: 'Standard'
  }
}

output id string = asplan.id
output name string = asplan.name
