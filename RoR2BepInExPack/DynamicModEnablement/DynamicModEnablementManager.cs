using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx;
using RoR2BepInExPack.ModListSystem;

namespace RoR2BepInExPack.DynamicModEnablement;

public static class DynamicModEnablementManager
{
    private static HashSet<string> _optedInPlugins = new();
    private static Dictionary<string, Action<BaseUnityPlugin>> _onModEnabledCallback = new();
    private static Dictionary<string, Action<BaseUnityPlugin>> _onModDisabledCallback = new();

    internal static void Init()
    {
        foreach (var guidAndInfo in BepInEx.Bootstrap.Chainloader.PluginInfos)
        {
            var guid = guidAndInfo.Key;
            var info = guidAndInfo.Value;

            var instance = info.Instance;

            if (!instance)
                continue;

            var type = instance.GetType();

            if (type.GetCustomAttribute<DynamicModEnablementAttribute>() != null)
            {
                _optedInPlugins.Add(guid);
            }
        }
    }

    public static void AddModEnabledCallback(string modGUID, Action<BaseUnityPlugin> callback)
    {
        _onModEnabledCallback[modGUID] += callback;
    }

    public static void AddModDisabledCallback(string modGUID, Action<BaseUnityPlugin> callback)
    {
        _onModDisabledCallback[modGUID] += callback;
    }

    public static bool IsModEnabled(ModDataInfo modDataInfo)
    {
        return IsModEnabled(modDataInfo.PluginInfo.Instance);
    }

    public static bool IsModEnabled(string modGUID)
    {
        if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue(modGUID, out var pInfo))
        {
            return IsModEnabled(pInfo.Instance);
        }

        return false;
    }

    public static bool IsModEnabled(BaseUnityPlugin plugin) => IsModEnabledInternal(plugin);

    private static bool IsModEnabledInternal(BaseUnityPlugin plugin)
    {
        return plugin ? plugin.enabled : false;
    }

    public static bool CanModBeDynamicallyEnabledOrDisabled(ModDataInfo info) => CanModBeDynamicallyEnabledOrDisabledInternal(info.PluginInfo.Metadata.GUID);

    public static bool CanModBeDynamicallyEnabledOrDisabled(string modGUID) => CanModBeDynamicallyEnabledOrDisabledInternal(modGUID);

    public static bool CanModBeDynamicallyEnabledOrDisabled(BaseUnityPlugin plugin) => CanModBeDynamicallyEnabledOrDisabledInternal(plugin.Info.Metadata.GUID);

    private static bool CanModBeDynamicallyEnabledOrDisabledInternal(string modGUID)
    {
        return _optedInPlugins.Contains(modGUID);
    }

    public static bool TryEnableMod(ModDataInfo modDataInfo) => TryEnableModInternal(modDataInfo.PluginInfo.Instance);

    public static bool TryEnableMod(string GUID)
    {
        if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue(GUID, out var pInfo))
        {
            return TryEnableModInternal(pInfo.Instance);
        }
        return false;
    }

    public static bool TryEnableMod(BaseUnityPlugin plugin) => TryEnableModInternal(plugin);

    private static bool TryEnableModInternal(BaseUnityPlugin plugin)
    {
        var metadata = plugin.Info.Metadata;

        if(!CanModBeDynamicallyEnabledOrDisabledInternal(metadata.GUID))
        {
            //TODO: log message relating to Enabling not supported, could lead to issues
            return false;
        }

        if (plugin.enabled)
        {
            //TODO: Log message, plugin already enabled.
            return false;
        }

        plugin.enabled = true;
        if(_onModEnabledCallback.TryGetValue(metadata.GUID, out var callback))
        {
            foreach(var invocation in callback.GetInvocationList())
            {
                try
                {
                    invocation.DynamicInvoke(plugin);
                }
                catch(Exception ex)
                {
                    //TODO: log message, exception during onModEnabledCallback.
                }
            }
        }
        return true;
    }

    public static bool TryDisableMod(ModDataInfo modDataInfo) => TryDisableModInternal(modDataInfo.PluginInfo.Instance);

    public static bool TryDisableMod(string GUID)
    {
        if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue(GUID, out var pInfo))
        {
            return TryDisableModInternal(pInfo.Instance);
        }
        return false;
    }

    public static bool TryDisableMod(BaseUnityPlugin plugin) => TryDisableModInternal(plugin);

    private static bool TryDisableModInternal(BaseUnityPlugin plugin)
    {
        var metadata = plugin.Info.Metadata;

        if (!CanModBeDynamicallyEnabledOrDisabledInternal(metadata.GUID))
        {
            //TODO: log message relating to Disabling not supported, could lead to issues
            return false;
        }

        if (!plugin.enabled)
        {
            //TODO: Log message, plugin already disabled.
            return false;
        }

        plugin.enabled = false;
        if (_onModDisabledCallback.TryGetValue(metadata.GUID, out var callback))
        {
            foreach (var invocation in callback.GetInvocationList())
            {
                try
                {
                    invocation.DynamicInvoke(plugin);
                }
                catch (Exception ex)
                {
                    //TODO: log message, exception during onModEnabledCallback.
                }
            }
        }
        return true;
    }


}
