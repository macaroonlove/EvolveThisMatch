using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetInventory
{
    public sealed class UnityPackageDownloadImporter : AssetImporter
    {
        public IEnumerator IndexOnline(Action callback)
        {
            List<AssetInfo> packages = AI.LoadAssets()
                .Where(info =>
                    info.AssetSource == Asset.Source.AssetStorePackage
                    && !info.Exclude
                    && info.ParentId <= 0
                    && !info.IsAbandoned && (!info.IsIndexed || info.CurrentState == Asset.State.SubInProcess) && !string.IsNullOrEmpty(info.OfficialState)
                    && !info.IsDownloaded)
                .ToList();

            for (int i = 0; i < packages.Count; i++)
            {
                if (CancellationRequested) break;
                AssetInfo info = packages[i];

                MainCount = packages.Count;
                SetProgress(info.GetDisplayName(), i + 1);

                // check if metadata is already available for triggering and monitoring
                if (string.IsNullOrWhiteSpace(info.OriginalLocation)) continue;

                // skip if too large or unknown download size yet
                if (AI.Config.limitAutoDownloads && (info.PackageSize == 0 || Mathf.RoundToInt(info.PackageSize / 1024f / 1024f) >= AI.Config.downloadLimit)) continue;

                AI.GetObserver().Attach(info);
                if (!info.PackageDownloader.IsDownloadSupported()) continue;

                // trigger already next one in background
                AssetInfo nextInfo = i < packages.Count - 1 ? packages[i + 1] : null;
                if (nextInfo != null)
                {
                    if (!AI.Config.limitAutoDownloads || nextInfo.PackageSize == 0 || Mathf.RoundToInt(nextInfo.PackageSize / 1024f / 1024f) < AI.Config.downloadLimit)
                    {
                        AI.GetObserver().Attach(nextInfo);
                        if (nextInfo.PackageDownloader.IsDownloadSupported() && !nextInfo.IsDownloading())
                        {
                            nextInfo.PackageDownloader.Download();
                        }
                    }
                }

                // refresh in case parallel download has finished by now
                info.Refresh();
                info.PackageDownloader.RefreshState();
                if (info.IsDownloading() || !info.IsDownloaded)
                {
                    CurrentMain = $"Downloading {info.GetDisplayName()}";
                    CurrentSub = IOUtils.RemoveInvalidChars(info.GetDisplayName());
                    SubCount = 0;
                    SubProgress = 0;

                    if (!info.IsDownloading()) info.PackageDownloader.Download();
                    do
                    {
                        if (CancellationRequested) break; // download will finish in that case and not be removed

                        AssetDownloadState state = info.PackageDownloader.GetState();
                        SubCount = Mathf.RoundToInt(state.bytesTotal / 1024f / 1024f);
                        SubProgress = Mathf.RoundToInt(state.bytesDownloaded / 1024f / 1024f);
                        if (SubCount == 0) SubCount = SubProgress; // in case total size was not available yet
                        yield return null;
                    } while (info.IsDownloading());
                }
                if (CancellationRequested) break;

                info.SetLocation(info.PackageDownloader.GetAsset().Location);
                info.Refresh();
                info.PackageDownloader.RefreshState();

                if (!info.IsDownloaded)
                {
                    Debug.LogError($"Downloading '{info}' failed. Continuing with next package.");
                    continue;
                }

                SubProgress = SubCount; // ensure 100% progress

                UnityPackageImporter unityPackageImporter = new UnityPackageImporter();
                AI.Actions.RegisterRunningAction(ActionHandler.ACTION_ASSET_STORE_CACHE_INDEX, unityPackageImporter, "Indexing downloaded package");
                unityPackageImporter.HandlePackage(true, AI.DeRel(info.Location), i);
                Task task = unityPackageImporter.IndexDetails(info.AssetId);
                yield return new WaitWhile(() => !task.IsCompleted);
                unityPackageImporter.FinishProgress();

                // remove again
                if (!AI.Config.keepAutoDownloads)
                {
                    // perform backup before deleting, as otherwise the file would not be considered
                    if (AI.Actions.CreateBackups)
                    {
                        AssetBackup backup = new AssetBackup();
                        Task task2 = backup.Backup(info.AssetId);
                        yield return new WaitWhile(() => !task2.IsCompleted);
                    }

                    IOUtils.TryDeleteFile(info.GetLocation(true));
                }

                info.Refresh();
            }

            callback?.Invoke();
        }
    }
}