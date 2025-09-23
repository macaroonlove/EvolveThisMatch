using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using TMPro;

namespace EvolveThisMatch.Lobby
{
    public class UIStageDisplay : UIBase
    {
        private LobbyWaveSystem _lobbyWaveSystem;

        private TextMeshProUGUI _text;

        protected override void Initialize()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            _lobbyWaveSystem = BattleManager.Instance.GetSubSystem<LobbyWaveSystem>();
            _lobbyWaveSystem.onChangeWave += OnChangeWave;

            OnChangeWave();
        }

        private void OnDestroy()
        {
            _lobbyWaveSystem.onChangeWave -= OnChangeWave;
            _lobbyWaveSystem = null;
        }

        private void OnChangeWave()
        {
            _text.text = _lobbyWaveSystem.currentWave.displayName;
        }
    }
}