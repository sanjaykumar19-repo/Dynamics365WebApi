namespace DynamicsCRM.Api.Services.Models
{
   using System;
   using System.Runtime.Serialization;

   [DataContract]
   public class WhoAmIResponse
   {
      [DataMember(Name = "BusinessUnitId")]
      public Guid? BusinessUnitId { get; set; }

      [DataMember(Name = "UserId")]
      public Guid? UserId { get; set; }

      [DataMember(Name = "OrganizationId")]
      public Guid? OrganizationId { get; set; }
   }
}
