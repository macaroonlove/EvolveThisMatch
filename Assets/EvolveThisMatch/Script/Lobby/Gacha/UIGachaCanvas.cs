using FrameWork.UI;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using ScriptableObjectArchitecture;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaCanvas : UIBase
    {
        #region 바인딩
        enum Images
        {
            Background,
        }
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        [Header("탭")]
        [SerializeField] private GameObject _tabPrefab;
        [SerializeField] private Transform _tabParent;

        [Header("버튼")]
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private RectTransform _buttonParent;

        [Header("가챠 데이터")]
        [SerializeField] private GachaDataTemplate _gachaDataTemplate;

        private List<UIGachaButton> _gachaButtons = new List<UIGachaButton>();
        private Image _background;
        private UIGachaResultCanvas _gachaResultCanvas;

        private UIGachaTab _firstTab;
        private UIGachaTab _currentTab;

        protected override void Initialize()
        {
            _gachaResultCanvas = GetComponentInChildren<UIGachaResultCanvas>();

            BindImage(typeof(Images));
            BindButton(typeof(Buttons));

            _background = GetImage((int)Images.Background);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            SetTab();
        }

        public override void Show(bool isForce = false)
        {
            base.Show(isForce);

            Select(_firstTab);
        }

        private void SetTab()
        {
            if (_gachaDataTemplate == null) return;

            var datas = _gachaDataTemplate.gachaDatas;

            bool isSelected = false;
            foreach (var data in datas)
            {
                var instance = Instantiate(_tabPrefab, _tabParent);
                var gachaTab = instance.GetComponent<UIGachaTab>();
                data.Initialize(_gachaResultCanvas);
                gachaTab.Initialize(data, Select);

                if (isSelected == false)
                {
                    _firstTab = gachaTab;
                    isSelected = true;
                }
            }
        }

        private void SetButtons(UIGachaTab tab)
        {
            var costs = tab.gachaData.costs;
            var infos = tab.gachaData.gachaButtons;
            int count = infos.Count;

            // 필요한 수만큼 생성
            while (_gachaButtons.Count < count)
            {
                var instance = Instantiate(_buttonPrefab, _buttonParent);
                var gachaButton = instance.GetComponent<UIGachaButton>();
                _gachaButtons.Add(gachaButton);
            }

            // 초기화
            for (int i = 0; i < _gachaButtons.Count; i++)
            {
                if (i < count)
                {
                    var info = infos[i];
                    _gachaButtons[i].Show(info.gachaCount, costs, info.color, PickUp);
                }
                else
                {
                    _gachaButtons[i].Hide();
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_buttonParent);
        }

        private void Select(UIGachaTab tab)
        {
            _currentTab?.UnSelect();
            _currentTab = tab;
            _currentTab?.Select();

            _background.sprite = tab.gachaData.background;
            SetButtons(tab);

            VariableDisplayManager.Instance.HideAll();
            
            foreach (var item in tab.gachaData.costs)
            {
                VariableDisplayManager.Instance.Show(item.variable);
            }
        }

        private void PickUp(int gachaCount, IReadOnlyList<GachaCost> costs)
        {
            Dictionary<ObscuredIntVariable, int> payCost = new Dictionary<ObscuredIntVariable, int>();

            int remainCount = gachaCount;
            foreach (var cost in costs)
            {
                if (remainCount <= 0) break;

                // 최대 소환할 수 있는 개수 구하기
                int maxPickUpCount = cost.variable.Value / cost.cost;

                // 소환 불가능
                if (maxPickUpCount <= 0) continue;

                // 해당 버튼의 최대 소환량을 넘지 않도록 제한
                int pickUpCount = Mathf.Min(remainCount, maxPickUpCount);

                // 사용 처리
                int useAmount = pickUpCount * cost.cost;
                remainCount -= pickUpCount;

                payCost[cost.variable] = useAmount;
            }

            if (remainCount > 0) return;

            foreach (var pc in payCost)
            {
                pc.Key.AddValue(-pc.Value);
            }

            _currentTab.gachaData.PickUp(gachaCount);

            SetButtons(_currentTab);
        }

        public void Hide()
        {
            VariableDisplayManager.Instance.HideAll();

            base.Hide(true);
        }
    }
}