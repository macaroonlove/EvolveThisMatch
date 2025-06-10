using FrameWork;
using FrameWork.VisualNovel;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EvolveThisMatch.Tutorial
{
    /// <summary>
    /// Tutorial 씬을 관리하는 매니져
    /// </summary>
    public class TutorialSceneManager : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<ChapterTemplate> _chapterTemplate;

        private void Start()
        {
            AddressableAssetManager.Instance.GetScriptableObject<ChapterTemplate>(_chapterTemplate.RuntimeKey.ToString(), (template) =>
            {
                VisualNovelManager.Instance.Load(template);
            });
        }
    }
}