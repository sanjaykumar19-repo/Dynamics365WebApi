namespace DynamicsCRM.Api.Services.Helpers
{
	using DynamicsCRM.Api.Services.Models;
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	///Custom HTTP message handler that uses OAuth authentication thru ADAL.
	/// </summary>
	internal class OAuthMessageHandler : DelegatingHandler
	{
		private static readonly HttpClient client = new HttpClient();
		private readonly WebApiConfiguration _configuration;

		private OAuthToken AuthToken { get; set; }

		protected internal OAuthMessageHandler(WebApiConfiguration configuration, HttpMessageHandler innerHandler) : base(innerHandler)
		{
			_configuration = configuration;
			GetOAuthToken(_configuration);
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (AuthToken != null && DateTime.UtcNow.Subtract(AuthToken.TokenInitDate).TotalSeconds >= AuthToken.ExpiresIn)
			{
				GetOAuthToken(_configuration);
			}

			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken?.AccessToken);
			return base.SendAsync(request, cancellationToken);
		}

		private void GetOAuthToken(WebApiConfiguration configuration)
		{
			//Note that an Azure AD access token has finite lifetime, default expiration is 60 minutes.			
			string tokenEndpoint = string.Format("https://login.microsoftonline.com/{0}/oauth2/token", configuration.TenantId);
			string request = "grant_type={0} &resource={1}&client_id={2}&client_secret={3}";
			string requestBody = string.Format(request,
				"client_credentials", configuration.RedirectUrl, configuration.ClientId, configuration.ClientSecret);
			client.DefaultRequestHeaders.Add("Accept", "application/json");
			HttpResponseMessage response = client.PostAsync(tokenEndpoint,
				new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded")).GetAwaiter().GetResult();

			string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			if (response.IsSuccessStatusCode)
			{
				AuthToken = JSONSerializer<OAuthToken>.DeSerialize(result);
			}
			else
			{
				throw new HttpRequestException($"Unable to generate Authentication Token. Please verify WebApi Configuration.\n" +
					$"reason:{response.ReasonPhrase}. with exeption: {result}");
			}
		}
	}
}