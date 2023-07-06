﻿#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using System;


namespace XLua
{
    public static partial class ObjectTranslator_Gen
    {
		public static void Init(LuaEnv luaenv, ObjectTranslator translator)
		{
		
			XLuaTestPedding_TypeID = -1;
		
			XLuaTestMyStruct_TypeID = -1;
		
			XLuaTestPushAsTableStruct_TypeID = -1;
		
			UnityEngineVector2_TypeID = -1;
		
			UnityEngineVector3_TypeID = -1;
		
			UnityEngineVector4_TypeID = -1;
		
			UnityEngineColor_TypeID = -1;
		
			UnityEngineQuaternion_TypeID = -1;
		
			UnityEngineRay_TypeID = -1;
		
			UnityEngineBounds_TypeID = -1;
		
			UnityEngineRay2D_TypeID = -1;
		
			TutorialTestEnum_TypeID = -1;
			TutorialTestEnum_EnumRef = -1;
		
			XLuaTestMyEnum_TypeID = -1;
			XLuaTestMyEnum_EnumRef = -1;
		
			TutorialDerivedClassTestEnumInner_TypeID = -1;
			TutorialDerivedClassTestEnumInner_EnumRef = -1;
		
			miniRAIDCostType_TypeID = -1;
			miniRAIDCostType_EnumRef = -1;
		

		
			translator.RegisterPushAndGetAndUpdate<XLuaTest.Pedding>(translator.PushXLuaTestPedding, translator.GetXLuaTestPedding, translator.UpdateXLuaTestPedding);
			translator.RegisterPushAndGetAndUpdate<XLuaTest.MyStruct>(translator.PushXLuaTestMyStruct, translator.GetXLuaTestMyStruct, translator.UpdateXLuaTestMyStruct);
			translator.RegisterPushAndGetAndUpdate<XLuaTest.PushAsTableStruct>(translator.PushXLuaTestPushAsTableStruct, translator.GetXLuaTestPushAsTableStruct, translator.UpdateXLuaTestPushAsTableStruct);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Vector2>(translator.PushUnityEngineVector2, translator.GetUnityEngineVector2, translator.UpdateUnityEngineVector2);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Vector3>(translator.PushUnityEngineVector3, translator.GetUnityEngineVector3, translator.UpdateUnityEngineVector3);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Vector4>(translator.PushUnityEngineVector4, translator.GetUnityEngineVector4, translator.UpdateUnityEngineVector4);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Color>(translator.PushUnityEngineColor, translator.GetUnityEngineColor, translator.UpdateUnityEngineColor);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Quaternion>(translator.PushUnityEngineQuaternion, translator.GetUnityEngineQuaternion, translator.UpdateUnityEngineQuaternion);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Ray>(translator.PushUnityEngineRay, translator.GetUnityEngineRay, translator.UpdateUnityEngineRay);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Bounds>(translator.PushUnityEngineBounds, translator.GetUnityEngineBounds, translator.UpdateUnityEngineBounds);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Ray2D>(translator.PushUnityEngineRay2D, translator.GetUnityEngineRay2D, translator.UpdateUnityEngineRay2D);
			translator.RegisterPushAndGetAndUpdate<Tutorial.TestEnum>(translator.PushTutorialTestEnum, translator.GetTutorialTestEnum, translator.UpdateTutorialTestEnum);
			translator.RegisterPushAndGetAndUpdate<XLuaTest.MyEnum>(translator.PushXLuaTestMyEnum, translator.GetXLuaTestMyEnum, translator.UpdateXLuaTestMyEnum);
			translator.RegisterPushAndGetAndUpdate<Tutorial.DerivedClass.TestEnumInner>(translator.PushTutorialDerivedClassTestEnumInner, translator.GetTutorialDerivedClassTestEnumInner, translator.UpdateTutorialDerivedClassTestEnumInner);
			translator.RegisterPushAndGetAndUpdate<miniRAID.Cost.Type>(translator.PushminiRAIDCostType, translator.GetminiRAIDCostType, translator.UpdateminiRAIDCostType);
		
		}
        
        static int XLuaTestPedding_TypeID = -1;
        public static void PushXLuaTestPedding(this ObjectTranslator thiz, RealStatePtr L, XLuaTest.Pedding val)
        {
            if (XLuaTestPedding_TypeID == -1)
            {
			    bool is_first;
                XLuaTestPedding_TypeID = thiz.getTypeId(L, typeof(XLuaTest.Pedding), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 1, XLuaTestPedding_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for XLuaTest.Pedding ,value="+val);
            }
			
        }
		
        public static void GetXLuaTestPedding(this ObjectTranslator thiz, RealStatePtr L, int index, out XLuaTest.Pedding val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != XLuaTestPedding_TypeID)
				{
				    throw new Exception("invalid userdata for XLuaTest.Pedding");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for XLuaTest.Pedding");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (XLuaTest.Pedding)thiz.objectCasters.GetCaster(typeof(XLuaTest.Pedding))(L, index, null);
            }
        }
		
        public static void UpdateXLuaTestPedding(this ObjectTranslator thiz, RealStatePtr L, int index, XLuaTest.Pedding val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != XLuaTestPedding_TypeID)
				{
				    throw new Exception("invalid userdata for XLuaTest.Pedding");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for XLuaTest.Pedding ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int XLuaTestMyStruct_TypeID = -1;
        public static void PushXLuaTestMyStruct(this ObjectTranslator thiz, RealStatePtr L, XLuaTest.MyStruct val)
        {
            if (XLuaTestMyStruct_TypeID == -1)
            {
			    bool is_first;
                XLuaTestMyStruct_TypeID = thiz.getTypeId(L, typeof(XLuaTest.MyStruct), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 25, XLuaTestMyStruct_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for XLuaTest.MyStruct ,value="+val);
            }
			
        }
		
        public static void GetXLuaTestMyStruct(this ObjectTranslator thiz, RealStatePtr L, int index, out XLuaTest.MyStruct val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != XLuaTestMyStruct_TypeID)
				{
				    throw new Exception("invalid userdata for XLuaTest.MyStruct");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for XLuaTest.MyStruct");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (XLuaTest.MyStruct)thiz.objectCasters.GetCaster(typeof(XLuaTest.MyStruct))(L, index, null);
            }
        }
		
        public static void UpdateXLuaTestMyStruct(this ObjectTranslator thiz, RealStatePtr L, int index, XLuaTest.MyStruct val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != XLuaTestMyStruct_TypeID)
				{
				    throw new Exception("invalid userdata for XLuaTest.MyStruct");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for XLuaTest.MyStruct ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int XLuaTestPushAsTableStruct_TypeID = -1;
        public static void PushXLuaTestPushAsTableStruct(this ObjectTranslator thiz, RealStatePtr L, XLuaTest.PushAsTableStruct val)
        {
            if (XLuaTestPushAsTableStruct_TypeID == -1)
            {
			    bool is_first;
                XLuaTestPushAsTableStruct_TypeID = thiz.getTypeId(L, typeof(XLuaTest.PushAsTableStruct), out is_first);
				
            }
			
			
			LuaAPI.xlua_pushcstable(L, 2, XLuaTestPushAsTableStruct_TypeID);
			
			LuaAPI.xlua_pushasciistring(L, "x");
			LuaAPI.xlua_pushinteger(L, val.x);
			LuaAPI.lua_rawset(L, -3);
			
			LuaAPI.xlua_pushasciistring(L, "y");
			LuaAPI.xlua_pushinteger(L, val.y);
			LuaAPI.lua_rawset(L, -3);
			
			
        }
		
        public static void GetXLuaTestPushAsTableStruct(this ObjectTranslator thiz, RealStatePtr L, int index, out XLuaTest.PushAsTableStruct val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != XLuaTestPushAsTableStruct_TypeID)
				{
				    throw new Exception("invalid userdata for XLuaTest.PushAsTableStruct");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for XLuaTest.PushAsTableStruct");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (XLuaTest.PushAsTableStruct)thiz.objectCasters.GetCaster(typeof(XLuaTest.PushAsTableStruct))(L, index, null);
            }
        }
		
        public static void UpdateXLuaTestPushAsTableStruct(this ObjectTranslator thiz, RealStatePtr L, int index, XLuaTest.PushAsTableStruct val)
        {
		    
			if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TTABLE)
            {
			    return;
			}
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineVector2_TypeID = -1;
        public static void PushUnityEngineVector2(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Vector2 val)
        {
            if (UnityEngineVector2_TypeID == -1)
            {
			    bool is_first;
                UnityEngineVector2_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Vector2), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 8, UnityEngineVector2_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Vector2 ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineVector2(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Vector2 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector2_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector2");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Vector2");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Vector2)thiz.objectCasters.GetCaster(typeof(UnityEngine.Vector2))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineVector2(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Vector2 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector2_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector2");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Vector2 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineVector3_TypeID = -1;
        public static void PushUnityEngineVector3(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Vector3 val)
        {
            if (UnityEngineVector3_TypeID == -1)
            {
			    bool is_first;
                UnityEngineVector3_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Vector3), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 12, UnityEngineVector3_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Vector3 ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineVector3(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Vector3 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector3_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector3");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Vector3");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Vector3)thiz.objectCasters.GetCaster(typeof(UnityEngine.Vector3))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineVector3(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Vector3 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector3_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector3");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Vector3 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineVector4_TypeID = -1;
        public static void PushUnityEngineVector4(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Vector4 val)
        {
            if (UnityEngineVector4_TypeID == -1)
            {
			    bool is_first;
                UnityEngineVector4_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Vector4), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 16, UnityEngineVector4_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Vector4 ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineVector4(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Vector4 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector4_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector4");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Vector4");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Vector4)thiz.objectCasters.GetCaster(typeof(UnityEngine.Vector4))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineVector4(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Vector4 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector4_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector4");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Vector4 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineColor_TypeID = -1;
        public static void PushUnityEngineColor(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Color val)
        {
            if (UnityEngineColor_TypeID == -1)
            {
			    bool is_first;
                UnityEngineColor_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Color), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 16, UnityEngineColor_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Color ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineColor(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Color val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineColor_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Color");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Color");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Color)thiz.objectCasters.GetCaster(typeof(UnityEngine.Color))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineColor(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Color val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineColor_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Color");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Color ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineQuaternion_TypeID = -1;
        public static void PushUnityEngineQuaternion(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Quaternion val)
        {
            if (UnityEngineQuaternion_TypeID == -1)
            {
			    bool is_first;
                UnityEngineQuaternion_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Quaternion), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 16, UnityEngineQuaternion_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Quaternion ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineQuaternion(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Quaternion val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineQuaternion_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Quaternion");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Quaternion");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Quaternion)thiz.objectCasters.GetCaster(typeof(UnityEngine.Quaternion))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineQuaternion(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Quaternion val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineQuaternion_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Quaternion");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Quaternion ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineRay_TypeID = -1;
        public static void PushUnityEngineRay(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Ray val)
        {
            if (UnityEngineRay_TypeID == -1)
            {
			    bool is_first;
                UnityEngineRay_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Ray), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 24, UnityEngineRay_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Ray ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineRay(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Ray val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRay_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Ray");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Ray");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Ray)thiz.objectCasters.GetCaster(typeof(UnityEngine.Ray))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineRay(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Ray val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRay_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Ray");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Ray ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineBounds_TypeID = -1;
        public static void PushUnityEngineBounds(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Bounds val)
        {
            if (UnityEngineBounds_TypeID == -1)
            {
			    bool is_first;
                UnityEngineBounds_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Bounds), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 24, UnityEngineBounds_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Bounds ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineBounds(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Bounds val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineBounds_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Bounds");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Bounds");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Bounds)thiz.objectCasters.GetCaster(typeof(UnityEngine.Bounds))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineBounds(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Bounds val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineBounds_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Bounds");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Bounds ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineRay2D_TypeID = -1;
        public static void PushUnityEngineRay2D(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Ray2D val)
        {
            if (UnityEngineRay2D_TypeID == -1)
            {
			    bool is_first;
                UnityEngineRay2D_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Ray2D), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 16, UnityEngineRay2D_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Ray2D ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineRay2D(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Ray2D val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRay2D_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Ray2D");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Ray2D");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Ray2D)thiz.objectCasters.GetCaster(typeof(UnityEngine.Ray2D))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineRay2D(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Ray2D val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRay2D_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Ray2D");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Ray2D ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int TutorialTestEnum_TypeID = -1;
		static int TutorialTestEnum_EnumRef = -1;
        
        public static void PushTutorialTestEnum(this ObjectTranslator thiz, RealStatePtr L, Tutorial.TestEnum val)
        {
            if (TutorialTestEnum_TypeID == -1)
            {
			    bool is_first;
                TutorialTestEnum_TypeID = thiz.getTypeId(L, typeof(Tutorial.TestEnum), out is_first);
				
				if (TutorialTestEnum_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(Tutorial.TestEnum));
				    TutorialTestEnum_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, TutorialTestEnum_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, TutorialTestEnum_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for Tutorial.TestEnum ,value="+val);
            }
			
			LuaAPI.lua_getref(L, TutorialTestEnum_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetTutorialTestEnum(this ObjectTranslator thiz, RealStatePtr L, int index, out Tutorial.TestEnum val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != TutorialTestEnum_TypeID)
				{
				    throw new Exception("invalid userdata for Tutorial.TestEnum");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for Tutorial.TestEnum");
                }
				val = (Tutorial.TestEnum)e;
                
            }
            else
            {
                val = (Tutorial.TestEnum)thiz.objectCasters.GetCaster(typeof(Tutorial.TestEnum))(L, index, null);
            }
        }
		
        public static void UpdateTutorialTestEnum(this ObjectTranslator thiz, RealStatePtr L, int index, Tutorial.TestEnum val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != TutorialTestEnum_TypeID)
				{
				    throw new Exception("invalid userdata for Tutorial.TestEnum");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for Tutorial.TestEnum ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int XLuaTestMyEnum_TypeID = -1;
		static int XLuaTestMyEnum_EnumRef = -1;
        
        public static void PushXLuaTestMyEnum(this ObjectTranslator thiz, RealStatePtr L, XLuaTest.MyEnum val)
        {
            if (XLuaTestMyEnum_TypeID == -1)
            {
			    bool is_first;
                XLuaTestMyEnum_TypeID = thiz.getTypeId(L, typeof(XLuaTest.MyEnum), out is_first);
				
				if (XLuaTestMyEnum_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(XLuaTest.MyEnum));
				    XLuaTestMyEnum_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, XLuaTestMyEnum_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, XLuaTestMyEnum_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for XLuaTest.MyEnum ,value="+val);
            }
			
			LuaAPI.lua_getref(L, XLuaTestMyEnum_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetXLuaTestMyEnum(this ObjectTranslator thiz, RealStatePtr L, int index, out XLuaTest.MyEnum val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != XLuaTestMyEnum_TypeID)
				{
				    throw new Exception("invalid userdata for XLuaTest.MyEnum");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for XLuaTest.MyEnum");
                }
				val = (XLuaTest.MyEnum)e;
                
            }
            else
            {
                val = (XLuaTest.MyEnum)thiz.objectCasters.GetCaster(typeof(XLuaTest.MyEnum))(L, index, null);
            }
        }
		
        public static void UpdateXLuaTestMyEnum(this ObjectTranslator thiz, RealStatePtr L, int index, XLuaTest.MyEnum val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != XLuaTestMyEnum_TypeID)
				{
				    throw new Exception("invalid userdata for XLuaTest.MyEnum");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for XLuaTest.MyEnum ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int TutorialDerivedClassTestEnumInner_TypeID = -1;
		static int TutorialDerivedClassTestEnumInner_EnumRef = -1;
        
        public static void PushTutorialDerivedClassTestEnumInner(this ObjectTranslator thiz, RealStatePtr L, Tutorial.DerivedClass.TestEnumInner val)
        {
            if (TutorialDerivedClassTestEnumInner_TypeID == -1)
            {
			    bool is_first;
                TutorialDerivedClassTestEnumInner_TypeID = thiz.getTypeId(L, typeof(Tutorial.DerivedClass.TestEnumInner), out is_first);
				
				if (TutorialDerivedClassTestEnumInner_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(Tutorial.DerivedClass.TestEnumInner));
				    TutorialDerivedClassTestEnumInner_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, TutorialDerivedClassTestEnumInner_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, TutorialDerivedClassTestEnumInner_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for Tutorial.DerivedClass.TestEnumInner ,value="+val);
            }
			
			LuaAPI.lua_getref(L, TutorialDerivedClassTestEnumInner_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetTutorialDerivedClassTestEnumInner(this ObjectTranslator thiz, RealStatePtr L, int index, out Tutorial.DerivedClass.TestEnumInner val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != TutorialDerivedClassTestEnumInner_TypeID)
				{
				    throw new Exception("invalid userdata for Tutorial.DerivedClass.TestEnumInner");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for Tutorial.DerivedClass.TestEnumInner");
                }
				val = (Tutorial.DerivedClass.TestEnumInner)e;
                
            }
            else
            {
                val = (Tutorial.DerivedClass.TestEnumInner)thiz.objectCasters.GetCaster(typeof(Tutorial.DerivedClass.TestEnumInner))(L, index, null);
            }
        }
		
        public static void UpdateTutorialDerivedClassTestEnumInner(this ObjectTranslator thiz, RealStatePtr L, int index, Tutorial.DerivedClass.TestEnumInner val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != TutorialDerivedClassTestEnumInner_TypeID)
				{
				    throw new Exception("invalid userdata for Tutorial.DerivedClass.TestEnumInner");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for Tutorial.DerivedClass.TestEnumInner ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int miniRAIDCostType_TypeID = -1;
		static int miniRAIDCostType_EnumRef = -1;
        
        public static void PushminiRAIDCostType(this ObjectTranslator thiz, RealStatePtr L, miniRAID.Cost.Type val)
        {
            if (miniRAIDCostType_TypeID == -1)
            {
			    bool is_first;
                miniRAIDCostType_TypeID = thiz.getTypeId(L, typeof(miniRAID.Cost.Type), out is_first);
				
				if (miniRAIDCostType_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(miniRAID.Cost.Type));
				    miniRAIDCostType_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, miniRAIDCostType_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, miniRAIDCostType_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for miniRAID.Cost.Type ,value="+val);
            }
			
			LuaAPI.lua_getref(L, miniRAIDCostType_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetminiRAIDCostType(this ObjectTranslator thiz, RealStatePtr L, int index, out miniRAID.Cost.Type val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != miniRAIDCostType_TypeID)
				{
				    throw new Exception("invalid userdata for miniRAID.Cost.Type");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for miniRAID.Cost.Type");
                }
				val = (miniRAID.Cost.Type)e;
                
            }
            else
            {
                val = (miniRAID.Cost.Type)thiz.objectCasters.GetCaster(typeof(miniRAID.Cost.Type))(L, index, null);
            }
        }
		
        public static void UpdateminiRAIDCostType(this ObjectTranslator thiz, RealStatePtr L, int index, miniRAID.Cost.Type val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != miniRAIDCostType_TypeID)
				{
				    throw new Exception("invalid userdata for miniRAID.Cost.Type");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for miniRAID.Cost.Type ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        
		// table cast optimze
		
        
    }
	
	public partial class StaticLuaCallbacks_Wrap
    {
	    internal static bool __tryArrayGet(Type type, RealStatePtr L, ObjectTranslator translator, object obj, int index)
		{
		
			if (type == typeof(XLuaTest.Pedding[]))
			{
			    XLuaTest.Pedding[] array = obj as XLuaTest.Pedding[];
				translator.PushXLuaTestPedding(L, array[index]);
				return true;
			}
			else if (type == typeof(XLuaTest.MyStruct[]))
			{
			    XLuaTest.MyStruct[] array = obj as XLuaTest.MyStruct[];
				translator.PushXLuaTestMyStruct(L, array[index]);
				return true;
			}
			else if (type == typeof(XLuaTest.PushAsTableStruct[]))
			{
			    XLuaTest.PushAsTableStruct[] array = obj as XLuaTest.PushAsTableStruct[];
				translator.PushXLuaTestPushAsTableStruct(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector2[]))
			{
			    UnityEngine.Vector2[] array = obj as UnityEngine.Vector2[];
				translator.PushUnityEngineVector2(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector3[]))
			{
			    UnityEngine.Vector3[] array = obj as UnityEngine.Vector3[];
				translator.PushUnityEngineVector3(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector4[]))
			{
			    UnityEngine.Vector4[] array = obj as UnityEngine.Vector4[];
				translator.PushUnityEngineVector4(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Color[]))
			{
			    UnityEngine.Color[] array = obj as UnityEngine.Color[];
				translator.PushUnityEngineColor(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Quaternion[]))
			{
			    UnityEngine.Quaternion[] array = obj as UnityEngine.Quaternion[];
				translator.PushUnityEngineQuaternion(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Ray[]))
			{
			    UnityEngine.Ray[] array = obj as UnityEngine.Ray[];
				translator.PushUnityEngineRay(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Bounds[]))
			{
			    UnityEngine.Bounds[] array = obj as UnityEngine.Bounds[];
				translator.PushUnityEngineBounds(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Ray2D[]))
			{
			    UnityEngine.Ray2D[] array = obj as UnityEngine.Ray2D[];
				translator.PushUnityEngineRay2D(L, array[index]);
				return true;
			}
			else if (type == typeof(Tutorial.TestEnum[]))
			{
			    Tutorial.TestEnum[] array = obj as Tutorial.TestEnum[];
				translator.PushTutorialTestEnum(L, array[index]);
				return true;
			}
			else if (type == typeof(XLuaTest.MyEnum[]))
			{
			    XLuaTest.MyEnum[] array = obj as XLuaTest.MyEnum[];
				translator.PushXLuaTestMyEnum(L, array[index]);
				return true;
			}
			else if (type == typeof(Tutorial.DerivedClass.TestEnumInner[]))
			{
			    Tutorial.DerivedClass.TestEnumInner[] array = obj as Tutorial.DerivedClass.TestEnumInner[];
				translator.PushTutorialDerivedClassTestEnumInner(L, array[index]);
				return true;
			}
			else if (type == typeof(miniRAID.Cost.Type[]))
			{
			    miniRAID.Cost.Type[] array = obj as miniRAID.Cost.Type[];
				translator.PushminiRAIDCostType(L, array[index]);
				return true;
			}
            return false;
		}
		
		internal static bool __tryArraySet(Type type, RealStatePtr L, ObjectTranslator translator, object obj, int array_idx, int obj_idx)
		{
		
			if (type == typeof(XLuaTest.Pedding[]))
			{
			    XLuaTest.Pedding[] array = obj as XLuaTest.Pedding[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(XLuaTest.MyStruct[]))
			{
			    XLuaTest.MyStruct[] array = obj as XLuaTest.MyStruct[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(XLuaTest.PushAsTableStruct[]))
			{
			    XLuaTest.PushAsTableStruct[] array = obj as XLuaTest.PushAsTableStruct[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector2[]))
			{
			    UnityEngine.Vector2[] array = obj as UnityEngine.Vector2[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector3[]))
			{
			    UnityEngine.Vector3[] array = obj as UnityEngine.Vector3[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector4[]))
			{
			    UnityEngine.Vector4[] array = obj as UnityEngine.Vector4[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Color[]))
			{
			    UnityEngine.Color[] array = obj as UnityEngine.Color[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Quaternion[]))
			{
			    UnityEngine.Quaternion[] array = obj as UnityEngine.Quaternion[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Ray[]))
			{
			    UnityEngine.Ray[] array = obj as UnityEngine.Ray[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Bounds[]))
			{
			    UnityEngine.Bounds[] array = obj as UnityEngine.Bounds[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Ray2D[]))
			{
			    UnityEngine.Ray2D[] array = obj as UnityEngine.Ray2D[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(Tutorial.TestEnum[]))
			{
			    Tutorial.TestEnum[] array = obj as Tutorial.TestEnum[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(XLuaTest.MyEnum[]))
			{
			    XLuaTest.MyEnum[] array = obj as XLuaTest.MyEnum[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(Tutorial.DerivedClass.TestEnumInner[]))
			{
			    Tutorial.DerivedClass.TestEnumInner[] array = obj as Tutorial.DerivedClass.TestEnumInner[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(miniRAID.Cost.Type[]))
			{
			    miniRAID.Cost.Type[] array = obj as miniRAID.Cost.Type[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
            return false;
		}
	}
}