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
		/******* data structure **********/

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

		[System.Serializable]
		public struct ResilientInfoV1
		{
			public ushort buildingID;

			public DateTime activatedDate;

			public bool resiliencyActivated;
			public ItemClass.Level chosenLevel;

			public string description;

			public List<string> namesBuffer; //track families or workers names

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

		public List<ResilientInfoV1> m_resilients;

        /*********** Manage List *************/

        public void InitializeList()
		{
			if (m_resilients == null) {
				m_resilients = new List<ResilientInfoV1> ();
				//CODebug.Log (LogChannel.Modding, "initialize resilient building list");
			} else {
				//CODebug.Log (LogChannel.Modding, "a resilient building list has benn loaded");
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
			
			ResilientInfoV1 ri;

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

			ri = new ResilientInfoV1();
			ri.buildingID = buildingID;
			ri.activatedDate = Singleton<SimulationManager>.instance.m_currentGameTime;
			ri.description = "";
			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
			ri.chosenLevel = build.Info.m_class.m_level;

			ri.namesBuffer = new List<string>();

			ri.goodsBuffer1 = 0;
			ri.goodsBuffer2 = 0;
			ri.goodsBuffer3 = 0;
            //ri.goodsBuffer3 = build.m_customBuffer2;
            ri.goodsBuffer4 = 0;

            ri.totalIncome = 0L;
            


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
				ResilientInfoV1 ri = m_resilients[index];
				ri.unsuscribed = true;
				m_resilients[index] = ri;
			}
		}

		public void RemoveBuilding(ushort buildingID)
		{
			int index = GetResilientBuildingIndex(buildingID);
				
			m_resilients.RemoveAt (index);
		}

		/*********** building updates *************/

		//residential building history updates
		public void UpdateResidentFamilies(int resilient_index)
		{
            if (!Settings.inst.listResidentsAndWorkers)
                return;

			List<uint> current_families_id = new List<uint>();

			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_resilients[resilient_index].buildingID];
			CitizenManager instance = Singleton<CitizenManager>.instance;
			uint num = build.m_citizenUnits;
			int num2 = 0;
			int aliveHomeCount = 0;
			while (num != 0u)
			{
				uint nextUnit = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
				if ((ushort)(instance.m_units.m_buffer[(int)((UIntPtr)num)].m_flags & CitizenUnit.Flags.Home) != 0)
				{
					current_families_id.Add(num);
					aliveHomeCount++;
				}
				num = nextUnit;

				if (++num2 > 524288)
				{
					CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
					break;
				}
			}

			//update history family list with any new names
			int index_families_before_now = Math.Max(m_resilients[resilient_index].namesBuffer.Count - aliveHomeCount, 0);
			for(int i = 0; i < current_families_id.Count; i++)
			{
//				if(!m_resilients[resilient_index].idsBuffer.Contains(current_families_id[i]))
//				{
//					m_resilients[resilient_index].idsBuffer.Add(current_families_id[i]);
//				}

				uint valid_citizen = 0u;
				CitizenUnit cu = Singleton<CitizenManager>.instance.m_units.m_buffer[current_families_id[i]];
				if(cu.m_citizen0 != 0u)
					valid_citizen = cu.m_citizen0;
				else if(cu.m_citizen1 != 0u)
					valid_citizen = cu.m_citizen1;
				else if(cu.m_citizen2 != 0u)
					valid_citizen = cu.m_citizen2;
				else if(cu.m_citizen3 != 0u)
					valid_citizen = cu.m_citizen3;
				else if(cu.m_citizen4 != 0u)
					valid_citizen = cu.m_citizen4;

				if(valid_citizen != 0u)
				{
					int family = Singleton<CitizenManager>.instance.m_citizens.m_buffer[valid_citizen].m_family;
					Randomizer randomizer2 = new Randomizer(family);
					string name = "NAME_FEMALE_LAST";
		//					if (Citizen.GetGender(citizenID) == Citizen.Gender.Male)
		//					{
		//						text = "NAME_MALE_FIRST";
		//						text2 = "NAME_MALE_LAST";
		//					}
					name = Locale.Get(name, randomizer2.Int32(Locale.Count(name)));
					name = name.Substring(4);//remove placeholder in front

					if(m_resilients[resilient_index].namesBuffer.Count == 0 || m_resilients[resilient_index].namesBuffer.LastIndexOf(name) < index_families_before_now) //-1 if family was never present, otherwise check if it is not currently in
					{
						m_resilients[resilient_index].namesBuffer.Add(name);
					}
				}
			}
		}

		public string GetFamiliesList(int buildIndex)
		{
			ResilientInfoV1 ri = m_resilients[buildIndex];

			string ret;
			if(ri.namesBuffer.Count > 0)
			{
				ret = Localization.trad.GetFamiliesHistory()+"\n";
				for(int i = 0; i < ri.namesBuffer.Count; i++)
				{
					ret += ri.namesBuffer[i];//remove placeholder in front
					if(i < ri.namesBuffer.Count-1)
						ret += Localization.trad.GetFamiliesSeparator();
				}
			}
			else
			{
				ret = Localization.trad.GetEmptyHouse();
			}

			return ret;
		}

		//workers building history update
		public void UpdateWorkers(int resilient_index)
		{
            if (!Settings.inst.listResidentsAndWorkers)
                return;

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
//				if(!m_resilients[resilient_index].idsBuffer.Contains(current_workers_ids[i]))
//				{
//					m_resilients[resilient_index].idsBuffer.Add(current_workers_ids[i]);
//				}
				string name = Singleton<CitizenManager>.instance.GetCitizenName(current_workers_ids[i]);
				if(name == null || name.Length == 0)
				{
					//CODebugBase<LogChannel>.Error(LogChannel.Modding, "empty citizen name for " + current_workers_ids[i]);
				}
				else 
				if(!m_resilients[resilient_index].namesBuffer.Contains(name))
				{
					m_resilients[resilient_index].namesBuffer.Add(name);
				}
			}
		}

		public string GetWorkersHistoryList(int buildIndex)
		{
			ResilientInfoV1 ri = m_resilients[buildIndex];

			string ret;
			if(ri.namesBuffer.Count > 0)
			{
				ret = Localization.trad.GetWorkersHistory(ri.namesBuffer.Count);//+"\n";
//				for(int i = 0; i < ri.namesBuffer.Count; i++)
//				{
//					ret += ri.namesBuffer[i];
//					if(i < ri.namesBuffer.Count-1)
//						ret += Localization.trad.GetFamiliesSeparator();
//				}
			}
			else
			{
				ret = Localization.trad.GetEmptyFacility();
			}

			return ret;
		}

		//commercial building history updates
		public void UpdateVisitsCount(int buildIndex)
		{
			ResilientInfoV1 ri = m_resilients[buildIndex];

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
			ResilientInfoV1 ri = m_resilients[buildIndex];

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
			ResilientInfoV1 ri = m_resilients[buildIndex];

			Building build = Singleton<BuildingManager>.instance.m_buildings.m_buffer[ri.buildingID];

			int cbn1 = (int) build.m_customBuffer1; //resources in stock
			int cbn2 = (int) build.m_customBuffer2; //goods in stock

			if(cbn1 > ri.goodsBuffer1) //resources have been brought
			{
				ri.goodsBuffer2 -= ri.goodsBuffer1 - cbn1; //total resources imported
			}
			ri.goodsBuffer1 = cbn1;

            if (cbn2 < ri.goodsBuffer3) //goods have been picked up
            {
                ri.goodsBuffer4 += ri.goodsBuffer3 - cbn2;
            }

            //if (cbn2 > ri.goodsBuffer3) //goods have not been picked up (track production)
            //{
            //    ri.goodsBuffer4 += cbn2 - ri.goodsBuffer3;
            //}

            ri.goodsBuffer3 = cbn2;

			m_resilients[buildIndex] = ri;
		}

        /********** clear lists (in settings) *********/

        public void clearResidentsList()
        {
            for(int i = 0; i < m_resilients.Count; i++)
            {
                m_resilients[i].namesBuffer.Clear();
            }
        }

	}
}

