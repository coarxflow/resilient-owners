using System;
using System.Collections.Generic;
using UnityEngine;
using ICities;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.Globalization;

namespace ResilientOwners
{
	public class ResilientBuildings : MonoBehaviour
	{
		[System.Serializable]
		public struct ResilientInfo
		{
			public ushort buildingID;

			public string name;
			public ItemClass.Layer layer;

			public DateTime activatedDate;

			public bool resiliencyActivated;
			public ItemClass.Level chosenLevel;

			public List<uint> families_id;
			public List<string> families_names;
			public string description;

			public long total_income;

			public bool unsuscribed;
			public int unsuscribeTimer;
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

		public void AddBuilding(ushort buildingID, bool resilient) {
			CODebug.Log(LogChannel.Modding, "add building "+Singleton<BuildingManager>.instance.GetBuildingName(buildingID, default(InstanceID))+" "+Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.m_class.m_service);

			ResilientInfo ri;

			int buildIndex = GetResilientBuildingIndex(buildingID);
			if(buildIndex != -1)
			{
				ri = m_resilients[buildIndex];
				ri.unsuscribed = false;
				ri.unsuscribeTimer = 0;
				ri.resiliencyActivated = resilient;
				m_resilients[buildIndex] = ri;
				Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 0;
				Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_problems = Notification.RemoveProblems(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_problems, Notification.Problem.MajorProblem);
				return;
			}

			ri = new ResilientInfo();
			ri.buildingID = buildingID;
			ri.activatedDate = Singleton<SimulationManager>.instance.m_currentGameTime;
			ri.name = Singleton<BuildingManager>.instance.GetBuildingName (buildingID, default(InstanceID));
			ri.description = "";
			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
			ri.layer = build.Info.m_class.m_layer;
			ri.chosenLevel = build.Info.m_class.m_level;

			ri.families_id = new List<uint>();

			//speed up abandonment
//			Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_electricityProblemTimer = 60;
//			Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 60;

			m_resilients.Add(ri);

			UpdateResidentFamilies(m_resilients.Count-1);
		}

		public void UnsuscribeBuilding(ushort buildingID)
		{
			int index = GetResilientBuildingIndex(buildingID);

			if(index != -1)
			{
				ResilientInfo ri = m_resilients[index];
				ri.unsuscribed = true;
				m_resilients[index] = ri;
			}
		}

		public void RemoveBuilding(ushort buildingID)
		{
			int index = GetResilientBuildingIndex(buildingID);
				
			m_resilients.RemoveAt (index);
		}

		//residential building history updates
		public void UpdateResidentFamilies(int resilient_index)
		{
			List<uint> current_families_id = new List<uint>();

			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_resilients[resilient_index].buildingID];
			CitizenManager instance = Singleton<CitizenManager>.instance;
			uint num = build.m_citizenUnits;
			int num2 = 0;
			while (num != 0u)
			{
				uint nextUnit = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
//				if (/*(ushort)(instance.m_units.m_buffer[(int)((UIntPtr)num)].m_flags & flag) != 0 && */!instance.m_units.m_buffer[(int)((UIntPtr)num)].Full())
//				{
//					break;
//				}
				current_families_id.Add(num);
				num = nextUnit;

				if (++num2 > 524288)
				{
					CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
					break;
				}
			}

			//update history family list with any new names
			for(int i = 0; i < current_families_id.Count; i++)
			{
				if(!m_resilients[resilient_index].families_id.Contains(current_families_id[i]))
				{
					CODebugBase<LogChannel>.Log(LogChannel.Modding, "new unit uint "+num+" family byte "+instance.m_citizens.m_buffer[instance.m_units.m_buffer[num].m_citizen0].m_family);
					m_resilients[resilient_index].families_id.Add(current_families_id[i]);
//					int family = instance.m_citizens.m_buffer[instance.m_units.m_buffer[num].m_citizen0].m_family;
//					Randomizer randomizer2 = new Randomizer(family);
//					string text2 = "NAME_FEMALE_LAST";
////					if (Citizen.GetGender(citizenID) == Citizen.Gender.Male)
////					{
////						text = "NAME_MALE_FIRST";
////						text2 = "NAME_MALE_LAST";
////					}
//					text2 = Locale.Get(text2, randomizer2.Int32(Locale.Count(text2)));
				}
			}
		}

		public string GetFamiliesList(int buildIndex)
		{
			ResilientInfo ri = m_resilients[buildIndex];

			string ret;
			if(ri.families_id.Count > 0)
			{
				ret = Localization.GetFamiliesHistory();
				for(int i = 0; i < ri.families_id.Count; i++)
				{
					int family = Singleton<CitizenManager>.instance.m_citizens.m_buffer[Singleton<CitizenManager>.instance.m_units.m_buffer[ri.families_id[i]].m_citizen0].m_family;
					Randomizer randomizer2 = new Randomizer(family);
					string text2 = "NAME_FEMALE_LAST";
	//					if (Citizen.GetGender(citizenID) == Citizen.Gender.Male)
	//					{
	//						text = "NAME_MALE_FIRST";
	//						text2 = "NAME_MALE_LAST";
	//					}
					text2 = Locale.Get(text2, randomizer2.Int32(Locale.Count(text2)));
					ret += text2.Substring(4);//remove placeholder in front
					if(i < ri.families_id.Count-1)
						ret += Localization.GetFamiliesSeparator();
				}
			}
			else
			{
				ret = Localization.GetEmptyHouse();
			}

			return ret;
		}

		//commercial building history updates
		public int UpdateVisitsCount(ushort buildingID)
		{
			CitizenManager instance = Singleton<CitizenManager>.instance;
			Citizen.BehaviourData behaviour = new Citizen.BehaviourData();
			int aliveCount = 0;
			int totalCount = 0;

			uint num = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_citizenUnits;
			int num2 = 0;
			while (num != 0u)
			{
				if ((ushort)(instance.m_units.m_buffer[(int)((UIntPtr)num)].m_flags & CitizenUnit.Flags.Visit) != 0)
				{
					instance.m_units.m_buffer[(int)((UIntPtr)num)].GetCitizenVisitBehaviour(ref behaviour, ref aliveCount, ref totalCount);
				}
				num = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
				if (++num2 > 524288)
				{
					CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
					break;
				}
			}

			return aliveCount;
		}

	}
}

