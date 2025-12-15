using FrameWork.Editor;
using System.Collections;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class BarricadeController : MonoBehaviour
    {
        [SerializeField, Label("Y축 기준 바리케이드 범위")] private Vector2 _hitRange;

        private Material _material;
        private Coroutine _coroutine;

        private static readonly int _shaderID = Shader.PropertyToID("_AddColorFade");

        private void Awake()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            _material = spriteRenderer.material;
        }

        internal bool ContainsRange(float y)
        {
            return y >= _hitRange.x && y <= _hitRange.y;
        }

        internal void Regist(Unit unit)
        {
            unit.GetAbility<MoveBoundaryAbility>().onBarricadeAttack += OnBarricadeAttack;
        }

        internal void Deregist(Unit unit)
        {
            unit.GetAbility<MoveBoundaryAbility>().onBarricadeAttack -= OnBarricadeAttack;
        }

        private void OnBarricadeAttack()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(CoHit());
        }

        private IEnumerator CoHit()
        {
            float elapsedTime = 0f;
            float duration = 0.1f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float currentValue = Mathf.Lerp(0, 0.1f, t);

                _material.SetFloat(_shaderID, currentValue);
                yield return null;
            }

            _material.SetFloat(_shaderID, 0.1f);

            yield return new WaitForSeconds(0.2f);

            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float currentValue = Mathf.Lerp(0.1f, 0f, t);

                _material.SetFloat(_shaderID, currentValue);
                yield return null;
            }

            _material.SetFloat(_shaderID, 0f);
            _coroutine = null;
        }
    }
}