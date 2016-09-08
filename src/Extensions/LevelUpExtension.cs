using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;

namespace ResilientOwners {
    public class LevelUpExtension : LevelUpExtensionBase {
        
		public static ResilientBuildings s_info;

        public override void OnCreated(ILevelUp levelUp) {
            
        }

        public override void OnReleased() {
           	
        }

        public override ResidentialLevelUp OnCalculateResidentialLevelUp(ResidentialLevelUp levelUp, int averageEducation, int landValue, ushort buildingID, Service service, SubService subService, Level currentLevel) {
            levelUp.targetLevel = this.controlLevelUp(levelUp.targetLevel, currentLevel, buildingID);
            return levelUp;
        }

        public override OfficeLevelUp OnCalculateOfficeLevelUp(OfficeLevelUp levelUp, int averageEducation, int serviceScore, ushort buildingID, Service service, SubService subService, Level currentLevel) {
            levelUp.targetLevel = this.controlLevelUp(levelUp.targetLevel, currentLevel, buildingID);
            return levelUp;
        }
        
        public override CommercialLevelUp OnCalculateCommercialLevelUp(CommercialLevelUp levelUp, int averageWealth, int landValue, ushort buildingID, Service service, SubService subService, Level currentLevel) {
            levelUp.targetLevel = this.controlLevelUp(levelUp.targetLevel, currentLevel, buildingID);
            return levelUp;
        }

        public override IndustrialLevelUp OnCalculateIndustrialLevelUp(IndustrialLevelUp levelUp, int averageEducation, int serviceScore, ushort buildingID, Service service, SubService subService, Level currentLevel) {
			levelUp.targetLevel = this.controlLevelUp(levelUp.targetLevel, currentLevel, buildingID);
            return levelUp;
        }

        private Level controlLevelUp(Level targetLevel, Level currentLevel, ushort buildingID) {
			int bi = s_info.GetResilientBuildingIndex (buildingID);

			if (bi != -1) {//lock level
				targetLevel = (Level) s_info.m_resilients[bi].chosenLevel;
			}
            return targetLevel;
        }
    }
}