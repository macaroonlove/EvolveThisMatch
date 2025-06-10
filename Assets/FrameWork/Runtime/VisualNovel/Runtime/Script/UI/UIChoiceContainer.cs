using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.VisualNovel
{
    public class UIChoiceContainer : UIBase
    {
        [SerializeField] private GameObject _choiceControllerPrefab;

        private Transform _parent;
        private List<UIChoiceController> _controllers = new List<UIChoiceController>();
        private UnityAction onComplete;

        protected override void Initialize()
        {
            _parent = transform.GetChild(0);
        }

        private void OnEnable()
        {
            CommandExecutor.Instance.choice += Choice;
        }

        private void OnDisable()
        {
            CommandExecutor.Instance.choice -= Choice;
        }

        private void Choice(List<KeyValuePair<string, ChapterTemplate>> choiceList, UnityAction onComplete)
        {
            this.onComplete = onComplete;
            foreach (var controller in _controllers)
            {
                controller.Hide(true);
            }

            for (int i = 0; i < choiceList.Count; i++)
            {
                UIChoiceController controller;

                if (i < _controllers.Count)
                {
                    controller = _controllers[i];
                }
                else
                {
                    GameObject instance = Instantiate(_choiceControllerPrefab, _parent);
                    controller = instance.GetComponent<UIChoiceController>();
                    controller.Binding();
                    _controllers.Add(controller);
                }

                controller.Initialize(choiceList[i].Key, choiceList[i].Value, OnClick);
                controller.Show(true);
            }

            Show(true);
        }

        private void OnClick()
        {
            foreach (var controller in _controllers)
            {
                controller.Hide(true);
            }

            Hide(true);

            onComplete?.Invoke();
        }
    }
}
