using System;

namespace ResilientOwners
{
	public class Localization
	{
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
	}
}

