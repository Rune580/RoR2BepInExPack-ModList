﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem;

public class ModDataBuilder
{
    private ModDataInternal _modDataInternal;
    
    public ModDataBuilder()
    {
        _modDataInternal = new ModDataInternal();
        
        var plugin = Assembly.GetCallingAssembly().GetExportedTypes()
            .Select(type => type.GetCustomAttribute<BepInPlugin>()).FirstOrDefault(plugin => plugin != null);

        if (plugin != default)
            _modDataInternal.Plugin = plugin;
    }

    public ModDataBuilder WithBepInPlugin(string modGuid, string modName, string modVersion)
    {
        return this.WithBepInPlugin(modGuid, modName, new Version(modVersion));
    }
    
    public ModDataBuilder WithBepInPlugin(string modGuid, string modName, Version modVersion)
    {
        return this.WithGuid(modGuid).WithName(modName).WithVersion(modVersion);
    }

    public ModDataBuilder WithBepInPlugin(BepInPlugin plugin)
    {
        return this.WithBepInPlugin(plugin.GUID, plugin.Name, plugin.Version);
    }

    public ModDataBuilder WithGuid(string modGuid)
    {
        _modDataInternal.Guid = modGuid;
        return this;
    }

    public ModDataBuilder WithName(string modName)
    {
        _modDataInternal.Name = modName;
        return this;
    }

    public ModDataBuilder WithAuthor(string modAuthor)
    {
        _modDataInternal.Author = modAuthor;
        return this;
    }

    public ModDataBuilder WithVersion(Version modVersion)
    {
        _modDataInternal.Version = modVersion;
        return this;
    }

    public ModDataBuilder WithVersion(string modVersion)
    {
        _modDataInternal.Version = new Version(modVersion);
        return this;
    }

    public ModDataBuilder WithIcon(Sprite icon)
    {
        _modDataInternal.Icon = icon;
        return this;
    }

    public ModDataBuilder WithLinks(params HyperLink[] links)
    {
        _modDataInternal.Links = links.ToList();
        return this;
    }

    public ModDataBuilder AddLink(HyperLink link)
    {
        _modDataInternal.Links.Add(link);
        return this;
    }

    public ModDataBuilder AddLinks(params HyperLink[] links)
    {
        _modDataInternal.Links.AddRange(links);
        return this;
    }

    public ModDataBuilder SupportsRuntimeToggling()
    {
        _modDataInternal.SupportsRuntimeToggling = true;
        return this;
    }

    public void Submit()
    { 
        InternalBuild();
    }

    public ModData InternalBuild()
    {
        ModData modData = new ModData
        {
            Guid = _modDataInternal.Guid,
            Name = _modDataInternal.Name,
            Author = _modDataInternal.Author,
            Version = _modDataInternal.Version,
            DescriptionToken = _modDataInternal.DescriptionToken,
            Icon = _modDataInternal.Icon,
            Links = _modDataInternal.Links.ToArray(),
            SupportsRuntimeToggling = _modDataInternal.SupportsRuntimeToggling
        };

        if (_modDataInternal.Plugin != default && string.IsNullOrEmpty(modData.Guid))
            modData.Guid = _modDataInternal.Plugin.GUID;
        
        ModData.Instances.Add(modData);

        return modData;
    }

    // don't think too hard about it.
    private struct ModDataInternal
    {
        internal BepInPlugin Plugin = null;
        internal string Guid = "";
        internal string Name = "";
        internal string Author = "Unknown Author";
        internal Version Version = null;
        internal string DescriptionToken = "MOD_NO_DESCRIPTION";
        internal Sprite Icon = null;
        internal List<HyperLink> Links = new();
        internal bool SupportsRuntimeToggling = false;

        public ModDataInternal() { }
    }
}