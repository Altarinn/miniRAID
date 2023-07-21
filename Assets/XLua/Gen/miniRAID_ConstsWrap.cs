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
    public class miniRAIDConstsWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(miniRAID.Consts);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 17, 12, 12);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "IsDirect", _m_IsDirect_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsHeal", _m_IsHeal_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "parentType", _m_parentType_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetIdenticalDefense", _m_GetIdenticalDefense_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetHitRate", _m_GetHitRate_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetCriticalRate", _m_GetCriticalRate_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetDamage", _m_GetDamage_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetDefenseRate", _m_GetDefenseRate_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ValueFromItemLevel", _m_ValueFromItemLevel_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BaseStatsFromLevel", _m_BaseStatsFromLevel_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetHealth", _m_GetHealth_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "EnemyMask", _m_EnemyMask_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AllyMask", _m_AllyMask_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ApplyMask", _m_ApplyMask_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Distance", _m_Distance_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsPointWithinCollider", _m_IsPointWithinCollider_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "HealAggroMul", _g_get_HealAggroMul);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "additionalAttackerLevels", _g_get_additionalAttackerLevels);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "HitRangePerLevel", _g_get_HitRangePerLevel);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "BaseHit", _g_get_BaseHit);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "MaxHitAcc", _g_get_MaxHitAcc);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CritRangePerLevel", _g_get_CritRangePerLevel);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "BaseCrit", _g_get_BaseCrit);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "MaxListenerLevels", _g_get_MaxListenerLevels);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "baseStatAverageGrowth", _g_get_baseStatAverageGrowth);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "UnitGroupToMaskBit", _g_get_UnitGroupToMaskBit);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Enemies", _g_get_Enemies);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Allies", _g_get_Allies);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "HealAggroMul", _s_set_HealAggroMul);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "additionalAttackerLevels", _s_set_additionalAttackerLevels);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "HitRangePerLevel", _s_set_HitRangePerLevel);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "BaseHit", _s_set_BaseHit);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "MaxHitAcc", _s_set_MaxHitAcc);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CritRangePerLevel", _s_set_CritRangePerLevel);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "BaseCrit", _s_set_BaseCrit);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "MaxListenerLevels", _s_set_MaxListenerLevels);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "baseStatAverageGrowth", _s_set_baseStatAverageGrowth);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "UnitGroupToMaskBit", _s_set_UnitGroupToMaskBit);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "Enemies", _s_set_Enemies);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "Allies", _s_set_Allies);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "miniRAID.Consts does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsDirect_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    miniRAID.Consts.DamageHealFlags _flags;translator.Get(L, 1, out _flags);
                    
                        var gen_ret = miniRAID.Consts.IsDirect( _flags );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsHeal_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    miniRAID.Consts.DamageHeal_FrontEndInput _input;translator.Get(L, 1, out _input);
                    
                        var gen_ret = miniRAID.Consts.IsHeal( _input );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_parentType_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& translator.Assignable<miniRAID.Consts.AllElements>(L, 1)) 
                {
                    miniRAID.Consts.AllElements _e;translator.Get(L, 1, out _e);
                    
                        var gen_ret = miniRAID.Consts.parentType( _e );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& translator.Assignable<miniRAID.Consts.Elements>(L, 1)) 
                {
                    miniRAID.Consts.Elements _e;translator.Get(L, 1, out _e);
                    
                        var gen_ret = miniRAID.Consts.parentType( _e );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.Consts.parentType!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetIdenticalDefense_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _referenceLevel = LuaAPI.xlua_tointeger(L, 1);
                    
                        var gen_ret = miniRAID.Consts.GetIdenticalDefense( _referenceLevel );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetHitRate_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float _spellHit = (float)LuaAPI.lua_tonumber(L, 1);
                    float _defenderDodge = (float)LuaAPI.lua_tonumber(L, 2);
                    int _defenderLevel = LuaAPI.xlua_tointeger(L, 3);
                    
                        var gen_ret = miniRAID.Consts.GetHitRate( _spellHit, _defenderDodge, _defenderLevel );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCriticalRate_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float _spellCrit = (float)LuaAPI.lua_tonumber(L, 1);
                    float _defenderAntiCrit = (float)LuaAPI.lua_tonumber(L, 2);
                    int _defenderLevel = LuaAPI.xlua_tointeger(L, 3);
                    
                        var gen_ret = miniRAID.Consts.GetCriticalRate( _spellCrit, _defenderAntiCrit, _defenderLevel );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetDamage_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float _spellPower = (float)LuaAPI.lua_tonumber(L, 1);
                    int _attackerLevel = LuaAPI.xlua_tointeger(L, 2);
                    float _defense = (float)LuaAPI.lua_tonumber(L, 3);
                    int _defenderLevel = LuaAPI.xlua_tointeger(L, 4);
                    
                        var gen_ret = miniRAID.Consts.GetDamage( _spellPower, _attackerLevel, _defense, _defenderLevel );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetDefenseRate_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float _defense = (float)LuaAPI.lua_tonumber(L, 1);
                    int _defenderLevel = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = miniRAID.Consts.GetDefenseRate( _defense, _defenderLevel );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ValueFromItemLevel_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _iLvl = LuaAPI.xlua_tointeger(L, 1);
                    miniRAID.StatModifierSO.StatModTarget _entryKey;translator.Get(L, 2, out _entryKey);
                    float _val = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        var gen_ret = miniRAID.Consts.ValueFromItemLevel( _iLvl, _entryKey, _val );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BaseStatsFromLevel_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _lvl = LuaAPI.xlua_tointeger(L, 1);
                    float _growthRate = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        var gen_ret = miniRAID.Consts.BaseStatsFromLevel( _lvl, _growthRate );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetHealth_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _lvl = LuaAPI.xlua_tointeger(L, 1);
                    float _VIT = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        var gen_ret = miniRAID.Consts.GetHealth( _lvl, _VIT );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EnemyMask_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    miniRAID.Consts.UnitGroup _group;translator.Get(L, 1, out _group);
                    
                        var gen_ret = miniRAID.Consts.EnemyMask( _group );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AllyMask_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    miniRAID.Consts.UnitGroup _group;translator.Get(L, 1, out _group);
                    
                        var gen_ret = miniRAID.Consts.AllyMask( _group );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ApplyMask_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _mask = LuaAPI.xlua_tointeger(L, 1);
                    miniRAID.Consts.UnitGroup _group;translator.Get(L, 2, out _group);
                    
                        var gen_ret = miniRAID.Consts.ApplyMask( _mask, _group );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Distance_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3Int _a;translator.Get(L, 1, out _a);
                    UnityEngine.Vector3Int _b;translator.Get(L, 2, out _b);
                    
                        var gen_ret = miniRAID.Consts.Distance( _a, _b );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsPointWithinCollider_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Collider _collider = (UnityEngine.Collider)translator.GetObject(L, 1, typeof(UnityEngine.Collider));
                    UnityEngine.Vector3 _point;translator.Get(L, 2, out _point);
                    
                        var gen_ret = miniRAID.Consts.IsPointWithinCollider( _collider, _point );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_HealAggroMul(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushnumber(L, miniRAID.Consts.HealAggroMul);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_additionalAttackerLevels(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.xlua_pushinteger(L, miniRAID.Consts.additionalAttackerLevels);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_HitRangePerLevel(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushnumber(L, miniRAID.Consts.HitRangePerLevel);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BaseHit(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushnumber(L, miniRAID.Consts.BaseHit);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MaxHitAcc(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushnumber(L, miniRAID.Consts.MaxHitAcc);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CritRangePerLevel(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushnumber(L, miniRAID.Consts.CritRangePerLevel);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BaseCrit(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushnumber(L, miniRAID.Consts.BaseCrit);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MaxListenerLevels(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.xlua_pushinteger(L, miniRAID.Consts.MaxListenerLevels);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_baseStatAverageGrowth(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushnumber(L, miniRAID.Consts.baseStatAverageGrowth);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_UnitGroupToMaskBit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, miniRAID.Consts.UnitGroupToMaskBit);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Enemies(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, miniRAID.Consts.Enemies);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Allies(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, miniRAID.Consts.Allies);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_HealAggroMul(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.HealAggroMul = (float)LuaAPI.lua_tonumber(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_additionalAttackerLevels(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.additionalAttackerLevels = LuaAPI.xlua_tointeger(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_HitRangePerLevel(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.HitRangePerLevel = (float)LuaAPI.lua_tonumber(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BaseHit(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.BaseHit = (float)LuaAPI.lua_tonumber(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_MaxHitAcc(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.MaxHitAcc = (float)LuaAPI.lua_tonumber(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CritRangePerLevel(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.CritRangePerLevel = (float)LuaAPI.lua_tonumber(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BaseCrit(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.BaseCrit = (float)LuaAPI.lua_tonumber(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_MaxListenerLevels(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.MaxListenerLevels = LuaAPI.xlua_tointeger(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_baseStatAverageGrowth(RealStatePtr L)
        {
		    try {
                
			    miniRAID.Consts.baseStatAverageGrowth = (float)LuaAPI.lua_tonumber(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_UnitGroupToMaskBit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    miniRAID.Consts.UnitGroupToMaskBit = (int[])translator.GetObject(L, 1, typeof(int[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Enemies(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    miniRAID.Consts.Enemies = (int[])translator.GetObject(L, 1, typeof(int[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Allies(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    miniRAID.Consts.Allies = (int[])translator.GetObject(L, 1, typeof(int[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
