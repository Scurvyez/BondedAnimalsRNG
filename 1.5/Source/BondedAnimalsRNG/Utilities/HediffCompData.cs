using System.Xml;
using RimWorld;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffCompData
    {
        public StatDef randomStat = null;
        public FloatRange statAdjustmentRange = new (1, 1);

        public HediffCompData()
        {
            
        }

        public HediffCompData(StatDef randomStat, FloatRange statAdjustmentRange)
        {
            this.randomStat = randomStat;
            this.statAdjustmentRange = statAdjustmentRange;
        }

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "randomStat", xmlRoot.Name);
            
            string spawnRangeStr = xmlRoot.InnerText.Trim();
            string[] rangeValues = spawnRangeStr.Split('~');
            if (rangeValues.Length == 2)
            {
                float min = ParseHelper.FromString<float>(rangeValues[0]);
                float max = ParseHelper.FromString<float>(rangeValues[1]);
                statAdjustmentRange = new FloatRange(min, max);
            }
        }
    }
}