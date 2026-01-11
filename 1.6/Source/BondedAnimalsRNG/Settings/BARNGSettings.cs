using System.Collections.Generic;
using Verse;

namespace BondedAnimalsRNG
{
    public class BARNGSettings : ModSettings
    {
        public static BARNGSettings Instance;
        
        public BARNGSettings()
        {
            Instance = this;
        }
        
        public static bool OnlyBondedChanges => Instance.onlyBondedChanges;
        public bool onlyBondedChanges = true;
        
        public Dictionary<string, bool> HediffToggles = new ();
        private List<string> _hediffToggleKeys;
        private List<bool> _hediffToggleValues;
        
        public bool IsHediffEnabled(string defName)
        {
            return !HediffToggles.TryGetValue(defName, out bool isEnabled) || !isEnabled;
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref onlyBondedChanges, "onlyBondedChanges", true);
            Scribe_Collections.Look(ref HediffToggles, "hediffToggles", 
                LookMode.Value, LookMode.Value, ref _hediffToggleKeys, ref _hediffToggleValues);
        }
    }
}