using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;

namespace HistoricBuildings {
    public class LevelUpExtension : LevelUpExtensionBase {
        
		public static HistoricBuildings s_info;

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
			if (s_info.buildings.ContainsKey(buildingID)) {//lock level
				targetLevel = (Level) s_info.buildings[buildingID]; //use dictionnary instead!!
			}
            return targetLevel;
        }
    }
}