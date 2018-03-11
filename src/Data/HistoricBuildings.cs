using System;
using System.Collections.Generic;
using UnityEngine;
using ICities;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.Globalization;

namespace HistoricBuildings
{
	public class HistoricBuildings : MonoBehaviour
	{
        public Dictionary<ushort, ItemClass.Level> buildings;
        public Dictionary<byte, ushort> districts;

        /*********** Manage Building List *************/

        public void InitializeList()
		{
			if (buildings == null) {
                buildings = new Dictionary < ushort, ItemClass.Level > ();
				//CODebug.Log (LogChannel.Modding, "initialize resilient building list");
			} else {
				//CODebug.Log (LogChannel.Modding, "a resilient building list has benn loaded");
			}

            if (districts == null)
            {
                districts = new Dictionary<byte, ushort>();
                //CODebug.Log (LogChannel.Modding, "initialize resilient building list");
            }
            else
            {
                //CODebug.Log (LogChannel.Modding, "a resilient building list has benn loaded");
            }
        }

		public bool AddBuilding(ushort buildingID) {
            buildings.Add(buildingID, Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.GetClassLevel());
            return true;
		}

		public void RemoveBuilding(ushort buildingID)
		{
            buildings.Remove (buildingID);
		}

        /*********** Manage Districts List *************/

        public void AddDistrict(byte districtID)
        {
            districts.Add(districtID, 0);
        }

        public void RemoveDistrict(byte districtID)
        {
            if (districts.ContainsKey(districtID))
                districts[districtID] = 1; //start unsuscribe timer
        }
    }
}

