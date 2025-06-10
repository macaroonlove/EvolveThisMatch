using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class FXAbility : AlwaysAbility
    {
        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            InitializeParticleFX();
            InitializeShaderFX();
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

        private List<GameObject> _fxObjectList = new List<GameObject>();
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

        internal void AddFX(GameObject fxObj)
        {
            if (!_fxObjectList.Contains(fxObj))
            {
                _fxObjectList.Add(fxObj);
            }
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
                if (_fxObjectList[i] == null || _fxObjectList[i].activeSelf == false)
                {
                    _fxObjectList.RemoveAt(i);
                    continue;
                }

                if (_fxObjectList[i].name == fxName)
                {
                    _poolSystem.DeSpawn(_fxObjectList[i]);
                    _fxObjectList.RemoveAt(i);
                }                
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
                if (_fxObjectList[i] == null || _fxObjectList[i].activeSelf == false)
                {
                    continue;
                }

                _poolSystem.DeSpawn(_fxObjectList[i]);
            }
            _fxObjectList.Clear();

            // 코루틴 중지
            foreach (var coroutine in _activeCoroutines.Values)
            {
                StopCoroutine(coroutine);
            }
            _activeCoroutines.Clear();
        }
        #endregion

        #region 셰이더
        #region Mesh Renderer
        //private MeshRenderer _renderer;
        //private MaterialPropertyBlock _propertyBlock;
        //private bool _isDirty;

        //private void InitializeShaderFX()
        //{
        //    _renderer = GetComponentInChildren<MeshRenderer>();
        //    _propertyBlock = new MaterialPropertyBlock();

        //    _renderer?.SetPropertyBlock(_propertyBlock);
        //}

        //private void UpdateShaderFX()
        //{
        //    if (_isDirty)
        //    {
        //        _renderer?.SetPropertyBlock(_propertyBlock);
        //        _isDirty = false;
        //    }
        //}

        //public void SetShaderKeyword(string keywordName, bool isOn)
        //{
        //    if (isOn)
        //    {
        //        _renderer.material.EnableKeyword(keywordName);
        //    }
        //    else
        //    {
        //        _renderer.material.DisableKeyword(keywordName);
        //    }
        //    _isDirty = true;
        //}
        #endregion

        #region Sprite Renderer (SPUM 기준)
        private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
        private MaterialPropertyBlock _propertyBlock;
        private bool _isDirty;

        private void InitializeShaderFX()
        {
            _renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
            _propertyBlock = new MaterialPropertyBlock();

            foreach (var renderer in _renderers)
            {
                renderer?.material?.SetTexture("_MainTex", renderer?.sprite?.texture);
                renderer?.SetPropertyBlock(_propertyBlock);
            }
        }

        private void UpdateShaderFX()
        {
            if (_isDirty)
            {
                foreach (var renderer in _renderers)
                {
                    renderer?.material?.SetTexture("_MainTex", renderer?.sprite?.texture);
                    renderer?.SetPropertyBlock(_propertyBlock);
                }
                _isDirty = false;
            }
        }

        public void SetShaderKeyword(string keywordName, bool isOn)
        {
            if (isOn)
            {
                foreach (var renderer in _renderers)
                {
                    renderer?.material?.EnableKeyword(keywordName);
                }
            }
            else
            {
                foreach (var renderer in _renderers)
                {
                    renderer?.material?.DisableKeyword(keywordName);
                }
            }
            _isDirty = true;
        }
        #endregion

        public void SetShaderProperty(string propertyName, float value)
        {
            _propertyBlock.SetFloat(propertyName, value);
            _isDirty = true;
        }

        #region Fade
        public void FadeIn(string propertyName, float duration)
        {
            StartCoroutine(CoFade(propertyName, 0f, 1f, duration));
        }

        public void FadeOut(string propertyName, float duration)
        {
            StartCoroutine(CoFade(propertyName, 1f, 0f, duration));
        }

        public void Fade(string propertyName, float duration, float startValue, float endValue)
        {
            StartCoroutine(CoFade(propertyName, startValue, endValue, duration));
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