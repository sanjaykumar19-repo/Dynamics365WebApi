namespace DynamicsCRM.Api.Services.Helpers
{
   using DynamicsCRM.Api.Services.Models;
   using System;
   using System.Net.Http;

   public abstract class CrmClientHelper
   {
      protected static HttpClient CrmService { get; private set; }

      protected CrmClientHelper(WebApiConfiguration configuration)
      {
         ThrowIf.ArgumentNull("WebApiConfiguration can't be null", configuration, configuration.CRMApiEndpointUri);
         try
         {
            HttpMessageHandler messageHandler = new OAuthMessageHandler(configuration, new HttpClientHandler());
            CrmService = new HttpClient(messageHandler)
            {
               Timeout = new TimeSpan(0, 2, 0)  //2 minutes
            };
         }
         catch (Exception)
         {
            throw;
         }
      }
   }
}
