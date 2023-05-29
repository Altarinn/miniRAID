using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public static class WrapConfig
{
    [CSharpCallLua]
    public static List<System.Type> List_CSCallLua = new List<System.Type>()
    {
        typeof(IEnumerator),
        typeof(Coroutine),
        typeof(System.Func<System.ValueTuple<miniRAID.MobRenderer, miniRAID.Spells.SpellTarget>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.MobRenderer, miniRAID.Spells.SpellTarget, UnityEngine.Vector3>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.MobRenderer, Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.MobRenderer, UnityEngine.Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<miniRAID.MobRenderer, float>),

        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobRenderer, miniRAID.Spells.SpellTarget>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobRenderer, miniRAID.Spells.SpellTarget, UnityEngine.Vector3>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobRenderer, Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobRenderer, UnityEngine.Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.Buff.Buff, miniRAID.Cost, miniRAID.RuntimeAction, miniRAID.MobRenderer>, miniRAID.None>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobRenderer, miniRAID.MobRenderer>, System.Collections.IEnumerator>),
        
        typeof(System.Func<System.ValueTuple<miniRAID.MobData, miniRAID.Spells.SpellTarget>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.MobData, miniRAID.Spells.SpellTarget, UnityEngine.Vector3>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.MobData, Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.MobData, UnityEngine.Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<miniRAID.MobData, float>),

        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobData, miniRAID.Spells.SpellTarget>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobData, miniRAID.Spells.SpellTarget, UnityEngine.Vector3>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobData, Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobData, UnityEngine.Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.Buff.Buff, miniRAID.Cost, miniRAID.RuntimeAction, miniRAID.MobData>, miniRAID.None>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.MobData, miniRAID.MobData>, System.Collections.IEnumerator>),
    };

    [LuaCallCSharp]
    public static List<System.Type> List_LuaCallCS = new List<System.Type>()
    {
        typeof(IEnumerator),
        typeof(Coroutine)
    };

    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.Light", "SetLightDirty"},
                new List<string>(){"UnityEngine.Light", "shadowRadius"},
                new List<string>(){"UnityEngine.Light", "shadowAngle"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
                
                new List<string>(){"UnityEngine.Transform"},
                new List<string>(){"UnityEngine.Resources"},
            };
}
