using System;
using ICities;
using ColossalFramework;

namespace ResilientOwners
{
	public class IncomeTracker : EconomyExtensionBase
	{
		
		uint checkedFrameIndex;
		ushort buildingID;
		ushort maxBuildingID = 0;

		public static ResilientBuildings s_info;

		public override int OnAddResource (EconomyResource resource, int amount, Service service, SubService subService, Level level)
		{
			if(resource.Equals(EconomyResource.PrivateIncome))
			{
				SimulationManager instance = Singleton<SimulationManager>.instance;
				BuildingManager instance2 = Singleton<BuildingManager>.instance;

				//sync with simulation frame
				if(checkedFrameIndex != instance.m_currentFrameIndex)
				{
					checkedFrameIndex = instance.m_currentFrameIndex;
					int num6 = (int)(instance.m_currentFrameIndex & 255u);
					buildingID = (ushort) (num6 * 192);
					maxBuildingID = (ushort) ((num6 + 1) * 192 - 1);
				}

				//sync with BuildingManager SimulationStep loop
				bool skipping = true;
				bool error = false;
				do {
					if((instance2.m_buildings.m_buffer[buildingID].m_flags & Building.Flags.Created) == Building.Flags.None)
					{
						skipping = true;
					}
					//abandoned buildings do no go through SimulationStepActive
					else if ((instance2.m_buildings.m_buffer[buildingID].m_flags & (Building.Flags.Abandoned | Building.Flags.BurnedDown)) != Building.Flags.None)
					{
						skipping = true;
					}
					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Residential)
					{
						skipping = false;
						if(service != Service.Residential)
							error = true;
					}
					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Industrial)
					{
						skipping = false;
						if(service != Service.Industrial)
							error = true;
					}
					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Commercial)
					{
						skipping = false;
						if(service != Service.Commercial)
							error = true;
					}
					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Office)
					{
						skipping = false;
						if(service != Service.Office)
							error = true;
					}
					else
					{
						skipping = true;
					}

					if(skipping)
					{
						buildingID++;
						if(buildingID > maxBuildingID)
						{
							error = true;
							skipping = false;
						}
					}
				}
				while(skipping);


				//check if active and history is activated
				if(!error)
				{
					int buildIndex = s_info.GetResilientBuildingIndex((ushort) buildingID);
					if(buildIndex != -1)
					{
						ResilientBuildings.ResilientInfo ri = s_info.m_resilients[buildIndex];
						ri.totalIncome += amount;
						s_info.m_resilients[buildIndex] = ri;
					}
				}
				else
				{
					CODebug.Log(LogChannel.Modding, Mod.modName+" - IncomeTracker could not meet building with service "+service+" subservice "+subService+" level "+level);
				}
			}

			return amount;
		}

	}
}

