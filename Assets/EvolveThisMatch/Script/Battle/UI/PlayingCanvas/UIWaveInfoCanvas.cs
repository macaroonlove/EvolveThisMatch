using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using System.Collections;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class UIWaveInfoCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            WaveStep,
            WaveTime,
        }
        #endregion

        private TextMeshProUGUI _waveStepText;
        private TextMeshProUGUI _waveTimeText;

        private WaveSystem _waveSystem;

        private WaitForSeconds _wfs = new WaitForSeconds(1f);

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _waveStepText = GetText((int)Texts.WaveStep);
            _waveTimeText = GetText((int)Texts.WaveTime);

            _waveSystem = BattleManager.Instance.GetSubSystem<WaveSystem>();
            _waveSystem.onWaveChanged += OnWaveChanged;
        }

        private void OnDestroy()
        {
            _waveSystem.onWaveChanged -= OnWaveChanged;
        }

        private void OnWaveChanged(int waveInfo, float remaining)
        {
            _waveStepText.text = $"Wave {waveInfo}";
            StartCoroutine(UpdateRemainingTime(remaining));
        }

        private IEnumerator UpdateRemainingTime(float remaining)
        {
            while (remaining > 1)
            {
                remaining -= 1f;

                int minutes = Mathf.FloorToInt(remaining / 60);
                int seconds = Mathf.FloorToInt(remaining % 60);
                _waveTimeText.text = $"{minutes:00}:{seconds:00}";
                yield return _wfs;
            }
        }
    }
}