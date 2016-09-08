using ICities;
using UnityEngine;

namespace ResilientOwners
{
	public class LoadingExtension : LoadingExtensionBase
	{
		GameObject m_infoObject;
		ResilientBuildings m_info;
		ResilientUI m_UI;
		
		public override void OnCreated(ILoading loading)
		{
			base.OnCreated(loading);

		}
		
		
		public override void OnLevelLoaded(LoadMode mode)
		{
			if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            //create data object
			m_infoObject = new GameObject("ResilientBuildings");
			m_info = m_infoObject.AddComponent<ResilientBuildings>();
			if(BookKeeper.s_data != null)
				m_info.m_resilients = BookKeeper.s_data;
			else
				m_info.InitializeList();

			//add mod's components to the UI
			m_UI = ResilientUI.Install(m_infoObject, m_info);

			//bind objects
			BookKeeper.s_info = m_info;
			LevelUpExtension.s_info = m_info;
			ResilientExpresser.s_info = m_info;
			IncomeTracker.s_info = m_info;

			CODebug.Log (LogChannel.Modding, "Resilient Owners mod launched");
		}
		
		public override void OnLevelUnloading()
		{
			if(m_UI != null)
				ResilientUI.Uninstall(m_UI);
			
			if(m_infoObject != null)
				GameObject.Destroy(m_infoObject);
		}
		
		public override void OnReleased()
		{
			
		}


	}
}
