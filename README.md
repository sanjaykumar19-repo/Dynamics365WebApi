# Dynamics365WebApi
Repo for Dynamics 365 WebApi implementation

Pre-requisites:
1.	Create an Azure Active Directory App: go to https://portal.azure.com  Active Directory App Registrations.
 
Click on the New Registration: Provide Name of the App and redirect URL(your dynamics CRM domain URL)
 

Provide CRM API permission to the App follow below image:
 

You need to copy the application ID (client ID) to create a user in CRM.
Now In CRM go to Users select Application Users view.
 


 Click on the New to create a User for the App in CRM: Provide below details:
User Name: name for the app user.
Application ID : the one which you have copied from Azure App Registration.
Primary email : any email id for this user.
Full Name: Name of the User
 

Once all the information is provided save the user and assign all the relevant roles.

To User DynamicsCRM.API.Services you need below WebApiConfiguration details:
	WebApiConfiguration config = new WebApiConfiguration()
			{
				ClientId = "<Azure App registration Application ID>",
				ClientSecret = "<Azure App registration Secret of the App>",
				CRMApiEndpointUri ="https://orgname.api.crm.dynamics.com/api/data/v9.0/",
				RedirectUrl = “https://orgname.crm.dynamics.com/” OR <URL provided in the Azure App registration App >,
				TenantId = "GUID of your Azure Tenant"
		};

