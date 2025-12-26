using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class FXAbility : AlwaysAbility
    {
        private void Awake()
        {
            InitializeShaderFX();
        }

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            InitializeParticleFX();
        }

        internal override void Deinitialize()
        {
            DeinitializeParticleFX();
        }

        internal override void UpdateAbility()
        {
            UpdateShaderFX();
        }

        private void OnDestroy()
        {
            DestroyParticleFX();
        }

        #region 파티클
        private PoolSystem _poolSystem;

        private List<FXEntry> _fxObjectList = new List<FXEntry>();
        private Dictionary<string, Coroutine> _activeCoroutines = new Dictionary<string, Coroutine>();

        private void InitializeParticleFX()
        {
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();

            unit.healthAbility.onDeath += DespawnAll;
        }

        private void DeinitializeParticleFX()
        {
            unit.healthAbility.onDeath -= DespawnAll;
        }

        private void DestroyParticleFX()
        {
            DespawnAll();

            _poolSystem = null;
        }

        internal void AddFX(GameObject fxObj, Follow follow, bool isDeathDespawn)
        {
            for (int i = 0; i < _fxObjectList.Count; i++)
            {
                if (_fxObjectList[i].fxObj == fxObj) return;
            }

            _fxObjectList.Add(new FXEntry { fxObj = fxObj, follow = follow, isDeathDespawn = isDeathDespawn });
        }

        internal void AddCoroutineFX(GameObject fxObj, Coroutine coroutine)
        {
            string fxName = fxObj.name;

            if (!_activeCoroutines.ContainsKey(fxName))
            {
                _activeCoroutines.Add(fxName, coroutine);
            }
        }

        internal void DespawnFX(GameObject fxObj)
        {
            string fxName = fxObj.name;

            // 파티클 제거
            for (int i = _fxObjectList.Count - 1; i >= 0; i--)
            {
                var entry = _fxObjectList[i];

                if (entry.fxObj == null)
                {
                    _fxObjectList.RemoveAt(i);
                    continue;
                }

                if (entry.fxObj != fxObj) continue;

                _poolSystem.DeSpawn(entry.fxObj);
                _fxObjectList.RemoveAt(i);
                break;
            }

            // 코루틴 중지
            if (_activeCoroutines.TryGetValue(fxName, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                _activeCoroutines.Remove(fxName);
            }
        }

        private void DespawnAll()
        {
            // 파티클 제거
            for (int i = _fxObjectList.Count - 1; i >= 0; i--)
            {
                var entry = _fxObjectList[i];
                var follow = entry.follow;
                if (follow != null) follow.enabled = false;

                if (!entry.isDeathDespawn) continue;

                var fx = entry.fxObj;
                if (fx == null || !fx.activeSelf) continue;

                _poolSystem.DeSpawn(fx);
            }
            _fxObjectList.Clear();

            // 코루틴 중지
            foreach (var coroutine in _activeCoroutines.Values)
            {
                StopCoroutine(coroutine);
            }
            _activeCoroutines.Clear();
        }

        public struct FXEntry
        {
            public GameObject fxObj;
            public Follow follow;
            public bool isDeathDespawn;
        }
        #endregion

        #region 셰이더
        private static readonly Dictionary<string, float> _defaultFX = new Dictionary<string, float>
        {
            { "_HologramFade", 0f },
            { "_FrozenFade", 0f },
            { "_CamouflageFade", 0f },
            { "_BurnFade", 0f },
            { "_PoisonFade", 0f },
            { "_EnchantedFade", 0f },
            { "_TextureLayer1Fade", 0f },
            { "_FullAlphaDissolveFade", 1f },
            { "_FullGlowDissolveFade", 1f },
            { "_FullDistortionFade", 1f },
            { "_WiggleFade", 0f },
        };

        private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
        private MaterialPropertyBlock _propertyBlock;
        private bool _isDirty;

        private void InitializeShaderFX()
        {
            _renderers.Clear();
            _renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());

            if (_propertyBlock == null) _propertyBlock = new MaterialPropertyBlock();

            ResetAllFX();
        }

        private void ResetAllFX()
        {
            _propertyBlock.Clear();

            foreach (var fx in _defaultFX) 
            {
                _propertyBlock.SetFloat(fx.Key, fx.Value);
            }

            ApplyPropertyBlock();
            _isDirty = false;
        }

        private void ApplyPropertyBlock()
        {
            for (int i = 0; i < _renderers.Count; i++)
            {
                var renderer = _renderers[i];
                if (renderer == null) continue;

                renderer.SetPropertyBlock(_propertyBlock);
            }
        }

        private void UpdateShaderFX()
        {
            if (!_isDirty) return;

            ApplyPropertyBlock();
            _isDirty = false;
        }

        public void SetShaderProperty(string propertyName, float value)
        {
            _propertyBlock.SetFloat(propertyName, value);
            _isDirty = true;
        }

        #region Fade
        public void FadeIn(string propertyName, float duration)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CoFade(propertyName, 0f, 1f, duration));
            }
        }

        public void FadeOut(string propertyName, float duration)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CoFade(propertyName, 1f, 0f, duration));
            }
        }

        public void Fade(string propertyName, float duration, float startValue, float endValue)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CoFade(propertyName, startValue, endValue, duration));
            }
        }

        private IEnumerator CoFade(string propertyName, float startValue, float endValue, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float currentValue = Mathf.Lerp(startValue, endValue, t);

                SetShaderProperty(propertyName, currentValue);
                yield return null;
            }

            SetShaderProperty(propertyName, endValue);
        }
        #endregion

        #endregion
    }
}