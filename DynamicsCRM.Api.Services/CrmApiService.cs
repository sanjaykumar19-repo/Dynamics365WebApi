namespace DynamicsCRM.Api.Services
{
	using DynamicsCRM.Api.Services.Models;
	using System;

	public sealed partial class CrmApiService
	{

		private static ICrmApiClient _instance;

		/// <summary>
		/// Access CrmApiService.Instance to get the object.
		/// Then call methods on that instance.
		/// </summary>
		public static ICrmApiClient Instance
		{
			get
			{
				if (_instance == null)
				{
					throw new Exception("Object not created");
				}

				return _instance;
			}
		}

		/// <summary>
		/// Is User successfully connected
		/// </summary>
		public bool IsReady { get => WhoAmI.UserId.HasValue; }

		/// <summary>
		/// Provides Authenticated User Info and Organization details
		/// </summary>
		public WhoAmIResponse WhoAmI { get; }


		/// <summary>
		/// Provides secure connection to access Dynamics CrmApi
		/// </summary>
		/// <param name="configuration"> WebApi Configuration</param>
		/// <see cref="WebApiConfiguration"/>
		public static void CreateCrmApiInstance(WebApiConfiguration configuration)
		{
			//	if (_instance != null)
			//	{
			//		throw new Exception("Object for CrmApiConnection already created");
			//	}

			_instance = new CrmApiService(configuration);
		}
	}
}