using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork.UIPopup;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITalentPresenter
    {
        private readonly UITalentModel _model;
        private readonly UITalentView _view;

        private bool _isRolling;

        public string currentButtonText
        {
            get
            {
                return $"<sprite name=Powder> {_model.CalculateCost()}\n개화";
            }
        }

        public UITalentPresenter(UITalentModel model, UITalentView view)
        {
            _model = model;
            _view = view;
        }

        public void Show(AgentSaveData.Agent owned)
        {
            _model.Bind(owned);
            Render();
        }

        #region 재능 돌리기
        public void OnRollOnce()
        {
            if (_model.TryRollOnce())
                Render();
        }

        public async UniTask OnRollFiltered(TalentFilterCondition condition)
        {
            _isRolling = true;

            try
            {
                while (_isRolling)
                {
                    bool matched = _model.TryRollFiltered(condition.rarity, condition.talentIds);

                    Render();

                    if (matched)
                    {
                        _isRolling = false;
                        UIPopupManager.Instance.ShowConfirmPopup("원하는 재능을 찾았습니다.");
                        break;
                    }

                    await UniTask.Yield();
                }
            }
            finally
            {
                _isRolling = false;
                Render();
            }
        }

        public void StopRolling()
        {
            _isRolling = false;
        }
        #endregion

        #region 잠금 관리
        public void OnLockChanged(int index, bool isLock)
        {
            _model.SetLock(index, isLock);
            Render();
        }
        #endregion

        #region 뷰 표시
        private void Render()
        {
            _view.Render(BuildViewState());
        }

        private TalentViewState BuildViewState()
        {
            var saveData = _model.saveData;
            var slots = new TalentSlotState[saveData.finalTalent.Length];

            for (int i = 0; i < slots.Length; i++)
            {
                var slot = saveData.finalTalent[i];

                if (slot.id < 0)
                {
                    slots[i] = new TalentSlotState
                    {
                        id = -1,
                        value = 0,
                        isLocked = slot.isLock,
                        rarity = null
                    };
                    continue;
                }

                var config = AgentSaveDataTemplate.talentTitleData.talentData[slot.id];

                slots[i] = new TalentSlotState
                {
                    id = slot.id,
                    value = slot.value,
                    isLocked = slot.isLock,
                    rarity = _model.GetRarity(config, slot.value)
                };
            }

            return new TalentViewState
            {
                showTalent = _model.owned.tier > 2,
                canPay = _model.CanPay(),
                buttonText = currentButtonText,
                slots = slots
            };
        }
        #endregion

        #region 랜덤 시드 다시 설정
        public void ClearRNG()
        {
            _model.ClearRNG();
        }
        #endregion
    }
}