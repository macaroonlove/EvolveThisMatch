using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.ClickEffect
{
    public class ClickEffect : MonoBehaviour
    {
        [SerializeField] private GameObject prefab_click;
        [SerializeField] private int poolSize = 10;
        [SerializeField] private float lifeTime = 2;
        [SerializeField] private float parentScale = 100;

        private WaitForSeconds waitForSeconds;
        private Stack<GameObject> pool;

        private void Start()
        {
            DontDestroyOnLoad(transform.parent);
            waitForSeconds = new WaitForSeconds(lifeTime);
            InitPool();
        }

        private void OnRectTransformDimensionsChange()
        {
            parentScale = (transform.parent.localScale.x * 100);
        }

        private void InitPool()
        {
            pool = new Stack<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject particle = Instantiate(prefab_click, Vector3.zero, Quaternion.identity, transform);
                particle.SetActive(false);
                pool.Push(particle);
            }
        }

        private void Update()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 clickPosition = Input.mousePosition;
                StartCoroutine(SpawnParticle(clickPosition));
            }
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector3 inputPosition = Input.GetTouch(0).position;
                StartCoroutine(SpawnParticle(inputPosition));
            }
#endif
        }

        private IEnumerator SpawnParticle(Vector3 clickPosition)
        {
            GameObject particle;
            if (pool.Count == 0)
            {
                particle = Instantiate(prefab_click, Vector3.zero, Quaternion.identity, transform);
                particle.SetActive(false);
                pool.Push(particle);
            }
            else
            {
                particle = pool.Pop();
            }

            clickPosition.y -= parentScale;
            particle.transform.position = clickPosition;
            particle.SetActive(true);

            yield return waitForSeconds;

            particle.SetActive(false);
            pool.Push(particle);
        }
    }
}