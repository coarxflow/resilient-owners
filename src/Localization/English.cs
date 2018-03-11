using System;

namespace HistoricBuildings
{
	public class English : Localization
	{
        public static string modName = "Historic Buildings";
        public static string modDescription = "Makes zoned buildings historic, preventing abandonment.";

		//tooltips
		public override string GetTooltipOff()
		{
			return Mod.modName+": disabled";
		}

		public override string GetTooltipHistoryOn()
		{
			return Mod.modName+": enabled";
		}

        public override string GetTooltipDistrictOff()
        {
            return Mod.modName + ": click to make all buildings in district historic";
        }

        public override string GetTooltipDistrictOn()
        {
            return Mod.modName + ": click to disable historic buildings in all the district";
        }

        //settings

        public override string GetExtinguishFiresSetting()
        {
            return "Auto extinguish fires for historic buildings";
        }

        public override string GetAbandonmentSetting()
        {
            return "Prevent historic buildings getting abandoned";
        }

        public override string GetSettingsPerCities()
        {
            return "Settings are saved per city.";
        }
    }
}

