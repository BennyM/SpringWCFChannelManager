//Copyright 2009 Benny Michielsen

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Spring.Proxy;

namespace WCFChannelManager
{
    public class WcfMethodBuilder
        : AbstractProxyMethodBuilder
    {
        private const string ExecuteMethodName = "Execute";

        public WcfMethodBuilder(TypeBuilder builder, IProxyTypeGenerator generator, Type channelManager)
            : base(builder, generator, false)
        {
            InvokeMethod = channelManager.GetMethod(ExecuteMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected MethodInfo InvokeMethod
        {
            get;
            set;
        }

        protected override void GenerateMethod(ILGenerator il, System.Reflection.MethodInfo method, System.Reflection.MethodInfo interfaceMethod)
        {
            // In Parameters
            ArrayList inParams = new ArrayList();
            // Ref or Out Parameters
            ArrayList refOutParams = new ArrayList();

            foreach (ParameterInfo paramInfo in interfaceMethod.GetParameters())
            {
                if (paramInfo.IsRetval || paramInfo.IsOut)
                {
                    refOutParams.Add(paramInfo);
                }
                else
                {
                    inParams.Add(paramInfo);
                }
            }

            proxyGenerator.PushTarget(il);

            //LocalBuilder methodToCall = il.DeclareLocal(typeof(MethodInfo));
            //il.Emit(OpCodes.Newobj, interfaceMethod);
            //il.Emit(OpCodes.Ldloc, methodToCall);
            il.Emit(OpCodes.Ldstr, interfaceMethod.Name);



            // Parameter #2
            LocalBuilder parameters = il.DeclareLocal(typeof(Object[]));
            il.Emit(OpCodes.Ldc_I4, inParams.Count);
            il.Emit(OpCodes.Newarr, typeof(Object));
            il.Emit(OpCodes.Stloc, parameters);

            int paramIndex = 0;
            foreach (ParameterInfo paramInfo in inParams)
            {
                il.Emit(OpCodes.Ldloc, parameters);
                il.Emit(OpCodes.Ldc_I4, paramIndex);
                il.Emit(OpCodes.Ldarg, paramInfo.Position + 1);
                if (paramInfo.ParameterType.IsValueType)
                {
                    il.Emit(OpCodes.Box, paramInfo.ParameterType);
                }
                il.Emit(OpCodes.Stelem_Ref);

                paramIndex++;
            }

            il.Emit(OpCodes.Ldloc, parameters);

            // Call Invoke method and save result
            LocalBuilder results = il.DeclareLocal(typeof(Object[]));
            il.EmitCall(OpCodes.Callvirt, InvokeMethod, null);
            il.Emit(OpCodes.Stloc, results);


            int resultIndex = (interfaceMethod.ReturnType == typeof(void) ? 0 : 1);
            foreach (ParameterInfo paramInfo in refOutParams)
            {
                il.Emit(OpCodes.Ldarg, paramInfo.Position + 1);
                il.Emit(OpCodes.Ldloc, results);

                // Cast / Unbox the return value
                il.Emit(OpCodes.Ldc_I4, resultIndex);
                il.Emit(OpCodes.Ldelem_Ref);

                Type elementType = paramInfo.ParameterType.GetElementType();
                if (elementType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox, elementType);
                    il.Emit(OpCodes.Ldobj, elementType);
                    il.Emit(OpCodes.Stobj, elementType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, elementType);
                    il.Emit(OpCodes.Stind_Ref);
                }

                resultIndex++;
            }

            if (interfaceMethod.ReturnType != typeof(void))
            {
                il.Emit(OpCodes.Ldloc, results);

                // Cast / Unbox the return value
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ldelem_Ref);
                if (interfaceMethod.ReturnType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox, interfaceMethod.ReturnType);
                    il.Emit(OpCodes.Ldobj, interfaceMethod.ReturnType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, interfaceMethod.ReturnType);
                }
            }

        }
    }

}
