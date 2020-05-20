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
		/// Create a new entity record in Dynamics CRM and returns record GUID
		/// </summary>
		/// <param name="entitySetName">Entity Set name </param>
		/// <example>contacts, opportunities </example>
		/// <param name="jsonRequest"> Request Object</param>
		/// <returns> Record Id</returns>
		async Task<Guid> ICrmApiService.CreateRecord(string entitySetName, string jsonRequest)
		{
			ThrowIf.ArgumentNull("entityLogicalName can't be null", entitySetName);
			ThrowIf.ArgumentNull("jsonRequest can't be null", jsonRequest);

			string requestUri = string.Format("{0}{1}", HttpCrmClient.BaseAddress, entitySetName);
			HttpRequestMessage createRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = new StringContent(jsonRequest)
			};
			createRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

			HttpResponseMessage createResponse = await HttpCrmClient.SendAsync(createRequest, HttpCompletionOption.ResponseHeadersRead);
			if (createResponse.IsSuccessStatusCode)
			{
				string recordUri = createResponse.Headers.GetValues("OData-EntityId").FirstOrDefault();
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
		/// delete entity record 
		/// </summary>
		/// <param name="entitySetName">entity name</param>
		/// <example>contacts, opportunities </example>
		/// <param name="id">record id</param>
		/// <returns>sucesss</returns>
		async Task<bool> ICrmApiService.DeleteRecord(string entitySetName, Guid id)
		{
			ThrowIf.ArgumentNull("entityLogicalName can't be null", entitySetName);
			ThrowIf.ArgumentNull("Record Id can't be blank", id);
			string requestUri = string.Format("{0}{1}({2})", HttpCrmClient.BaseAddress, entitySetName, id);
			HttpResponseMessage deleteResponse = await HttpCrmClient.DeleteAsync(requestUri);
			string responseBody = await deleteResponse.Content.ReadAsStringAsync();
			if (deleteResponse.IsSuccessStatusCode) //200-299 
			{
				Console.WriteLine("Entity deleted: \n{0}.", id);
				return true;
			}
			else if (deleteResponse.StatusCode == HttpStatusCode.NotFound) //404 
			{
				throw new HttpRequestException($"Entity not found: {id} with execption: {responseBody}.\nReasonPhase:{deleteResponse.ReasonPhrase}");
			}
			else
			{
				throw new HttpRequestException($"Failed to delete record: {id} with execption: {responseBody}.\nReasonPhase:{deleteResponse.ReasonPhrase}");
			}
		}

		/// <summary>
		/// Retieve Record by Id
		/// </summary>
		/// <param name="entitySetName"></param>
		/// <example>contacts, opportunities </example>
		/// <param name="id"></param>
		/// <param name="queryOptions"></param>
		/// <returns>Record</returns>
		async Task<string> ICrmApiService.RetrieveRecord(string entitySetName, Guid id, string queryOptions)
		{
			ThrowIf.ArgumentNull("entityLogicalName can't be null", entitySetName);
			ThrowIf.ArgumentNull("Record Id can't be blank", id);

			string requestUri = string.Format("{0}{1}({2}){3}", HttpCrmClient.BaseAddress, entitySetName, id, queryOptions);

			HttpResponseMessage retrieveResponse = await HttpCrmClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
			if (retrieveResponse.IsSuccessStatusCode) //200
			{
				return await retrieveResponse.Content?.ReadAsStringAsync();
			}
			else
			{
				throw new Exception(string.Format("Failed to retrieve contact for reason: {0}\n Exception:{1}",
					retrieveResponse.Content, await retrieveResponse.Content.ReadAsStringAsync()));
			}
		}

		/// <summary>
		/// retruns list of matching records
		/// </summary>
		/// <param name="entitySetName"></param>
		/// <param name="query">select and filter query and optional orderby</param>
		/// <param name="isFetchXml">define is the query is fetch xml or not</param>
		/// <see cref="https://msdn.microsoft.com/en-us/library/mt607843.aspx"/>
		/// <seealso cref="https://msdn.microsoft.com/en-us/library/gg334767.aspx#bkmk_filter"/>
		/// <seealso cref="https://msdn.microsoft.com/en-us/library/gg334767.aspx#bkmk_filter"/>
		/// <seealso cref="https://msdn.microsoft.com/en-us/library/gg309638.aspx#bkmk_passParametersToFunctions"/>
		/// <see cref="https://msdn.microsoft.com/en-us/library/gg328117.aspx"/>
		/// <example>?$select=firstname,lastname&$filter=contains(fullname,'(sample)')&$orderby=jobtitle asc, annualincome desc</example>
		/// <returns>entity Collection</returns>
		async Task<string> ICrmApiService.RetrieveRecords(string entitySetName, string query, bool isFetchXml = false, int? maxpagesize = null)
		{
			ThrowIf.ArgumentNull("entityLogicalName can't be null", entitySetName);
			ThrowIf.ArgumentNull("query can't be blank", query);
			if (maxpagesize != null)
			{
				HttpCrmClient.DefaultRequestHeaders.Remove("Prefer");
				HttpCrmClient.DefaultRequestHeaders.Add("Prefer", $"odata.maxpagesize={maxpagesize}");
			}

			if (isFetchXml)
			{
				return await this.GetRecordsUsingFetchXml(entitySetName, query);
			}
			else
			{
				string requestUri = $"{ entitySetName}{query}";
				HttpResponseMessage response = await HttpCrmClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);

				if (response.IsSuccessStatusCode) //200
				{
					string entityCollection = await response.Content.ReadAsStringAsync();
					return entityCollection;
				}
				else
				{
					throw new Exception(string.Format("Failed to retrieve {0}. Reason: {1}.\n Exception: {2}. \n Query {3}",
						entitySetName, response.ReasonPhrase, await response.Content.ReadAsStringAsync(), query));
				}
			}
		}

		/// <summary>
		/// retruns list of matching record with paging when odataNextLink is provided
		/// </summary>
		/// <param name="odataNextLink"></param>
		/// <param name="maxpagesize"></param>
		/// <returns>list of records with odataNextLink for Next Retrieve </returns>
		async Task<string> ICrmApiService.RetrieveRecordsByNextLink(string odataNextLink, int maxpagesize)
		{
			HttpCrmClient.DefaultRequestHeaders.Remove("Prefer");
			HttpCrmClient.DefaultRequestHeaders.Add("Prefer", $"odata.maxpagesize={maxpagesize}");

			HttpResponseMessage response = await HttpCrmClient.GetAsync(odataNextLink, HttpCompletionOption.ResponseHeadersRead);

			if (response.IsSuccessStatusCode) //200
			{
				string entityCollection = await response.Content.ReadAsStringAsync();
				return entityCollection;
			}
			else
			{
				throw new Exception(string.Format("Failed to retrieve {0}. Reason: {1}.\n Exception: {2}",
					odataNextLink, response.ReasonPhrase, await response.Content.ReadAsStringAsync()));
			}
		}

		/// <summary>
		/// Update the entity (Record Id) in Dynamics CRM
		/// </summary>
		/// <param name="entitySetName">Entity LogicalName</param>
		/// <param name="id">Record Id</param>
		/// <param name="jsonUpdateRequest"> Request Object</param>
		/// <returns> success </returns>
		async Task<bool> ICrmApiService.UpdateRecord(string entitySetName, Guid id, string jsonUpdateRequest)
		{
			ThrowIf.ArgumentNull("entityLogicalName can't be null", entitySetName);
			ThrowIf.ArgumentNull("Record Id can't be blank", id);
			ThrowIf.ArgumentNull("jsonUpdateRequest can't be blank", jsonUpdateRequest);

			string requestUri = string.Format("{0}{1}({2})", HttpCrmClient.BaseAddress, entitySetName, id);
			HttpRequestMessage updateRequest = new HttpRequestMessage(HttpMethod.Patch, requestUri)
			{
				Content = new StringContent(jsonUpdateRequest, Encoding.UTF8, "application/json")
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

		private async Task<string> GetRecordsUsingFetchXml(string entitySetName, string fetchXmlQuery)
		{
			//Must encode the FetchXML query because it's a part of the request (GET) string .
			string requestUri = string.Format("{0}s?{1}", entitySetName, WebUtility.UrlEncode(fetchXmlQuery));
			HttpResponseMessage response = await HttpCrmClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);

			if (response.IsSuccessStatusCode) //200
			{
				string entityCollection = await response.Content.ReadAsStringAsync();
				return entityCollection;
			}
			else
			{
				throw new Exception(string.Format("Failed to retrieve {0}. Reason: {1}.\n Exception: {2}",
					entitySetName, response.Content, await response.Content.ReadAsStringAsync()));
			}
		}
	}
}