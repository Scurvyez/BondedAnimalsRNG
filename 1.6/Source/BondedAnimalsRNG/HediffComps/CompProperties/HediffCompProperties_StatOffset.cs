using System.Collections.Generic;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffCompProperties_StatOffset : HediffCompProperties
    {
        public List<HediffCompData> hediffCompData = [];
        
        public HediffCompProperties_StatOffset()
        {
            compClass = typeof(HediffComp_StatOffset);
        }
    }
}