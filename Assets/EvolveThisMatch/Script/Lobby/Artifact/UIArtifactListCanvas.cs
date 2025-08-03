using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIArtifactListCanvas : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Content,
        }
        #endregion

        protected Transform _parent;
        protected List<UIArtifactListItem> _artifactListItems;
        protected List<ArtifactTemplate> _artifactTemplates;

        protected UnityAction<ArtifactTemplate, ProfileSaveData.Artifact> _action;

        internal virtual void Initialize(UnityAction<ArtifactTemplate, ProfileSaveData.Artifact> action = null)
        {
            _action = action;

            BindObject(typeof(Objects));

            _parent = GetObject((int)Objects.Content).transform;
        }

        protected void Start()
        {
            InitializeArtifactListItem();

            _artifactListItems[0].SelectItem();
        }

        #region 리스트 아이템 생성
        private void InitializeArtifactListItem()
        {
            _artifactTemplates = GameDataManager.Instance.artifactTemplates.ToList();
            int count = _artifactTemplates.Count;

            _artifactListItems = new List<UIArtifactListItem>(count);

            var artifactListItem = GetComponentInChildren<UIArtifactListItem>();

            // 나머지 프리팹 인스턴스 생성
            for (int i = 0; i < count; i++)
            {
                var item = Instantiate(artifactListItem.gameObject, _parent).GetComponent<UIArtifactListItem>();
                item.Initialize(ChangeArtifact);
                _artifactListItems.Add(item);
            }

            Destroy(artifactListItem.gameObject);

            RegistArtifactListItem();
        }

        private void ChangeArtifact(ArtifactTemplate template, ProfileSaveData.Artifact owned)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _artifactListItems) item.DeSelectItem();

            _action?.Invoke(template, owned);
        }

        internal void RegistArtifactListItem()
        {
            var ownedArtifacts = GameDataManager.Instance.profileSaveData.ownedArtifacts;
            int count = _artifactTemplates.Count;

            // 보유한 아티팩트의 아이디
            var ownedArtifactDic = ownedArtifacts.ToDictionary(a => a.id);

            for (int i = 0; i < count; i++)
            {
                var template = _artifactTemplates[i];

                if (ownedArtifactDic.TryGetValue(template.id, out var owned))
                {
                    // 보유한 아티팩트
                    _artifactListItems[i].Show(template, owned);
                }
                else
                {
                    // 미보유 아티팩트
                    _artifactListItems[i].Hide();
                }
            }
        }
        #endregion
    }
}
