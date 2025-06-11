using System.Collections.Generic;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffCompProperties_StatOffset : HediffCompProperties
    {
        public List<HediffCompData> hediffCompData = new ();
        
        public HediffCompProperties_StatOffset()
        {
            compClass = typeof(HediffComp_StatOffset);
        }
    }
}