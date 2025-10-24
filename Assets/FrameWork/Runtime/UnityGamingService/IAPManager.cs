using Cysharp.Threading.Tasks;
using FrameWork.UIPopup;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

namespace FrameWork.Service
{
    public class IAPManager : PersistentSingleton<IAPManager>
    {
        private StoreController _storeController;

        private void Start()
        {
            _ = InitializeIAP();
        }

        private async UniTask InitializeIAP()
        {
            // 스토어 컨트롤러 가져오기
            _storeController = UnityIAPServices.StoreController();

            await _storeController.Connect();

            // 제품 등록
            RegistrationProduct();
        }

        private void RegistrationProduct()
        {
            TextAsset catalogText = Resources.Load<TextAsset>("IAPProductCatalog");
            if (catalogText == null)
            {
#if UNITY_EDITOR
                Debug.LogError("IAPProductCatalog.json을 찾을 수 없습니다.");
#endif
                return;
            }

            JObject catalogJson = JObject.Parse(catalogText.text);
            var products = new List<ProductDefinition>();

            foreach (var prod in catalogJson["products"])
            {
                string id = prod["id"].ToString();
                int typeInt = prod["type"].ToObject<int>();
                ProductType type = typeInt == 0 ? ProductType.Consumable :
                                   typeInt == 1 ? ProductType.NonConsumable :
                                   ProductType.Subscription;

                products.Add(new ProductDefinition(id, type));
            }

            _storeController.FetchProducts(products);
        }

        public async UniTask<string> PurchaseProductAsync(string itemId)
        {
            var tcs = new UniTaskCompletionSource<string>();

            void Clean()
            {
                _storeController.OnPurchaseConfirmed -= OnConfirmed;
                _storeController.OnPurchaseFailed -= OnFailed;
                _storeController.OnPurchasePending -= OnPending;
            }

            // 구매에 성공했다면
            void OnConfirmed(Order order)
            {
                Clean();

                tcs.TrySetResult(order.Info.Receipt);
            }

            // 구매에 실패했다면
            void OnFailed(FailedOrder fail)
            {
                Clean();

                tcs.TrySetResult(null);
            }

            void OnPending(PendingOrder pending)
            {
#if UNITY_EDITOR
                _storeController.ConfirmPurchase(pending);
#else
                CheckReceipt(pending.Info.Receipt, itemId, () =>
                {
                    _storeController.ConfirmPurchase(pending);
                }, () =>
                {
                    Clean();
                    tcs.TrySetResult(null);
                });
#endif
            }

            _storeController.OnPurchaseConfirmed += OnConfirmed;
            _storeController.OnPurchaseFailed += OnFailed;
            _storeController.OnPurchasePending += OnPending;

            var productId = itemId.ToLower();

            _storeController.PurchaseProduct(productId);

            return await tcs.Task;
        }

        #region 명세서 검증 (PlayFab 서버)
        private void CheckReceipt(string receipt, string itemId, UnityAction onComplete, UnityAction onFailed)
        {
            var (receiptJson, signature) = ParseGoogleReceipt(receipt);

            var request = new ValidateGooglePlayPurchaseRequest
            {
                ReceiptJson = receiptJson,
                Signature = signature
            };

            PlayFabClientAPI.ValidateGooglePlayPurchase(request,
                (ValidateGooglePlayPurchaseResult result) =>
                {
                    CheckBuyAble(itemId, onComplete, onFailed);
                }, 
                (PlayFabError error) =>
                {
                    DebugPlayFabError(error);
                    onFailed?.Invoke();
                });
        }

        private void CheckBuyAble(string itemId, UnityAction onComplete, UnityAction onFailed)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "CheckBuyAble",
                FunctionParameter = new { itemId = itemId },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        onComplete?.Invoke();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                        onFailed?.Invoke();
                    }
                }, DebugPlayFabError);
        }

        #region 영수증 파싱
        [Serializable] private class Wrapper { public string Payload; }
        [Serializable] private class PayloadData { public string json; public string signature; }

        private (string json, string signature) ParseGoogleReceipt(string unityReceipt)
        {
            var wrapper = JsonUtility.FromJson<Wrapper>(unityReceipt);
            var payload = JsonUtility.FromJson<PayloadData>(wrapper.Payload);
            return (payload.json, payload.signature);
        }
        #endregion

        private void DebugPlayFabError(PlayFabError error)
        {
            switch (error.Error)
            {
                case PlayFabErrorCode.ConnectionError:
                case PlayFabErrorCode.ExperimentationClientTimeout:
                    UIPopupManager.Instance.ShowConfirmPopup("네트워크 연결을 확인해주세요.", () =>
                    {
                        SceneManager.LoadScene("Login");
                    });
                    break;
                case PlayFabErrorCode.ServiceUnavailable:
                    UIPopupManager.Instance.ShowConfirmPopup("게임 서버가 불안정합니다.\n나중에 다시 접속해주세요.\n죄송합니다.", () =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    });
                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogError($"PlayFab Error: {error.ErrorMessage}");
#endif
                    break;
            }
        }
        #endregion
    }
}

