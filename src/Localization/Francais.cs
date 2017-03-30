using System;

namespace ResilientOwners
{
	public class Francais : Localization
	{
        public static string modName = "Proprios résilients";
        public static string modDescription = "Ajoute un historique aux bâtiments de zone et les rend résilients.";

        public override string GetHistoryTitle()
		{
			return "Histoire";
		}

		public override string GetRecordTitle()
		{
			return "Compte rendu";
		}

		//description textfield
		public override string GetDescriptionEmpty()
		{
			return "Entrer une description";
		}
		
		//activation date label
		public override string GetActivationDate()
		{
			return "Depuis ";
		}

		public override string GetAge(int years, int days)
		{
			if(years == 0)
				return "Age: "+days+" jours";
			else if(years == 1)
				return "Age: "+years+" an, "+days+" jours";
			else
				return "Age: "+years+" ans, "+days+" jours";
		}

		//office wealth label
		public override string GetAccumulatedIncome()
		{
			return "Richesse générée: ";
		}

		//families history label
		public override string GetEmptyHouse()
		{
			return "Aucune famille n'a vécu ici";
		}

		public override string GetFamiliesHistory()
		{
			return "Familles ayant vécu ici : ";
		}

		public override string GetFamiliesSeparator()
		{
			return ", ";
		}

		public override string GetWorkersHistory(int amount)
		{
			return amount+" citoyens ont travaillé ici.";
		}

		public override string GetEmptyFacility()
		{
			return "Aucun citoyen n'a travaillé ici.";
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
                ret = amount + " biens manufacturés";
            else
                ret = amount + " biens";
            return ret;
        }

        public override string GetIndustrialFarmingGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = amount*2 + " livres récoltées";
            else
                ret = amount*2 + " livres";
            return ret;
        }

        public override string GetIndustrialForestryExtractorDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = amount + " arbres sciés";
            else
                ret = amount + " arbres";
            return ret;
        }

        public override string GetIndustrialForestryGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = amount + " commodités fabriquées";
            else
                ret = amount + " commodités";
            return ret;
        }

        public override string GetIndustrialOilExtractorDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "pompé "+amount + " barrils";
            else
                ret = amount + " barrils";
            return ret;
        }

        public override string GetIndustrialOilGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = "traité "+amount + " kg de produits chimiques";
            else
                ret = amount + " kg";
            return ret;
        }

        public override string GetIndustrialOreExtractorDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = amount + " tonnes minées";
            else
                ret = amount + " tonnes";
            return ret;
        }

        public override string GetIndustrialOreGoodsDenomination(float amount, bool litteral)
        {
            string ret;
            if (litteral)
                ret = amount + " barres d'acier coulées";
            else
                ret = amount + " barres d'acier";
            return ret;
        }

		//commercial clients stats
		public override string GetClientsAmount()
		{
			return "Clients servis: ";
		}

		//tooltips
		public override string GetTooltipOff()
		{
			return Mod.modName+": désactivé";
		}

		public override string GetTooltipHistoryOn()
		{
			return Mod.modName+": histoire activée";
		}

		public override string GetTooltipResiliencyOn()
		{
			return Mod.modName+": histoire et résilience activées";
		}

        //settings

        public override string GetResidentsListingSetting()
        {
            return "Enregistrer l'historique des familles et travailleurs";
        }

        public override string GetExtinguishFiresSetting()
        {
            return "Stopper automatiquement les incendies pour les bâtiments résilients";
        }

        public override string GetSettingsPerCities()
        {
            return "Options sauvegardées séparément par ville.";
        }
    }
}

