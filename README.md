# ASP.NET Web API for Power Platform Custom Connector (feat. Azure OpenAI Service)

This is a sample application that shows:

1. How easy to intergate ASP.NET Web API with Azure API Management within Visual Studio
1. How easy to set up an authorisation feature directly on Azure API Management
1. How easy to export a Power Platform Custom Connector directly fron Azure API Management

## Goal

- You can call Azure OpenAI Service API through ASP.NET Core Web API.
- You can protect Azure OpenAI Service API with Azure Active Directory.
- You can build and deploy an ASP.NET Core Web API to Azure App Service within Visual Studio.
- You can integrate the Aure App Service instance with Azure API Management within Visual Studio.
- You can create an access key to Azure Active Directory through Azure API Management.
- You can create a Power Platform Custom Connector directly from Azure API Management.

## Getting Started

### Prerequisites

- GitHub Account
- Azure Free Account
- Azure OpenAI Service
- Visual Studio
- GitHub CLI
- Azure CLI
- Azure Developer CLI

### Quickstart

1. Fork this repository to your GitHub account.
1. Follow the steps in the following order:

    ```bash
    # Provision resources on Azure
    azd auth login
    azd init
    azd up
    azd config pipeline
    
    # Deploy app to Azure
    gh auth login
    gh workflow run "Azure Dev" --repo {{GITHUB_USERNAME}}/aspnet-web-api-for-power-platform-custom-connector
    ```

## Resources

TBD
