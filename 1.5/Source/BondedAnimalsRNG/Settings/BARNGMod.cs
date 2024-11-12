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
        private const float _spacing = 3f;
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
            DrawRightSideSettings(rightRect);
        }
        
        private void DrawLeftSideSettings(Rect leftRect)
        {
            Listing_Standard listLeft = new();
            listLeft.Begin(leftRect);
            
            listLeft.Label("BARNG_ConditionToggle".Translate().Colorize(_headerTextColor) + 
                           "BARNG_DefaultTrue".Translate().Colorize(_headerTextColor));
            listLeft.Label("BARNG_ConditionToggleDesc".Translate());
            listLeft.Gap(_headerTextGap);
            
            listLeft.CheckboxLabeled("BARNG_OnlyBondedChanges".Translate(), ref _settings.onlyBondedChanges);
            listLeft.Gap(_newSectionGap);
            
            listLeft.GapLine();
            listLeft.Gap(_newSectionGap);
            
            listLeft.Label("BARNG_CapacityToggles".Translate().Colorize(_headerTextColor) + 
                           "BARNG_DefaultFalse".Translate().Colorize(_headerTextColor));
            listLeft.Label("BARNG_CapacityTogglesDesc".Translate());
            listLeft.Gap(_headerTextGap);
            
            foreach (HediffDef hediff in HediffCollections.CapacityChangeHediffs)
            {
                bool isEnabled = _settings.hediffToggles.TryGetValue(hediff.defName, out bool currentValue) && currentValue;
                listLeft.CheckboxLabeled(hediff.LabelCap, ref isEnabled);
                _settings.hediffToggles[hediff.defName] = isEnabled;
                listLeft.Gap(_spacing);
            }
            
            listLeft.End();
        }
        
        private void DrawRightSideSettings(Rect rightRect)
        {
            Listing_Standard listRight = new();
            listRight.Begin(rightRect);
            
            listRight.Label("BARNG_FrequencyToggle".Translate().Colorize(_headerTextColor) + 
                            "BARNG_DefaultFalse".Translate().Colorize(_headerTextColor));
            listRight.Label("BARNG_AllowYearlyChangesDesc".Translate());
            listRight.Gap(_headerTextGap);
            
            listRight.CheckboxLabeled("BARNG_AllowYearlyChanges".Translate(), ref _settings.allowYearlyChanges);
            listRight.Gap(_newSectionGap);
            
            listRight.End();
        }
    }
}