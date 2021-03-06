﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColossalFramework;
using ICities;

namespace ResilientOwners
{
	public class ResilientExpresser : ThreadingExtensionBase
	{
		public static ResilientBuildings s_info;
		public static ResilientUI s_UI;
		

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
			for(int i = 0; i < s_info.m_resilients.Count; i++)
			{
				if(s_info.m_resilients[i].resiliencyActivated)
				{

					//building will not be removed when dezoning, it must be bulldozed
					instance.m_buildings.m_buffer[s_info.m_resilients[i].buildingID].m_flags &= ~Building.Flags.ZonesUpdated;
				}
			}
		}

//		public override void OnAfterSimulationFrame()
//		{
//		}

		//public static int UPDATE_EACH_TICKS = 20;
		public static int REMOVE_AFTER_UPDATES = 50;
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
                int districtIndex = s_info.GetResilientDistrictIndex(districtID);
                if (districtIndex != -1)
                {
                    if (s_info.m_districts[districtIndex].unsuscribed)
                    {
                        //s_info.RemoveBuilding(i);
                        s_info.UnsuscribeBuilding(i);
                    }
                    else if (s_info.m_districts[districtIndex].resiliencyActivated)
                    {
                        bool newly_added = s_info.AddBuilding(i, true);
                        if (newly_added)
                        {
                            ResilientBuildings.ResilientDistrict rd = s_info.m_districts[districtIndex];
                            rd.totalBuildings++;
                            s_info.m_districts[districtIndex] = rd;
                        }
                    }
                }

            }

            //districts timers
            for(int i = 0; i < s_info.m_districts.Count; i++)
            {
                if(s_info.m_districts[i].unsuscribed)
                {
                    ResilientBuildings.ResilientDistrict rd = s_info.m_districts[i];
                    rd.unsuscribeTimer++;
                    s_info.m_districts[i] = rd;
                    if(rd.unsuscribeTimer > REMOVE_DISTRICT_AFTER_UPDATES)
                    {
                        s_info.RemoveDistrict(rd.districtID);
                    }
                }
            }


            //check all buildings in resilients list
			for (int i = 0; i < s_info.m_resilients.Count; i++)
			{
				ushort buildingID = s_info.m_resilients[i].buildingID;

				//sync with buildManager, update only buildings that just had a SimulationStep
				if(buildingID < minBuildingID || buildingID >= maxBuildingID)
					continue;

				ResilientBuildings.ResilientInfoV1 build = s_info.m_resilients[i];

				if(s_info.m_resilients[i].unsuscribed) //building unsuscribed
				{
					build.unsuscribeTimer++;
					if(build.unsuscribeTimer > REMOVE_AFTER_UPDATES)
					{
						s_info.m_resilients.RemoveAt(i);
						i--;
						continue;
					}
					s_info.m_resilients[i] = build;
					continue;
				}

				if(instance.m_buildings.m_buffer[buildingID].m_flags == Building.Flags.None) //building was bulldozed, remove it from the list
				{
					s_info.m_resilients.RemoveAt(i);
                    i--;
                    continue;
				}


				//update infos
				BuildingInfo buildinfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info;

				if(buildinfo.m_class.m_service == ItemClass.Service.Residential)
				{
					s_info.UpdateResidentFamilies(i);
				}
				else
				{
					s_info.UpdateWorkers(i);

					if(buildinfo.m_class.m_service == ItemClass.Service.Commercial)
						s_info.UpdateVisitsCount(i);
					else if(buildinfo.m_class.m_service == ItemClass.Service.Industrial)
					{
						if(buildinfo.m_buildingAI.GetType().Equals(typeof(IndustrialExtractorAI)))
							{
							s_info.UpdatePrimaryResourceExport(i);
							}
							else if(buildinfo.m_buildingAI.GetType().Equals(typeof(IndustrialBuildingAI)))
							{
							s_info.UpdateGoodsExport(i);
							}
					}
				}

				if(build.resiliencyActivated)
				{
					//instance.m_buildings.m_buffer[buildingID].m_electricityProblemTimer = 0;
					//instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 0;
					//instance.m_buildings.m_buffer[buildingID].m_problems = Notification.Problem.TurnedOff;

					//building will not be removed when dezoning, it must be bulldozed
					//instance.m_buildings.m_buffer[buildingID].m_flags &= ~Building.Flags.ZonesUpdated;

					//avoid major problem to trigger abandonment
					instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 0;
					instance.m_buildings.m_buffer[buildingID].m_problems &= ~Notification.Problem.MajorProblem;

					//lower cap on unhappiness if problems
					if(instance.m_buildings.m_buffer[buildingID].m_problems != Notification.Problem.None)
						instance.m_buildings.m_buffer[buildingID].m_happiness = 60;

					//reoccupy building when it is abandoned/burned down
					if((instance.m_buildings.m_buffer[buildingID].m_flags & (Building.Flags.Abandoned | Building.Flags.BurnedDown)) != Building.Flags.None)
					{
						//reset timers
						instance.m_buildings.m_buffer[buildingID].m_electricityProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_waterProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_deathProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_serviceProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_taxProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_outgoingProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_incomingProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_heatingProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_healthProblemTimer = 0;
						instance.m_buildings.m_buffer[buildingID].m_workerProblemTimer = 0;

						//reset buffers
						instance.m_buildings.m_buffer[buildingID].m_crimeBuffer = 0;
						instance.m_buildings.m_buffer[buildingID].m_customBuffer1 = 0;
						instance.m_buildings.m_buffer[buildingID].m_customBuffer2 = 0;
						instance.m_buildings.m_buffer[buildingID].m_electricityBuffer = 0;
						instance.m_buildings.m_buffer[buildingID].m_heatingBuffer = 0;
						instance.m_buildings.m_buffer[buildingID].m_garbageBuffer = 0;
						instance.m_buildings.m_buffer[buildingID].m_sewageBuffer = 0;
						instance.m_buildings.m_buffer[buildingID].m_waterBuffer = 0;

						//renovate building
						Building.Frame carpentry = instance.m_buildings.m_buffer[buildingID].GetLastFrameData();
						carpentry.m_constructState /= 2;
						carpentry.m_fireDamage = 0;
						instance.m_buildings.m_buffer[buildingID].SetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex, carpentry);

						//repopulate building
						instance.m_buildings.m_buffer[buildingID].m_flags = Building.Flags.Created;
						instance.m_buildings.m_buffer[buildingID].Info.m_buildingAI.CreateBuilding(buildingID, ref instance.m_buildings.m_buffer[buildingID]);

						//user feedback : remove notification
						instance.m_buildings.m_buffer[buildingID].m_problems = Notification.Problem.None;
					}

					if(instance.m_buildings.m_buffer[buildingID].GetLastFrameData().m_fireDamage > 230 && Settings.inst.extinguishFires) //extinguish fire at last minute
					{
						instance.m_buildings.m_buffer[buildingID].m_fireIntensity = 0;
//						Building.Frame carpentry = instance.m_buildings.m_buffer[buildingID].GetLastFrameData();
//						carpentry.m_fireDamage = 0;
//						instance.m_buildings.m_buffer[buildingID].SetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex+1, carpentry);

					}
				}

				s_UI.CheckUpdateUI(buildingID);
			}
		}

	}
}

