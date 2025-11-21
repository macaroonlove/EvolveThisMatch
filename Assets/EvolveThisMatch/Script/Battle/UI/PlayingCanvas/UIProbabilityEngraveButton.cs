using EvolveThisMatch.Core;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class UIProbabilityEngraveButton : UIEngraveButton
    {
        private UIEngraveCanvas _engraveCanvas;
        private CoinSystem _coinSystem;

        internal override void InitializeBattle(UIEngraveCanvas engraveCanvas)
        {
            _engraveCanvas = engraveCanvas;
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();
            _coinSystem.onChangedCoin += OnChangedCoin;

            OnChangedCoin(_coinSystem.currentCoin);
            Refrash();
        }

        internal override void DeinitializeBattle()
        {
            if (_coinSystem != null)
            {
                _coinSystem.onChangedCoin -= OnChangedCoin;
            }
        }

        private void OnChangedCoin(int value)
        {
            var needCoin = GameDataManager.Instance.GetProbabilityList().needCoin;

            if (needCoin > value)
            {
                _payText.color = Color.red;
            }
            else
            {
                _payText.color = _originTextColor;
            }
        }

        protected override void Engrave()
        {
            var needCoin = GameDataManager.Instance.GetProbabilityList().needCoin;

            if (!_coinSystem.CheckCoin(needCoin)) return;

            if (GameDataManager.Instance.UpgradeProbabilityLevel())
            {
                _coinSystem.PayCoin(needCoin);

                Refrash();
            }
        }

        internal override void ForceEngrave()
        {
            if (GameDataManager.Instance.UpgradeProbabilityLevel())
            {
                Refrash();
            }
        }

        private void Refrash()
        {
            var probability = GameDataManager.Instance.GetProbabilityList();
            var probabilityLevel = GameDataManager.Instance.probabilityLevel + 1;

            if (probability.needCoin == -1)
            {
                _payText.text = "Max";
            }
            else
            {
                _payText.text = $"<sprite name=\"Coin\"> {probability.needCoin}";
            }

            _levelText.text = $"LV. {probabilityLevel}";
            _engraveCanvas.ProbabilityRefrash(probability);
        }
    }
}