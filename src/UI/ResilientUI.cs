//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;

namespace ResilientOwners
{
	public class ResilientUI : MonoBehaviour
	{

		ResilientBuildings m_info;

		ZonedBuildingWorldInfoPanel m_zonedBuildingInfoPanel;
		float m_zonedBuildingInfoPanelInitialHeight = 0f;

		UIMultilineTextField m_descriptionTextField;
		StateButton m_resilientStateButton;
		UILabel m_familiesHistoryLabel;
		UILabel m_activatedDateLabel;
		UILabel m_statsLabel;

		UICurrencyWrapper income =  new UICurrencyWrapper(0L);

		ushort m_currentSelectedBuildingID;

		public static ResilientUI Install(GameObject go, ResilientBuildings info)
		{
			ResilientUI rui =go.AddComponent<ResilientUI>();
			//ResilientUI rui = new ResilientUI();
			rui.m_info = info;

			rui.m_zonedBuildingInfoPanel = GameObject.Find("(Library) ZonedBuildingWorldInfoPanel").GetComponent<ZonedBuildingWorldInfoPanel>();

			rui.AddComponents();

			return rui;
		}

		public static void Uninstall(ResilientUI rui)
		{

		}

		/********** UI building ***********/

		void AddComponents()
		{

			m_descriptionTextField = m_zonedBuildingInfoPanel.component.AddUIComponent<UIMultilineTextField> ();
			m_descriptionTextField.name = "Building Description";
			m_descriptionTextField.text = "Enter description";
			m_descriptionTextField.textScale = 0.8f;
			m_descriptionTextField.width = m_zonedBuildingInfoPanel.component.width/2;
			//m_descriptionTextField.height = 70;
//			textfield.normalBgSprite = "ButtonMenu";
//			textfield.disabledBgSprite = "ButtonMenuDisabled";
//			textfield.hoveredBgSprite = "ButtonMenuHovered";
//			textfield.focusedBgSprite = "ButtonMenu";
			m_descriptionTextField.disabledTextColor = new Color32(7, 7, 7, 255);
			m_descriptionTextField.eventTextSubmitted += (component, param) =>
			{
				SaveDescription(param);
			};
			m_descriptionTextField.AlignTo(m_zonedBuildingInfoPanel.component, UIAlignAnchor.BottomLeft);
			m_descriptionTextField.relativePosition += new Vector3 (10f, 30f, 0f);
			m_descriptionTextField.FixPositionAndActivateAutoHeight();
			//m_descriptionTextField.relativePosition += new Vector3 (10f, 40f, 0f);

			m_descriptionTextField.title = "Description";
			m_descriptionTextField.showTitle = true;
			m_descriptionTextField.defaultText = Localization.GetDescriptionEmpty();

			m_descriptionTextField.eventHeightChange += (component, height) => {
				ResizePanelHeight(height);
			};

			int spriteWidth = 32;
			int spriteHeight = 32;
			string[] spriteNames = {
				"ResilientDisabled", 
				"ResilientEnabled",
				"Resilient+"
			};
			m_resilientStateButton = new StateButton(m_zonedBuildingInfoPanel.component, spriteWidth, spriteHeight, spriteNames, "icons.book.png");

			m_resilientStateButton.msb.eventActiveStateIndexChanged += (component, value) => {

				if(!m_allowEvents)
					return;

				CODebug.Log(LogChannel.Modding, "multistate button in state "+value);

				switch(value)
				{
				case 0:
					m_info.UnsuscribeBuilding(m_currentSelectedBuildingID);
					HideHistory();
					break;
				case 1:
					m_info.AddBuilding(m_currentSelectedBuildingID, false);
					ShowHistory();
					break;
				case 2:
					m_info.AddBuilding(m_currentSelectedBuildingID, true);
					ShowHistory();
					break;
				}
			};
			m_resilientStateButton.msb.AlignTo(m_zonedBuildingInfoPanel.component, UIAlignAnchor.TopRight);
			m_resilientStateButton.msb.relativePosition += new Vector3 (-50f, 50f, 0f);

			m_familiesHistoryLabel = m_zonedBuildingInfoPanel.component.AddUIComponent<UILabel> ();
			m_familiesHistoryLabel.name = "Families History";
			m_familiesHistoryLabel.text = Localization.GetEmptyHouse();
			m_familiesHistoryLabel.textScale = 0.8f;
			m_familiesHistoryLabel.width = m_zonedBuildingInfoPanel.component.width/2;
			//m_familiesHistoryLabel.wordWrap = true;
			//m_familiesHistoryLabel.autoSize = true;

			m_activatedDateLabel = m_zonedBuildingInfoPanel.component.AddUIComponent<UILabel> ();
			m_activatedDateLabel.name = "Activation Date";
			m_activatedDateLabel.text = Localization.GetActivationDate();
			m_activatedDateLabel.textScale = 0.8f;
			m_activatedDateLabel.width = m_zonedBuildingInfoPanel.component.width/2;

			m_statsLabel = m_zonedBuildingInfoPanel.component.AddUIComponent<UILabel> ();
			m_statsLabel.name = "Stats";
			m_statsLabel.text = Localization.GetAccumulatedIncome();
			m_statsLabel.textScale = 0.8f;
			m_statsLabel.width = m_zonedBuildingInfoPanel.component.width/2;

//			m_zonedBuildingInfoPanel.component.eventVisibilityChanged +=(component, param) =>
//			{
//				if(param)
//					OnSelected();
//					//m_info.StartCoroutine(OnSelected());//StartCoroutine on a MonoBehaviour...
//			};

			m_zonedBuildingInfoPanel.component.eventPositionChanged += (inst1, inst2) =>
			{
				if(m_zonedBuildingInfoPanel.component.isVisible)
					OnSelected();
					//m_info.StartCoroutine(OnSelected());//StartCoroutine on a MonoBehaviour...
			};

			m_zonedBuildingInfoPanelInitialHeight = m_zonedBuildingInfoPanel.component.height;

		}

		/********** Event Handlers ***************/

		bool m_allowEvents = true;
		void SaveDescription(string desc)
		{
			if (!m_allowEvents)
				return;
			int buildIndex = m_info.GetResilientBuildingIndex (m_currentSelectedBuildingID);
			if (buildIndex != -1) {
				ResilientBuildings.ResilientInfo ri = m_info.m_resilients [buildIndex];
				ri.description = desc;
				m_info.m_resilients [buildIndex] = ri;
			}
		}

		/********** UI update methods ***************/

		void/*IEnumerator*/ OnSelected()
		{
			//yield return new WaitForEndOfFrame();
			//get selected building ID (after waiting it has been actualized)
			FieldInfo baseSub = m_zonedBuildingInfoPanel.GetType().GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);
			InstanceID instanceId = (InstanceID)baseSub.GetValue(m_zonedBuildingInfoPanel);
			if (instanceId.Type == InstanceType.Building && instanceId.Building != 0) {
				if(m_currentSelectedBuildingID == instanceId.Building) //no update needed
					return;//yield break;
				m_currentSelectedBuildingID = instanceId.Building;
			} else
				m_currentSelectedBuildingID = 0;

			int buildIndex = m_info.GetResilientBuildingIndex (m_currentSelectedBuildingID);
			m_allowEvents = false;
			if (buildIndex != -1) {
				if(m_info.m_resilients[buildIndex].resiliencyActivated)
					m_resilientStateButton.SetState(2);
				else
					m_resilientStateButton.SetState(1);
				ShowHistory();
			} else {
				m_resilientStateButton.SetState(0);
				HideHistory();
			}

			m_allowEvents = true;

			m_zonedBuildingInfoPanel.component.Invalidate();
		}

		public void HideHistory()
		{
			m_descriptionTextField.isVisible = false;
			m_familiesHistoryLabel.isVisible = false;
			m_activatedDateLabel.isVisible = false;
			m_statsLabel.isVisible = false;
			ResizePanelHeight(0);
		}

		public void ShowHistory()
		{
			int buildIndex = m_info.GetResilientBuildingIndex (m_currentSelectedBuildingID);

			m_descriptionTextField.text = m_info.m_resilients [buildIndex].description;
			m_descriptionTextField.isVisible = true;

			m_familiesHistoryLabel.text = m_info.GetFamiliesList(buildIndex);
			m_familiesHistoryLabel.isVisible = true;

			m_activatedDateLabel.text = Localization.GetActivationDate() + '\n' + m_info.m_resilients [buildIndex].activatedDate.Date.ToString("dd/MM/yyyy");
			m_activatedDateLabel.isVisible = true;

			m_statsLabel.isVisible = true;

			ResizePanelHeight(0f);
		}

		float last_desc_height;
		public void ResizePanelHeight(float desc_height)
		{
			if(desc_height == 0f)
				desc_height = last_desc_height;

			float hist_height = m_familiesHistoryLabel.height + m_activatedDateLabel.height;

			float add_height = Mathf.Max(desc_height, hist_height);

			m_familiesHistoryLabel.AlignTo(m_zonedBuildingInfoPanel.component, UIAlignAnchor.BottomRight);
			m_activatedDateLabel.AlignTo(m_zonedBuildingInfoPanel.component, UIAlignAnchor.BottomRight);
			m_statsLabel.AlignTo(m_zonedBuildingInfoPanel.component, UIAlignAnchor.BottomRight);
			m_activatedDateLabel.relativePosition += new Vector3 (0, -add_height+2*m_activatedDateLabel.height, 0f);
			m_statsLabel.relativePosition += new Vector3 (0, -add_height+2*m_activatedDateLabel.height+2*m_statsLabel.height, 0f);
			m_familiesHistoryLabel.relativePosition += new Vector3 (0, -add_height+2*m_activatedDateLabel.height+2*m_statsLabel.height+2*m_familiesHistoryLabel.height, 0f);

			m_descriptionTextField.AlignTo(m_zonedBuildingInfoPanel.component, UIAlignAnchor.BottomLeft);
			m_familiesHistoryLabel.relativePosition += new Vector3 (0, -add_height+desc_height, 0f);


			m_zonedBuildingInfoPanel.component.height = m_zonedBuildingInfoPanelInitialHeight + add_height;

			last_desc_height = desc_height;
		}

		private void Update()
		{
			int buildIndex = m_info.GetResilientBuildingIndex (m_currentSelectedBuildingID);
			if(buildIndex != -1)
			{
				income.Check(m_info.m_resilients [buildIndex].total_income);
				m_statsLabel.text = Localization.GetAccumulatedIncome() + income.result+" "+m_info.GetVisitsCount(m_info.m_resilients [buildIndex].buildingID));
			}
		}

	}
}

