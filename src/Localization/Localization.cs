using System.Globalization;

namespace ResilientOwners
{
	public abstract class Localization
	{
		public static Localization trad;

        public static CultureInfo culture;

        public abstract string GetHistoryTitle();

		public abstract string GetRecordTitle();

		//description textfield
		public abstract string GetDescriptionEmpty();
		
		//activation date label
		public abstract string GetActivationDate();

		public abstract string GetAge(int years, int days);

		//office wealth label
		public abstract string GetAccumulatedIncome();

		//families history label
		public abstract string GetEmptyHouse();

		public abstract string GetFamiliesHistory();

		public abstract string GetFamiliesSeparator();

        public abstract string GetWorkersHistory(int amount);

		public abstract string GetEmptyFacility();

		//industrial production stats
		public abstract string GetExtractedAmount();

		public abstract string GetProducedAmount();

        public abstract string GetIndustrialGenericGoodsDenomination(float amount, bool litteral);

        public abstract string GetIndustrialFarmingGoodsDenomination(float amount, bool litteral);

        public abstract string GetIndustrialForestryExtractorDenomination(float amount, bool litteral);

        public abstract string GetIndustrialForestryGoodsDenomination(float amount, bool litteral);

        public abstract string GetIndustrialOilExtractorDenomination(float amount, bool litteral);

        public abstract string GetIndustrialOilGoodsDenomination(float amount, bool litteral);

        public abstract string GetIndustrialOreExtractorDenomination(float amount, bool litteral);

        public abstract string GetIndustrialOreGoodsDenomination(float amount, bool litteral);

		//commercial clients stats
		public abstract string GetClientsAmount();

		//tooltips
		public abstract string GetTooltipOff();

		public abstract string GetTooltipHistoryOn();

		public abstract string GetTooltipResiliencyOn();

        //settings
        public abstract string GetResidentsListingSetting();

        public abstract string GetExtinguishFiresSetting();


    }
}

