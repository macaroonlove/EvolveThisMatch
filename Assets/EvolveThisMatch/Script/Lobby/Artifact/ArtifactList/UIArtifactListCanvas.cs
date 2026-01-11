using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.Editor;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
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

        [Header("이벤트")]
        [SerializeField, Label("아티팩트 데이터 변경 시")] protected GameEvent _artifactDataChangedGameEvent;

        protected Transform _parent;
        protected List<UIArtifactListItem> _artifactListItems;
        protected List<ArtifactTemplate> _artifactTemplates;

        public event UnityAction<ArtifactTemplate, ItemSaveData.Artifact> onSelected;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));
            _parent = GetObject((int)Objects.Content).transform;

            _artifactDataChangedGameEvent.AddListener(RefreshArtifactListItem);
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

            RefreshArtifactListItem();
        }

        private void ChangeArtifact(ArtifactTemplate template, ItemSaveData.Artifact owned)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _artifactListItems) item.DeSelectItem();

            onSelected?.Invoke(template, owned);
        }
        #endregion

        #region 리스트 갱신
        private void RefreshArtifactListItem()
        {
            var ownedArtifacts = SaveManager.Instance.itemData.ownedArtifacts;
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
