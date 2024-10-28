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
        
        public bool _onlyBondedCapacityChanges = true;
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _onlyBondedCapacityChanges, "_onlyBondedCapacityChanges", true);
        }
    }
}