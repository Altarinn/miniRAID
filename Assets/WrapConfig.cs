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
        typeof(System.Func<System.ValueTuple<miniRAID.Mob, miniRAID.Spells.SpellTarget>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.Mob, miniRAID.Spells.SpellTarget, UnityEngine.Vector3>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.Mob, Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.Mob, UnityEngine.Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<miniRAID.Mob, float>),

        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.Mob, miniRAID.Spells.SpellTarget>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.Mob, miniRAID.Spells.SpellTarget, UnityEngine.Vector3>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.Mob, Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.Mob, UnityEngine.Vector2Int>, System.Collections.IEnumerator>),
        typeof(System.Func<System.ValueTuple<miniRAID.Buff.Buff, miniRAID.Cost, miniRAID.RuntimeAction, miniRAID.Mob>, miniRAID.None>),
        typeof(System.Func<System.ValueTuple<miniRAID.GeneralCombatData, miniRAID.Mob, miniRAID.Mob>, System.Collections.IEnumerator>),
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
            };
}
