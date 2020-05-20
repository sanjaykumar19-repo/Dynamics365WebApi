namespace DynamicsCRM.Api.Services.Helpers
{
	using Microsoft.Xrm.Sdk;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Net.Http;

	internal static class EntityExtensions
	{
		internal static string GetEntityJson(this Entity entity, HttpClient httpClient)
		{
			JObject obj = new JObject();
			foreach (var attribute in entity.Attributes)
			{
				var fieldKey = attribute.Key;
				switch (attribute.Value)
				{
					case EntityReference _:
						//parentcontactid @odata.bind: "/contacts(25a17064-1ae7-e611-80f4-e0071b661f01)"
						string key = $"{attribute.Key}@odata.bind";
						var value = entity.GetAttributeValue<EntityReference>(fieldKey);
						string entitySetName = Helper.GetEntitySetName(httpClient, value.LogicalName);
						obj[key] = $"/{entitySetName}({value.Id})";
						break;
					case Money _:
						obj[attribute.Key] = entity.GetAttributeValue<Money>(fieldKey)?.Value;
						break;
					case OptionSetValue _:
						obj[attribute.Key] = entity.GetAttributeValue<OptionSetValue>(fieldKey).Value;
						break;
					case Boolean _:
						obj[attribute.Key] = entity.GetAttributeValue<bool>(fieldKey);
						break;
					case int _:
						obj[attribute.Key] = (int)entity[fieldKey];
						break;
					case long _:
						obj[attribute.Key] = (long)entity[fieldKey];
						break;
					case float _:
						obj[attribute.Key] = (float)entity[fieldKey];
						break;
					case double _:
						obj[attribute.Key] = (double)entity[fieldKey];
						break;
					case decimal _:
						obj[attribute.Key] = (decimal)entity[fieldKey];
						break;
					case DateTime _:
						obj[attribute.Key] = entity.GetAttributeValue<DateTime>(fieldKey).ToString("yyyy-MM-dd");
						break;
					default:
						obj[attribute.Key] = entity.Attributes[fieldKey].ToString();
						break;
				}
			}

			return JsonConvert.SerializeObject(obj);
		}
	}
}
