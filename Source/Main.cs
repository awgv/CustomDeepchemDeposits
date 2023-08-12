using System.Collections.Generic;
using System.Xml;

using UnityEngine;
using Verse;

namespace CustomDeepchemDeposits
{
    public class Settings : ModSettings
    {
        public int deepCountPerCell = 4;
        public int deepchemTankStorageCapacity = 120;
        public int chemfuelTankStorageCapacity = 1350;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref deepCountPerCell, "deepCountPerCell");
            Scribe_Values.Look(ref deepchemTankStorageCapacity, "deepchemTankStorageCapacity");
            Scribe_Values.Look(ref chemfuelTankStorageCapacity, "chemfuelTankStorageCapacity");
            base.ExposeData();
        }
    }

    public class CustomDeepchemDepositsMod : Mod
    {
        readonly Settings settings;

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
            listingStandard.Gap(48f);

            listingStandard.Label(
                "Amount of deepchem available per cell (VCE’s default is 300; 1 deepchem becomes 3 chemfuel):"
            );
            listingStandard.ColumnWidth = inRect.width / 8;

            string deepCountPerCellBuffer = settings.deepCountPerCell.ToString();
            listingStandard.TextFieldNumeric(
                ref settings.deepCountPerCell,
                ref deepCountPerCellBuffer,
                0.0f,
                65000.0f
            );
            listingStandard.Gap(24f);

            listingStandard.ColumnWidth = inRect.width;
            listingStandard.Label("Deepchem tank storage capacity:");
            listingStandard.ColumnWidth = inRect.width / 8;

            string deepchemTankStorageCapacityBuffer =
                settings.deepchemTankStorageCapacity.ToString();
            listingStandard.TextFieldNumeric(
                ref settings.deepchemTankStorageCapacity,
                ref deepchemTankStorageCapacityBuffer,
                0.0f,
                65000.0f
            );
            listingStandard.Gap(24f);

            listingStandard.ColumnWidth = inRect.width;
            listingStandard.Label("Chemfuel tank storage capacity:");
            listingStandard.ColumnWidth = inRect.width / 8;

            string chemfuelTankStorageCapacityBuffer =
                settings.chemfuelTankStorageCapacity.ToString();
            listingStandard.TextFieldNumeric(
                ref settings.chemfuelTankStorageCapacity,
                ref chemfuelTankStorageCapacityBuffer,
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
        private readonly Settings settings = LoadedModManager
            .GetMod<CustomDeepchemDepositsMod>()
            .GetSettings<Settings>();

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode xmlNode;

            Dictionary<string, int> xpathToValueMap = new Dictionary<string, int>()
            {
                {
                    "Defs/ThingDef[defName=\"VCHE_Deepchem\"]/deepCountPerCell",
                    settings.deepCountPerCell
                },
                {
                    "Defs/ThingDef[defName=\"PS_DeepchemTank\"]/comps/li[@Class=\"PipeSystem.CompProperties_ResourceStorage\"]/storageCapacity",
                    settings.deepchemTankStorageCapacity
                },
                {
                    "Defs/ThingDef[defName=\"PS_ChemfuelTank\"]/comps/li[@Class=\"PipeSystem.CompProperties_ResourceStorage\"]/storageCapacity",
                    settings.chemfuelTankStorageCapacity
                },
            };

            foreach (KeyValuePair<string, int> pair in xpathToValueMap)
            {
                xmlNode = xml.SelectSingleNode(pair.Key);
                xmlNode.InnerText = pair.Value.ToString();
            }

            return true;
        }
    }
}
