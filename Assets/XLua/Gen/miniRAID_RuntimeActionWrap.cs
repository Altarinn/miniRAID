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
    public class miniRAIDRuntimeActionWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(miniRAID.RuntimeAction);
			Utils.BeginObjectRegister(type, L, translator, 0, 10, 12, 8);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetData", _m_SetData);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Do", _m_Do);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Activate", _m_Activate);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetTooltip", _m_GetTooltip);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RequestInUI", _m_RequestInUI);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RecalculateStats", _m_RecalculateStats);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnRecalculateStatsFinish", _m_OnRecalculateStatsFinish);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetCoolDown", _m_SetCoolDown);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnNextTurn", _m_OnNextTurn);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnAttach", _m_OnAttach);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "Level", _g_get_Level);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "MaxLevel", _g_get_MaxLevel);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "flags", _g_get_flags);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "type", _g_get_type);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "data", _g_get_data);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "costBounds", _g_get_costBounds);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "cooldownRemain", _g_get_cooldownRemain);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "power", _g_get_power);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "auxPower", _g_get_auxPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "hit", _g_get_hit);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "crit", _g_get_crit);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "shape", _g_get_shape);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "data", _s_set_data);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "costBounds", _s_set_costBounds);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "cooldownRemain", _s_set_cooldownRemain);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "power", _s_set_power);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "auxPower", _s_set_auxPower);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "hit", _s_set_hit);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "crit", _s_set_crit);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "shape", _s_set_shape);
            
			
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
				if(LuaAPI.lua_gettop(L) == 3 && translator.Assignable<miniRAID.MobData>(L, 2) && translator.Assignable<miniRAID.ActionDataSO>(L, 3))
				{
					miniRAID.MobData _source = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
					miniRAID.ActionDataSO _data = (miniRAID.ActionDataSO)translator.GetObject(L, 3, typeof(miniRAID.ActionDataSO));
					
					var gen_ret = new miniRAID.RuntimeAction(_source, _data);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.RuntimeAction constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetData(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.ActionDataSO _data = (miniRAID.ActionDataSO)translator.GetObject(L, 2, typeof(miniRAID.ActionDataSO));
                    
                    gen_to_be_invoked.SetData( _data );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Do(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.Do( _mob, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Activate(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    miniRAID.Spells.SpellTarget _target = (miniRAID.Spells.SpellTarget)translator.GetObject(L, 3, typeof(miniRAID.Spells.SpellTarget));
                    
                        var gen_ret = gen_to_be_invoked.Activate( _mob, _target );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetTooltip(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    
                        var gen_ret = gen_to_be_invoked.GetTooltip( _mob );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RequestInUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    
                        var gen_ret = gen_to_be_invoked.RequestInUI( _mob );
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
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.RecalculateStats( _mob );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnRecalculateStatsFinish(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.OnRecalculateStatsFinish( _mob );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetCoolDown(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _cd = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.SetCoolDown( _cd );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnNextTurn(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.OnNextTurn( _mob );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnAttach(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.MobData _mob = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
                    
                    gen_to_be_invoked.OnAttach( _mob );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Level(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Level);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MaxLevel(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.MaxLevel);
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
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.flags);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_type(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.type);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_data(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.data);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_costBounds(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.costBounds);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_cooldownRemain(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.cooldownRemain);
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
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.auxPower);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_hit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.hit);
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
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.crit);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_shape(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.shape);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_data(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.data = (miniRAID.ActionDataSO)translator.GetObject(L, 2, typeof(miniRAID.ActionDataSO));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_costBounds(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.costBounds = (System.Collections.Generic.List<System.ValueTuple<miniRAID.Cost, miniRAID.Cost>>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<System.ValueTuple<miniRAID.Cost, miniRAID.Cost>>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_cooldownRemain(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.cooldownRemain = LuaAPI.xlua_tointeger(L, 2);
            
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
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.power = gen_value;
            
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
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.auxPower = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_hit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.hit = gen_value;
            
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
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.crit = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_shape(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.RuntimeAction gen_to_be_invoked = (miniRAID.RuntimeAction)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.shape = (miniRAID.GridShape)translator.GetObject(L, 2, typeof(miniRAID.GridShape));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
