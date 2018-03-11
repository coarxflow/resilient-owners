using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace HistoricBuildings
{
	public class LoadingExtension : LoadingExtensionBase
	{
		GameObject m_infoObject;
		HistoricBuildings m_info;
		HistoricUI m_UI;
        public static bool installed = false;

        public override void OnCreated(ILoading loading)
		{
			base.OnCreated(loading);


        }
		
		
		public override void OnLevelLoaded(LoadMode mode)
		{
			if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            //Mod.updateSettingsPanel(); //update settings after loading city

            //create data object
            m_infoObject = new GameObject(Mod.modName);
			m_info = m_infoObject.AddComponent<HistoricBuildings>();
			if(BookKeeper.s_buildings != null)
				m_info.buildings = BookKeeper.s_buildings;
            if (BookKeeper.s_districts != null)
                m_info.districts = BookKeeper.s_districts;

            //localization
            if (Localization.trad == null)
                Mod.ChooseLocalization();

            m_info.InitializeList();

			UIPanel extensionTarget = null;
			//does extended building info mods exist?
			GameObject extendedBuildingsInfo = GameObject.Find("buildingWindowObject");
			if(extendedBuildingsInfo != null)
				extensionTarget = (UIPanel) extendedBuildingsInfo.GetComponent("BuildingInfoWindow5");

			//add mod's components to the UI
			m_UI = HistoricUI.Install(m_infoObject, m_info, extensionTarget);


			//bind objects
			BookKeeper.s_info = m_info;
			LevelUpExtension.s_info = m_info;
			HistoricExpresser.s_info = m_info;
			HistoricExpresser.s_UI = m_UI;
            Mod.s_info = m_info;

            //CODebug.Log (LogChannel.Modding, "Resilient Owners mod launched");
            installed = true;

        }
		
		public override void OnLevelUnloading()
		{
			if(m_UI != null)
				HistoricUI.Uninstall(m_UI);
			
			if(m_infoObject != null)
				GameObject.Destroy(m_infoObject);

            installed = false;

        }
		
		public override void OnReleased()
		{
			
		}



        }
}
