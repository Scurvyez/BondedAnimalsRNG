using Verse;

namespace BondedAnimalsRNG
{
    public class BARNGSettings : ModSettings
    {
        private static BARNGSettings _instance;
        
        public BARNGSettings()
        {
            _instance = this;
        }
        
        public static bool OnlyBondedCapacityChanges => _instance._onlyBondedCapacityChanges;
        public static bool AllowYearlyCapacityChanges => _instance._allowYearlyCapacityChanges;
        
        public bool _onlyBondedCapacityChanges = true;
        public bool _allowYearlyCapacityChanges = true;
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _onlyBondedCapacityChanges, "_onlyBondedCapacityChanges", true);
            Scribe_Values.Look(ref _allowYearlyCapacityChanges, "_allowYearlyCapacityChanges", true);
        }
    }
}