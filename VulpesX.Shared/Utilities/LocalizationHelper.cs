using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public class LocalizationHelper
    {
        public dynamic GetDynamicClassFromDictionary(Dictionary<string, string> dict)
        {
            dynamic obj = new ExpandoObject();
            var objDict = (IDictionary<string, object>)obj;

            foreach (var kv in dict)
            {
                objDict[kv.Key] = kv.Value;
            }

            return obj;
        }

        public object? CreateClassFromDictionary(Dictionary<string, string> dict)
        {
            var typeName = "LinguaDictionaryClass";
            var assemblyName = new AssemblyName(typeName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("VulpesX");
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);

            foreach (var kv in dict)
            {
                var field = typeBuilder.DefineField("_" + kv.Key, typeof(string), FieldAttributes.Private);
                var prop = typeBuilder.DefineProperty(kv.Key, PropertyAttributes.HasDefault, typeof(string), null);

                var getter = typeBuilder.DefineMethod("get_" + kv.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(string), Type.EmptyTypes);
                var il = getter.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, field);
                il.Emit(OpCodes.Ret);

                var setter = typeBuilder.DefineMethod("set_" + kv.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { typeof(string) });
                il = setter.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, field);
                il.Emit(OpCodes.Ret);

                prop.SetGetMethod(getter);
                prop.SetSetMethod(setter);
            }

            var dynamicType = typeBuilder.CreateType();
            var instance = Activator.CreateInstance(dynamicType);

            foreach (var kv in dict)
            {
                dynamicType.GetProperty(kv.Key)?.SetValue(instance, kv.Value);
            }

            return instance;
        }
    }
}
