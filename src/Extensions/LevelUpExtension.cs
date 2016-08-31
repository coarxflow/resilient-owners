/*
    Copyright (c) 2015, Max Stark <max.stark88@web.de> 
        All rights reserved.
    
    This file is part of ControlBuildingLevelUpMod, which is free 
    software: you can redistribute it and/or modify it under the terms 
    of the GNU General Public License as published by the Free 
    Software Foundation, either version 2 of the License, or (at your 
    option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
    General Public License for more details. 
    
    You should have received a copy of the GNU General Public License 
    along with this program; if not, see <http://www.gnu.org/licenses/>.
*/

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