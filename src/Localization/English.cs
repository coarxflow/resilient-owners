using System;

namespace ResilientOwners
{
	public class English : Localization
	{
		public override string GetHistoryTitle()
		{
			return "History";
		}

		public override string GetRecordTitle()
		{
			return "Record";
		}

		//description textfield
		public override string GetDescriptionEmpty()
		{
			return "Enter Description";
		}
		
		//activation date label
		public override string GetActivationDate()
		{
			return "Since ";
		}

		public override string GetAge(int years, int days)
		{
			if(years == 0)
				return "Age: "+days+" days";
			else if(years == 1)
				return "Age: "+years+" year, "+days+" days";
			else
				return "Age: "+years+" years, "+days+" days";
		}

		//office wealth label
		public override string GetAccumulatedIncome()
		{
			return "Wealth generated: ";
		}

		//families history label
		public override string GetEmptyHouse()
		{
			return "No families have lived here";
		}

		public override string GetFamiliesHistory()
		{
			return "Families having lived here: ";
		}

		public override string GetFamiliesSeparator()
		{
			return ", ";
		}

		public override string GetWorkersHistory()
		{
			return "Worked here: ";
		}

		public override string GetEmptyFacility()
		{
			return "No Workers have worked here";
		}

		//industrial production stats
		public override string GetExtractedAmount()
		{
			return "Extracted: ";
		}

		public override string GetProducedAmount()
		{
			return "Exported: ";
		}

		//commercial clients stats
		public override string GetClientsAmount()
		{
			return "Clients served: ";
		}

		//tooltips
		public override string GetTooltipOff()
		{
			return Mod.modName+": disabled";
		}

		public override string GetTooltipHistoryOn()
		{
			return Mod.modName+": history is enabled";
		}

		public override string GetTooltipResiliencyOn()
		{
			return Mod.modName+": history and resiliency enabled";
		}
	}
}

