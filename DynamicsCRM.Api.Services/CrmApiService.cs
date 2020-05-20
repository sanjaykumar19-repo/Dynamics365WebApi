namespace DynamicsCRM.Api.Services
{
	using DynamicsCRM.Api.Services.Helpers;
	using DynamicsCRM.Api.Services.Models;
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;

	public sealed partial class CrmApiService : ICrmApiService
	{
		private static WebApiConfiguration _configuration;
		private static HttpClient HttpCrmClient { get; set; }

		private static ICrmApiService _instance;
		/// <summary>
		/// Access CrmApiService.Instance to get the object.
		/// Then call methods on that instance.
		/// </summary>
		public static ICrmApiService Instance => _instance ?? throw new Exception("Object not created");

		private WhoAmIResponse _whoAmI;
		/// <summary>
		/// Provides Authenticated User Info and Organization details
		/// </summary>
		public WhoAmIResponse WhoAmI => _whoAmI ?? GetWhoAmIRequest();

		/// <summary>
		/// Is User successfully connected
		/// </summary>
		public bool IsReady => _whoAmI?.UserId.HasValue ?? false;

		private WhoAmIResponse GetWhoAmIRequest()
		{
			HttpCrmClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
			HttpCrmClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
			HttpCrmClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Use the WhoAmI function
			HttpResponseMessage response = HttpCrmClient.GetAsync("WhoAmI").GetAwaiter().GetResult();

			//Get the response content and parse it.  
			string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			if (response.IsSuccessStatusCode)
			{
				_whoAmI = JSONSerializer<WhoAmIResponse>.DeSerialize(responseBody);
				return _whoAmI;
			}
			else
			{
				throw new Exception(string.Format(
				 "The WhoAmI request failed with a status of '{0}' and exception:{1}",
				  response.ReasonPhrase, responseBody));
			}
		}

		/// <summary>
		/// Establises the connection to Dynamics 365 Api
		/// </summary>
		/// <param name="crmApiUrl"> CRM Api url</param>
		/// <param name="redirectUrl"> redirect uri registered in Azure AAD App</param>
		/// <param name="clientId"> clientId(AppId) of registered Azure AAD App</param>
		/// <param name="clientSecret">clientSecret of registered Azure AAD App</param>
		/// <param name="tenentId">Tenent Id of Azure AAD endpoint</param>
		private CrmApiService(WebApiConfiguration configuration)
		{
			ThrowIf.ArgumentNull("WebApiConfiguration can't be null", configuration, configuration.CRMApiEndpointUri);
			_configuration = configuration;
			try
			{
				HttpMessageHandler messageHandler = new OAuthMessageHandler(configuration, new HttpClientHandler());
				HttpCrmClient = new HttpClient(messageHandler)
				{
					Timeout = new TimeSpan(0, 2, 0)  //2 minutes
				};

				HttpCrmClient.BaseAddress = new Uri($"{configuration.CRMApiEndpointUri.TrimEnd('/')}/");
				_whoAmI = GetWhoAmIRequest();
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Provides secure connection to access Dynamics CrmApi
		/// </summary>
		/// <param name="configuration"> WebApi Configuration</param>
		/// <see cref="WebApiConfiguration"/>
		public static void CreateCrmApiServiceInstance(WebApiConfiguration configuration)
		{
			ThrowIf.ArgumentNull("WebApiConfiguration can't be null", configuration);

			if (_instance != null && _instance.IsReady && configuration.CRMApiEndpointUri.Equals(_configuration?.CRMApiEndpointUri))
				throw new Exception("Instance is already created");
			else
				_instance = new CrmApiService(configuration);
		}
	}
}