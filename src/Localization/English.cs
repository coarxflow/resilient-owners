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

		public override string GetWorkersHistory(int amount)
		{
			return amount+" citizens have worked here.";
		}

		public override string GetEmptyFacility()
		{
			return "No citizen have worked here";
		}

		//industrial production stats
		public override string GetExtractedAmount()
		{
			return "Extraction: ";
		}

		public override string GetProducedAmount()
		{
			return "Production: ";
		}

        public override string GetIndustrialGenericGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "manufactured "+amount + " goods";
            else
                ret = amount + " goods";
            return ret;
        }

        public override string GetIndustrialFarmingGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "grown "+amount*2 + " pounds";
            else
                ret = amount*2 + " pounds";
            return ret;
        }

        public override string GetIndustrialForestryExtractorDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = amount + " logs sawn";
            else
                ret = amount + " logs";
            return ret;
        }

        public override string GetIndustrialForestryGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "crafted "+amount + " utilities";
            else
                ret = amount + " utilities";
            return ret;
        }

        public override string GetIndustrialOilExtractorDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "pumped "+amount + " barrils";
            else
                ret = amount + " barrils";
            return ret;
        }

        public override string GetIndustrialOilGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "processed "+amount + " kg of chemicals";
            else
                ret = amount + " kg";
            return ret;
        }

        public override string GetIndustrialOreExtractorDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "mined "+amount + " tons";
            else
                ret = amount + " tons";
            return ret;
        }

        public override string GetIndustrialOreGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "casted "+amount + " steel bars";
            else
                ret = amount + " steel bars";
            return ret;
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

