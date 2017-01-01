using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;

using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle ("ResilientOwners")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("")]
[assembly: AssemblyProduct ("ResilientOwners")]
[assembly: AssemblyCopyright ("CoarxFlow")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion ("1.0.1")]

namespace ResilientOwners {
    public class Mod: IUserMod {

		public static string modName = "Resilient Owners";
		public static String modID = "ResilientOwners";
		public static string version = "1.0.1";

        public string Name {
			get { return modName; }
        }
        public string Description {
			get { return "Adds history and make zoned buildings resilient."; }
        }
    }
}
