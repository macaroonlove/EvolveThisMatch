using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UITomeExecuteButton : UIBase, IPointerDownHandler, IPointerMoveHandler
    {
        #region 바인딩
        enum Images
        {
            Icon,
            CooldownTimeImage,
        }
        enum Texts
        {
            NeedCoin,
            CooldownTimeText,
        }
        #endregion

        private enum EActionType
        {
            Item_1,
            Item_2,
            Item_3,
        }

        [SerializeField] private EActionType _actionType;

        private Image _cooldownTimeImage;

        private TextMeshProUGUI _needCoinText;
        private TextMeshProUGUI _coolDownTimeText;

        private Color _lackCostColor = new Color(1, 0.27f, 0);

        private InputSystem _inputSystem;
        private CoinSystem _coinSystem;
        private ActiveItemRangeRenderer _rangeRenderer;
        private TomeTemplate _template;
        private ProfileSaveData.Tome _owned;

        private int _currentCoin;
        private float _inverseMaxCoolDownTime;
        private float _currentCoolDownTime;

        private bool _isActiveRangeRenderer;
        private bool _isInteractable;
        private float _threshold = 0.1f;
        private bool _isPointerDown;
        private Vector2 _cachedPointerDownPosition;
        private Vector3 _cachedLastRendererPosition;

        #region 프로퍼티
        private bool IsInteractable
        {
            get
            {
                if (_isInteractable == false) return false;

                return true;
            }
        }

        private int finalNeedCoin
        {
            get
            {
                int result = _template.needCoin;

                return result;
            }
        }

        private float finalCoolDownTime
        {
            get
            {
                float result = _template.cooldownTime;

                return result;
            }
        }
        #endregion

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _cooldownTimeImage = GetImage((int)Images.CooldownTimeImage);
            _needCoinText = GetText((int)Texts.NeedCoin);
            _coolDownTimeText = GetText((int)Texts.CooldownTimeText);

            _cooldownTimeImage.gameObject.SetActive(false);

            _threshold *= _threshold;
        }

        internal void GetTome()
        {
            var template = CoreManager.Instance.GetSubSystem<TomeSystem>().GetSelectedItem((int)_actionType);
            if (template != null)
            {
                Show(template);
            }
            else
            {
                base.Hide(true);
            }
        }

        private void Show(TomeTemplate template)
        {
            GetImage((int)Images.Icon).sprite = template.sprite;

            var ownedTomes = GameDataManager.Instance.profileSaveData.ownedTomes;

            _template = template;
            _owned = ownedTomes.Find(t => t.id == template.id);

            _needCoinText.text = $"<sprite name=\"Coin\"> {finalNeedCoin}";

            _rangeRenderer = BattleManager.Instance.GetSubSystem<ActiveItemRangeRenderer>();
            _inputSystem = BattleManager.Instance.GetSubSystem<InputSystem>();
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();
            _coinSystem.onChangedCoin += OnChangeCoin;
            _currentCoin = _coinSystem.currentCoin;

            CalcMaxCoolDownTime();
            _currentCoolDownTime = 0;

            CheckInteractable();

            InputBinding();

            base.Show(true);
        }

        internal void Hide()
        {
            base.Hide(true);

            _template = null;
            _rangeRenderer = null;

            _coinSystem.onChangedCoin -= OnChangeCoin;
            _coinSystem = null;

            InputCancelBinding();
            _inputSystem = null;
        }

        #region Input Binding
        private void InputBinding()
        {
            if (_template.unitType == EUnitType.None)
            {
                switch (_actionType)
                {
                    case EActionType.Item_1:
                        _inputSystem.onItem_1 += ShortcutNoneRenderer;
                        break;
                    case EActionType.Item_2:
                        _inputSystem.onItem_2 += ShortcutNoneRenderer;
                        break;
                    case EActionType.Item_3:
                        _inputSystem.onItem_3 += ShortcutNoneRenderer;
                        break;
                }

                return;
            }

            switch (_template.rangeType)
            {
                case ETomeRangeType.All:
                    switch (_actionType)
                    {
                        case EActionType.Item_1:
                            _inputSystem.onItem_1 += ShortcutAllRenderer;
                            break;
                        case EActionType.Item_2:
                            _inputSystem.onItem_2 += ShortcutAllRenderer;
                            break;
                        case EActionType.Item_3:
                            _inputSystem.onItem_3 += ShortcutAllRenderer;
                            break;
                    }
                    break;
                case ETomeRangeType.Circle:
                    switch (_actionType)
                    {
                        case EActionType.Item_1:
                            _inputSystem.onItem_1 += ShortcutCircleRenderer;
                            break;
                        case EActionType.Item_2:
                            _inputSystem.onItem_2 += ShortcutCircleRenderer;
                            break;
                        case EActionType.Item_3:
                            _inputSystem.onItem_3 += ShortcutCircleRenderer;
                            break;
                    }
                    break;
            }
        }

        private void InputCancelBinding()
        {
            if (_template.unitType == EUnitType.None)
            {
                switch (_actionType)
                {
                    case EActionType.Item_1:
                        _inputSystem.onItem_1 -= ShortcutNoneRenderer;
                        break;
                    case EActionType.Item_2:
                        _inputSystem.onItem_2 -= ShortcutNoneRenderer;
                        break;
                    case EActionType.Item_3:
                        _inputSystem.onItem_3 -= ShortcutNoneRenderer;
                        break;
                }

                return;
            }

            switch (_template.rangeType)
            {
                case ETomeRangeType.All:
                    switch (_actionType)
                    {
                        case EActionType.Item_1:
                            _inputSystem.onItem_1 -= ShortcutAllRenderer;
                            break;
                        case EActionType.Item_2:
                            _inputSystem.onItem_2 -= ShortcutAllRenderer;
                            break;
                        case EActionType.Item_3:
                            _inputSystem.onItem_3 -= ShortcutAllRenderer;
                            break;
                    }
                    break;
                case ETomeRangeType.Circle:
                    switch (_actionType)
                    {
                        case EActionType.Item_1:
                            _inputSystem.onItem_1 -= ShortcutCircleRenderer;
                            break;
                        case EActionType.Item_2:
                            _inputSystem.onItem_2 -= ShortcutCircleRenderer;
                            break;
                        case EActionType.Item_3:
                            _inputSystem.onItem_3 -= ShortcutCircleRenderer;
                            break;
                    }
                    break;
            }
        }

        private void ShortcutNoneRenderer()
        {
            InitializeNoneRenderer();
        }

        private void ShortcutAllRenderer()
        {
            _isPointerDown = true;
            InitializeAllRenderer();
        }

        private void ShortcutCircleRenderer()
        {
            _isPointerDown = true;
            _isActiveRangeRenderer = true;
            InitializeCircleRenderer();
        }
        #endregion

        #region 아이템 사용 조건 로직
        private void OnChangeCoin(int coin)
        {
            _currentCoin = coin;
            CheckInteractable();
        }

        private void CalcMaxCoolDownTime()
        {
            if (finalCoolDownTime == 0)
            {
                _inverseMaxCoolDownTime = 0;
            }
            else
            {
                _inverseMaxCoolDownTime = 1 / finalCoolDownTime;
            }
        }

        private void UpdateCoolDownTime()
        {
            if (_currentCoolDownTime == 0) return;

            _currentCoolDownTime -= Time.deltaTime;

            if (_currentCoolDownTime < 0)
            {
                _currentCoolDownTime = 0;
                _cooldownTimeImage.gameObject.SetActive(false);
                CheckInteractable();
            }
            else
            {
                _coolDownTimeText.text = _currentCoolDownTime.ToString("F1");
                _cooldownTimeImage.fillAmount = _currentCoolDownTime * _inverseMaxCoolDownTime;
            }
        }

        /// <summary>
        /// 해당 아이템이 사용 가능한지 여부 체크
        /// </summary>
        private void CheckInteractable()
        {
            bool isInteractable = true;
            if (_currentCoolDownTime > 0)
            {
                isInteractable = false;
                _isInteractable = false;
                _cooldownTimeImage.gameObject.SetActive(true);
                ResetPointer();
            }
            if (_currentCoin < finalNeedCoin)
            {
                isInteractable = false;
                _isInteractable = false;
                _needCoinText.color = _lackCostColor;
                ResetPointer();
            }

            if (isInteractable == true)
            {
                _isInteractable = true;
                _cooldownTimeImage.gameObject.SetActive(false);
                _needCoinText.color = Color.white;
            }
        }
        #endregion

        private void Update()
        {
            UpdateCoolDownTime();

            CheckPointerUp();
        }

        #region 포인터(마우스 및 터치) 관리
        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsInteractable == false) return;
            if (_isPointerDown == true) return;

            _isPointerDown = true;
            _cachedPointerDownPosition = eventData.position;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (IsInteractable == false) return;
            if (_isPointerDown == false) return;
            if (_isActiveRangeRenderer == true) return;
            if (_template.rangeType == ETomeRangeType.All) return;

            float distSquare = (_cachedPointerDownPosition - eventData.position).sqrMagnitude;

            if (distSquare > _threshold)
            {
                switch (_template.rangeType)
                {
                    case ETomeRangeType.Circle:
                        InitializeCircleRenderer();
                        break;
                }
            }
        }

        private void CheckPointerUp()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0))
            {
                OnPointerUp();
            }
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                OnPointerUp();
            }
#endif
        }

        public void OnPointerUp()
        {
            if (IsInteractable == false) return;
            if (_isPointerDown == false) return;

            if (_template.unitType == EUnitType.None)
            {
                InitializeNoneRenderer();
            }
            else if (_isActiveRangeRenderer)
            {
                _rangeRenderer.Confirm(_template.delay, ExecuteItem);
            }
            else
            {
                switch (_template.rangeType)
                {
                    case ETomeRangeType.All:
                        InitializeAllRenderer();
                        break;
                    case ETomeRangeType.Circle:
                        InitializeCircleRenderer();
                        return;
                }
            }

            ResetPointer();
        }

        private void ResetPointer()
        {
            _isPointerDown = false;
            _cachedPointerDownPosition = Vector2.zero;
            _isActiveRangeRenderer = false;
        }
        #endregion

        #region 렌더러 초기화
        #region None Renderer
        private void InitializeNoneRenderer()
        {
            // TODO: 맵의 중앙 or 화면의 중앙에 있는 땅을 반환하도록 수정
            ExecuteItem(Vector3.zero);

            if (_template.delay > 0)
            {
                StartCoroutine(CoConfirmNoneRenderer(_template.delay));
            }
            else
            {
                ConfirmNoneRenderer();
            }
        }

        private IEnumerator CoConfirmNoneRenderer(float delay)
        {
            yield return new WaitForSeconds(delay);
            ConfirmNoneRenderer();
        }

        private void ConfirmNoneRenderer()
        {
            ExecuteEffect();

            ExecuteAfterDelayFX();
        }
        #endregion

        #region All Renderer
        private void InitializeAllRenderer()
        {
            _rangeRenderer.Show_AllRange(_template.unitType, _template.delay, (units) =>
            {
                if (units.Count > 0)
                {
                    ExecuteEffect(units);

                    ExecuteAfterDelayFX();
                }
                else
                {
                    // TODO: 대상 검출에 실패했다는 UI를 보여주거나 실패 사운드 들려주기
                }
            }, ExecuteItem);
        }
        #endregion

        #region Circle Renderer
        private void InitializeCircleRenderer()
        {
            _rangeRenderer.Show_CircleRange(_template.unitType, _template.range, (units) =>
            {
                if (units.Count > 0)
                {
                    ExecuteEffect(units);

                    ExecuteAfterDelayFX();
                }
                else
                {
                    // TODO: 대상 검출에 실패했다는 UI를 보여주거나 실패 사운드 들려주기
                }
            });
            _isActiveRangeRenderer = true;
        }
        #endregion
        #endregion

        private void ExecuteItem(Vector3 pos)
        {
            // FX 적용
            ExecuteItemFX(pos);

            // 쿨타임 적용
            _currentCoolDownTime = finalCoolDownTime;
            CalcMaxCoolDownTime();

            // 코스트 지불
            _coinSystem.PayCoin(finalNeedCoin);

            CheckInteractable();
        }

        #region 효과 수행
        private void ExecuteEffect()
        {
            foreach (var effect in _template.effects)
            {
                if (effect is GlobalEffect globalEffect)
                {
                    globalEffect.Execute(_owned.level);
                }
            }
        }

        private void ExecuteEffect(List<Unit> units)
        {
            foreach (var effect in _template.effects)
            {
                if (effect is TomeEffect activeItemEffect)
                {
                    activeItemEffect.Execute(units, _owned.level);
                }
            }
        }
        #endregion

        #region FX
        private void ExecuteItemFX(Vector3 pos)
        {
            if (_template.itemFX != null)
            {
                _template.itemFX.Play(pos);
            }

            _cachedLastRendererPosition = pos;
        }

        private void ExecuteAfterDelayFX()
        {
            if (_template.delay <= 0) return;

            if (_template.afterDelayFX != null)
            {
                _template.afterDelayFX.Play(_cachedLastRendererPosition);
            }
        }
        #endregion
    }
}