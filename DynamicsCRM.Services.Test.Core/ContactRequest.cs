using System.Runtime.Serialization;

namespace D365.ConsoleAppTest
{
   [DataContract]
   public class ContactRequest
   {
      [DataMember(Name = "lastname")]
      public string lastname { get; set; }

      [DataMember(Name = "firstname")]
      public string firstname { get; set; }


      [DataMember(Name = "emailaddress1")]
      public string emailaddress1 { get; set; }
   }
}
