// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace AssemblyCSharp
{
	[XmlRoot("PlayerCollection")]
	public class PlayerInformationContainer
	{
		[XmlArray("players"),XmlArrayItem("player")]
		public List<PlayerInformation> players = new List<PlayerInformation>();
		
		public void Save(string path)
		{
			var serializer = new XmlSerializer(typeof(PlayerInformationContainer));
			using(var stream = new FileStream(path, FileMode.Create))
			{
				serializer.Serialize(stream, this);
			}
		}
		
		public static PlayerInformationContainer Load(string path)
		{
			var serializer = new XmlSerializer(typeof(PlayerInformationContainer));
			using(var stream = new FileStream(path, FileMode.Open))
			{
				return serializer.Deserialize(stream) as PlayerInformationContainer;
			}
		}
		
		//Loads the xml directly from the given string. Useful in combination with www.text.
		public static PlayerInformationContainer LoadFromText(string text) 
		{
			var serializer = new XmlSerializer(typeof(PlayerInformationContainer));
			return serializer.Deserialize(new StringReader(text)) as PlayerInformationContainer;
		}
	}
}


