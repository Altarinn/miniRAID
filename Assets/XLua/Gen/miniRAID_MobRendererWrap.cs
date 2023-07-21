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
    public class miniRAIDMobRendererWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(miniRAID.MobRenderer);
			Utils.BeginObjectRegister(type, L, translator, 0, 11, 16, 3);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "MoveTowards", _m_MoveTowards);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UpdateStatusColor", _m_UpdateStatusColor);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnWakeUp", _m_OnWakeUp);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnNewPhase", _m_OnNewPhase);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "HealAnimation", _m_HealAnimation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DamageAnimation", _m_DamageAnimation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Killed", _m_Killed);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "WaitForAnimation", _m_WaitForAnimation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ShowMenu", _m_ShowMenu);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetActiveAnim", _m_SetActiveAnim);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnShowMobMenu", _e_OnShowMobMenu);
			
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
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isBoss", _g_get_isBoss);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "data", _g_get_data);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "animator", _g_get_animator);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "isBoss", _s_set_isBoss);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "data", _s_set_data);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "animator", _s_set_animator);
            
			
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
					
					var gen_ret = new miniRAID.MobRenderer();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobRenderer constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_MoveTowards(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3Int _targetPos;translator.Get(L, 2, out _targetPos);
                    
                        var gen_ret = gen_to_be_invoked.MoveTowards( _targetPos );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UpdateStatusColor(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.UpdateStatusColor(  );
                    
                    
                    
                    return 0;
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
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
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
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.OnNewPhase(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HealAnimation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.HealAnimation(  );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DamageAnimation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.DamageAnimation(  );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Killed(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.Consts.DamageHeal_Result _info = (miniRAID.Consts.DamageHeal_Result)translator.GetObject(L, 2, typeof(miniRAID.Consts.DamageHeal_Result));
                    
                        var gen_ret = gen_to_be_invoked.Killed( _info );
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
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
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
        static int _m_ShowMenu(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    miniRAID.UI.UnitMenu _menu = (miniRAID.UI.UnitMenu)translator.GetObject(L, 2, typeof(miniRAID.UI.UnitMenu));
                    miniRAID.UI.UIMenu_UIContainer _container = (miniRAID.UI.UIMenu_UIContainer)translator.GetObject(L, 3, typeof(miniRAID.UI.UIMenu_UIContainer));
                    
                    gen_to_be_invoked.ShowMenu( _menu, _container );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetActiveAnim(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    bool _value = LuaAPI.lua_toboolean(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.SetActiveAnim( _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_VIT(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.AggroMul);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isBoss(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isBoss);
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
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.data);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_animator(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.animator);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_isBoss(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.isBoss = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_data(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.data = (miniRAID.MobData)translator.GetObject(L, 2, typeof(miniRAID.MobData));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_animator(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.animator = (UnityEngine.Animator)translator.GetObject(L, 2, typeof(UnityEngine.Animator));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnShowMobMenu(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			miniRAID.MobRenderer gen_to_be_invoked = (miniRAID.MobRenderer)translator.FastGetCSObj(L, 1);
                miniRAID.MobRenderer.MobMenuGUIDelegate gen_delegate = translator.GetDelegate<miniRAID.MobRenderer.MobMenuGUIDelegate>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need miniRAID.MobRenderer.MobMenuGUIDelegate!");
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
			LuaAPI.luaL_error(L, "invalid arguments to miniRAID.MobRenderer.OnShowMobMenu!");
            return 0;
        }
        
		
		
    }
}
