using System;
using System.Collections.Generic;
using UnityEngine;
using ICities;
using ColossalFramework;

namespace ResilientOwners
{
	public class ResilientBuildings : MonoBehaviour
	{
		[System.Serializable]
		public struct ResilientInfo
		{
			public ushort buildingID;

			public string name;
			public ItemClass.Service service;
			public ItemClass.Layer layer;
			public Building.Flags flags;

			public List<uint> residents;
			public List<uint> residents_units;
			public List<string> residents_names;



			public string description;
			public ItemClass.Level chosenLevel;
		}

		public List<ResilientInfo> m_resilients;

		public void InitializeList()
		{
			if (m_resilients == null) {
				m_resilients = new List<ResilientInfo> ();
				CODebug.Log (LogChannel.Modding, "initialize resilient building list");
			} else {
				CODebug.Log (LogChannel.Modding, "a resilient building list has benn loaded");
			}
		}

		public int GetResilientBuildingIndex(ushort buildingID)
		{
			for(int i = 0; i < m_resilients.Count; i++)
			{
				if(buildingID == m_resilients[i].buildingID)
					return i;
			}

			return -1;
		}

		public void AddBuilding(ushort buildingID) {
			CODebug.Log(LogChannel.Modding, "add building "+Singleton<BuildingManager>.instance.GetBuildingName(buildingID, default(InstanceID))+" "+Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.m_class.m_service);

			if(GetResilientBuildingIndex(buildingID) != -1)
				return;

			ResilientInfo ri = new ResilientInfo();
			ri.buildingID = buildingID;
			ri.name = Singleton<BuildingManager>.instance.GetBuildingName (buildingID, default(InstanceID));
			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
			ri.service = build.Info.m_class.m_service;
			ri.layer = build.Info.m_class.m_layer;
			ri.chosenLevel = build.Info.m_class.m_level;

			ri.residents = new List<uint>();
			ri.residents_units = new List<uint>();
			ri.residents_names = new List<string>();


			//speed up abandonment
			Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_electricityProblemTimer = 60;
			Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 60;

			m_resilients.Add(ri);
			UpdateResidents (m_resilients.Count-1);

			ri = m_resilients[m_resilients.Count-1];
			ri.description = ri.name+"\n\n";
			foreach (string cname in ri.residents_names)
				ri.description += cname + "\n";

			m_resilients [m_resilients.Count - 1] = ri;
		}

		public void RemoveBuilding(ushort buildingID)
		{
			int index = GetResilientBuildingIndex(buildingID);
				
			m_resilients.RemoveAt (index);
		}

		public void UpdateResidents(int resilient_index)
		{
			m_resilients [resilient_index].residents_units.Clear ();
			m_resilients [resilient_index].residents.Clear ();
			m_resilients [resilient_index].residents_names.Clear ();

			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_resilients[resilient_index].buildingID];
			CitizenManager instance = Singleton<CitizenManager>.instance;
			uint num = build.m_citizenUnits;
			CODebugBase<LogChannel>.Log(LogChannel.Modding, "citizen unit = "+num+" citizen count = "+build.m_citizenCount);
			int num2 = 0;
			while (num != 0u)
			{
				uint nextUnit = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
//				if (/*(ushort)(instance.m_units.m_buffer[(int)((UIntPtr)num)].m_flags & flag) != 0 && */!instance.m_units.m_buffer[(int)((UIntPtr)num)].Full())
//				{
//					break;
//				}
				m_resilients[resilient_index].residents_units.Add(num);
				m_resilients[resilient_index].residents.Add(instance.m_units.m_buffer[num].m_citizen0);
				if(instance.m_units.m_buffer[num].m_citizen0 != 0)
					m_resilients[resilient_index].residents_names.Add(instance.GetCitizenName(instance.m_units.m_buffer[num].m_citizen0));
				m_resilients[resilient_index].residents.Add(instance.m_units.m_buffer[num].m_citizen1);
				if(instance.m_units.m_buffer[num].m_citizen1 != 0)
					m_resilients[resilient_index].residents_names.Add(instance.GetCitizenName(instance.m_units.m_buffer[num].m_citizen1));
				m_resilients[resilient_index].residents.Add(instance.m_units.m_buffer[num].m_citizen2);
				if(instance.m_units.m_buffer[num].m_citizen2 != 0)
					m_resilients[resilient_index].residents_names.Add(instance.GetCitizenName(instance.m_units.m_buffer[num].m_citizen2));
				m_resilients[resilient_index].residents.Add(instance.m_units.m_buffer[num].m_citizen3);
				if(instance.m_units.m_buffer[num].m_citizen3 != 0)
					m_resilients[resilient_index].residents_names.Add(instance.GetCitizenName(instance.m_units.m_buffer[num].m_citizen3));
				m_resilients[resilient_index].residents.Add(instance.m_units.m_buffer[num].m_citizen4);
				if(instance.m_units.m_buffer[num].m_citizen4 != 0)
					m_resilients[resilient_index].residents_names.Add(instance.GetCitizenName(instance.m_units.m_buffer[num].m_citizen4));

				num = nextUnit;
				if (++num2 > 524288)
				{
					CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
					break;
				}
			}
		}


	}
}

