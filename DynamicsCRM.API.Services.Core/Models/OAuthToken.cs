namespace DynamicsCRM.Api.Services.Models
{
   using System;
   using System.Runtime.Serialization;

   [DataContract]
   public class OAuthToken
   {
      public DateTime TokenInitDate { get; set; } = DateTime.UtcNow;

      [DataMember(Name = "token_type")]
      public string TokenType { get; set; }

      [DataMember(Name = "expires_in")]
      public double ExpiresIn { get; set; }

      [DataMember(Name = "expires_on")]
      public string ExpiresOn { get; set; }

      [DataMember(Name = "not_before")]
      public string NotBefore { get; set; }

      [DataMember(Name = "resource")]
      public string ResourceUri { get; set; }

      [DataMember(Name = "access_token")]
      public string AccessToken { get; set; }
   }
}
