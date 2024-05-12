using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class ThematicBreakBlockObject : BaseMarkdownBlockObject
{
    public Image line;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (!line)
            return;
        
        if (block is not ThematicBreakBlock thematicBreakBlock)
            return;
        
        AnchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);

        Height = renderCtx.FontSize * 2f;
        renderCtx.YPos += Height;
    }
}
