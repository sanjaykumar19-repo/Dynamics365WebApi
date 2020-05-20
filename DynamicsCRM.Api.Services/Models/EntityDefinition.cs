namespace DynamicsCRM.Api.Services.Models
{
	using System.Runtime.Serialization;

	[DataContract]
	public class EntityDefinition
	{
		[DataMember(Name = "@odata.context")]
		public string OdataContext { get; set; }

		[DataMember(Name = "EntitySetName")]
		public string EntitySetName { get; set; }

		[DataMember(Name = "MetadataId")]
		public string MetadataId { get; set; }
	}
}
