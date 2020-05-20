namespace DynamicsCRM.Api.Services.Helpers
{
	using System.IO;
	using System.Runtime.Serialization.Json;
	using System.Text;

	public static class JSONSerializer<TObject>
	{
		/// <summary>
		/// Serializes an object to JSON
		/// </summary>
		public static string Serialize(TObject instance)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TObject));
			using (MemoryStream stream = new MemoryStream())
			{
				serializer.WriteObject(stream, instance);
				return Encoding.Default.GetString(stream.ToArray());
			}
		}

		/// <summary>
		/// DeSerializes an object from JSON
		/// </summary>
		public static TObject DeSerialize(string json)
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(json)))
			{
				var serializer = new DataContractJsonSerializer(typeof(TObject));
				return (TObject)serializer.ReadObject(stream);
			}
		}
	}
}
