namespace DynamicsCRM.Api.Services.Models
{
	/// <summary>
	/// Provides the D365 web service and Azure app registration information to connect with Dynamics 365
	/// </summary>
	public class WebApiConfiguration
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string TenantId { get; set; }
		public string RedirectUrl { get; set; }
		/// <summary>
		/// Settings->Customization->Service Root URL
		/// </summary>
		public string CRMApiEndpointUri { get; set; }

		public WebApiConfiguration()
		{ }
	}
}
