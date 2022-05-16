using System.Collections;
using System.Threading.Tasks;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem;

internal class ModListContent : IContentPackProvider
{
    internal static ModListContent Content { get; private set; }

    internal static void Init()
    {
        Content = new ModListContent();
        ContentManager.collectContentPackProviders += AddContentPack;
    }

    private static void AddContentPack(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
    {
        addContentPackProvider(Content);
    }

    public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
    {
        Task assetLoadingTask = AssetReferences.Init();
        
        yield return new WaitUntil(() => assetLoadingTask.IsCompleted);
        args.ReportProgress(1f);
    }

    public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
    {
        args.ReportProgress(1f);
        yield break;
    }

    public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
    {
        args.ReportProgress(1f);
        yield break;
    }
    
    public string identifier => RoR2BepInExPack.PluginGUID + ".ModListSystem";
}
