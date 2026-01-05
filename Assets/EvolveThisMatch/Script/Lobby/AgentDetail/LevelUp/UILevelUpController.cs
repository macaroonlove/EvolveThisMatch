using EvolveThisMatch.Save;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public sealed class UILevelUpController
    {
        private readonly UILevelUpView _view;
        private readonly UILevelUpModel _model;
        private UnityAction _reShow;

        public UILevelUpController(UILevelUpView view, UILevelUpModel model)
        {
            _view = view;
            _model = model;
        }

        public void Show(AgentSaveData.Agent owned, UnityAction reShow)
        {
            _reShow = reShow;
            _model.Bind(owned);

            _view.Render(_model.BuildViewState());
        }

        public void Clear()
        {
            _model.Clear();

            _view.Render(_model.BuildViewState());
        }

        #region 음식 올리기·내리기
        public bool AddFood(int index)
        {
            if (!_model.AddFood(index)) return false;

            _view.Render(_model.BuildViewState());
            return true;
        }

        public bool RemoveFood(int index)
        {
            if (!_model.RemoveFood(index)) return false;

            _view.Render(_model.BuildViewState());
            return true;
        }
        #endregion

        #region 음식 자동 선택
        public int[] AutoSelect(int[] stockCounts)
        {
            int[] result = _model.AutoSelect(stockCounts);

            _view.Render(_model.BuildViewState());
            return result;
        }
        #endregion

        #region 레벨업
        public void LevelUp()
        {
            _model.LevelUp(() =>
            {
                _view.PayFoods();
                _reShow?.Invoke();
            });
        }
        #endregion
    }
}