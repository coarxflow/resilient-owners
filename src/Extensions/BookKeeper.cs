using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using ICities;

namespace ResilientOwners
{
	public class BookKeeper : SerializableDataExtensionBase
	{
		private const String RESILIENTS_DATA_ID = "ResilientOwnersMod";

		ISerializableData m_serializedData;

		public static ResilientBuildings s_info;

		public static List<ResilientBuildings.ResilientInfo> s_data;

		public override void OnCreated(ISerializableData serializedData) {
			base.OnCreated(serializedData);
			m_serializedData = serializedData;
			CODebug.Log (LogChannel.Modding, Mod.modName+" - bookkepper created");
		}
		
		public override void OnReleased() {
		}
		
		public override void OnLoadData() {
			base.OnLoadData();

			CODebug.Log (LogChannel.Modding, Mod.modName+" - try loading data");
//			if (s_info == null) {
//				s_info
//			}

			try {
				if (m_serializedData != null) {
					byte[] data = m_serializedData.LoadData(RESILIENTS_DATA_ID);
					if (data != null) {
						BinaryFormatter bFormatter = new BinaryFormatter();
						MemoryStream mStream       = new MemoryStream(data);
						s_data = (List<ResilientBuildings.ResilientInfo>)bFormatter.Deserialize(mStream);
						for(int i = 0; i < s_data.Count; i++)
						{
							s_data[i].idsBuffer.Clear();
						}
						CODebug.Log (LogChannel.Modding, Mod.modName+" - successful loading data");
						
					} else {

					}
				}
				
			} catch (Exception e) {
				CODebug.Log (LogChannel.Modding, Mod.modName+" - Error loading data "+e.Message);
			}
		}
		
		public override void OnSaveData() {
		base.OnSaveData();

			CODebug.Log (LogChannel.Modding, Mod.modName+" - try saving data");
			try {
				if (m_serializedData != null) {
					BinaryFormatter bFormatter = new BinaryFormatter();
					MemoryStream mStream       = new MemoryStream();
					bFormatter.Serialize(mStream, s_info.m_resilients);

					byte[] data = mStream.ToArray();
					if (data != null) {
						m_serializedData.SaveData(RESILIENTS_DATA_ID, data);
					}
					CODebug.Log (LogChannel.Modding, Mod.modName+" - successful saving data");
					
				} else {

				}
			} catch (Exception e) {
				CODebug.Log (LogChannel.Modding, Mod.modName+" - Error saving data "+e.StackTrace);
			}
		}
	}
}

