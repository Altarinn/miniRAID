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
    public class miniRAIDdNumberWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(miniRAID.dNumber);
			Utils.BeginObjectRegister(type, L, translator, 2, 5, 8, 7);
			Utils.RegisterFunc(L, Utils.OBJ_META_IDX, "__add", __AddMeta);
            Utils.RegisterFunc(L, Utils.OBJ_META_IDX, "__mul", __MulMeta);
            
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Add", _m_Add);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Substract", _m_Substract);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Mul", _m_Mul);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "MulMul", _m_MulMul);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ToString", _m_ToString);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "Value", _g_get_Value);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "valueType", _g_get_valueType);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "staticValue", _g_get_staticValue);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "source", _g_get_source);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "baseValue", _g_get_baseValue);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "addedValue", _g_get_addedValue);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "addMultiplier", _g_get_addMultiplier);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mulMultiplier", _g_get_mulMultiplier);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "valueType", _s_set_valueType);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "staticValue", _s_set_staticValue);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "source", _s_set_source);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "baseValue", _s_set_baseValue);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "addedValue", _s_set_addedValue);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "addMultiplier", _s_set_addMultiplier);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "mulMultiplier", _s_set_mulMultiplier);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 4, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateStatic", _m_CreateStatic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateComposite", _m_CreateComposite_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "NewWithBase", _m_NewWithBase_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				
				if (LuaAPI.lua_gettop(L) == 1)
				{
				    translator.Push(L, default(miniRAID.dNumber));
			        return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.dNumber constructor!");
            
        }
        
		
        
		
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __AddMeta(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
			
				if (translator.Assignable<miniRAID.dNumber>(L, 1) && translator.Assignable<miniRAID.dNumber>(L, 2))
				{
					miniRAID.dNumber leftside;translator.Get(L, 1, out leftside);
					miniRAID.dNumber rightside;translator.Get(L, 2, out rightside);
					
					translator.Push(L, leftside + rightside);
					
					return 1;
				}
            
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to right hand of + operator, need miniRAID.dNumber!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __MulMeta(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
			
				if (translator.Assignable<miniRAID.dNumber>(L, 1) && translator.Assignable<miniRAID.dNumber>(L, 2))
				{
					miniRAID.dNumber leftside;translator.Get(L, 1, out leftside);
					miniRAID.dNumber rightside;translator.Get(L, 2, out rightside);
					
					translator.Push(L, leftside * rightside);
					
					return 1;
				}
            
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to right hand of * operator, need miniRAID.dNumber!");
            
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateStatic_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    double _v = LuaAPI.lua_tonumber(L, 1);
                    string _source = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = miniRAID.dNumber.CreateStatic( _v, _source );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double _v = LuaAPI.lua_tonumber(L, 1);
                    
                        var gen_ret = miniRAID.dNumber.CreateStatic( _v );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.dNumber.CreateStatic!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateComposite_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    double _baseValue = LuaAPI.lua_tonumber(L, 1);
                    string _source = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = miniRAID.dNumber.CreateComposite( _baseValue, _source );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double _baseValue = LuaAPI.lua_tonumber(L, 1);
                    
                        var gen_ret = miniRAID.dNumber.CreateComposite( _baseValue );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to miniRAID.dNumber.CreateComposite!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Add(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
            
            
                
                {
                    miniRAID.dNumber _v;translator.Get(L, 2, out _v);
                    
                    gen_to_be_invoked.Add( _v );
                    
                    
                        translator.Update(L, 1, gen_to_be_invoked);
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Substract(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
            
            
                
                {
                    miniRAID.dNumber _v;translator.Get(L, 2, out _v);
                    
                    gen_to_be_invoked.Substract( _v );
                    
                    
                        translator.Update(L, 1, gen_to_be_invoked);
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Mul(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
            
            
                
                {
                    miniRAID.dNumber _f;translator.Get(L, 2, out _f);
                    
                    gen_to_be_invoked.Mul( _f );
                    
                    
                        translator.Update(L, 1, gen_to_be_invoked);
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_MulMul(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
            
            
                
                {
                    miniRAID.dNumber _multiplier;translator.Get(L, 2, out _multiplier);
                    
                    gen_to_be_invoked.MulMul( _multiplier );
                    
                    
                        translator.Update(L, 1, gen_to_be_invoked);
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_NewWithBase_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    miniRAID.dNumber _baseVal;translator.Get(L, 1, out _baseVal);
                    
                        var gen_ret = miniRAID.dNumber.NewWithBase( _baseVal );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToString(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.ToString(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                        translator.Update(L, 1, gen_to_be_invoked);
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Value(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.Value);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_valueType(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                translator.Push(L, gen_to_be_invoked.valueType);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_staticValue(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.staticValue);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_source(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                translator.Push(L, gen_to_be_invoked.source);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_baseValue(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.baseValue);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_addedValue(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.addedValue);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_addMultiplier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.addMultiplier);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mulMultiplier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.mulMultiplier);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_valueType(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                miniRAID.dNumber.ValueType gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.valueType = gen_value;
            
                translator.Update(L, 1, gen_to_be_invoked);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_staticValue(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                gen_to_be_invoked.staticValue = LuaAPI.lua_tonumber(L, 2);
            
                translator.Update(L, 1, gen_to_be_invoked);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_source(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                miniRAID.dNumber.DynamicValueSource gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.source = gen_value;
            
                translator.Update(L, 1, gen_to_be_invoked);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_baseValue(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                gen_to_be_invoked.baseValue = LuaAPI.lua_tonumber(L, 2);
            
                translator.Update(L, 1, gen_to_be_invoked);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_addedValue(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                gen_to_be_invoked.addedValue = LuaAPI.lua_tonumber(L, 2);
            
                translator.Update(L, 1, gen_to_be_invoked);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_addMultiplier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                gen_to_be_invoked.addMultiplier = LuaAPI.lua_tonumber(L, 2);
            
                translator.Update(L, 1, gen_to_be_invoked);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mulMultiplier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                miniRAID.dNumber gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                gen_to_be_invoked.mulMultiplier = LuaAPI.lua_tonumber(L, 2);
            
                translator.Update(L, 1, gen_to_be_invoked);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
