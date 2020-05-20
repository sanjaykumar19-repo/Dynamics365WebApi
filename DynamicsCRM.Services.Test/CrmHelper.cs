using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;

namespace Helper
{
	public static class CrmConnectionHelper
	{
		public static CrmServiceClient GetCrmServiceClient()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			try
			{
				string tokenCachePath = @"c:\\cache.txt";
				Uri instanceUrl = new Uri("https://orgname.crm8.dynamics.com");
				var connection = new CrmServiceClient(instanceUrl, "clientId",
									  "secret", true, tokenCachePath);

				return connection.IsReady ? connection
					: throw new Exception($"unable to connect: {connection.LastCrmError}");
			}
			catch
			{
				throw;
			}
		}
	}
}

