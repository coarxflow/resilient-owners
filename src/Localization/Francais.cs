using System;

namespace HistoricBuildings
{
	public class Francais : Localization
	{
        public static string modName = "Bâtiments historiques";
        public static string modDescription = "Rends les bâtiments zonés historiques, les empêchant d'être abandonnés";

		//tooltips
		public override string GetTooltipOff()
		{
			return Mod.modName+": désactivé";
		}

		public override string GetTooltipHistoryOn()
		{
			return Mod.modName+": activé";
		}

        public override string GetTooltipDistrictOff()
        {
            return Mod.modName + ": cliquez pour rendre tous les bâtiments du district historiques";
        }

        public override string GetTooltipDistrictOn()
        {
            return Mod.modName + ": cliquez pour rendre tous les bâtiments du district non historiques";
        }

        //settings

        public override string GetExtinguishFiresSetting()
        {
            return "Stopper automatiquement les incendies pour les bâtiments historiques";
        }

        public override string GetAbandonmentSetting()
        {
            return "Empêcher les bâtiments historiques d'être abandonnés";
        }

        public override string GetSettingsPerCities()
        {
            return "Options sauvegardées séparément par ville.";
        }
    }
}

