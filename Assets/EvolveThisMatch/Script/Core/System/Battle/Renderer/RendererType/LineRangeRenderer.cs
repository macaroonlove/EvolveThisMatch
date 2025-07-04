using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class LineRangeRenderer : MonoBehaviour
    {
        private List<GameObject> _lineGroup = new List<GameObject>();
        private List<List<Vector2>> _lines = new List<List<Vector2>>();

        internal IReadOnlyList<IReadOnlyList<Vector2>> lines => _lines;

        internal void Initialize()
        {
            _lines.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                var line = transform.GetChild(i);

                List<Vector2> row = new List<Vector2>();

                for (int j = 0; j < line.childCount; j++)
                {
                    var tile = line.GetChild(j).position;
                    row.Add(new Vector2(tile.x, tile.y));
                }

                _lines.Add(row);
                _lineGroup.Add(line.gameObject);
            }

            Hide();
        }

        internal void Show(int lineCount)
        {
            for (int i = 0; i < lineCount; i++)
            {
                _lineGroup[i].SetActive(true);
            }
        }

        internal void Hide()
        {
            foreach (var line in _lineGroup)
            {
                line.SetActive(false);
            }
        }
    }
}