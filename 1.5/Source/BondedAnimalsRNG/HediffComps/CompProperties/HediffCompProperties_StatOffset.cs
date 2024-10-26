using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffCompProperties_StatOffset : HediffCompProperties
    {
        public List<StatDef> possibleStatDefs = new ();
        
        public HediffCompProperties_StatOffset()
        {
            compClass = typeof(HediffComp_StatOffset);
        }
    }
}