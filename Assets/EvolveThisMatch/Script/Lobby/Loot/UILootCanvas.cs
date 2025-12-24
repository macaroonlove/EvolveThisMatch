using CodeStage.AntiCheat.ObscuredTypes;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UILootCanvas : UIBase
    {
        #region 바인딩
        enum Texts
        {
            ElapsedTimeText,
            KillCountText,
        }
        enum Buttons
        {
            CloseButton,
            GetRewardButton,
        }
        enum Objects
        {
            Content,
        }
        #endregion

        [SerializeField] private GameObject _prefab;

        private Transform _parent;
        private TextMeshProUGUI _elapsedTimeText;
        private TextMeshProUGUI _killCountText;

        private EnemyRecordSystem _enemyRecordSystem;
        private LobbyWaveSystem _waveSystem;
        private GlobalStatusSystem _globalStatusSystem;

        private int _elapsedSeconds;
        private StringBuilder _stringBuilder = new StringBuilder();
        private Coroutine _timerRoutine;
        private WaitForSecondsRealtime _wfs = new WaitForSecondsRealtime(1f);

        private WaveTemplate _waveTemplate;
        private List<UILootItem> _items = new List<UILootItem>();

        protected override void Initialize()
        {
            _enemyRecordSystem = BattleManager.Instance.GetSubSystem<EnemyRecordSystem>();
            _waveSystem = BattleManager.Instance.GetSubSystem<LobbyWaveSystem>();
            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();

            BindText(typeof(Texts));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _parent = GetObject((int)Objects.Content).transform;
            _elapsedTimeText = GetText((int)Texts.ElapsedTimeText);
            _killCountText = GetText((int)Texts.KillCountText);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            GetButton((int)Buttons.GetRewardButton).onClick.AddListener(() =>
            {
                if (_elapsedSeconds < 300)
                {
                    UIPopupManager.Instance.ShowNotificationPopup("기록한지 5분 이후부터 정산이 가능합니다.");
                    return;
                }

                _elapsedSeconds = 0;
                IdleManager.onForceOnlineIdleReward?.Invoke();
            });

            _timerRoutine = StartCoroutine(CoUpdateElapsedTime());

            _waveSystem.onChangeWave += OnChangedStage;
        }

        private void OnDestroy()
        {
            if (_timerRoutine != null) StopCoroutine(_timerRoutine);

            if (_waveSystem != null) _waveSystem.onChangeWave -= OnChangedStage;
        }

        public override void Show(bool isForce = false)
        {
            OnChangedRecords(_enemyRecordSystem.records);

            _enemyRecordSystem.onChangedRecords += OnChangedRecords;

            base.Show(isForce);
        }

        public override void Hide(bool isForce = false)
        {
            base.Hide(isForce);

            BattleManager.Instance.GetSubSystem<EnemyRecordSystem>().onChangedRecords -= OnChangedRecords;
        }

        private void OnChangedStage()
        {
            _waveTemplate = BattleManager.Instance.GetSubSystem<LobbyWaveSystem>().currentWave;
        }

        private IEnumerator CoUpdateElapsedTime()
        {
            while (true)
            {
                _elapsedSeconds += 1;

                int hours = _elapsedSeconds / 3600;
                int minutes = (_elapsedSeconds % 3600) / 60;
                int seconds = _elapsedSeconds % 60;

                _stringBuilder.Clear();

                if (hours > 0) _stringBuilder.Append(hours).Append("시간 ");
                if (minutes > 0) _stringBuilder.Append(minutes).Append("분 ");
                _stringBuilder.Append(seconds).Append("초");

                _elapsedTimeText.text = _stringBuilder.ToString();

                yield return _wfs;
            }
        }

        private void OnChangedRecords(Dictionary<int, ObscuredInt> records)
        {
            int totalKill;
            var totalDrops = CalculateIdleDrops(records, out totalKill);

            UpdateUI(totalKill, totalDrops);
        }

        private Dictionary<EVariableType, int> CalculateIdleDrops(Dictionary<int, ObscuredInt> records, out int totalKill)
        {
            totalKill = 0;
            var totalDrops = new Dictionary<EVariableType, int>();

            foreach (var record in records)
            {
                // 처치 수 계산
                totalKill += record.Value;

                // 예상 획득량 계산
                var rarity = (EEnemyRarity)record.Key;

                foreach (var dropData in _waveTemplate.GetEnemyData(rarity).idleDropDatas)
                {
                    if (!totalDrops.ContainsKey(dropData.type))
                    {
                        totalDrops[dropData.type] = 0;
                    }

                    totalDrops[dropData.type] += dropData.amount * record.Value;
                }
            }

            return totalDrops;
        }

        private void UpdateUI(int totalKill, Dictionary<EVariableType, int> totalDrops)
        {
            _killCountText.text = $"{totalKill:N0} 마리";

            while (_items.Count < totalDrops.Count)
            {
                var instance = Instantiate(_prefab, _parent);
                var item = instance.GetComponent<UILootItem>();
                _items.Add(item);
            }

            int index = 0;
            foreach (var drop in totalDrops)
            {
                var variable = SaveManager.Instance.profileData.GetVariable(drop.Key);

                _items[index].Show(variable, drop.Value);
                index++;
            }

            for (; index < _items.Count; index++)
            {
                _items[index].Hide(true);
            }
        }

        private int CalculateGoldAmount(int amount)
        {
            float result = amount;

            // 추가·차감
            foreach (var instance in _globalStatusSystem.GoldGainAdditionalDataEffects)
            {
                result += instance.effect.GetValue(instance.context);
            }

            // 증가·감소
            float increase = 1;
            foreach (var instance in _globalStatusSystem.GoldGainIncreaseDataEffects)
            {
                increase += instance.effect.GetValue(instance.context);
            }
            result *= increase;

            // 상승·하락
            foreach (var instance in _globalStatusSystem.GoldGainMultiplierDataEffects)
            {
                result *= instance.effect.GetValue(instance.context);
            }

            return (int)result;
        }
    }
}