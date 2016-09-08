using System;

namespace ResilientOwners
{
	public class Localization
	{
		public static string GetHistoryTitle()
		{
			return "History - "+Mod.modName;
		}

		//description textfield
		public static string GetDescriptionEmpty()
		{
			return "Enter Description";
		}
		
		//activation date label
		public static string GetActivationDate()
		{
			return "Activation date : ";
		}

		//stats label
		public static string GetAccumulatedIncome()
		{
			return "AccumulatedIncome : ";
		}

		//families history label
		public static string GetEmptyHouse()
		{
			return "No families have lived here";
		}

		public static string GetFamiliesHistory()
		{
			return "Families having lived here: ";
		}

		public static string GetFamiliesSeparator()
		{
			return ", ";
		}

		public static string GetWorkersHistory()
		{
			return "Worked here: ";
		}

		public static string GetEmptyFacility()
		{
			return "No Workers have worked here";
		}

		//industrial production stats

	}
}

