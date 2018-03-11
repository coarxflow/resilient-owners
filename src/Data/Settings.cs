namespace HistoricBuildings
{
    [System.Serializable]
    public struct ResilientSettings
    {
        public bool extinguishFires;
        public bool noAbandonment;
    }

    public class Settings
    {
        public static ResilientSettings inst;

		public static void defaultSettings()
        {
            inst = new ResilientSettings();

            inst.noAbandonment = false;
            inst.extinguishFires = true;
        }
    }
}