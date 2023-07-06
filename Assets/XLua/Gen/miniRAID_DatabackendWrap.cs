#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class miniRAIDDatabackendWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(miniRAID.Databackend);
			Utils.BeginObjectRegister(type, L, translator, 0, 25, 8, 4);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetMap", _m_GetMap);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetMob", _m_SetMob);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ClearMob", _m_ClearMob);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddFx", _m_AddFx);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddFxAt", _m_AddFxAt);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveFx", _m_RemoveFx);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "MoveMob", _m_MoveMob);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetGridPos", _m_GetGridPos);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GridToWorldPos", _m_GridToWorldPos);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetDominantDirection", _m_GetDominantDirection);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RecordDamageHeal", _m_RecordDamageHeal);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RecordBuff", _m_RecordBuff);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DealDmgHeal", _m_DealDmgHeal);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "InMap", _m_InMap);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsMoveable", _m_IsMoveable);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetGridsWithMob", _m_GetGridsWithMob);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "FindPathTo", _m_FindPathTo);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsPathValid", _m_IsPathValid);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CanGridPlaceMob", _m_CanGridPlaceMob);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "FindNearestEmptyGrid", _m_FindNearestEmptyGrid);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Distance", _m_Distance);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetAllMobs", _m_GetAllMobs);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RegisterState", _m_RegisterState);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "onMobAdded", _e_onMobAdded);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "onMobRemoved", _e_onMobRemoved);
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "allMobs", _g_get_allMobs);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "allGridEffects", _g_get_allGridEffects);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mapSizeX", _g_get_mapSizeX);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mapHeight", _g_get_mapHeight);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mapSizeZ", _g_get_mapSizeZ);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "testSpell", _g_get_testSpell);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "dirc_dxyz", _g_get_dirc_dxyz);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "possibleDirections", _g_get_possibleDirections);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "mapSizeX", _s_set_mapSizeX);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "mapHeight", _s_set_mapHeight);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "mapSizeZ", _s_set_mapSizeZ);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "testSpell", _s_set_testSpell);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 2, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "GetSingleton", _m_GetSingleton_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "miniRAID.Databackend does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetSingleton_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        var gen_ret = miniRAID.Databackend.GetSingleton(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetMap(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    int _x = LuaAPI.xlua_tointeger(L, 2);
                    int _y = LuaAPI.xlua_tointeger(L, 3);
                    int _z = LuaAPI.xlua_tointeger(L, 4);
                    
                        var gen_ret = gen_to_be_invoked.GetMap( _x, _y, _z );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)) 
                {
                    UnityEngine.Vector3Int _pos;translator.Get(L, 2, out _pos);
                    
                        var gen_ret = gen_to_be_invoked.GetMap( _pos );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.Databackend.GetMap!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetMob(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 6&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& translator.Assignable<miniRAID.GridShape>(L, 5)&& translator.Assignable<miniRAID.MobData>(L, 6)) 
                {
                    int _x = LuaAPI.xlua_tointeger(L, 2);
                    int _y = LuaAPI.xlua_tointeger(L, 3);
                    int _z = LuaAPI.xlua_tointeger(L, 4);
                    miniRAID.GridShape _body = (miniRAID.GridShape)translator.GetObject(L, 5, typeof(miniRAID.GridShape));
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 6, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.SetMob( _x, _y, _z, _body, _mob );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)&& translator.Assignable<miniRAID.GridShape>(L, 3)&& translator.Assignable<miniRAID.MobData>(L, 4)) 
                {
                    UnityEngine.Vector3Int _pos;translator.Get(L, 2, out _pos);
                    miniRAID.GridShape _body = (miniRAID.GridShape)translator.GetObject(L, 3, typeof(miniRAID.GridShape));
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 4, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.SetMob( _pos, _body, _mob );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.Databackend.SetMob!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearMob(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 7&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& translator.Assignable<miniRAID.GridShape>(L, 5)&& translator.Assignable<miniRAID.MobData>(L, 6)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 7)) 
                {
                    int _x = LuaAPI.xlua_tointeger(L, 2);
                    int _y = LuaAPI.xlua_tointeger(L, 3);
                    int _z = LuaAPI.xlua_tointeger(L, 4);
                    miniRAID.GridShape _body = (miniRAID.GridShape)translator.GetObject(L, 5, typeof(miniRAID.GridShape));
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 6, typeof(miniRAID.MobData));
                    bool _remove = LuaAPI.lua_toboolean(L, 7);
                    
                    gen_to_be_invoked.ClearMob( _x, _y, _z, _body, _mob, _remove );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 6&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& translator.Assignable<miniRAID.GridShape>(L, 5)&& translator.Assignable<miniRAID.MobData>(L, 6)) 
                {
                    int _x = LuaAPI.xlua_tointeger(L, 2);
                    int _y = LuaAPI.xlua_tointeger(L, 3);
                    int _z = LuaAPI.xlua_tointeger(L, 4);
                    miniRAID.GridShape _body = (miniRAID.GridShape)translator.GetObject(L, 5, typeof(miniRAID.GridShape));
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 6, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.ClearMob( _x, _y, _z, _body, _mob );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 5&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)&& translator.Assignable<miniRAID.GridShape>(L, 3)&& translator.Assignable<miniRAID.MobData>(L, 4)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 5)) 
                {
                    UnityEngine.Vector3Int _pos;translator.Get(L, 2, out _pos);
                    miniRAID.GridShape _body = (miniRAID.GridShape)translator.GetObject(L, 3, typeof(miniRAID.GridShape));
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 4, typeof(miniRAID.MobData));
                    bool _remove = LuaAPI.lua_toboolean(L, 5);
                    
                    gen_to_be_invoked.ClearMob( _pos, _body, _mob, _remove );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)&& translator.Assignable<miniRAID.GridShape>(L, 3)&& translator.Assignable<miniRAID.MobData>(L, 4)) 
                {
                    UnityEngine.Vector3Int _pos;translator.Get(L, 2, out _pos);
                    miniRAID.GridShape _body = (miniRAID.GridShape)translator.GetObject(L, 3, typeof(miniRAID.GridShape));
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 4, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.ClearMob( _pos, _body, _mob );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.Databackend.ClearMob!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddFx(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Buff.GridEffect _fx = (miniRAID.Buff.GridEffect)translator.GetObject(L, 2, typeof(miniRAID.Buff.GridEffect));
                    
                    gen_to_be_invoked.AddFx( _fx );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddFxAt(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Buff.GridEffect _fx = (miniRAID.Buff.GridEffect)translator.GetObject(L, 2, typeof(miniRAID.Buff.GridEffect));
                    UnityEngine.Vector3Int _pos;translator.Get(L, 3, out _pos);
                    
                    gen_to_be_invoked.AddFxAt( _fx, _pos );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveFx(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Buff.GridEffect _fx = (miniRAID.Buff.GridEffect)translator.GetObject(L, 2, typeof(miniRAID.Buff.GridEffect));
                    
                    gen_to_be_invoked.RemoveFx( _fx );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_MoveMob(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3Int _from;translator.Get(L, 2, out _from);
                    UnityEngine.Vector3Int _to;translator.Get(L, 3, out _to);
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 4, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.MoveMob( _from, _to, _mob );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetGridPos(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    
                        var gen_ret = gen_to_be_invoked.GetGridPos( _pos );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GridToWorldPos(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3Int _gridPos;translator.Get(L, 2, out _gridPos);
                    
                        var gen_ret = gen_to_be_invoked.GridToWorldPos( _gridPos );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetDominantDirection(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3Int _from;translator.Get(L, 2, out _from);
                    UnityEngine.Vector3Int _to;translator.Get(L, 3, out _to);
                    
                        var gen_ret = gen_to_be_invoked.GetDominantDirection( _from, _to );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RecordDamageHeal(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Consts.DamageHeal_Result _result = (miniRAID.Consts.DamageHeal_Result)translator.GetObject(L, 2, typeof(miniRAID.Consts.DamageHeal_Result));
                    
                    gen_to_be_invoked.RecordDamageHeal( _result );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RecordBuff(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Consts.BuffEvents _result;translator.Get(L, 2, out _result);
                    
                    gen_to_be_invoked.RecordBuff( _result );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DealDmgHeal(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _target = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    miniRAID.Consts.DamageHeal_FrontEndInput _input;translator.Get(L, 3, out _input);
                    
                        var gen_ret = gen_to_be_invoked.DealDmgHeal( _target, _input );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InMap(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3Int _pos;translator.Get(L, 2, out _pos);
                    
                        var gen_ret = gen_to_be_invoked.InMap( _pos );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsMoveable(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.GridData _grid = (miniRAID.GridData)translator.GetObject(L, 2, typeof(miniRAID.GridData));
                    miniRAID.MobData.MovementType _type;translator.Get(L, 3, out _type);
                    int _cost;
                    
                        var gen_ret = gen_to_be_invoked.IsMoveable( _grid, _type, out _cost );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    LuaAPI.xlua_pushinteger(L, _cost);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetGridsWithMob(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Databackend.IsMobValidFunc _mobFilter = translator.GetDelegate<miniRAID.Databackend.IsMobValidFunc>(L, 2);
                    miniRAID.Databackend.IsGridValidFunc _gridFilter = translator.GetDelegate<miniRAID.Databackend.IsGridValidFunc>(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.GetGridsWithMob( _mobFilter, _gridFilter );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindPathTo(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 5&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)&& translator.Assignable<UnityEngine.Vector3Int>(L, 3)&& translator.Assignable<miniRAID.MobData.MovementType>(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    UnityEngine.Vector3Int _from;translator.Get(L, 2, out _from);
                    UnityEngine.Vector3Int _to;translator.Get(L, 3, out _to);
                    miniRAID.MobData.MovementType _movementType;translator.Get(L, 4, out _movementType);
                    int _maxDistance = LuaAPI.xlua_tointeger(L, 5);
                    
                        var gen_ret = gen_to_be_invoked.FindPathTo( _from, _to, _movementType, _maxDistance );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)&& translator.Assignable<UnityEngine.Vector3Int>(L, 3)&& translator.Assignable<miniRAID.MobData.MovementType>(L, 4)) 
                {
                    UnityEngine.Vector3Int _from;translator.Get(L, 2, out _from);
                    UnityEngine.Vector3Int _to;translator.Get(L, 3, out _to);
                    miniRAID.MobData.MovementType _movementType;translator.Get(L, 4, out _movementType);
                    
                        var gen_ret = gen_to_be_invoked.FindPathTo( _from, _to, _movementType );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)&& translator.Assignable<UnityEngine.Vector3Int>(L, 3)) 
                {
                    UnityEngine.Vector3Int _from;translator.Get(L, 2, out _from);
                    UnityEngine.Vector3Int _to;translator.Get(L, 3, out _to);
                    
                        var gen_ret = gen_to_be_invoked.FindPathTo( _from, _to );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.Databackend.FindPathTo!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsPathValid(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    miniRAID.GridPath _path = (miniRAID.GridPath)translator.GetObject(L, 3, typeof(miniRAID.GridPath));
                    
                        var gen_ret = gen_to_be_invoked.IsPathValid( _mob, _path );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CanGridPlaceMob(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3Int _center;translator.Get(L, 2, out _center);
                    miniRAID.GridShape _body = (miniRAID.GridShape)translator.GetObject(L, 3, typeof(miniRAID.GridShape));
                    
                        var gen_ret = gen_to_be_invoked.CanGridPlaceMob( _center, _body );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindNearestEmptyGrid(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)) 
                {
                    UnityEngine.Vector3Int _center;translator.Get(L, 2, out _center);
                    
                        var gen_ret = gen_to_be_invoked.FindNearestEmptyGrid( _center );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.Vector3Int>(L, 2)&& translator.Assignable<miniRAID.GridShape>(L, 3)) 
                {
                    UnityEngine.Vector3Int _center;translator.Get(L, 2, out _center);
                    miniRAID.GridShape _body = (miniRAID.GridShape)translator.GetObject(L, 3, typeof(miniRAID.GridShape));
                    
                        var gen_ret = gen_to_be_invoked.FindNearestEmptyGrid( _center, _body );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.Databackend.FindNearestEmptyGrid!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Distance(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3Int _a;translator.Get(L, 2, out _a);
                    UnityEngine.Vector3Int _b;translator.Get(L, 3, out _b);
                    
                        var gen_ret = gen_to_be_invoked.Distance( _a, _b );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetAllMobs(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetAllMobs(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterState(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Backend.BackendState _state = (miniRAID.Backend.BackendState)translator.GetObject(L, 2, typeof(miniRAID.Backend.BackendState));
                    
                    gen_to_be_invoked.RegisterState( _state );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_allMobs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.allMobs);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_allGridEffects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.allGridEffects);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mapSizeX(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.mapSizeX);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mapHeight(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.mapHeight);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mapSizeZ(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.mapSizeZ);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_testSpell(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.testSpell);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_dirc_dxyz(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.dirc_dxyz);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_possibleDirections(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.possibleDirections);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mapSizeX(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mapSizeX = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mapHeight(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mapHeight = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mapSizeZ(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mapSizeZ = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_testSpell(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.testSpell = (miniRAID.Spells.Spell)translator.GetObject(L, 2, typeof(miniRAID.Spells.Spell));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onMobAdded(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobArgumentDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobArgumentDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobArgumentDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.onMobAdded += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.onMobAdded -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.Databackend.onMobAdded!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onMobRemoved(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.Databackend gen_to_be_invoked = (miniRAID.Databackend)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobArgumentDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobArgumentDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobArgumentDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.onMobRemoved += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.onMobRemoved -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.Databackend.onMobRemoved!");
            return 0;
        }
        
		
		
    }
}
