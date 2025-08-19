using FrameWork.Editor;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public abstract class GachaData
    {
        [SerializeField, Label("탭 이름")] private string _tabName;
        [SerializeField, Label("탭 배경")] private Sprite _tabBackground;

        [Header("뽑기 기타 설정")]
        [SerializeField, Label("메인 배경")] private Sprite _background;
        [SerializeField, Label("천장 변수")] protected ObscuredIntVariable _confirmedPickUpVariable;
        [SerializeField, Label("추가 획득 변수")] protected ObscuredIntVariable _additionalVariable;
        [SerializeField] private List<GachaCost> _costs = new List<GachaCost>();

        [Header("뽑기 버튼 설정")]
        [SerializeField] private List<GachaButton> _gachaButtons = new List<GachaButton>();

        protected UIGachaResultCanvas _gachaResultCanvas;

        #region 프로퍼티
        internal string tabName => _tabName;
        internal Sprite tabBackground => _tabBackground;
        internal Sprite background => _background;
        internal ObscuredIntVariable additionalVariable => _additionalVariable;
        internal IReadOnlyList<GachaCost> costs => _costs;
        internal IReadOnlyList<GachaButton> gachaButtons => _gachaButtons;
        #endregion

        internal virtual void Initialize(UIGachaResultCanvas gachaResultCanvas)
        {
            _gachaResultCanvas = gachaResultCanvas;
        }

        internal abstract void PickUp(int gachaCount);
    }

    [Serializable]
    public class GachaButton
    {
        [Label("뽑을 횟수")] public int gachaCount;
        [Label("버튼 색")] public Color color;
    }

    [Serializable]
    public class GachaCost
    {
        [Label("필요 재화 종류")] public ObscuredIntVariable variable;
        [Label("1회 뽑는데 사용되는 비용")] public int cost;
    }
}