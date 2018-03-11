using System.Globalization;

namespace HistoricBuildings
{
	public abstract class Localization
	{
		public static Localization trad;

        public static CultureInfo culture;

		//tooltips
		public abstract string GetTooltipOff();

		public abstract string GetTooltipHistoryOn();

        public abstract string GetTooltipDistrictOff();

        public abstract string GetTooltipDistrictOn();

        //settings
        public abstract string GetSettingsPerCities();

        public abstract string GetExtinguishFiresSetting();

        public abstract string GetAbandonmentSetting();

    }
}

