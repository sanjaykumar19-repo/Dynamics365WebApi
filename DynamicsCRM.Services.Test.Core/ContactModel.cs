namespace D365.ConsoleAppTest
{
   using Newtonsoft.Json;
   using System;
   using System.Collections.Generic;

   public class Contact
   {

      [JsonProperty("@odata.etag")]
      public string OdataETag { get; set; }

      [JsonProperty("fullname")]
      public string FullName { get; set; }

      [JsonProperty("firstname")]
      public string Firstname { get; set; }

      [JsonProperty("lastname")]
      public string Lastname { get; set; }

      [JsonProperty("emailaddress1")]
      public object Emailaddress1 { get; set; }

      [JsonProperty("createdon")]
      public DateTime Createdon { get; set; }

      [JsonProperty("description")]
      public string Description { get; set; }

      [JsonProperty("contactid")]
      public Guid Contactid { get; set; }
   }

   public class EntityCollection
   {

      [JsonProperty("@odata.context")]
      public string OdataContext { get; set; }

      [JsonProperty("value")]
      public IList<Contact> Results { get; set; }

      [JsonProperty("@odata.nextLink")]
      public string OdataNextLink { get; set; }
   }
}
