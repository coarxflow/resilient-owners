using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColossalFramework;
using ICities;

namespace HistoricBuildings
{
	public class HistoricExpresser : ThreadingExtensionBase
	{
		public static HistoricBuildings s_info;
		public static HistoricUI s_UI;
		

		public override void OnCreated(IThreading threading)
		{
			base.OnCreated(threading);


		}

		public override void OnReleased()
		{
			var go = GameObject.Find("ResilientBuildings");
			if (go != null)
			{
				UnityEngine.Object.Destroy(go);
			}
		}

		/*public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
		{

		}*/

		/*public override void OnBeforeSimulationTick()
		{
		}*/

		public override void OnBeforeSimulationFrame()
		{
            if (!LoadingExtension.installed)
                return;

			BuildingManager instance = Singleton<BuildingManager>.instance;
			foreach(ushort buildingID in s_info.buildings.Keys)
			{
				//building will not be removed when dezoning, it must be bulldozed
		        instance.m_buildings.m_buffer[buildingID].m_flags &= ~Building.Flags.ZonesUpdated;
			}
		}

        public static int REMOVE_DISTRICT_AFTER_UPDATES = 255;

        public override void OnAfterSimulationFrame()
		{
            if (!LoadingExtension.installed)
                return;

            if (s_info == null)
				return;

			BuildingManager instance = Singleton<BuildingManager>.instance;

			int num6 = (int)(Singleton<SimulationManager>.instance.m_currentFrameIndex & 255u);
            ushort minBuildingID = (ushort) (num6 * 192);
            ushort maxBuildingID = (ushort) ((num6 + 1) * 192 - 1);

            //check if building should be added as part of a resilient district
            DistrictManager instance2 = Singleton<DistrictManager>.instance;
            for(ushort i = minBuildingID; i < maxBuildingID; i++)
            {
                if (instance.m_buildings.m_buffer[i].m_flags == Building.Flags.None)
                    continue;
                Building build = instance.m_buildings.m_buffer[i];
                byte districtID = instance2.GetDistrict(build.m_position);
                if (s_info.districts.ContainsKey(districtID))
                {
                    if (s_info.districts[districtID] > 0)
                    {
                        s_info.RemoveBuilding(i);
                    }
                    else if (s_info.districts[districtID] == 0 && !s_info.buildings.ContainsKey(i))
                    {
                        bool newly_added = s_info.AddBuilding(i);
                    }
                }
            }

            //districts timers
            foreach(byte districtID in s_info.districts.Keys)
            {
                if(s_info.districts[districtID] > 0)
                {
                    s_info.districts[districtID]++;
                    if (s_info.districts[districtID] > REMOVE_DISTRICT_AFTER_UPDATES)
                    {
                        s_info.RemoveDistrict(districtID);
                    }
                }
            }


            //check all buildings in resilients list
			foreach (ushort buildingID in s_info.buildings.Keys)
			{
				//sync with buildManager, update only buildings that just had a SimulationStep
				if(buildingID < minBuildingID || buildingID >= maxBuildingID)
					continue;

				if(instance.m_buildings.m_buffer[buildingID].m_flags == Building.Flags.None) //building was bulldozed, remove it from the list
				{
					s_info.RemoveBuilding(buildingID);
                    continue;
				}


				//update infos
				BuildingInfo buildinfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info;

				
                    //avoid major problem to trigger abandonment
                    if (Settings.inst.noAbandonment)
                    {
                        instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 0;
                        instance.m_buildings.m_buffer[buildingID].m_problems &= ~Notification.Problem.MajorProblem;
                    }


					if(instance.m_buildings.m_buffer[buildingID].GetLastFrameData().m_fireDamage > 230 && Settings.inst.extinguishFires) //extinguish fire at last minute
					{
						instance.m_buildings.m_buffer[buildingID].m_fireIntensity = 0;
//						Building.Frame carpentry = instance.m_buildings.m_buffer[buildingID].GetLastFrameData();
//						carpentry.m_fireDamage = 0;
//						instance.m_buildings.m_buffer[buildingID].SetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex+1, carpentry);

					}
			
               
			}
		}

	}
}

