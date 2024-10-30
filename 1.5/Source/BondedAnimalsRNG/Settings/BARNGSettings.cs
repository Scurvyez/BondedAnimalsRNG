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
        
        public static bool OnlyBondedChanges => _instance._onlyBondedChanges;
        
        public bool _onlyBondedChanges = true;
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _onlyBondedChanges, "_onlyBondedChanges", true);
        }
    }
}