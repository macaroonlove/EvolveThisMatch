using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UI;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIAgentDetailController
    {
        private readonly UIAgentDetailCanvas _view;
        private readonly PoolSystem _pool;

        private AgentTemplate _template;
        private AgentSaveData.Agent _owned;
        private GameObject _spawned;

        public UIAgentDetailController(UIAgentDetailCanvas view)
        {
            _view = view;
            _pool = CoreManager.Instance.GetSubSystem<PoolSystem>();
        }

        public void Show(AgentTemplate template, AgentSaveData.Agent owned)
        {
            // 새로운 유닛이거나, 스폰된 상태가 아니라면
            if (_owned != owned || _spawned == null)
            {
                _template = template;
                _owned = owned;

                ClearSpawn();

                if (template.overUIPrefab != null)
                {
                    _spawned = _pool.Spawn(template.overUIPrefab);
                    _spawned.transform.position = new Vector2(-1.5f, -4);
                }
            }

            // 기본적으로 보여지는 정보
            _view.ShowTemplate(template, owned);

            if (owned != null)
            {
                // 보유하고 있어야 보여지는 정보
                _view.ShowOwnedData(template, owned);
            }
        }

        public void Refresh()
        {
            Show(_template, _owned);
        }

        public void SelectPanel(int index)
        {
            if (_owned == null) return;

            VariableDisplayManager.Instance.HideAll();

            if (index == 4)
                VariableDisplayManager.Instance.Show(EVariableType.Essence);
            else if (index == 5)
                VariableDisplayManager.Instance.Show(EVariableType.Powder);

            _view.ShowPanel(index);
        }

        public void Hide()
        {
            SaveManager.Instance.agentData.VerifyTalents(() =>
            {
                _view.Clear();
            });
        }

        private void ClearSpawn()
        {
            if (_spawned != null)
            {
                _pool.DeSpawn(_spawned);
                _spawned = null;
            }
        }
    }
}