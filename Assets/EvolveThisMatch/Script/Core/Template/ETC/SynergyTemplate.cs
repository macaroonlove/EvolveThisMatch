using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Synergy", fileName = "Synergy", order = 0)]
    public class SynergyTemplate : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Color _textColor;

        #region 프로퍼티
        public int id => _id;
        public string displayName => _displayName;
        public Sprite icon => _icon;
        public Color textColor => _textColor;
        #endregion
    }
}