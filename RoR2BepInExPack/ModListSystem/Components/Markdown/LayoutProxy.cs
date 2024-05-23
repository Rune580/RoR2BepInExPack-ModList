using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown;

[ExecuteAlways]
public class LayoutProxy : UIBehaviour, ILayoutElement
{
    private ILayoutElement[] _children = [];
    
    public override void Awake()
    {
        base.Awake();

        GetChildren();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        
        GetChildren();
    }

    private void OnTransformChildrenChanged()
    {
        GetChildren();
    }

    private void GetChildren()
    {
        var children = new List<ILayoutElement>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var layoutElement = child.GetComponent(typeof(ILayoutElement)) as ILayoutElement;

            if (layoutElement is null)
                continue;
            
            children.Add(layoutElement);
        }

        _children = children.ToArray();
    }

    public void CalculateLayoutInputHorizontal()
    {
        foreach (var element in _children)
            element.CalculateLayoutInputHorizontal();
    }

    public void CalculateLayoutInputVertical()
    {
        foreach (var element in _children)
            element.CalculateLayoutInputVertical();
    }

    public float minWidth => _children.Aggregate(0f, (current, element) => Mathf.Max(current, element.minWidth));
    public float preferredWidth => _children.Aggregate(0f, (current, element) => Mathf.Max(current, element.preferredHeight));
    public float flexibleWidth => _children.Aggregate(0f, (current, element) => Mathf.Max(current, element.flexibleWidth));
    public float minHeight => _children.Aggregate(0f, (current, element) => Mathf.Max(current, element.minHeight));
    public float preferredHeight => _children.Aggregate(0f, (current, element) => Mathf.Max(current, element.preferredHeight));
    public float flexibleHeight => _children.Aggregate(0f, (current, element) => Mathf.Max(current, element.flexibleHeight));
    public int layoutPriority => 1;
}
