using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using System;
using System.Globalization;

using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle ("HistoricBuildings")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("")]
[assembly: AssemblyProduct ("HistoricBuildings")]
[assembly: AssemblyCopyright ("CoarxFlow")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion ("1.0.0")]

namespace HistoricBuildings {
    public class Mod: IUserMod {

		public static string modName = English.modName;
		public static String modID = "HistoricBuildings";
		public static string version = "1.0.0";

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

        public static HistoricBuildings s_info;

        static UICheckBox abandonmentSetting;
        static UICheckBox extinguishFiresSetting;

        public void OnSettingsUI(UIHelperBase helper)
        {
            //localization
            if (Localization.trad == null)
                ChooseLocalization();

            //per city settings
            Settings.defaultSettings();

            abandonmentSetting = (UICheckBox)helper.AddCheckbox(Localization.trad.GetAbandonmentSetting(), Settings.inst.noAbandonment, toggleAbandonmentSetting);

            helper.AddSpace(40);

            extinguishFiresSetting = (UICheckBox)helper.AddCheckbox(Localization.trad.GetExtinguishFiresSetting(), Settings.inst.extinguishFires, toggleExtinguishFiresSetting);

            helper.AddSpace(40);

            helper.AddGroup(Localization.trad.GetSettingsPerCities());

        }


        public void toggleExtinguishFiresSetting(bool toggle)
        {
            Settings.inst.extinguishFires = toggle;
        }

        public void toggleAbandonmentSetting(bool toggle)
        {
            Settings.inst.noAbandonment = toggle;
        }

        public static void updateSettingsPanel()
        {
            extinguishFiresSetting.isChecked = Settings.inst.extinguishFires;
            abandonmentSetting.isChecked = Settings.inst.noAbandonment;
        }
    }
}
