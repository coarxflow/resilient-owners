using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using System;
using System.Globalization;

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

[assembly: AssemblyVersion ("1.2.2")]

namespace ResilientOwners {
    public class Mod: IUserMod {

		public static string modName = English.modName;
		public static String modID = "ResilientOwners";
		public static string version = "1.2.2";

        public string Name {
			get
            {
                switch (SingletonLite<LocaleManager>.instance.language)
                {
                    case "fr":
                        return Francais.modName;
                    default:
                        return modName;
                }
            }
        }
        public string Description
        {
            get
            {
                switch (SingletonLite<LocaleManager>.instance.language)
                {
                    case "fr":
                        return Francais.modDescription;
                    default:
                        return English.modDescription;
                }
            }
        }

        public static void ChooseLocalization()
        {
            //CODebug.Log(LogChannel.Modding, "language code " + SingletonLite<LocaleManager>.instance.language);

            switch(SingletonLite<LocaleManager>.instance.language)
            {
                case "fr":
                    Localization.trad = new Francais();
                    Localization.culture = new CultureInfo("fr-FR");
                    break;
                default:
                    Localization.trad = new English();
                    Localization.culture = new CultureInfo("en-US");
                    break;
            }
        }

        public static ResilientBuildings s_info;

        static UICheckBox listResidentsSetting;
        static UICheckBox extinguishFiresSetting;

        public void OnSettingsUI(UIHelperBase helper)
        {
            //localization
            if (Localization.trad == null)
                ChooseLocalization();

            //per city settings
            Settings.defaultSettings();

            listResidentsSetting = (UICheckBox) helper.AddCheckbox(Localization.trad.GetResidentsListingSetting(), Settings.inst.listResidentsAndWorkers, toggleResidentsListingSetting);

            helper.AddSpace(20);

            extinguishFiresSetting = (UICheckBox)helper.AddCheckbox(Localization.trad.GetExtinguishFiresSetting(), Settings.inst.extinguishFires, toggleExtinguishFiresSetting);

            helper.AddSpace(40);

            helper.AddGroup(Localization.trad.GetSettingsPerCities());

        }

        public void toggleResidentsListingSetting(bool toggle)
        {
            Settings.inst.listResidentsAndWorkers = toggle;

            if (toggle && s_info != null)
                s_info.clearResidentsList();
        }

        public void toggleExtinguishFiresSetting(bool toggle)
        {
            Settings.inst.extinguishFires = toggle;
        }

        public static void updateSettingsPanel()
        {
            listResidentsSetting.isChecked = Settings.inst.listResidentsAndWorkers;
            extinguishFiresSetting.isChecked = Settings.inst.extinguishFires;
        }
    }
}
