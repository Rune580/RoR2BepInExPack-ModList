using System;
using System.Collections.Generic;
using System.Text;

namespace RoR2BepInExPack.DynamicModEnablement;

/// <summary>
/// Attribute that decorates classes, specifically BaseUnityPlugin Inheriting Classes to mark that they can be Enabled or Disabled at Runtime in a Dynamic fashion.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DynamicModEnablementAttribute : Attribute
{
}
