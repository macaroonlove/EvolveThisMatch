using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UILevelUpView : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            OriginLevel,
            TargetLevel,
            ExpText,
        }
        enum Images
        {
            ExpSlider,
        }
        enum Buttons
        {
            ClearButton,
            LevelUpButton,
        }
        enum Objects
        {
            Arrow,
            EatFood,
            StockFood,
        }
        #endregion

        private TextMeshProUGUI _originLevel;
        private TextMeshProUGUI _targetLevel;
        private TextMeshProUGUI _expText;
        private Image _expSlider;
        private GameObject _arrow;

        private UILevelUpFoodSelectItem[] _eats;
        private UILevelUpFoodSelectItem[] _stocks;
        private UILevelUpAutoSelectItem _autoSelect;

        private UILevelUpPresenter _presenter;

        protected override void Initialize()
        {
            var model = new UILevelUpModel();
            _presenter = new UILevelUpPresenter(this, model);

            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _originLevel = GetText((int)Texts.OriginLevel);
            _targetLevel = GetText((int)Texts.TargetLevel);
            _expText = GetText((int)Texts.ExpText);
            _expSlider = GetImage((int)Images.ExpSlider);
            _arrow = GetObject((int)Objects.Arrow);

            _eats = GetObject((int)Objects.EatFood).GetComponentsInChildren<UILevelUpFoodSelectItem>();
            _stocks = GetObject((int)Objects.StockFood).GetComponentsInChildren<UILevelUpFoodSelectItem>();
            _autoSelect = GetComponentInChildren<UILevelUpAutoSelectItem>();

            for (int i = 0; i < _eats.Length; i++)
            {
                int index = i;
                _eats[i].InitializeItem(() =>
                {
                    if (_presenter.RemoveFood(index))
                    {
                        _eats[index].Decrement();
                        _stocks[index].Increment();
                    }
                });

                _stocks[i].InitializeItem(() =>
                {
                    if (_presenter.AddFood(index))
                    {
                        _eats[index].Increment();
                        _stocks[index].Decrement();
                    }
                });
            }

            _autoSelect.Initialize(() => 
            {
                int[] stockCounts = new int[_stocks.Length];
                for (int i = 0; i < _stocks.Length; i++)
                {
                    stockCounts[i] = _stocks[i].count;
                }

                int[] result = _presenter.AutoSelect(stockCounts);

                for (int i = 0; i < result.Length; i++)
                {
                    for (int j = 0; j < result[i]; j++)
                    {
                        _eats[i].Increment();
                        _stocks[i].Decrement();
                    }
                }
            });
            GetButton((int)Buttons.ClearButton).onClick.AddListener(Clear);
            GetButton((int)Buttons.LevelUpButton).onClick.AddListener(_presenter.LevelUp);
        }

        internal void Show(AgentSaveData.Agent owned, UnityAction reShow)
        {
            _presenter.Show(owned, reShow);

            Clear();
        }

        public void Render(LevelupViewState state)
        {
            _originLevel.text = $"Lv. {state.originLevel}";
            _expText.text = state.expText;
            _expSlider.fillAmount = state.expPercent;

            _arrow.SetActive(state.showTarget);
            _targetLevel.gameObject.SetActive(state.showTarget);

            if (state.showTarget)
                _targetLevel.text = $"Lv. {state.targetLevel}";
        }

        public void PayFoods()
        {
            foreach (var eat in _eats)
            {
                eat.PayFood();
            }
        }

        private void Clear()
        {
            _presenter.Clear();

            for (int i = 0; i < _eats.Length; i++)
            {
                _eats[i].ResetItem();
                _stocks[i].ResetItem();
            }
        }
    }

    public struct LevelupViewState
    {
        public int originLevel;
        public int targetLevel;
        public string expText;
        public float expPercent;
        public bool showTarget;
    }
}