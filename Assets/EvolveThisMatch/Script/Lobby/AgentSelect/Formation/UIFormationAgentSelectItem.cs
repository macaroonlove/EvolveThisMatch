using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIFormationAgentSelectItem : UIAgentSelectItem
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            IndexText,
        }
        #endregion

        private GameObject _indexObj;
        private TextMeshProUGUI _indexText;

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _indexText = GetText((int)Texts.IndexText);
            _indexObj = _indexText.transform.parent.gameObject;

            base.Initialize();
        }

        internal void Select(int index)
        {
            _indexText.text = index.ToString();
            _indexObj.SetActive(true);

            base.Select();
        }

        internal override void UnSelect()
        {
            _indexObj.SetActive(false);

            base.UnSelect();
        }
    }
}