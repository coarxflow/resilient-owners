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

			//check if OnAddResource call match what we are looking for
			if(!resource.Equals(EconomyResource.PrivateIncome)) //interedted in private incomes only
			{
				return amount;
			}

			if(service != Service.Office) //only office buildings tracked here
			{
				return amount;
			}

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
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Residential)
//					{
//						skipping = false;
//						if(service != Service.Residential)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Industrial)
//					{
//						skipping = false;
//						if(service != Service.Industrial)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Commercial)
//					{
//						skipping = false;
//						if(service != Service.Commercial)
//							error = true;
//					}
					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Office)
					{
						skipping = false;
					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Beautification)
//					{
//						skipping = false;
//						if(service != Service.Beautification)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Citizen)
//					{
//						skipping = false;
//						if(service != Service.Citizen)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Education)
//					{
//						skipping = false;
//						if(service != Service.Education)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Electricity)
//					{
//						skipping = false;
//						if(service != Service.Electricity)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.FireDepartment)
//					{
//						skipping = false;
//						if(service != Service.FireDepartment)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Garbage)
//					{
//						skipping = false;
//						if(service != Service.Garbage)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Government)
//					{
//						skipping = false;
////						if(service != Service.Governement)
////							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.HealthCare)
//					{
//						skipping = false;
//						if(service != Service.HealthCare)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Monument)
//					{
//						skipping = false;
//						if(service != Service.Monument)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.PoliceDepartment)
//					{
//						skipping = false;
//						if(service != Service.PoliceDepartment)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.PublicTransport)
//					{
//						skipping = false;
//						if(service != Service.PublicTransport)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Road)
//					{
//						skipping = false;
//						if(service != Service.Road)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Water)
//					{
//						skipping = false;
//						if(service != Service.Water)
//							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Unused1)
//					{
//						skipping = false;
////						if(service != Service.)
////							error = true;
//					}
//					else if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Unused2)
//					{
//						skipping = false;
////						if(service != Service.Education)
////							error = true;
//					}
//					else
//					{
////						CODebug.Log(LogChannel.Modding, "income tracker private income for service "+instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service);
////						skipping = true;
//						skipping = false;
//						if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service.ToString() != service.ToString())
//						{
//							CODebug.Log(LogChannel.Modding, "income tracker mismatch "+instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service.ToString()+" "+service.ToString());
//						}
//					}

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
					if(instance2.m_buildings.m_buffer[buildingID].Info.m_class.m_service == ItemClass.Service.Office)
					{
						int buildIndex = s_info.GetResilientBuildingIndex((ushort) buildingID);
						if(buildIndex != -1)
						{
							ResilientBuildings.ResilientInfoV1 ri = s_info.m_resilients[buildIndex];
							ri.totalIncome += amount;
							s_info.m_resilients[buildIndex] = ri;
						}
					}
				}
				else
				{
					CODebug.Log(LogChannel.Modding, Mod.modName+" - IncomeTracker could not meet building with service "+service+" subservice "+subService+" level "+level);
				}

			return amount;
		}

	}
}

