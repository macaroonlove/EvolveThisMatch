using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Unit/Skin", fileName = "Skin_Battle_", order = 1)]
    public class SkinBattleTemplate : ScriptableObject
    {
        [Header("프리팹")]
        [SerializeField, Label("유닛")] private GameObject _prefab;

        [Header("보이스")]
        [SerializeField, Label("소환 시")] private AudioClip _spawnVoice;
        [SerializeField, Label("사망 시")] private AudioClip _deathVoice;

        [Header("기본 공격시 FX")]
        [SerializeField, Label("시전자")] private FX _casterFX;
        [SerializeField, Label("대상자")] private FX _targetFX;

        #region 프로퍼티
        public GameObject prefab => _prefab;

        public AudioClip spawnVoice => _spawnVoice;
        public AudioClip deathVoice => _deathVoice;

        public FX casterFX => _casterFX;
        public FX targetFX => _targetFX;
        #endregion
    }
}