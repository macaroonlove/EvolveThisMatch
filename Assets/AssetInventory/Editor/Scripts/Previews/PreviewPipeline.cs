using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetInventory
{
    public sealed class PreviewPipeline : AssetImporter
    {
        public async Task<int> RecreateScheduledPreviews(List<AssetInfo> assets, List<AssetInfo> allAssets)
        {
            string assetFilter = GetAssetFilter(assets);
            string query = $"select *, AssetFile.Id as Id from AssetFile inner join Asset on Asset.Id = AssetFile.AssetId where Asset.Exclude = false and (AssetFile.PreviewState=? or AssetFile.PreviewState=?) {assetFilter} order by Asset.Id";
            List<AssetInfo> files = DBAdapter.DB.Query<AssetInfo>(query, AssetFile.PreviewOptions.Redo, AssetFile.PreviewOptions.RedoMissing).ToList();
            AI.ResolveParents(files, allAssets);

            return await RecreatePreviews(files);
        }

        public static string GetAssetFilter(List<AssetInfo> assets)
        {
            string assetFilter = "";
            if (assets != null && assets.Count > 0)
            {
                assetFilter = "and Asset.Id in (";
                foreach (AssetInfo asset in assets)
                {
                    assetFilter += asset.AssetId + ",";
                }

                assetFilter = assetFilter.Substring(0, assetFilter.Length - 1) + ")";
            }
            return assetFilter;
        }

        public async Task<int> RecreatePreviews(List<AssetInfo> files, bool autoRemoveCache = true, Action<PreviewRequest> onDone = null)
        {
            int created = 0;

            UnityPreviewGenerator.Init(files.Count);

            Asset curAsset = null;
            bool wasCurCached = false;
            string curTempPath = null;

            MainCount = files.Count;

            foreach (AssetInfo info in files.OrderBy(info => info.AssetId))
            {
                MainProgress++;
                SetProgress(info.FileName, MainProgress);

                if (CancellationRequested) break;
                await AI.Cooldown.Do();
                if (MainProgress % 5000 == 0) await Task.Yield(); // let editor breath in case there are many non-previewable files 

                if (!info.IsDownloaded && !info.IsMaterialized)
                {
                    Debug.Log($"Could not recreate preview for '{info}' since the package is not downloaded.");
                    continue;
                }

                // check if previewable at all
                if (!PreviewManager.IsPreviewable(info.FileName, true, info)) continue;

                // check if handling next package already
                if (curAsset != null && info.AssetId != curAsset.Id)
                {
                    if (!wasCurCached) RemoveWorkFolder(curAsset, curTempPath);
                    curAsset = null;
                }

                // persist extraction state
                if (curAsset == null)
                {
                    curAsset = info.ToAsset();
                    curTempPath = AI.GetMaterializedAssetPath(curAsset);
                    wasCurCached = Directory.Exists(curTempPath);
                }

                if (MainProgress % 10 == 0) await Task.Yield(); // let editor breath

                await PreviewManager.Create(info, null, () => created++, onDone);
            }
            await UnityPreviewGenerator.ExportPreviews();
            UnityPreviewGenerator.CleanUp();

            if (!wasCurCached && autoRemoveCache) RemoveWorkFolder(curAsset, curTempPath);

            return created;
        }

        public async Task<int> RestorePreviews(List<AssetInfo> assets, List<AssetInfo> allAssets)
        {
            int restored = 0;

            string previewPath = AI.GetPreviewFolder();
            string assetFilter = GetAssetFilter(assets);
            string query = $"select *, AssetFile.Id as Id from AssetFile inner join Asset on Asset.Id = AssetFile.AssetId where Asset.Exclude = false and (Asset.AssetSource = ? or Asset.AssetSource = ?) and AssetFile.PreviewState != ? {assetFilter} order by Asset.Id";
            List<AssetInfo> files = DBAdapter.DB.Query<AssetInfo>(query, Asset.Source.AssetStorePackage, Asset.Source.CustomPackage, AssetFile.PreviewOptions.Provided).ToList();
            AI.ResolveParents(files, allAssets);

            MainCount = files.Count;
            foreach (AssetInfo info in files)
            {
                MainProgress++;
                SetProgress(info.FileName, MainProgress);

                if (CancellationRequested) break;
                await AI.Cooldown.Do();
                if (MainProgress % 50 == 0) await Task.Yield(); // let editor breath 

                if (!info.IsDownloaded && !info.IsMaterialized) continue;

                string previewFile = info.GetPreviewFile(previewPath);
                string sourcePath = await AI.EnsureMaterializedAsset(info);
                if (sourcePath == null)
                {
                    if (info.PreviewState != AssetFile.PreviewOptions.NotApplicable)
                    {
                        info.PreviewState = AssetFile.PreviewOptions.None;
                        DBAdapter.DB.Execute("update AssetFile set PreviewState=? where Id=?", info.PreviewState, info.Id);
                    }
                    continue;
                }

                string originalPreviewFile = PreviewManager.DerivePreviewFile(sourcePath);
                if (!File.Exists(originalPreviewFile))
                {
                    if (info.PreviewState != AssetFile.PreviewOptions.NotApplicable)
                    {
                        info.PreviewState = AssetFile.PreviewOptions.None;
                        DBAdapter.DB.Execute("update AssetFile set PreviewState=? where Id=?", info.PreviewState, info.Id);
                    }
                    continue;
                }

                File.Copy(originalPreviewFile, previewFile, true);
                info.PreviewState = AssetFile.PreviewOptions.Provided;
                info.Hue = -1f;
                DBAdapter.DB.Execute("update AssetFile set PreviewState=?, Hue=? where Id=?", info.PreviewState, info.Hue, info.Id);

                restored++;
            }

            return restored;
        }
    }
}