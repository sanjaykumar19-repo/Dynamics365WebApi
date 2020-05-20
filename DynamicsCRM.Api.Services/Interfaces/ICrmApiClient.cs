namespace DynamicsCRM.Api.Services
{
   using DynamicsCRM.Api.Services.Models;
   using Microsoft.Xrm.Sdk;
   using System;
   using System.Threading.Tasks;

   public interface ICrmApiClient
   {
      /// <summary>
      /// User successfully connected or not
      /// </summary>
      bool IsReady { get; }

      /// <summary>
      /// Provides Authenticated User Info and Organization details
      /// </summary>
      WhoAmIResponse WhoAmI { get; }

      /// <summary>
      /// Create a new entity record in Dynamics CRM and returns record GUID
      /// </summary>
      /// <param name="entitySetName">Entity LogicalName</param>
      /// <param name="jsonRequest"> Request Object</param>
      /// <returns> Record Id</returns>
      Task<Guid> CreateRecord(string entitySetName, string jsonRequest);

      /// <summary>
      /// Update the entity (Record Id) in Dynamics CRM
      /// </summary>
      /// <param name="entitySetName">Entity LogicalName</param>
      /// <param name="id">Record Id</param>
      /// <param name="jsonUpdateRequest"> Request Object</param>
      /// <returns> success </returns>
      Task<bool> UpdateRecord(string entitySetName, Guid id, string jsonUpdateRequest);

      /// <summary>
      /// delete entity record 
      /// </summary>
      /// <param name="entitySetName">entity name</param>
      /// <param name="id">record id</param>
      /// <returns>sucesss</returns>
      Task<bool> DeleteRecord(string entitySetName, Guid id);

      /// <summary>
      /// Return Entity Record
      /// </summary>
      /// <param name="entitySetName"> Entity Name</param>
      /// <param name="id">Record Id</param>
      /// <param name="queryOptions">select and filter query</param>
      /// <see cref="https://msdn.microsoft.com/en-us/library/mt607843.aspx"/>
      /// <returns> Entity Record Object</returns>
      Task<string> RetrieveRecord(string entitySetName, Guid id, string queryOptions);

      /// <summary>
      /// retruns list of matching records
      /// </summary>
      /// <param name="entitySetName"> Entity Name</param>
      /// <param name="queryOptions">select and filter query</param>
      /// <param name="isFetchXml">define is provided query is fetch query or not</param>
      /// <param name="maxpagesize">maxpagesize for paging</param>
      /// <see cref="https://msdn.microsoft.com/en-us/library/mt607843.aspx"/>
      /// <seealso cref="https://msdn.microsoft.com/en-us/library/gg334767.aspx#bkmk_filter"/>
      /// <seealso cref="https://msdn.microsoft.com/en-us/library/gg334767.aspx#bkmk_filter"/>
      /// <example>?$select=firstname,lastname&$filter=contains(fullname,'(sample)')</example>
      /// <returns>entity Collection</returns>
      Task<string> RetrieveRecords(string entitySetName, string queryOptions, bool isFetchXml = false, int? maxpagesize = null);

      /// <summary>
      /// retruns list of matching record with paging when odataNextLink is provided
      /// </summary>
      /// <param name="odataNextLink"></param>
      /// <param name="maxpagesize"></param>
      /// <returns>list of records with odataNextLink for Next Retrieve </returns>
      Task<string> RetrieveRecordsByNextLink(string odataNextLink, int maxpagesize);

      /// <summary>
      /// Create a new entity record in Dynamics CRM and returns record GUID
      /// </summary>
      /// <param name="entitySetName">Entity LogicalName</param>
      /// <param name="entity"> Request Object</param>
      /// <returns> Record Id</returns>
      Task<Guid> CreateRecord(string entitySetName, Entity entity);

      /// <summary>
      /// Update the entity (Record Id) in Dynamics CRM
      /// </summary>
      /// <param name="entitySetName">Entity LogicalName</param>
      /// <param name="id">Record Id</param>
      /// <param name="updateRequest"> Request Object</param>
      /// <returns> success </returns>
      Task<bool> UpdateRecord(string entitySetName, Guid id, Entity updateRequest);
   }
}