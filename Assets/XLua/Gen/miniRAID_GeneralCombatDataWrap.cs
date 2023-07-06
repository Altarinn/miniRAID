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
    public class miniRAIDGeneralCombatDataWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(miniRAID.GeneralCombatData);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 6, 6);
			
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "power", _g_get_power);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "auxPower", _g_get_auxPower);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "shape", _g_get_shape);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "ract", _g_get_ract);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "gameObjects", _g_get_gameObjects);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "sprites", _g_get_sprites);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "power", _s_set_power);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "auxPower", _s_set_auxPower);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "shape", _s_set_shape);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "ract", _s_set_ract);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "gameObjects", _s_set_gameObjects);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "sprites", _s_set_sprites);
            
			
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
					
					var gen_ret = new miniRAID.GeneralCombatData();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 2 && LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2))
				{
					float _p = (float)LuaAPI.lua_tonumber(L, 2);
					
					var gen_ret = new miniRAID.GeneralCombatData(_p);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 2 && translator.Assignable<miniRAID.dNumber>(L, 2))
				{
					miniRAID.dNumber _p;translator.Get(L, 2, out _p);
					
					var gen_ret = new miniRAID.GeneralCombatData(_p);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.GeneralCombatData constructor!");
            
        }
        
		
        
		
        
        
        
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_power(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.auxPower);
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
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.shape);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_ract(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.ract);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_gameObjects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.gameObjects);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_sprites(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.sprites);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_power(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
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
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                miniRAID.dNumber gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.auxPower = gen_value;
            
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
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.shape = (miniRAID.GridShape)translator.GetObject(L, 2, typeof(miniRAID.GridShape));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_ract(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.ract = (miniRAID.RuntimeAction)translator.GetObject(L, 2, typeof(miniRAID.RuntimeAction));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_gameObjects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.gameObjects = (UnityEngine.GameObject[])translator.GetObject(L, 2, typeof(UnityEngine.GameObject[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_sprites(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.GeneralCombatData gen_to_be_invoked = (miniRAID.GeneralCombatData)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.sprites = (UnityEngine.Sprite[])translator.GetObject(L, 2, typeof(UnityEngine.Sprite[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
