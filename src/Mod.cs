using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;

namespace ResilientOwners {
    public class Mod: IUserMod {

		public static string modName = "Resilient Owners";
		public static String modID = "ResilientOwners";
		public static string version = "1.0.0";

        public string Name {
			get { return modName; }
        }
        public string Description {
			get { return "ResilientOwners"; }
        }
    }
}
