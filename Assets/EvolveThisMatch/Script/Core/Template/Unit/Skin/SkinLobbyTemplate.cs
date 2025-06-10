using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Unit/Skin", fileName = "Skin_Lobby_", order = 0)]
    public class SkinLobbyTemplate : ScriptableObject
    {
        [Header("스프라이트")]
        [SerializeField, Label("전체 일러스트")] private Sprite _fullBodySprite;

        [Header("보이스")]
        [SerializeField, Label("캐릭터 목록")] private AudioClip _infoVoice;
        [SerializeField, Label("획득")] private AudioClip _getVoice;
        [SerializeField, Label("목소리 1")] private AudioClip _talk1Voice;
        [SerializeField, Label("목소리 2")] private AudioClip _talk2Voice;
        [SerializeField, Label("목소리 3")] private AudioClip _talk3Voice;

        #region 프로퍼티
        public Sprite fullBodySprite => _fullBodySprite;

        public AudioClip infoVoice => _infoVoice;
        public AudioClip getVoice => _getVoice;
        public AudioClip talk1Voice => _talk1Voice;
        public AudioClip talk2Voice => _talk2Voice;
        public AudioClip talk3Voice => _talk3Voice;
        #endregion
    }
}