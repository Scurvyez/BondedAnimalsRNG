using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    public class BARNGMod : Mod
    {
        public static BARNGMod mod;
        
        private readonly BARNGSettings _settings;
        private float _halfWidth;
        private readonly Color _headerTextColor;
        private const float NewSectionGap = 6f;
        private const float HeaderTextGap = 3f;
        private const float Spacing = 3f;
        private const float SliderSpacing = 120f;
        private const float LabelWidth = 200f;
        private const float TextFieldWidth = 100f;
        private const float ElementHeight = 25f;
        
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
            _halfWidth = (inRect.width - 30) / 2;
            
            Rect leftRect = new (inRect.x, inRect.y, _halfWidth - 10f, inRect.height);
            Rect rightRect = new (inRect.x + _halfWidth + 10f, inRect.y, _halfWidth - 10f, inRect.height);
            
            DrawLeftSideSettings(leftRect);
            //DrawRightSideSettings(rightRect);
        }
        
        private void DrawLeftSideSettings(Rect leftRect)
        {
            Listing_Standard listLeft = new();
            listLeft.Begin(leftRect);
            
            listLeft.Label("BARNG_ConditionToggle".Translate().Colorize(_headerTextColor) + 
                           "BARNG_DefaultTrue".Translate().Colorize(_headerTextColor));
            listLeft.Label("BARNG_ConditionToggleDesc".Translate());
            listLeft.Gap(HeaderTextGap);
            
            listLeft.CheckboxLabeled("BARNG_OnlyBondedChanges".Translate(), ref _settings.onlyBondedChanges);
            listLeft.Gap(NewSectionGap);
            
            listLeft.GapLine();
            listLeft.Gap(NewSectionGap);
            
            listLeft.Label("BARNG_CapacityToggles".Translate().Colorize(_headerTextColor) + 
                           "BARNG_DefaultFalse".Translate().Colorize(_headerTextColor));
            listLeft.Label("BARNG_CapacityTogglesDesc".Translate());
            listLeft.Gap(HeaderTextGap);
            
            foreach (HediffDef hediff in HediffCollections.CapacityChangeHediffs)
            {
                bool isEnabled = _settings.HediffToggles.TryGetValue(hediff.defName, out bool currentValue) 
                                 && currentValue;
                listLeft.CheckboxLabeled(hediff.LabelCap, ref isEnabled);
                _settings.HediffToggles[hediff.defName] = isEnabled;
                listLeft.Gap(Spacing);
            }
            
            listLeft.End();
        }
        
        private void DrawRightSideSettings(Rect rightRect)
        {
            Listing_Standard listRight = new();
            listRight.Begin(rightRect);

            // nothing here atm lol
            
            listRight.End();
        }
    }
}