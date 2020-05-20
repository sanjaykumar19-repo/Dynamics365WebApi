namespace DynamicsCRM.Api.Services
{
	using DynamicsCRM.Api.Services.Helpers;
	using DynamicsCRM.Api.Services.Models;
	using System;

	public sealed class CrmApiInstance : CrmApiService
	{
		private static ICrmApiService _instance;

		private static WebApiConfiguration _configuration;
		/// <summary>
		/// Access CrmApiService.Instance to get the object.
		/// Then call methods on that instance.
		/// </summary>
		public static ICrmApiService Instance => _instance ?? throw new Exception("Object not created");

		private CrmApiInstance(WebApiConfiguration configuration) : base(configuration)
		{ }

		/// <summary>
		/// Provides secure connection to access Dynamics CrmApi
		/// </summary>
		/// <param name="configuration"> WebApi Configuration</param>
		/// <see cref="WebApiConfiguration"/>
		public static ICrmApiService CreateCrmApiInstance(WebApiConfiguration configuration)
		{
			ThrowIf.ArgumentNull("WebApiConfiguration can't be null", configuration);

			if (_instance != null && _instance.IsReady && configuration.CRMApiEndpointUri.Equals(_configuration?.CRMApiEndpointUri))
				throw new Exception("Instance is already created");
			else
				_instance = new CrmApiInstance(configuration);

			_configuration = configuration;
			return _instance;
		}
	}
}