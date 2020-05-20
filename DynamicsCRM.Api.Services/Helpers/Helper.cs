namespace DynamicsCRM.Api.Services.Helpers
{
	using DynamicsCRM.Api.Services.Models;
	using System;
	using System.Net.Http;
	using System.Text;

	internal class Helper
	{
		/// <summary> Displays exception information to the console. </summary>
		/// <param name="ex">The exception to output</param>
		internal static string GetExceptionDetail(Exception ex)
		{
			StringBuilder exceptionMessage = new StringBuilder();

			exceptionMessage.Append(ex.Message);
			while (ex.InnerException != null)
			{
				exceptionMessage.Append(string.Format("\t* {0}", ex.InnerException?.Message));
				ex = ex.InnerException;
			}

			exceptionMessage.Append(ex);
			return exceptionMessage.ToString();
		}

		internal static string GetEntitySetName(HttpClient httpClient, string entityLogicalName)
		{
			string requestUri = $"EntityDefinitions(LogicalName='{entityLogicalName}')?$select=EntitySetName";
			var response = httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
			if (response.IsSuccessStatusCode)
			{
				string content = response.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
				EntityDefinition entityDefinition = JSONSerializer<EntityDefinition>.DeSerialize(content);
				return entityDefinition?.EntitySetName;
			}
			else
			{
				string content = response.Content?.ReadAsStringAsync().GetAwaiter().GetResult(); ;
				throw new InvalidOperationException($"Missing entityset name for entity:{entityLogicalName}. Error Details:{content}");
			}
		}
	}
}
