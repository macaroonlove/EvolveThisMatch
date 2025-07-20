using EvolveThisMatch.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Battle
{
    public class ActiveItemRangeRenderer : MonoBehaviour, IBattleSystem
    {
        private Transform _parent;
        private Transform _circle;

        private AllySystem _allySystem;
        private EnemySystem _enemySystem;

        private Camera _camera;
        private Plane _plane;

        private ERangeType _rangeType;
        private EUnitType _unitType;
        private float _radius;

        private bool _isShowRenderer;

        private UnityAction<List<Unit>> _action;

        public void Initialize()
        {
            _allySystem = BattleManager.Instance.GetSubSystem<AllySystem>();
            _enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();

            _camera = Camera.main;
            _plane = new Plane(Vector3.forward, 0.3f);

            _parent = transform.GetChild(0);
            _circle = _parent.GetChild(0);

            Hide();
        }

        public void Deinitialize()
        {
            _allySystem = null;
            _enemySystem = null;
        }

        /// <summary>
        /// 타입에 맞는 모든 유닛을 반환
        /// </summary>
        internal void Show_AllRange(EUnitType unitType, float delay, UnityAction<List<Unit>> action, UnityAction<Vector3> processAction)
        {
            _rangeType = ERangeType.All;
            _unitType = unitType;
            _action = action;

            var units = GetUnits_AllRange();

            if (units.Count > 0)
            {
                StartCoroutine(CoConfirm(delay, units));

                // TODO: 맵의 중앙 or 화면의 중앙에 있는 땅을 반환하도록 수정
                processAction?.Invoke(Vector3.zero);
            }
        }

        /// <summary>
        /// 타입에 맞는 모든 유닛을 반환
        /// </summary>
        internal void Show_CircleRange(EUnitType unitType, float radius, UnityAction<List<Unit>> action)
        {
            _rangeType = ERangeType.Circle;
            _unitType = unitType;
            _radius = radius;
            _action = action;

            _circle.localScale = radius * Vector3.one;
            _circle.gameObject.SetActive(true);

            _isShowRenderer = true;

            // TODO: 시간 조정?
        }

        /// <summary>
        /// 범위 확정하기
        /// </summary>
        internal void Confirm(float delay, UnityAction<Vector3> processAction)
        {
            var units = GetUnits_CircleRange(_parent.position);

            if (units.Count > 0)
            {
                StartCoroutine(CoConfirm(delay, units));

                processAction?.Invoke(_parent.position);
            }

            Hide();
        }

        private IEnumerator CoConfirm(float delay, List<Unit> units)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            _action?.Invoke(units);
        }

        private void Update()
        {
            if (_isShowRenderer == false) return;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (_plane.Raycast(ray, out float distance))
            {
                _parent.position = ray.GetPoint(distance);
            }
        }

        private void Hide()
        {
            _isShowRenderer = false;
            _circle.gameObject.SetActive(false);
        }

        #region 유닛 가져오기
        private List<Unit> GetUnits_AllRange()
        {
            List<Unit> units = new List<Unit>();

            if ((_unitType & (EUnitType.Agent | EUnitType.Summon)) != 0)
            {
                units.AddRange(_allySystem.GetAllAllies(_unitType));
            }

            if ((_unitType & EUnitType.Enemy) != 0)
            {
                units.AddRange(_enemySystem.GetAllEnemies());
            }

            return units;
        }

        private List<Unit> GetUnits_CircleRange(Vector3 pos)
        {
            List<Unit> units = new List<Unit>();

            if ((_unitType & (EUnitType.Agent | EUnitType.Summon)) != 0)
            {
                units.AddRange(_allySystem.GetAlliesInCircle(pos, _radius, _unitType));
            }

            if ((_unitType & EUnitType.Enemy) != 0)
            {
                units.AddRange(_enemySystem.GetEnemiesInCircle(pos, _radius));
            }

            return units;
        }
        #endregion
    }
}
