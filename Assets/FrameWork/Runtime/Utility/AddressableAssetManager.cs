using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace FrameWork
{
    public class AddressableAssetManager : PersistentSingleton<AddressableAssetManager>
    {
        private Dictionary<string, AsyncOperationHandle<Sprite>> _sprites = new Dictionary<string, AsyncOperationHandle<Sprite>>();
        private Dictionary<string, AsyncOperationHandle<AudioClip>> _audioClips = new Dictionary<string, AsyncOperationHandle<AudioClip>>();
        private Dictionary<string, AsyncOperationHandle> _scriptableObjects = new Dictionary<string, AsyncOperationHandle>();

        #region Sprite
        public void GetSprite(string key, UnityAction<Sprite> onComplete)
        {
            if (string.IsNullOrEmpty(key))
            {
                onComplete?.Invoke(null);
                return;
            }

            if (_sprites.ContainsKey(key))
            {
                onComplete?.Invoke(_sprites[key].Result);
                return;
            }

            Addressables.LoadAssetAsync<Sprite>(key).Completed += (AsyncOperationHandle<Sprite> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _sprites.TryAdd(key, handle);
                    onComplete?.Invoke(handle.Result);
                }
                else
                {
                    onComplete?.Invoke(null);
                    Addressables.Release(handle);
                }
            };
        }

        public void ReleaseSprite(string key)
        {
            if (_sprites.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _sprites.Remove(key);
            }
        }

        public void ReleaseAllSprites()
        {
            foreach (var handle in _sprites.Values)
            {
                Addressables.Release(handle);
            }
            _sprites.Clear();
        }
        #endregion

        #region AudioClip
        public void GetAudioClip(string key, UnityAction<AudioClip> onComplete)
        {
            if (string.IsNullOrEmpty(key))
            {
                onComplete?.Invoke(null);
                return;
            }

            if (_audioClips.ContainsKey(key))
            {
                onComplete?.Invoke(_audioClips[key].Result);
                return;
            }

            Addressables.LoadAssetAsync<AudioClip>(key).Completed += (AsyncOperationHandle<AudioClip> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _audioClips[key] = handle;
                    onComplete?.Invoke(handle.Result);
                }
                else
                {
                    onComplete?.Invoke(null);
                    Addressables.Release(handle);
                }
            };
        }

        public void ReleaseAudioClip(string key)
        {
            if (_audioClips.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _audioClips.Remove(key);
            }
        }

        public void ReleaseAllAudioClips()
        {
            foreach (var handle in _audioClips.Values)
            {
                Addressables.Release(handle);
            }
            _audioClips.Clear();
        }
        #endregion

        #region ScriptableObject
        
        #region ¹®ÀÚ¿­
        public void GetScriptableObject<T>(string key, UnityAction<T> onComplete) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(key))
            {
                onComplete?.Invoke(null);
                return;
            }

            if (_scriptableObjects.ContainsKey(key))
            {
                onComplete?.Invoke(_scriptableObjects[key].Result as T);
                return;
            }

            Addressables.LoadAssetAsync<T>(key).Completed += (AsyncOperationHandle<T> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _scriptableObjects.TryAdd(key, handle);
                    onComplete?.Invoke(handle.Result);
                }
                else
                {
                    onComplete?.Invoke(null);
                    Addressables.Release(handle);
                }
            };
        }

        public void ReleaseScriptableObject(string key)
        {
            if (_scriptableObjects.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _scriptableObjects.Remove(key);
            }
        }

        #endregion

        #region AssetReference
        public void GetScriptableObject<T>(AssetReference assetReference, UnityAction<T> onComplete) where T : ScriptableObject
        {
            if (assetReference == null || string.IsNullOrEmpty(assetReference.AssetGUID))
            {
                onComplete?.Invoke(null);
                return;
            }

            string key = assetReference.AssetGUID;

            if (_scriptableObjects.TryGetValue(key, out var cachedHandle))
            {
                onComplete?.Invoke(cachedHandle.Result as T);
                return;
            }

            assetReference.LoadAssetAsync<T>().Completed += (AsyncOperationHandle<T> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _scriptableObjects.TryAdd(key, handle);
                    onComplete?.Invoke(handle.Result);
                }
                else
                {
                    onComplete?.Invoke(null);
                    Addressables.Release(handle);
                }
            };
        }

        public void ReleaseScriptableObject(AssetReference assetReference)
        {
            if (assetReference == null || string.IsNullOrEmpty(assetReference.AssetGUID))
                return;

            ReleaseScriptableObject(assetReference.AssetGUID);
        }
        #endregion

        public void ReleaseAllScriptableObjects()
        {
            foreach (var handle in _scriptableObjects.Values)
            {
                Addressables.Release(handle);
            }
            _scriptableObjects.Clear();
        }
        #endregion

        public void ReleaseAll()
        {
            ReleaseAllSprites();
            ReleaseAllAudioClips();
            ReleaseAllScriptableObjects();
        }

        protected override void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            ReleaseAll();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ReleaseAll();
        }
    }
}