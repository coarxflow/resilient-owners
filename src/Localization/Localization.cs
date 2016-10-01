using System;

namespace ResilientOwners
{
	public abstract class Localization
	{
		public static Localization trad;
		
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

		public abstract string GetWorkersHistory();

		public abstract string GetEmptyFacility();

		//industrial production stats
		public abstract string GetExtractedAmount();

		public abstract string GetProducedAmount();

		//commercial clients stats
		public abstract string GetClientsAmount();

		//tooltips
		public abstract string GetTooltipOff();

		public abstract string GetTooltipHistoryOn();

		public abstract string GetTooltipResiliencyOn();

	}
}

