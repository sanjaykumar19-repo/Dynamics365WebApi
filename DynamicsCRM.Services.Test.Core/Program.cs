namespace D365.ConsoleAppTest
{
	using DynamicsCRM.Api.Services;
	using DynamicsCRM.Api.Services.Helpers;
	using DynamicsCRM.Api.Services.Models;
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Net.Http;

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			WebApiConfiguration config = new WebApiConfiguration()
			{
				ClientId = " ",
				ClientSecret = " ",
				CRMApiEndpointUri = "https://orgname.api.crm8.dynamics.com/api/data/v9.1/",
				RedirectUrl = "https://orgname.crm8.dynamics.com/",
				TenantId = ""
			};

			CrmApiService.CreateCrmApiConnection(config);

			ContactRequest contactRequest = new ContactRequest()
			{
				firstname = "IwthRemove",
				lastname = "lastnameKumar"
			};

			var contactId = CrmApiService.Instance.CreateRecord("contact", JSONSerializer<ContactRequest>.Serialize(contactRequest)).Result;

			contactRequest = new ContactRequest()
			{
				emailaddress1 = "sanjayemail@email.com"
			};

			string json = JsonConvert.SerializeObject(contactRequest, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Ignore,
			});

			bool updated = CrmApiService.Instance.UpdateRecord("contact", contactId, json).Result;
			string queryOptions = "?$select=fullname,firstname,lastname,emailaddress1,createdon,description&$orderby=createdon desc";
			string result = CrmApiService.Instance.RetrieveRecord("contact", contactId, queryOptions).Result;
			string result2 = CrmApiService.Instance.RetrieveRecords("contact", queryOptions, maxpagesize: 5000).Result;

			EntityCollection collection = JsonConvert.DeserializeObject<EntityCollection>(result2);
			IList<string> results = new List<string>
			{
				result2
			};

			while (string.IsNullOrEmpty(collection?.OdataNextLink) != true)
			{
				var result3 = CrmApiService.Instance.RetrieveRecordsByNextLink(collection.OdataNextLink, maxpagesize: 10).Result;
				results.Add(result3);
				collection = JsonConvert.DeserializeObject<EntityCollection>(result3);
			}
		}
	}
}
