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
    public class miniRAIDMobDataWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(miniRAID.MobData);
			Utils.BeginObjectRegister(type, L, translator, 0, 38, 72, 52);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddAction", _m_AddAction);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetAction", _m_GetAction);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "MoveToCoroutine", _m_MoveToCoroutine);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ActionPrecheck", _m_ActionPrecheck);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ActionBegin", _m_ActionBegin);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ActionDone", _m_ActionDone);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetModifiedCost", _m_GetModifiedCost);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CheckCost", _m_CheckCost);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ApplyCost", _m_ApplyCost);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DoAction", _m_DoAction);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DoActionWithDefaultCosts", _m_DoActionWithDefaultCosts);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "WaitForAnimation", _m_WaitForAnimation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Init", _m_Init);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UseActionPoint", _m_UseActionPoint);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnWakeUp", _m_OnWakeUp);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnNewPhase", _m_OnNewPhase);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetGCD", _m_SetGCD);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsInGCD", _m_IsInGCD);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddListener", _m_AddListener);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddBuff", _m_AddBuff);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveListener", _m_RemoveListener);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "FindListener", _m_FindListener);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetResource", _m_GetResource);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetActive", _m_SetActive);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TryAutoEndTurn", _m_TryAutoEndTurn);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "_OnNextTurn", _m__OnNextTurn);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ReceiveDamage", _m_ReceiveDamage);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RecalculateStats", _m_RecalculateStats);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RefreshActions", _m_RefreshActions);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnBaseStatCalculation", _e_OnBaseStatCalculation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnStatCalculation", _e_OnStatCalculation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnActionStatCalculation", _e_OnActionStatCalculation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnStatCalculationFinish", _e_OnStatCalculationFinish);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnMobMoved", _e_OnMobMoved);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnQueryActions", _e_OnQueryActions);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnShowMobMenu", _e_OnShowMobMenu);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnCostQuery", _e_OnCostQuery);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnCostQueryDisplay", _e_OnCostQueryDisplay);
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "Position", _g_get_Position);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isActive", _g_get_isActive);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isDead", _g_get_isDead);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isControllable", _g_get_isControllable);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "level", _g_get_level);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "race", _g_get_race);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "job", _g_get_job);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "actionPoints", _g_get_actionPoints);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "VIT", _g_get_VIT);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "STR", _g_get_STR);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "MAG", _g_get_MAG);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "INT", _g_get_INT);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "DEX", _g_get_DEX);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "TEC", _g_get_TEC);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "AttackPower", _g_get_AttackPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "SpellPower", _g_get_SpellPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "HealPower", _g_get_HealPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "BuffPower", _g_get_BuffPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Def", _g_get_Def);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "SpDef", _g_get_SpDef);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "AggroMul", _g_get_AggroMul);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "baseDescriptor", _g_get_baseDescriptor);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "enemyDebug", _g_get_enemyDebug);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "initialized", _g_get_initialized);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "movementType", _g_get_movementType);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "gridBody", _g_get_gridBody);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "unitGroup", _g_get_unitGroup);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "nickname", _g_get_nickname);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "apRecovery", _g_get_apRecovery);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "apMax", _g_get_apMax);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "attackPower", _g_get_attackPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "spellPower", _g_get_spellPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "healPower", _g_get_healPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "buffPower", _g_get_buffPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "defense", _g_get_defense);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "spDefense", _g_get_spDefense);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "hitAcc", _g_get_hitAcc);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "dodge", _g_get_dodge);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "crit", _g_get_crit);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "antiCrit", _g_get_antiCrit);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "extraRange", _g_get_extraRange);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "aggroMul", _g_get_aggroMul);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "GCDstatus", _g_get_GCDstatus);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "baseStats", _g_get_baseStats);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "battleStats", _g_get_battleStats);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "health", _g_get_health);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "maxHealth", _g_get_maxHealth);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "damageShield", _g_get_damageShield);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mainWeapon", _g_get_mainWeapon);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "subWeapon", _g_get_subWeapon);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "listeners", _g_get_listeners);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "actions", _g_get_actions);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "availableActions", _g_get_availableActions);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mobRenderer", _g_get_mobRenderer);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnWakeup", _g_get_OnWakeup);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnAgentWakeUp", _g_get_OnAgentWakeUp);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnNextTurn", _g_get_OnNextTurn);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnDealDmg", _g_get_OnDealDmg);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnDamageDealt", _g_get_OnDamageDealt);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnDealHeal", _g_get_OnDealHeal);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnHealDealt", _g_get_OnHealDealt);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnReceiveDamage", _g_get_OnReceiveDamage);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnDamageReceived", _g_get_OnDamageReceived);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnReceiveHeal", _g_get_OnReceiveHeal);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnHealReceived", _g_get_OnHealReceived);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnKill", _g_get_OnKill);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnPreDeath", _g_get_OnPreDeath);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnRealDeath", _g_get_OnRealDeath);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnActionChosen", _g_get_OnActionChosen);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnActionPrecast", _g_get_OnActionPrecast);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnActionPostcast", _g_get_OnActionPostcast);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnCostApply", _g_get_OnCostApply);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "Position", _s_set_Position);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "baseDescriptor", _s_set_baseDescriptor);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "enemyDebug", _s_set_enemyDebug);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "initialized", _s_set_initialized);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "movementType", _s_set_movementType);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "gridBody", _s_set_gridBody);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "unitGroup", _s_set_unitGroup);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "nickname", _s_set_nickname);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "apRecovery", _s_set_apRecovery);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "apMax", _s_set_apMax);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "attackPower", _s_set_attackPower);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "spellPower", _s_set_spellPower);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "healPower", _s_set_healPower);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "buffPower", _s_set_buffPower);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "defense", _s_set_defense);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "spDefense", _s_set_spDefense);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "hitAcc", _s_set_hitAcc);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "dodge", _s_set_dodge);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "crit", _s_set_crit);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "antiCrit", _s_set_antiCrit);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "extraRange", _s_set_extraRange);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "aggroMul", _s_set_aggroMul);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "GCDstatus", _s_set_GCDstatus);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "baseStats", _s_set_baseStats);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "battleStats", _s_set_battleStats);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "health", _s_set_health);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "maxHealth", _s_set_maxHealth);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "damageShield", _s_set_damageShield);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "mainWeapon", _s_set_mainWeapon);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "subWeapon", _s_set_subWeapon);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "listeners", _s_set_listeners);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "actions", _s_set_actions);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "availableActions", _s_set_availableActions);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "mobRenderer", _s_set_mobRenderer);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnWakeup", _s_set_OnWakeup);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnAgentWakeUp", _s_set_OnAgentWakeUp);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnNextTurn", _s_set_OnNextTurn);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnDealDmg", _s_set_OnDealDmg);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnDamageDealt", _s_set_OnDamageDealt);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnDealHeal", _s_set_OnDealHeal);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnHealDealt", _s_set_OnHealDealt);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnReceiveDamage", _s_set_OnReceiveDamage);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnDamageReceived", _s_set_OnDamageReceived);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnReceiveHeal", _s_set_OnReceiveHeal);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnHealReceived", _s_set_OnHealReceived);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnKill", _s_set_OnKill);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnPreDeath", _s_set_OnPreDeath);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnRealDeath", _s_set_OnRealDeath);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnActionChosen", _s_set_OnActionChosen);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnActionPrecast", _s_set_OnActionPrecast);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnActionPostcast", _s_set_OnActionPostcast);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnCostApply", _s_set_OnCostApply);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new miniRAID.MobData();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddAction(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.ActionDataSO _actSO = (miniRAID.ActionDataSO)translator.GetObject(L, 2, typeof(miniRAID.ActionDataSO));
                    
                        var gen_ret = gen_to_be_invoked.AddAction( _actSO );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetAction(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _ActionName = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetAction( _ActionName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<miniRAID.ActionDataSO>(L, 2)) 
                {
                    miniRAID.ActionDataSO _action = (miniRAID.ActionDataSO)translator.GetObject(L, 2, typeof(miniRAID.ActionDataSO));
                    
                        var gen_ret = gen_to_be_invoked.GetAction( _action );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.GetAction!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_MoveToCoroutine(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3Int _targetPos;translator.Get(L, 2, out _targetPos);
                    miniRAID.GridPath _path = (miniRAID.GridPath)translator.GetObject(L, 3, typeof(miniRAID.GridPath));
                    
                        var gen_ret = gen_to_be_invoked.MoveToCoroutine( _targetPos, _path );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ActionPrecheck(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.RuntimeAction _raction = (miniRAID.RuntimeAction)translator.GetObject(L, 2, typeof(miniRAID.RuntimeAction));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.ActionPrecheck( _raction, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ActionBegin(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.RuntimeAction _raction = (miniRAID.RuntimeAction)translator.GetObject(L, 2, typeof(miniRAID.RuntimeAction));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.ActionBegin( _raction, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ActionDone(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.RuntimeAction _raction = (miniRAID.RuntimeAction)translator.GetObject(L, 2, typeof(miniRAID.RuntimeAction));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.ActionDone( _raction, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetModifiedCost(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Cost _cost = (miniRAID.Cost)translator.GetObject(L, 2, typeof(miniRAID.Cost));
                    miniRAID.RuntimeAction _ract = (miniRAID.RuntimeAction)translator.GetObject(L, 3, typeof(miniRAID.RuntimeAction));
                    
                        var gen_ret = gen_to_be_invoked.GetModifiedCost( _cost, _ract );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CheckCost(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Cost _cost = (miniRAID.Cost)translator.GetObject(L, 2, typeof(miniRAID.Cost));
                    miniRAID.RuntimeAction _ract = (miniRAID.RuntimeAction)translator.GetObject(L, 3, typeof(miniRAID.RuntimeAction));
                    
                        var gen_ret = gen_to_be_invoked.CheckCost( _cost, _ract );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ApplyCost(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Cost _cost = (miniRAID.Cost)translator.GetObject(L, 2, typeof(miniRAID.Cost));
                    miniRAID.RuntimeAction _ract = (miniRAID.RuntimeAction)translator.GetObject(L, 3, typeof(miniRAID.RuntimeAction));
                    
                        var gen_ret = gen_to_be_invoked.ApplyCost( _cost, _ract );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DoAction(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& translator.Assignable<miniRAID.ActionDataSO>(L, 2)&& translator.Assignable<miniRAID.Spells.SpellTarget>(L, 3)&& translator.Assignable<System.Collections.Generic.List<miniRAID.Cost>>(L, 4)) 
                {
                    miniRAID.ActionDataSO _actionSO = (miniRAID.ActionDataSO)translator.GetObject(L, 2, typeof(miniRAID.ActionDataSO));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    System.Collections.Generic.List<miniRAID.Cost> _costs = (System.Collections.Generic.List<miniRAID.Cost>)translator.GetObject(L, 4, typeof(System.Collections.Generic.List<miniRAID.Cost>));
                    
                        var gen_ret = gen_to_be_invoked.DoAction( _actionSO, _target, _costs );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<miniRAID.ActionDataSO>(L, 2)&& translator.Assignable<miniRAID.Spells.SpellTarget>(L, 3)) 
                {
                    miniRAID.ActionDataSO _actionSO = (miniRAID.ActionDataSO)translator.GetObject(L, 2, typeof(miniRAID.ActionDataSO));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.DoAction( _actionSO, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 4&& translator.Assignable<miniRAID.RuntimeAction>(L, 2)&& translator.Assignable<miniRAID.Spells.SpellTarget>(L, 3)&& translator.Assignable<System.Collections.Generic.List<miniRAID.Cost>>(L, 4)) 
                {
                    miniRAID.RuntimeAction _raction = (miniRAID.RuntimeAction)translator.GetObject(L, 2, typeof(miniRAID.RuntimeAction));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    System.Collections.Generic.List<miniRAID.Cost> _costs = (System.Collections.Generic.List<miniRAID.Cost>)translator.GetObject(L, 4, typeof(System.Collections.Generic.List<miniRAID.Cost>));
                    
                        var gen_ret = gen_to_be_invoked.DoAction( _raction, _target, _costs );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<miniRAID.RuntimeAction>(L, 2)&& translator.Assignable<miniRAID.Spells.SpellTarget>(L, 3)) 
                {
                    miniRAID.RuntimeAction _raction = (miniRAID.RuntimeAction)translator.GetObject(L, 2, typeof(miniRAID.RuntimeAction));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.DoAction( _raction, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.DoAction!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DoActionWithDefaultCosts(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.RuntimeAction _raction = (miniRAID.RuntimeAction)translator.GetObject(L, 2, typeof(miniRAID.RuntimeAction));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.DoActionWithDefaultCosts( _raction, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitForAnimation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _animationState = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.WaitForAnimation( _animationState );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Init(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Init(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UseActionPoint(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    float _v = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.UseActionPoint( _v );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnWakeUp(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.OnWakeUp(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnNewPhase(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.OnNewPhase(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetGCD(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.GCDGroup _group;translator.Get(L, 2, out _group);
                    
                    gen_to_be_invoked.SetGCD( _group );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsInGCD(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.GCDGroup _group;translator.Get(L, 2, out _group);
                    
                        var gen_ret = gen_to_be_invoked.IsInGCD( _group );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddListener(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<miniRAID.MobListenerSO>(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    miniRAID.MobListenerSO _listenerSO = (miniRAID.MobListenerSO)translator.GetObject(L, 2, typeof(miniRAID.MobListenerSO));
                    bool _addToList = LuaAPI.lua_toboolean(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.AddListener( _listenerSO, _addToList );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<miniRAID.MobListenerSO>(L, 2)) 
                {
                    miniRAID.MobListenerSO _listenerSO = (miniRAID.MobListenerSO)translator.GetObject(L, 2, typeof(miniRAID.MobListenerSO));
                    
                        var gen_ret = gen_to_be_invoked.AddListener( _listenerSO );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<miniRAID.MobListener>(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    miniRAID.MobListener _listener = (miniRAID.MobListener)translator.GetObject(L, 2, typeof(miniRAID.MobListener));
                    bool _addToList = LuaAPI.lua_toboolean(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.AddListener( _listener, _addToList );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<miniRAID.MobListener>(L, 2)) 
                {
                    miniRAID.MobListener _listener = (miniRAID.MobListener)translator.GetObject(L, 2, typeof(miniRAID.MobListener));
                    
                        var gen_ret = gen_to_be_invoked.AddListener( _listener );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.AddListener!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddBuff(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<miniRAID.Buff.Buff>(L, 2)) 
                {
                    miniRAID.Buff.Buff _buff = (miniRAID.Buff.Buff)translator.GetObject(L, 2, typeof(miniRAID.Buff.Buff));
                    
                        var gen_ret = gen_to_be_invoked.AddBuff( _buff );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<miniRAID.Buff.BuffSO>(L, 2)&& translator.Assignable<miniRAID.MobData>(L, 3)) 
                {
                    miniRAID.Buff.BuffSO _buff = (miniRAID.Buff.BuffSO)translator.GetObject(L, 2, typeof(miniRAID.Buff.BuffSO));
                    miniRAID.MobData _source = (miniRAID.MobData)translator.GetObject(L, 3, typeof(miniRAID.MobData));
                    
                        var gen_ret = gen_to_be_invoked.AddBuff( _buff, _source );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.AddBuff!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveListener(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<miniRAID.MobListenerSO>(L, 2)) 
                {
                    miniRAID.MobListenerSO _listenerSO = (miniRAID.MobListenerSO)translator.GetObject(L, 2, typeof(miniRAID.MobListenerSO));
                    
                    gen_to_be_invoked.RemoveListener( _listenerSO );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<miniRAID.MobListener>(L, 2)) 
                {
                    miniRAID.MobListener _listener = (miniRAID.MobListener)translator.GetObject(L, 2, typeof(miniRAID.MobListener));
                    
                    gen_to_be_invoked.RemoveListener( _listener );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.RemoveListener!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindListener(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<miniRAID.MobListenerSO>(L, 2)) 
                {
                    miniRAID.MobListenerSO _data = (miniRAID.MobListenerSO)translator.GetObject(L, 2, typeof(miniRAID.MobListenerSO));
                    
                        var gen_ret = gen_to_be_invoked.FindListener( _data );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<System.Predicate<miniRAID.MobListener>>(L, 2)) 
                {
                    System.Predicate<miniRAID.MobListener> _condition = translator.GetDelegate<System.Predicate<miniRAID.MobListener>>(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.FindListener( _condition );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.FindListener!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetResource(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Cost _cost = (miniRAID.Cost)translator.GetObject(L, 2, typeof(miniRAID.Cost));
                    
                    gen_to_be_invoked.GetResource( _cost );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetActive(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    bool _value = LuaAPI.lua_toboolean(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.SetActive( _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TryAutoEndTurn(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.TryAutoEndTurn(  );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m__OnNextTurn(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked._OnNextTurn(  );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ReceiveDamage(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Consts.DamageHeal_FrontEndInput _info;translator.Get(L, 2, out _info);
                    miniRAID.Consts.DamageHeal_Result _result = (miniRAID.Consts.DamageHeal_Result)translator.GetObject(L, 3, typeof(miniRAID.Consts.DamageHeal_Result));
                    
                        var gen_ret = gen_to_be_invoked.ReceiveDamage( _info, _result );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RecalculateStats(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.RecalculateStats(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RefreshActions(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.RefreshActions(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Position(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Position);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isActive(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isActive);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isDead(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isDead);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isControllable(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isControllable);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_level(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.level);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_race(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.race);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_job(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.job);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_actionPoints(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.actionPoints);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_VIT(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.VIT);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_STR(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.STR);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MAG(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.MAG);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_INT(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.INT);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_DEX(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.DEX);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TEC(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.TEC);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_AttackPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.AttackPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_SpellPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.SpellPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_HealPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.HealPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BuffPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.BuffPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Def(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Def);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_SpDef(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.SpDef);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_AggroMul(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.AggroMul);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_baseDescriptor(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.baseDescriptor);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_enemyDebug(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.enemyDebug);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_initialized(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.initialized);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_movementType(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.movementType);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_gridBody(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.gridBody);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_unitGroup(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.unitGroup);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_nickname(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.nickname);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_apRecovery(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.apRecovery);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_apMax(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.apMax);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_attackPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.attackPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_spellPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.spellPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_healPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.healPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_buffPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.buffPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_defense(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.defense);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_spDefense(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.spDefense);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_hitAcc(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.hitAcc);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_dodge(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.dodge);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_crit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.crit);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_antiCrit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.antiCrit);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_extraRange(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.extraRange);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_aggroMul(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.aggroMul);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_GCDstatus(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.GCDstatus);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_baseStats(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.baseStats);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_battleStats(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.battleStats);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_health(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.health);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_maxHealth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.maxHealth);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_damageShield(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.damageShield);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mainWeapon(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.mainWeapon);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_subWeapon(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.subWeapon);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_listeners(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.listeners);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_actions(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.actions);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_availableActions(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.availableActions);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mobRenderer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.mobRenderer);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnWakeup(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnWakeup);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnAgentWakeUp(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnAgentWakeUp);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnNextTurn(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnNextTurn);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnDealDmg(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnDealDmg);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnDamageDealt(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnDamageDealt);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnDealHeal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnDealHeal);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnHealDealt(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnHealDealt);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnReceiveDamage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnReceiveDamage);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnDamageReceived(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnDamageReceived);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnReceiveHeal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnReceiveHeal);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnHealReceived(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnHealReceived);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnKill(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnKill);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnPreDeath(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnPreDeath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnRealDeath(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnRealDeath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnActionChosen(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnActionChosen);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnActionPrecast(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnActionPrecast);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnActionPostcast(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnActionPostcast);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnCostApply(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnCostApply);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Position(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                UnityEngine.Vector3Int gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.Position = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_baseDescriptor(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.baseDescriptor = (miniRAID.BaseMobDescriptorSO)translator.GetObject(L, 2, typeof(miniRAID.BaseMobDescriptorSO));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_enemyDebug(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.enemyDebug = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_initialized(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.initialized = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_movementType(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MovementType gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.movementType = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_gridBody(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.gridBody = (miniRAID.GridShape)translator.GetObject(L, 2, typeof(miniRAID.GridShape));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_unitGroup(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.Consts.UnitGroup gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.unitGroup = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_nickname(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.nickname = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_apRecovery(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.apRecovery = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_apMax(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.apMax = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_attackPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.attackPower = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_spellPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.spellPower = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_healPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.healPower = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_buffPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.buffPower = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_defense(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.defense = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_spDefense(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.spDefense = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_hitAcc(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.hitAcc = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_dodge(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.dodge = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_crit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.crit = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_antiCrit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.antiCrit = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_extraRange(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.extraRange = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_aggroMul(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.aggroMul = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_GCDstatus(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.GCDstatus = (System.Collections.Generic.HashSet<miniRAID.GCDGroup>)translator.GetObject(L, 2, typeof(System.Collections.Generic.HashSet<miniRAID.GCDGroup>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_baseStats(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.Consts.BaseStats gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.baseStats = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_battleStats(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.Consts.BattleStats gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.battleStats = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_health(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.health = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_maxHealth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.maxHealth = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_damageShield(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.damageShield = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mainWeapon(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mainWeapon = (miniRAID.Weapon.Weapon)translator.GetObject(L, 2, typeof(miniRAID.Weapon.Weapon));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_subWeapon(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.subWeapon = (miniRAID.Weapon.Weapon)translator.GetObject(L, 2, typeof(miniRAID.Weapon.Weapon));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_listeners(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.listeners = (System.Collections.Generic.List<miniRAID.MobListener>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<miniRAID.MobListener>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_actions(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.actions = (System.Collections.Generic.List<miniRAID.RuntimeAction>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<miniRAID.RuntimeAction>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_availableActions(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.availableActions = (System.Collections.Generic.HashSet<miniRAID.RuntimeAction>)translator.GetObject(L, 2, typeof(System.Collections.Generic.HashSet<miniRAID.RuntimeAction>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mobRenderer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mobRenderer = (miniRAID.MobRenderer)translator.GetObject(L, 2, typeof(miniRAID.MobRenderer));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnWakeup(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnWakeup = (miniRAID.CoroutineEvent<miniRAID.MobData>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnAgentWakeUp(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnAgentWakeUp = (miniRAID.CoroutineEvent<miniRAID.MobData>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnNextTurn(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnNextTurn = (miniRAID.CoroutineEvent<miniRAID.MobData>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnDealDmg(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnDealDmg = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_FrontEndInput_ByRef>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_FrontEndInput_ByRef>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnDamageDealt(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnDamageDealt = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnDealHeal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnDealHeal = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_FrontEndInput_ByRef>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_FrontEndInput_ByRef>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnHealDealt(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnHealDealt = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnReceiveDamage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnReceiveDamage = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_FrontEndInput_ByRef>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_FrontEndInput_ByRef>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnDamageReceived(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnDamageReceived = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnReceiveHeal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnReceiveHeal = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_FrontEndInput_ByRef>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_FrontEndInput_ByRef>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnHealReceived(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnHealReceived = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnKill(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnKill = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnPreDeath(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnPreDeath = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnRealDeath(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnRealDeath = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.Consts.DamageHeal_Result>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnActionChosen(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnActionChosen = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.RuntimeAction, miniRAID.Spells.SpellTarget>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.RuntimeAction, miniRAID.Spells.SpellTarget>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnActionPrecast(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnActionPrecast = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.RuntimeAction, miniRAID.Spells.SpellTarget>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.RuntimeAction, miniRAID.Spells.SpellTarget>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnActionPostcast(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnActionPostcast = (miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.RuntimeAction, miniRAID.Spells.SpellTarget>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.MobData, miniRAID.RuntimeAction, miniRAID.Spells.SpellTarget>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnCostApply(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnCostApply = (miniRAID.CoroutineEvent<miniRAID.Cost, miniRAID.RuntimeAction, miniRAID.MobData>)translator.GetObject(L, 2, typeof(miniRAID.CoroutineEvent<miniRAID.Cost, miniRAID.RuntimeAction, miniRAID.MobData>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnBaseStatCalculation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobArgumentDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobArgumentDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobArgumentDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnBaseStatCalculation += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnBaseStatCalculation -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnBaseStatCalculation!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnStatCalculation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobArgumentDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobArgumentDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobArgumentDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnStatCalculation += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnStatCalculation -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnStatCalculation!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnActionStatCalculation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobArgumentDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobArgumentDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobArgumentDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnActionStatCalculation += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnActionStatCalculation -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnActionStatCalculation!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnStatCalculationFinish(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobArgumentDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobArgumentDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobArgumentDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnStatCalculationFinish += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnStatCalculationFinish -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnStatCalculationFinish!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnMobMoved(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobArgumentDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobArgumentDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobArgumentDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnMobMoved += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnMobMoved -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnMobMoved!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnQueryActions(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobActionQueryDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobActionQueryDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobActionQueryDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnQueryActions += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnQueryActions -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnQueryActions!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnShowMobMenu(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.MobMenuGUIDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.MobMenuGUIDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.MobMenuGUIDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnShowMobMenu += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnShowMobMenu -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnShowMobMenu!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnCostQuery(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.CostQueryDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.CostQueryDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.CostQueryDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnCostQuery += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnCostQuery -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnCostQuery!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnCostQueryDisplay(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobData gen_to_be_invoked = (miniRAID.MobData)translator.FastGetCSObj(L, 1);
                miniRAID.MobData.CostQueryDelegate gen_delegate = translator.GetDelegate<miniRAID.MobData.CostQueryDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobData.CostQueryDelegate!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnCostQueryDisplay += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnCostQueryDisplay -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobData.OnCostQueryDisplay!");
            return 0;
        }
        
		
		
    }
}
