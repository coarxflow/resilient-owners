using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace ResilientOwners
{
	public class LoadingExtension : LoadingExtensionBase
	{
		GameObject m_infoObject;
		ResilientBuildings m_info;
		ResilientUI m_UI;
        public static bool installed = false;

        public override void OnCreated(ILoading loading)
		{
			base.OnCreated(loading);


        }
		
		
		public override void OnLevelLoaded(LoadMode mode)
		{
			if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            Mod.updateSettingsPanel(); //update settings after loading city

            //create data object
            m_infoObject = new GameObject("ResilientBuildings");
			m_info = m_infoObject.AddComponent<ResilientBuildings>();
			if(BookKeeper.s_data != null)
				m_info.m_resilients = BookKeeper.s_data;
            if (BookKeeper.s_districts != null)
                m_info.m_districts = BookKeeper.s_districts;

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
			m_UI = ResilientUI.Install(m_infoObject, m_info, extensionTarget);


			//bind objects
			BookKeeper.s_info = m_info;
			LevelUpExtension.s_info = m_info;
			ResilientExpresser.s_info = m_info;
			ResilientExpresser.s_UI = m_UI;
			IncomeTracker.s_info = m_info;
            Mod.s_info = m_info;

            //CODebug.Log (LogChannel.Modding, "Resilient Owners mod launched");
            installed = true;

        }
		
		public override void OnLevelUnloading()
		{
			if(m_UI != null)
				ResilientUI.Uninstall(m_UI);
			
			if(m_infoObject != null)
				GameObject.Destroy(m_infoObject);

            installed = false;

        }
		
		public override void OnReleased()
		{
			
		}



        }
}
