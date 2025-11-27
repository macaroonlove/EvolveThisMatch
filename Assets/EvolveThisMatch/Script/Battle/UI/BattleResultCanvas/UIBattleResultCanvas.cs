using DG.Tweening;
using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UIBattleResultCanvas : UIBase
    {
        #region 바인딩
        enum Images
        {
            Icon,
        }
        enum Buttons
        {
            Background,
        }
        #endregion

        private Button _background;
        private Image _icon;

        private UIBattleResultWindow _battleResultWindow;
        private BattleResultSystem _battleResultSystem;

        private Sprite _victoryIcon;
        private Sprite _defeatIcon;

        protected override void Initialize()
        {
            _battleResultWindow = GetComponentInChildren<UIBattleResultWindow>();
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));

            _background = GetButton((int)Buttons.Background);
            _icon = GetImage((int)Images.Icon);

            _battleResultSystem = BattleManager.Instance.GetSubSystem<BattleResultSystem>();

            _battleResultSystem.onBattleEnd += BattleEnd;
            _background.onClick.AddListener(ResultWindow);
            _background.interactable = false;

            AddressableAssetManager.Instance.GetSprite("VictoryIcon", (sprite) => { _victoryIcon = sprite; });
            AddressableAssetManager.Instance.GetSprite("DefeatIcon", (sprite) => { _defeatIcon = sprite; });
        }

        private void OnDestroy()
        {
            Deinitialize();
        }

        private void Deinitialize()
        {
            _battleResultSystem.onBattleEnd -= BattleEnd;
        }

        [ContextMenu("승리")]
        private void TestVictoryAnimation()
        {
            BattleEnd(true);
        }

        [ContextMenu("패배")]
        private void TestDefeatAnimation()
        {
            BattleEnd(false);
        }

        private void BattleEnd(bool battleResult)
        {
            _icon.sprite = battleResult ? _victoryIcon : _defeatIcon;
            base.Show();
            Deinitialize();
            ResultAnimation();
        }

        private void ResultAnimation()
        {
            _icon.rectTransform.anchoredPosition = new Vector2(0, 3000);
            _icon.transform.localScale = Vector3.one * 400f;

            var seq = DOTween.Sequence();

            seq.AppendInterval(2)
                // 1차 깊이 박히기
                .Append(_icon.rectTransform.DOAnchorPosY(0, 0.45f).SetEase(Ease.OutExpo))
                .Join(_icon.transform.DOScale(0.85f, 0.45f).SetEase(Ease.OutExpo))
                // 2차 다시 올라오기
                .Append(_icon.transform.DOScale(1f, 0.08f).SetEase(Ease.InOutSine))
               .OnComplete(() =>
               {
                   _background.interactable = true;
               });
        }

        private void ResultWindow()
        {
            transform.GetChild(1).DOLocalMoveX(-500, 0.2f).OnComplete(() =>
            {
                _battleResultWindow.Show(_battleResultSystem.battleResultData);
            });
        }
    }
}