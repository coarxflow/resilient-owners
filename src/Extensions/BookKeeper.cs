using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using ICities;
using UnityEngine;

namespace HistoricBuildings
{
	public class BookKeeper : SerializableDataExtensionBase
	{
		private const String HISTORICS_DATA_ID = "HistoricBuildingsMod";
        private const String HISTORICS_DISTRICTS_ID = "HistoricBuildingsModDistricts";
        private const String HISTORICS_VERSION_ID = "HistoricBuildingsModVersion";
        private const String HISTORICS_SETTINGS_ID = "HistoricBuildingsModSettings";

        private const String RESILIENTS_DISTRICTS_ID = "ResilientOwnersModDistricts";
        private const String RESILIENTS_VERSION_ID = "ResilientOwnersModVersion";

        private const int SAVE_DATA_VERSION = 1;

        ISerializableData m_serializedData;

		public static HistoricBuildings s_info;
		public static int s_savedDataVersion = 0;

        public static Dictionary<ushort, ItemClass.Level> s_buildings;
        public static Dictionary<byte, ushort> s_districts;

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

					byte[] data2 = m_serializedData.LoadData(HISTORICS_VERSION_ID);
					if (data2 != null) {
						BinaryFormatter bFormatter = new BinaryFormatter();
						MemoryStream mStream       = new MemoryStream(data2);
						s_savedDataVersion = (int)bFormatter.Deserialize(mStream);


					} else {
						//save had no data

                        //attempt recover data from ResilientOwners districts
					}

					if(s_savedDataVersion >= 0 && s_savedDataVersion <= SAVE_DATA_VERSION)
					{
						byte[] data = m_serializedData.LoadData(HISTORICS_DATA_ID);
						if (data != null) {
							BinaryFormatter bFormatter = new BinaryFormatter();
							MemoryStream mStream       = new MemoryStream(data);
                            s_buildings = (Dictionary<ushort,ItemClass.Level>)bFormatter.Deserialize(mStream);
                            CODebug.Log (LogChannel.Modding, Mod.modName+" - successful loading buildings data");
						} else {

						}

                        data = m_serializedData.LoadData(HISTORICS_DISTRICTS_ID);
                        if (data != null)
                        {
                            BinaryFormatter bFormatter = new BinaryFormatter();
                            MemoryStream mStream = new MemoryStream(data);
                            s_districts = (Dictionary<byte, ushort>)bFormatter.Deserialize(mStream);
                            CODebug.Log(LogChannel.Modding, Mod.modName + " - successful loading districts data");
                        }
                        else
                        {

                        }

                        data = m_serializedData.LoadData(HISTORICS_SETTINGS_ID);
                        if (data != null)
                        {
                            BinaryFormatter bFormatter = new BinaryFormatter();
                            MemoryStream mStream = new MemoryStream(data);
                            switch (s_savedDataVersion)
                            {
                                case 2:
                                case 3:
                                    Settings.inst = (ResilientSettings) bFormatter.Deserialize(mStream);
                                    break;
                            }

                            CODebug.Log(LogChannel.Modding, Mod.modName + " - successful loading settings");

                        }
                        else
                        {

                        }
                    }
					else {
						CODebug.Error (LogChannel.Modding, Mod.modName+" - invalid saved data version");
					}

				}
				
			} catch (Exception e) {
				CODebug.Log (LogChannel.Modding, Mod.modName+" - Error loading data "+e.Message);
			}

            if(s_districts == null)
            {

            }
		}

		public override void OnSaveData() {
		base.OnSaveData();

            if (!LoadingExtension.installed)
                return;

            CODebug.Log (LogChannel.Modding, Mod.modName+" - try saving data");
			try {
				if (m_serializedData != null) {
					BinaryFormatter bFormatter2 = new BinaryFormatter();
					MemoryStream mStream2       = new MemoryStream();
					bFormatter2.Serialize(mStream2, SAVE_DATA_VERSION);
					byte[] data2 = mStream2.ToArray();
					if (data2 != null) {
						m_serializedData.SaveData(HISTORICS_VERSION_ID, data2);
					}
					
					BinaryFormatter bFormatter = new BinaryFormatter();
					MemoryStream mStream       = new MemoryStream();
					bFormatter.Serialize(mStream, s_info.buildings);
					byte[] data = mStream.ToArray();
                    if (data != null) {
						m_serializedData.SaveData(HISTORICS_DATA_ID, data);
					}

                    BinaryFormatter bFormatter4 = new BinaryFormatter();
                    MemoryStream mStream4 = new MemoryStream();
                    bFormatter4.Serialize(mStream4, s_info.districts);
                    byte[] data4 = mStream4.ToArray();
                    if (data4 != null)
                    {
                        m_serializedData.SaveData(HISTORICS_DISTRICTS_ID, data4);
                    }

                    BinaryFormatter bFormatter3 = new BinaryFormatter();
                    MemoryStream mStream3 = new MemoryStream();
                    bFormatter3.Serialize(mStream3, Settings.inst);
                    byte[] data3 = mStream3.ToArray();

                    if (data3 != null)
                    {
                        m_serializedData.SaveData(HISTORICS_SETTINGS_ID, data3);
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

