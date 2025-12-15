using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UIEnemyCountCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Slider,
        }
        enum Texts
        {
            Counter,
        }
        #endregion

        private Image _slider;
        private TextMeshProUGUI _counter;

        private BlockSystem _blockSystem;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _slider = GetImage((int)Images.Slider);
            _counter = GetText((int)Texts.Counter);

            _slider.fillAmount = 0;
            _counter.text = "0/100";
        }

        internal void InitializeBattle()
        {
            _blockSystem = BattleManager.Instance.GetSubSystem<BlockSystem>();

            _blockSystem.onChangedBlockCount += OnChangedBlockCount;
        }

        internal void DeinitializeBattle()
        {
            _blockSystem.onChangedBlockCount -= OnChangedBlockCount;
        }

        private void OnChangedBlockCount(int count)
        {
            _slider.fillAmount = count * 0.01f;
            _counter.text = $"{count}/100";
        }
    }
}