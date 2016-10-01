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
		private const String RESILIENTS_VERSION_ID = "ResilientOwnersModVersion";

		private const int SAVE_DATA_VERSION = 1;

		ISerializableData m_serializedData;

		public static ResilientBuildings s_info;
		public static int s_savedDataVersion = 0;

		public static List<ResilientBuildings.ResilientInfoV1> s_data;

		public override void OnCreated(ISerializableData serializedData) {
			base.OnCreated(serializedData);
			m_serializedData = serializedData;
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

					byte[] data2 = m_serializedData.LoadData(RESILIENTS_VERSION_ID);
					if (data2 != null) {
						BinaryFormatter bFormatter = new BinaryFormatter();
						MemoryStream mStream       = new MemoryStream(data2);
						s_savedDataVersion = (int)bFormatter.Deserialize(mStream);


					} else {
						//save had no data
					}

					if(s_savedDataVersion >= 0 && s_savedDataVersion <= SAVE_DATA_VERSION)
					{
						byte[] data = m_serializedData.LoadData(RESILIENTS_DATA_ID);
						if (data != null) {
							BinaryFormatter bFormatter = new BinaryFormatter();
							MemoryStream mStream       = new MemoryStream(data);
							switch(s_savedDataVersion)
							{
							case 0:
								List<ResilientBuildings.ResilientInfo> legacyList = (List<ResilientBuildings.ResilientInfo>)bFormatter.Deserialize(mStream);
								s_data = convertVersionZeroListToOne(legacyList);
								break;
							case 1:
								s_data = (List<ResilientBuildings.ResilientInfoV1>)bFormatter.Deserialize(mStream);
								break;
							}

							CODebug.Log (LogChannel.Modding, Mod.modName+" - successful loading data");
							
						} else {

						}
					}
					else {
						CODebug.Error (LogChannel.Modding, Mod.modName+" - invalid saved data version");
					}

				}
				
			} catch (Exception e) {
				CODebug.Log (LogChannel.Modding, Mod.modName+" - Error loading data "+e.Message);
			}
		}

		public List<ResilientBuildings.ResilientInfoV1> convertVersionZeroListToOne(List<ResilientBuildings.ResilientInfo> list)
		{
			CODebug.Log (LogChannel.Modding, Mod.modName+" - Converting save data from version 0 to 1");
			List<ResilientBuildings.ResilientInfoV1> newList = new List<ResilientBuildings.ResilientInfoV1>();
			for(int i=0;i<list.Count;i++)
			{
				ResilientBuildings.ResilientInfo ri0 = list[i];
				ResilientBuildings.ResilientInfoV1 ri1 = new ResilientBuildings.ResilientInfoV1();
				ri1.activatedDate = ri0.activatedDate;
				ri1.buildingID = ri0.buildingID;
				ri1.chosenLevel = ri0.chosenLevel;
				ri1.currentVisits = ri0.currentVisits;
				ri1.description = ri0.description;
				ri1.goodsBuffer1 = ri0.goodsBuffer1;
				ri1.goodsBuffer2 = ri0.goodsBuffer2;
				ri1.goodsBuffer3 = ri0.goodsBuffer3;
				ri1.goodsBuffer4 = ri0.goodsBuffer4;
				ri1.resiliencyActivated = ri0.resiliencyActivated;
				ri1.totalIncome = ri0.totalIncome;
				ri1.totalVisits = ri0.totalVisits;
				ri1.unsuscribed = ri0.unsuscribed;
				ri1.unsuscribeTimer = ri0.unsuscribeTimer;
				ri1.namesBuffer = new List<string>();
				newList.Add(ri1);
			}
			return newList;
		}

		public override void OnSaveData() {
		base.OnSaveData();

			CODebug.Log (LogChannel.Modding, Mod.modName+" - try saving data");
			try {
				if (m_serializedData != null) {
					BinaryFormatter bFormatter2 = new BinaryFormatter();
					MemoryStream mStream2       = new MemoryStream();
					bFormatter2.Serialize(mStream2, SAVE_DATA_VERSION);
					byte[] data2 = mStream2.ToArray();
					if (data2 != null) {
						m_serializedData.SaveData(RESILIENTS_VERSION_ID, data2);
					}
					
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

