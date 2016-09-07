using System;
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

		public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
		{

		}

		public override void OnBeforeSimulationTick()
		{
		}

		public override void OnBeforeSimulationFrame()
		{
		}

		public override void OnAfterSimulationFrame()
		{
		}

		public static int UPDATE_EACH_TICKS = 20;
		public static int REMOVE_AFTER_UPDATES = 10;

		public override void OnAfterSimulationTick()
		{
			if(s_info == null)
				return;

			int chunksize = Mathf.CeilToInt(1f*s_info.m_resilients.Count/UPDATE_EACH_TICKS);
			int chunk = (int)Singleton<SimulationManager>.instance.m_currentFrameIndex%UPDATE_EACH_TICKS;
			int start_i = chunk*chunksize;
			int end_i = Mathf.Min(start_i+chunksize, s_info.m_resilients.Count);

			for(int i = start_i; i < end_i; i++)
			{
				ushort buildingID = s_info.m_resilients[i].buildingID;

				ResilientBuildings.ResilientInfo build = s_info.m_resilients[i];

				if(s_info.m_resilients[i].unsuscribed) //building unsuscribed
				{
					build.unsuscribeTimer++;
					if(build.unsuscribeTimer > REMOVE_AFTER_UPDATES)
					{
						s_info.m_resilients.RemoveAt(i);
					}
					s_info.m_resilients[i] = build;
					continue;
				}

				if(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_flags == Building.Flags.None) //building was bulldozed, remove it from the list
				{
					s_info.m_resilients.RemoveAt(i);
					continue;
				}

				s_info.UpdateResidentFamilies(i);

				if(build.resiliencyActivated)
				{
					//Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_electricityProblemTimer = 0;
					//Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 0;
					//Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_problems = Notification.Problem.TurnedOff;
					
					Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_flags &= ~Building.Flags.ZonesUpdated;
					//Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.m_class.m_service = ItemClass.Service.;

					if((Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_flags & Building.Flags.Demolishing) != Building.Flags.None)
					{
						Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_flags = Building.Flags.Upgrading;
					}

					if((Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_flags & Building.Flags.Deleted) != Building.Flags.None)
					{
						Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_flags = Building.Flags.Upgrading;
					}

					if((Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_flags & (Building.Flags.Abandoned | Building.Flags.BurnedDown)) != Building.Flags.None)
					{
						Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_electricityProblemTimer = 0;
						Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_waterProblemTimer = 0;
						Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_majorProblemTimer = 0;
						Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_flags = Building.Flags.Created;
						Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.m_buildingAI.CreateBuilding(buildingID, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID]);
					}

					if(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].GetLastFrameData().m_fireDamage > 250) //extinguish fire at last minute
					{
						Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].m_fireIntensity = 0;
					}
				}

			}
		}




	}
}

