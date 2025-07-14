using FrameWork;
using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EvolveThisMatch.Core
{
    public class UIBattleResultCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
            LSpear,
            RSpear,
            Icon,
            ResultTitle,
            ResultDesc,
        }
        #endregion

        private Button _button;
        private Image _background;
        private Image _lSpear;
        private Image _rSpear;
        private Image _icon;
        private Image _resultTitle;
        private Image _resultDesc;

        private UIBattleResultWindow _battleResultWindow;
        private VictorySystem _victorySystem;
        private DefeatSystem _defeatSystem;

        private Sprite _victoryBG;
        private Sprite _victoryIcon;
        private Sprite _victoryTitle;
        private Sprite _victoryDesc;
        private Sprite _defeatBG;
        private Sprite _defeatIcon;
        private Sprite _defeatTitle;
        private Sprite _defeatDesc;

        private Color _victoryColor;
        private Color _defeatColor;

        protected override void Initialize()
        {
            _battleResultWindow = GetComponentInChildren<UIBattleResultWindow>();
            BindImage(typeof(Images));

            _background = GetImage((int)Images.Background);
            _lSpear = GetImage((int)Images.LSpear);
            _rSpear = GetImage((int)Images.RSpear);
            _icon = GetImage((int)Images.Icon);
            _resultTitle = GetImage((int)Images.ResultTitle);
            _resultDesc = GetImage((int)Images.ResultDesc);
            _button = _background.GetComponent<Button>();
            _button.enabled = false;

            _victorySystem = BattleManager.Instance.GetSubSystem<VictorySystem>();
            _defeatSystem = BattleManager.Instance.GetSubSystem<DefeatSystem>();

            _victorySystem.onVictory += Victory;
            _defeatSystem.onDefeat += Defeat;
            _button.onClick.AddListener(ResultWindow);

            AddressableAssetManager.Instance.GetSprite("VictoryBG", (sprite) => { _victoryBG = sprite; });
            AddressableAssetManager.Instance.GetSprite("VictoryIcon", (sprite) => { _victoryIcon = sprite; });
            AddressableAssetManager.Instance.GetSprite("VictoryTitle", (sprite) => { _victoryTitle = sprite; });
            AddressableAssetManager.Instance.GetSprite("VictoryDesc", (sprite) => { _victoryDesc = sprite; });
            AddressableAssetManager.Instance.GetSprite("DefeatBG", (sprite) => { _defeatBG = sprite; });
            AddressableAssetManager.Instance.GetSprite("DefeatIcon", (sprite) => { _defeatIcon = sprite; });
            AddressableAssetManager.Instance.GetSprite("DefeatTitle", (sprite) => { _defeatTitle = sprite; });
            AddressableAssetManager.Instance.GetSprite("DefeatDesc", (sprite) => { _defeatDesc = sprite; });

            ColorUtility.TryParseHtmlString("#418FDE", out _victoryColor);
            ColorUtility.TryParseHtmlString("#DE414A", out _defeatColor);
        }

        private void OnDestroy()
        {
            Deinitialize();
        }

        private void Deinitialize()
        {
            _victorySystem.onVictory -= Victory;
            _defeatSystem.onDefeat -= Defeat;
        }

        private void Victory()
        {
            _background.sprite = _victoryBG;
            _icon.sprite = _victoryIcon;
            _resultTitle.sprite = _victoryTitle;
            _resultDesc.sprite = _victoryDesc;
            _lSpear.color = _victoryColor;
            _rSpear.color = _victoryColor;

            base.Show();
            Deinitialize();
            ResultAnimation();
        }

        private void Defeat()
        {
            _background.sprite = _defeatBG;
            _icon.sprite = _defeatIcon;
            _resultTitle.sprite = _defeatTitle;
            _resultDesc.sprite = _defeatDesc;
            _lSpear.color = _defeatColor;
            _rSpear.color = _defeatColor;

            base.Show();
            Deinitialize();
            ResultAnimation();
        }

        private void ResultAnimation()
        {
            _icon.transform.localScale = Vector3.zero;
            _resultTitle.DOFade(0, 0);
            _resultDesc.DOFade(0, 0);

            _icon.transform.DOScale(1, 0.4f).OnComplete(() => 
            {
                _lSpear.transform.DOLocalMove(new Vector2(0, 50), 0.5f);
                _rSpear.transform.DOLocalMove(new Vector2(0, 50), 0.5f).OnComplete(() =>
                {
                    _resultTitle.DOFade(1, 0.1f);
                    _resultDesc.DOFade(1, 0.1f);

                    _button.enabled = true;
                });
            });
        }

        private void ResultWindow()
        {
            transform.GetChild(1).DOLocalMoveX(-500, 0.2f).OnComplete(() =>
            {
                _battleResultWindow.Show();
            });
        }
    }
}