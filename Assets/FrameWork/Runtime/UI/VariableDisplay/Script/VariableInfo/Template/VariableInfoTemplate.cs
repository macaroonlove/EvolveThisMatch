using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UI
{
    [CreateAssetMenu(
        fileName = "VariableInfoTemplate.asset",
        menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "VariableInfoTemplate",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 100)]
    public class VariableInfoTemplate : ScriptableObject
    {
        [SerializeField] private List<VariableInfo> _infos = new List<VariableInfo>();

        internal IReadOnlyList<VariableInfo> infos => _infos;
    }

    [Serializable]
    public class VariableInfo
    {
        [SerializeField] private ObscuredIntVariable _vairable;
        [SerializeField, TextArea(2, 4)] private string _description;
        [SerializeField] private List<AcquisitionLocation> _acquisitionLocations = new List<AcquisitionLocation>();

        internal ObscuredIntVariable variable => _vairable;
        internal string description => _description;
        internal IReadOnlyList<AcquisitionLocation> acquisitionLocations => _acquisitionLocations;
    }

    [Serializable]
    public class AcquisitionLocation
    {
        [SerializeField] private string _text;

        internal string text => _text;
    }
}