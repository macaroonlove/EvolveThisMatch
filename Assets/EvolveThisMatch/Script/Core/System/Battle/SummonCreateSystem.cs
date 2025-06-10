using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class SummonCreateSystem : MonoBehaviour, IBattleSystem
    {
        private AllySystem _allySystem;
        private PoolSystem _poolSystem;

        public void Initialize()
        {
            _allySystem = BattleManager.Instance.GetSubSystem<AllySystem>();
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
        }

        public void Deinitialize()
        {

        }

        internal bool CreateUnit(SummonTemplate template, Vector3 pos, float duration = float.MaxValue, Unit summoner = null)
        {
            GameObject obj;

            if (duration == float.MaxValue)
            {
                obj = _poolSystem.Spawn(template.prefab, transform);
            }
            else
            {
                obj = _poolSystem.Spawn(template.prefab, duration, transform);
            }

            obj.transform.SetPositionAndRotation(pos, Quaternion.identity);

            if (obj.TryGetComponent(out SummonUnit unit))
            {
                unit.Initialize(template, summoner);

                _allySystem.Regist(unit);
            }
            else
            {
                _poolSystem.DeSpawn(obj);
                return false;
            }

            return true;
        }
    }
}