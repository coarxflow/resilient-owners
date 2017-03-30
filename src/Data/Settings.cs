namespace ResilientOwners
{
    [System.Serializable]
    public struct ResilientSettings
    {
        public bool listResidentsAndWorkers;
        public bool extinguishFires;
    }

    public class Settings
    {
        public static ResilientSettings inst;

		public static void defaultSettings()
        {
            inst = new ResilientSettings();

            inst.listResidentsAndWorkers = true;
            inst.extinguishFires = true;
        }
    }
}