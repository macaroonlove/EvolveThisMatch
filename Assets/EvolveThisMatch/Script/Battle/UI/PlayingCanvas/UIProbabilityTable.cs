using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using TMPro;

namespace EvolveThisMatch.Battle
{
    public class UIProbabilityTable : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            CommonProbability,
            RareProbability,
            EpicProbability,
            LegendProbability,
            MythProbability,
        }
        #endregion

        private TextMeshProUGUI _commonText;
        private TextMeshProUGUI _rareText;
        private TextMeshProUGUI _epicText;
        private TextMeshProUGUI _legendText;
        private TextMeshProUGUI _mythText;

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _commonText = GetText((int)Texts.CommonProbability);
            _rareText = GetText((int)Texts.RareProbability);
            _epicText = GetText((int)Texts.EpicProbability);
            _legendText = GetText((int)Texts.LegendProbability);
            _mythText = GetText((int)Texts.MythProbability);
        }

        internal void Show(AgentRarityProbabilityData probability)
        {
            _commonText.text = (probability.common).ToString("0.###") + "%";
            _rareText.text = (probability.rare).ToString("0.###") + "%";
            _epicText.text = (probability.epic).ToString("0.###") + "%";
            _legendText.text = (probability.legend).ToString("0.###") + "%";
            _mythText.text = (probability.myth).ToString("0.###") + "%";
        }
    }
}
