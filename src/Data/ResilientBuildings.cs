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

			public string description;

			public List<uint> idsBuffer; //track families or workers ids

			public long totalIncome;

			public int totalVisits;
			public int currentVisits;

			public int goodsBuffer1;
			public int goodsBuffer2;
			public int goodsBuffer3;
			public int goodsBuffer4;

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

			ri.idsBuffer = new List<uint>();

			ri.goodsBuffer1 = 0;
			ri.goodsBuffer2 = 0;
			ri.goodsBuffer3 = 0;
			ri.goodsBuffer4 = 0;



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
				if(!m_resilients[resilient_index].idsBuffer.Contains(current_families_id[i]))
				{
					CODebugBase<LogChannel>.Log(LogChannel.Modding, "new unit uint "+num+" family byte "+instance.m_citizens.m_buffer[instance.m_units.m_buffer[num].m_citizen0].m_family);
					m_resilients[resilient_index].idsBuffer.Add(current_families_id[i]);
				}
			}
		}

		public string GetFamiliesList(int buildIndex)
		{
			ResilientInfo ri = m_resilients[buildIndex];

			string ret;
			if(ri.idsBuffer.Count > 0)
			{
				ret = Localization.GetFamiliesHistory()+"\n";
				for(int i = 0; i < ri.idsBuffer.Count; i++)
				{
					int family = Singleton<CitizenManager>.instance.m_citizens.m_buffer[Singleton<CitizenManager>.instance.m_units.m_buffer[ri.idsBuffer[i]].m_citizen0].m_family;
					Randomizer randomizer2 = new Randomizer(family);
					string text2 = "NAME_FEMALE_LAST";
	//					if (Citizen.GetGender(citizenID) == Citizen.Gender.Male)
	//					{
	//						text = "NAME_MALE_FIRST";
	//						text2 = "NAME_MALE_LAST";
	//					}
					text2 = Locale.Get(text2, randomizer2.Int32(Locale.Count(text2)));
					ret += text2.Substring(4);//remove placeholder in front
					if(i < ri.idsBuffer.Count-1)
						ret += Localization.GetFamiliesSeparator();
				}
			}
			else
			{
				ret = Localization.GetEmptyHouse();
			}

			return ret;
		}

		//workers building history update
		public void UpdateWorkers(int resilient_index)
		{
			List<uint> current_workers_ids = new List<uint>();

			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_resilients[resilient_index].buildingID];
			CitizenManager instance = Singleton<CitizenManager>.instance;
			uint num = build.m_citizenUnits;
			int num2 = 0;
			while (num != 0u)
			{
				if ((ushort)(instance.m_units.m_buffer[(int)((UIntPtr)num)].m_flags & CitizenUnit.Flags.Work) != 0)
				{
					CitizenUnit work_unit = instance.m_units.m_buffer[(int)((UIntPtr)num)];
					if (work_unit.m_citizen0 != 0u)
					{
						current_workers_ids.Add(work_unit.m_citizen0);
					}
					if (work_unit.m_citizen1 != 0u)
					{
						current_workers_ids.Add(work_unit.m_citizen1);
					}
					if (work_unit.m_citizen2 != 0u)
					{
						current_workers_ids.Add(work_unit.m_citizen2);
					}
					if (work_unit.m_citizen3 != 0u)
					{
						current_workers_ids.Add(work_unit.m_citizen3);
					}
					if (work_unit.m_citizen4 != 0u)
					{
						current_workers_ids.Add(work_unit.m_citizen4);
					}
					current_workers_ids.Add(num);
				}

				num = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;

				if (++num2 > 524288)
				{
					CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
					break;
				}
			}

			//update history family list with any new names
			for(int i = 0; i < current_workers_ids.Count; i++)
			{
				if(!m_resilients[resilient_index].idsBuffer.Contains(current_workers_ids[i]))
				{
					m_resilients[resilient_index].idsBuffer.Add(current_workers_ids[i]);
				}
			}
		}

		public string GetWorkersHistoryList(int buildIndex)
		{
			ResilientInfo ri = m_resilients[buildIndex];

			string ret;
			if(ri.idsBuffer.Count > 0)
			{
				ret = Localization.GetWorkersHistory()+"\n";
				for(int i = 0; i < ri.idsBuffer.Count; i++)
				{
					ret += Singleton<CitizenManager>.instance.GetCitizenName(ri.idsBuffer[i]);
					if(i < ri.idsBuffer.Count-1)
						ret += Localization.GetFamiliesSeparator();
				}
			}
			else
			{
				ret = Localization.GetEmptyFacility();
			}

			return ret;
		}

		//commercial building history updates
		public void UpdateVisitsCount(int buildIndex)
		{
			ResilientInfo ri = m_resilients[buildIndex];

			CitizenManager instance = Singleton<CitizenManager>.instance;
			Citizen.BehaviourData behaviour = new Citizen.BehaviourData();
			int aliveCount = 0;
			int totalCount = 0;

			uint num = Singleton<BuildingManager>.instance.m_buildings.m_buffer[ri.buildingID].m_citizenUnits;
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


			if(totalCount < ri.currentVisits) //assume some visitors have left
			{
				ri.totalVisits += ri.currentVisits - totalCount;
			}

			ri.currentVisits = totalCount;

			m_resilients[buildIndex] = ri;
		}

		//industrial extractor export history
		public void UpdatePrimaryResourceExport(int buildIndex)
		{
			ResilientInfo ri = m_resilients[buildIndex];

			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[ri.buildingID];

			int cbn1 = (int) build.m_customBuffer1;

			if(cbn1 < ri.goodsBuffer1) //material has been picked up
			{
				ri.goodsBuffer2 += ri.goodsBuffer1 - cbn1;
			}
			else //material has been extracted
			{
				ri.goodsBuffer3 -= ri.goodsBuffer1 - cbn1;
			}

			ri.goodsBuffer1 = cbn1;

			m_resilients[buildIndex] = ri;
		}

		//industrial goods export history
		public void UpdateGoodsExport(int buildIndex)
		{
			ResilientInfo ri = m_resilients[buildIndex];

			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[ri.buildingID];

			int cbn1 = (int) build.m_customBuffer1;
			int cbn2 = (int) build.m_customBuffer2;

			if(cbn1 > ri.goodsBuffer1) //material has been brought
			{
				ri.goodsBuffer2 -= ri.goodsBuffer1 - cbn1;
			}
			ri.goodsBuffer1 = cbn1;

			if(cbn2 < ri.goodsBuffer3) //goods have been picked up
			{
				ri.goodsBuffer4 += ri.goodsBuffer3 - cbn2;
			}
			ri.goodsBuffer3 = cbn2;

			m_resilients[buildIndex] = ri;
		}

	}
}

