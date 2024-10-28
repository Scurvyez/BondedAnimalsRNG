using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    public class BARNGMod : Mod
    {
        public static BARNGMod mod;
        
        private BARNGSettings _settings;
        private float halfWidth;
        private readonly Color _headerTextColor;
        private const float _newSectionGap = 6f;
        private const float _headerTextGap = 3f;
        private const float _spacing = 10f;
        private const float _sliderSpacing = 120f;
        private const float _labelWidth = 200f;
        private const float _textFieldWidth = 100f;
        private const float _elementHeight = 25f;
        
        public BARNGMod(ModContentPack content) : base(content)
        {
            mod = this;
            _settings = GetSettings<BARNGSettings>();
            _headerTextColor = BARNGLog.MessageMsgCol;
        }
        
        public override string SettingsCategory()
        {
            return "BARNG_ModName".Translate();
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            halfWidth = (inRect.width - 30) / 2;
            
            Rect leftRect = new (inRect.x, inRect.y, halfWidth - 10f, inRect.height);
            Rect rightRect = new (inRect.x + halfWidth + 10f, inRect.y, halfWidth - 10f, inRect.height);
            
            DrawLeftSideSettings(leftRect);
        }
        
        private void DrawLeftSideSettings(Rect leftRect)
        {
            Listing_Standard listLeft = new();
            listLeft.Begin(leftRect);
            
            listLeft.CheckboxLabeled("BARNG_OnlyBondedCapacityChanges".Translate(), 
                ref _settings._onlyBondedCapacityChanges, "BARNG_OnlyBondedCapacityChangesDesc".Translate());
            listLeft.Gap(_newSectionGap);
            
            listLeft.End();
        }
    }
}