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
    public class miniRAIDActionDataSOWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(miniRAID.ActionDataSO);
			Utils.BeginObjectRegister(type, L, translator, 0, 7, 9, 9);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CheckCosts", _m_CheckCosts);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DoCosts", _m_DoCosts);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Equipable", _m_Equipable);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Check", _m_Check);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CheckWithTargets", _m_CheckWithTargets);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetRequesterTypes", _m_GetRequesterTypes);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnPerform", _m_OnPerform);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "ActionName", _g_get_ActionName);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "maxLevel", _g_get_maxLevel);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Description", _g_get_Description);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "flags", _g_get_flags);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "power", _g_get_power);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "auxPower", _g_get_auxPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "costs", _g_get_costs);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isActivelyUsed", _g_get_isActivelyUsed);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Requester", _g_get_Requester);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "ActionName", _s_set_ActionName);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "maxLevel", _s_set_maxLevel);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Description", _s_set_Description);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "flags", _s_set_flags);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "power", _s_set_power);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "auxPower", _s_set_auxPower);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "costs", _s_set_costs);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "isActivelyUsed", _s_set_isActivelyUsed);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Requester", _s_set_Requester);
            
			
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
					
					var gen_ret = new miniRAID.ActionDataSO();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.ActionDataSO constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CheckCosts(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobRenderer _mobRenderer = (miniRAID.MobRenderer)translator.GetObject(L, 2, typeof(miniRAID.MobRenderer));
                    miniRAID.RuntimeAction _ract = (miniRAID.RuntimeAction)translator.GetObject(L, 3, typeof(miniRAID.RuntimeAction));
                    
                        var gen_ret = gen_to_be_invoked.CheckCosts( _mobRenderer, _ract );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DoCosts(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobRenderer _mobRenderer = (miniRAID.MobRenderer)translator.GetObject(L, 2, typeof(miniRAID.MobRenderer));
                    
                    gen_to_be_invoked.DoCosts( _mobRenderer );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Equipable(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mobdata = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    
                        var gen_ret = gen_to_be_invoked.Equipable( _mobdata );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Check(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    
                        var gen_ret = gen_to_be_invoked.Check( _mob );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CheckWithTargets(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.CheckWithTargets( _mob, _target );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetRequesterTypes(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetRequesterTypes(  );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnPerform(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.RuntimeAction _ract = (miniRAID.RuntimeAction)translator.GetObject(L, 2, typeof(miniRAID.RuntimeAction));
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 3, typeof(miniRAID.MobData));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 4, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.OnPerform( _ract, _mob, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_ActionName(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.ActionName);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_maxLevel(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.maxLevel);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Description(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.Description);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_flags(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.flags);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_power(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.power);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_auxPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.auxPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_costs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.costs);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isActivelyUsed(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.isActivelyUsed);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Requester(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Requester);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_ActionName(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.ActionName = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_maxLevel(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.maxLevel = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Description(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Description = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_flags(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                miniRAID.Consts.ActionFlags gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.flags = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_power(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.power = (miniRAID.PowerGetter)translator.GetObject(L, 2, typeof(miniRAID.PowerGetter));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_auxPower(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.auxPower = (miniRAID.PowerGetter)translator.GetObject(L, 2, typeof(miniRAID.PowerGetter));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_costs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.costs = (System.Collections.Generic.Dictionary<miniRAID.Cost.Type, miniRAID.LuaBoundedGetter<System.ValueTuple<miniRAID.MobData, miniRAID.Spells.SpellTarget>, miniRAID.MobData, double>>)translator.GetObject(L, 2, typeof(System.Collections.Generic.Dictionary<miniRAID.Cost.Type, miniRAID.LuaBoundedGetter<System.ValueTuple<miniRAID.MobData, miniRAID.Spells.SpellTarget>, miniRAID.MobData, double>>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_isActivelyUsed(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.isActivelyUsed = (miniRAID.LuaGetter<miniRAID.MobData, bool>)translator.GetObject(L, 2, typeof(miniRAID.LuaGetter<miniRAID.MobData, bool>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Requester(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.ActionDataSO gen_to_be_invoked = (miniRAID.ActionDataSO)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Requester = (miniRAID.UI.TargetRequester.TargetRequesterBase)translator.GetObject(L, 2, typeof(miniRAID.UI.TargetRequester.TargetRequesterBase));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
