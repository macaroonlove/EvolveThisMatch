namespace EvolveThisMatch.Lobby
{
    public class UIShopGainListItem : UIShopGainItem
    {
        protected override string ShowVariableItemData(VariableGainShopItemData variableData)
        {
            return $"{variableData.variable.DisplayName} {base.ShowVariableItemData(variableData)}";
        }

        protected override string ShowUnitItemData(UnitGainShopItemData unitData)
        {
            return $"{unitData.agentTemplate.displayName} {base.ShowUnitItemData(unitData)}";
        }

        protected override string ShowArtifactItemData(ArtifactGainShopItemData artifactData)
        {
            return $"{artifactData.artifactTemplate.displayName} {base.ShowArtifactItemData(artifactData)}";
        }

        protected override string ShowTomeItemData(TomeGainShopItemData tomeData)
        {
            return $"{tomeData.tomeTemplate.displayName} {base.ShowTomeItemData(tomeData)}";
        }
    }
}