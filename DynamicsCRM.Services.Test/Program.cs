using DynamicsCRM.Api.Services;
using DynamicsCRM.Api.Services.Models;
using Helper;
using Microsoft.Xrm.Sdk;
using System;

namespace DynamicsCRM
{
	class Program
	{
		static void Main(string[] args)
		{
			var serviceClient = CrmConnectionHelper.GetCrmServiceClient();
			var entity = serviceClient.Retrieve("opportunity", new Guid("be0d0283-5bf2-e311-945f-6c3be5a8dd64"), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
			var entity1 = new Entity("opportunity");
			entity1.Attributes["parentaccountid"] = entity.Attributes["parentaccountid"];
			entity1.Attributes["parentcontactid"] = entity.Attributes["parentcontactid"];
			entity1.Attributes["name"] = (string)entity.Attributes["name"] + " - API " + DateTime.Now.ToLongDateString();
			entity1.Attributes["purchasetimeframe"] = entity.Attributes["purchasetimeframe"];
			entity1.Attributes["budgetamount"] = entity.Attributes["budgetamount"];
			entity1.Attributes["purchaseprocess"] = entity.Attributes["purchaseprocess"];
			entity1.Attributes["estimatedclosedate"] = entity.Attributes["estimatedclosedate"];
			entity1.Attributes["confirminterest"] = entity.Attributes["confirminterest"];

			var config = new WebApiConfiguration()
			{
				ClientId = " ",
				ClientSecret = " ",
				CRMApiEndpointUri = "https://orgname.api.crm8.dynamics.com/api/data/v9.1/",
				RedirectUrl = "https://orgname.crm8.dynamics.com/",
				TenantId = ""
			};

			CrmApiService.CreateCrmApiServiceInstance(config);
			
			var id = CrmApiService.Instance.CreateRecord("opportunities", entity1).GetAwaiter().GetResult();
		}
	}
}
