namespace DynamicsCRM.Api.Services
{
	using DynamicsCRM.Api.Services.Helpers;
	using DynamicsCRM.Api.Services.Models;
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;

	public abstract partial class CrmApiService : ICrmApiService
	{
		/// <summary>
		/// Is User successfully connected
		/// </summary>
		public bool IsReady { get => WhoAmI?.UserId.HasValue ?? false; }

		/// <summary>
		/// Provides Authenticated User Info and Organization details
		/// </summary>
		public WhoAmIResponse WhoAmI { get; private set; }

		private static HttpClient HttpCrmClient { get; set; }
		private readonly WebApiConfiguration _configuration;

		private CrmApiService() { }

		/// <summary>
		/// Establises the connection to Dynamics 365 Api
		/// </summary>
		/// <param name="crmApiUrl"> CRM Api url</param>
		/// <param name="redirectUrl"> redirect uri registered in Azure AAD App</param>
		/// <param name="clientId"> clientId(AppId) of registered Azure AAD App</param>
		/// <param name="clientSecret">clientSecret of registered Azure AAD App</param>
		/// <param name="tenentId">Tenent Id of Azure AAD endpoint</param>
		internal CrmApiService(WebApiConfiguration configuration)
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
				WhoAmI = GetWhoAmIRequest();
			}
			catch (Exception)
			{
				throw;
			}
		}

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
				this.WhoAmI = JSONSerializer<WhoAmIResponse>.DeSerialize(responseBody);
				return this.WhoAmI;
			}
			else
			{
				throw new Exception(string.Format(
				 "The WhoAmI request failed with a status of '{0}' and exception:{1}",
				  response.ReasonPhrase, responseBody));
			}
		}
	}
}