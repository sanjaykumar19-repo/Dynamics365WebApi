namespace DynamicsCRM.Api.Services
{
	using DynamicsCRM.Api.Services.Helpers;
	using Microsoft.Xrm.Sdk;
	using System;
	using System.Linq;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;

	public sealed partial class CrmApiService : ICrmApiService
	{
		/// <summary>
		/// Create a new entity record in Dynamics CRM and returns record GUID
		/// </summary>
		/// <param name="entitySetName">example: contacts, opportunities, accounts</param>
		/// <example>contacts, opportunities </example>
		/// <param name="entity"> Request Object</param>
		/// <returns> Record Id</returns>
		async Task<Guid> ICrmApiService.CreateRecord(string entitySetName, Entity entity)
		{
			ThrowIf.ArgumentNull("entityLogicalName can't be null", entitySetName);
			ThrowIf.ArgumentNull("jsonRequest can't be null", entity);
			string requestJson = entity.GetEntityJson(HttpCrmClient);
			string requestUri = string.Format("{0}{1}", HttpCrmClient.BaseAddress, entitySetName);
			HttpRequestMessage createRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = new StringContent(requestJson)
			};
			createRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

			HttpResponseMessage createResponse = await HttpCrmClient.SendAsync(createRequest, HttpCompletionOption.ResponseHeadersRead);
			if (createResponse.IsSuccessStatusCode)
			{
				string recordUri = createResponse.Headers.GetValues("OData-EntityId")?.FirstOrDefault();
				Match match = Regex.Match(recordUri, @"[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?");
				if (match.Success)
					return Guid.Parse(match.Value.Replace("(", "").Replace(")", "").Trim());
				else
				{
					match = Regex.Match(recordUri, @"[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?".ToLower());
					if (match.Success)
						return Guid.Parse(match.Value.Replace("(", "").Replace(")", "").Trim());
				}

				return Guid.Empty;
			}
			else
			{
				throw new Exception(string.Format("Failed to Post Records Reason{0}. Exception{1}", createResponse.ReasonPhrase, await createResponse.Content?.ReadAsStringAsync()));
			}
		}

		/// <summary>
		/// Update the entity (Record Id) in Dynamics CRM
		/// </summary>
		/// <param name="entitySetName">example: contacts, opportunities, accounts</param>
		/// <example>contacts, opportunities </example>
		/// <param name="id">Record Id</param>
		/// <param name="updateEntity"> Request Object</param>
		/// <returns> success </returns>
		async Task<bool> ICrmApiService.UpdateRecord(string entityLogicalName, Guid id, Entity updateEntity)
		{
			ThrowIf.ArgumentNull("entityLogicalName can't be null", entityLogicalName);
			ThrowIf.ArgumentNull("Record Id can't be blank", id);
			ThrowIf.ArgumentNull("jsonUpdateRequest can't be blank", updateEntity);

			string requestUri = string.Format("{0}{1}s({2})", HttpCrmClient.BaseAddress, entityLogicalName, id);
			HttpMethod httpMethod = new HttpMethod("Patch");
			HttpRequestMessage updateRequest = new HttpRequestMessage(httpMethod, requestUri)
			{
				Content = new StringContent(updateEntity.GetEntityJson(HttpCrmClient), Encoding.UTF8, "application/json")
			};

			HttpResponseMessage updateResponse = await HttpCrmClient.SendAsync(updateRequest, HttpCompletionOption.ResponseContentRead);
			if (updateResponse.IsSuccessStatusCode)
			{
				return true;
			}
			else
			{
				throw new Exception(string.Format("Failed to update contact for reason: {0}. Exception:{1}",
				  updateResponse.ReasonPhrase, updateResponse.Content?.ReadAsStringAsync()));
			}
		}
	}
}