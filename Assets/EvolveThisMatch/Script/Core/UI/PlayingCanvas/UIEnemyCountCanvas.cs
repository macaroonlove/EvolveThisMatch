using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
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

        private EnemySystem _enemySystem;

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
            _enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();

            _enemySystem.onRegist += OnChangeEnemyCount;
            _enemySystem.onDeregist += OnChangeEnemyCount;
        }

        internal void DeinitializeBattle()
        {
            _enemySystem.onRegist -= OnChangeEnemyCount;
            _enemySystem.onDeregist -= OnChangeEnemyCount;
        }

        private void OnChangeEnemyCount(Unit unit)
        {
            int count = _enemySystem.enemyCount;

            _slider.fillAmount = count * 0.01f;
            _counter.text = $"{count}/100";
        }
    }
}