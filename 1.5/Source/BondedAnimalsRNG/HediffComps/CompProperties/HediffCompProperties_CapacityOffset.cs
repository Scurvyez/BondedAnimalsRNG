using System.Collections.Generic;
using Verse;

namespace BondedAnimalsRNG;

public class HediffCompProperties_CapacityOffset : HediffCompProperties
{
    public FloatRange adjustmentRange = FloatRange.One;
        
    public HediffCompProperties_CapacityOffset()
    {
        compClass = typeof(HediffComp_CapacityOffset);
    }
}