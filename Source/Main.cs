using System.Xml;

using UnityEngine;
using Verse;

namespace CustomDeepchemDeposits
{
    public class Settings : ModSettings
    {
        public int deepCountPerCell = 4;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref deepCountPerCell, "deepCountPerCell");
            base.ExposeData();
        }
    }

    public class CustomDeepchemDepositsMod : Mod
    {
        Settings settings;

        public CustomDeepchemDepositsMod(ModContentPack content)
            : base(content)
        {
            settings = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.Label("(!) Restart the game to apply the changes.");
            listingStandard.Gap(24f);
            listingStandard.Label(
                "Amount of deepchem available per cell (VCE’s default is 300; 1 deepchem becomes 3 chemfuel):"
            );
            listingStandard.ColumnWidth = inRect.width / 8;

            string buffer = settings.deepCountPerCell.ToString();
            listingStandard.TextFieldNumeric(
                ref settings.deepCountPerCell,
                ref buffer,
                0.0f,
                65000.0f
            );

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Custom deepchem deposits — a Vanilla Chemfuel Expanded patch";
        }
    }

    public class PatchOperationUseSettings : PatchOperation
    {
        private Settings settings = LoadedModManager
            .GetMod<CustomDeepchemDepositsMod>()
            .GetSettings<Settings>();

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;

            foreach (
                var defNode in xml.SelectNodes(
                    "Defs/ThingDef[defName=\"VCHE_Deepchem\"]/deepCountPerCell"
                )
            )
            {
                result = true;

                var xmlNode = defNode as XmlNode;

                xmlNode.InnerText = settings.deepCountPerCell.ToString();
            }

            return result;
        }
    }
}
