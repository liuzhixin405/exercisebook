using Lzx.IOCDI.Framework.CustomAOP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Lzx.IOCDI.Framework.CustomContainer
{
    public class SelfContainer : ISelfContainer
    {
        private Dictionary<string, SelfContainerRegistModel> _selfContainerDictionary = new Dictionary<string, SelfContainerRegistModel>();

        private Dictionary<string, object[]> _selfContainerValueDictionary = new Dictionary<string, object[]>();

        private Dictionary<string, object> _selfContainerScopeDictionary = new Dictionary<string, object>(); //单例

        private string GetKey(string fullName,string shortName)=>$"{fullName}__{shortName}";
        public ISelfContainer CreateChildContainer()
        {
            return new SelfContainer(_selfContainerDictionary, _selfContainerValueDictionary,new Dictionary<string, object>());
        }
        public SelfContainer() { }
        private SelfContainer(Dictionary<string,SelfContainerRegistModel> selfContainerDictionary,Dictionary<string,object[]> selfContainerValueDictionary,Dictionary<string ,object> selfContainerScopeDictionary)
        {
            _selfContainerDictionary = selfContainerDictionary;
            _selfContainerValueDictionary = selfContainerValueDictionary;
            _selfContainerScopeDictionary = selfContainerScopeDictionary;
        }

        public void Register<TForm, TTo>(string shortName = null, object[] paraList = null, LifetimeType lifetimeType = LifetimeType.Transient) where TTo : TForm
        {
            _selfContainerDictionary.Add(GetKey(typeof(TForm).FullName, shortName), new SelfContainerRegistModel()
            {
                Lifetime = lifetimeType,
                TargetType = typeof(TTo)
            }) ;
            if(paraList!=null && paraList.Length > 0)
            {
                _selfContainerValueDictionary.Add(GetKey(typeof(TForm).FullName, shortName), paraList);
            }
        }

        public void RegisterType(Type typeFrom, Type typeTo, LifetimeType lifetimeType = LifetimeType.Transient)
        {
            _selfContainerDictionary.Add(GetKey(typeFrom.FullName, ""), new SelfContainerRegistModel() { 
            Lifetime = lifetimeType,
            TargetType = typeTo
            });
        }

        public TFrom Resolve<TFrom>(string shortName = null)
        {
            return (TFrom)ResolveObject(typeof(TFrom), shortName);
        }

        private object ResolveObject(Type abstractType, string shortName=null)
        {
            string key = GetKey(abstractType.FullName, shortName);
            var model = _selfContainerDictionary[key];

            switch (model.Lifetime)
            {
                case LifetimeType.Transient:
                    Console.WriteLine("Transient Do Nothing Before ~~");
                    break;
                case LifetimeType.Singleton:
                    if(model.SingletonInstance == null)
                    {
                        break;
                    }
                    else
                    {
                        return model.SingletonInstance;
                    }
                case LifetimeType.Scope:
                    if (_selfContainerScopeDictionary.ContainsKey(key))
                    {
                        return _selfContainerScopeDictionary[key];
                    }
                    else
                    {
                        break;
                    }
                case LifetimeType.PerThread:
                    object oValue = CustomCallContext<object>.GetData($"{key}{Thread.CurrentThread.ManagedThreadId}");
                    if (oValue == null)
                        break;
                    else
                        return oValue;
                default:
                    throw new Exception("出错了");
            }

            Type type = model.TargetType;

            ConstructorInfo ctor = null;

            ctor = type.GetConstructors().FirstOrDefault(c => c.IsDefined(typeof(SelfConstructorAttribute), true));

            if(ctor == null)
            {
                ctor = type.GetConstructors().OrderByDescending(s => s.GetParameters().Length).First();
            }

            List<object> paraList = new List<object>();    //准备构造函数参数

            object[] paraConstant = _selfContainerValueDictionary.ContainsKey(key) ? _selfContainerValueDictionary[key] : null;
            int iIndex = 0;
            //准备构造函数参数
            foreach (var para in ctor.GetParameters())
            {
                if (para.IsDefined(typeof(SelfConstructorAttribute), true))
                {
                    paraList.Add(paraConstant[iIndex]);
                    iIndex++;
                }
                else
                {
                    Type paraType = para.ParameterType;
                    string paraShortName = GetShortName(para);
                    object paraInstance = ResolveObject(paraType, paraShortName);
                    paraList.Add(paraInstance);
                }
            }

            object oInstance = null;
            oInstance = Activator.CreateInstance(type, paraList.ToArray());
            //属性注入
            foreach (var prop in type.GetProperties().Where(p=>p.IsDefined(typeof(SelfPropertyInjectionAttribute),true)))
            {
                Type propType = prop.PropertyType;
                string paraShortName = GetShortName(prop);
                object propInstance = ResolveObject(propType, paraShortName);
                prop.SetValue(oInstance, propInstance);
            }
            //方法注入
            foreach (var method in type.GetMethods().Where(m=>m.IsDefined(typeof(SelfPropertyInjectionAttribute),true)))
            {
                List<object> paraInjectionList = new List<object>();
                foreach (var para in method.GetParameters())
                {
                    Type paraType = para.ParameterType;
                    string paraShortName = GetShortName(para);
                    object paraInstance = ResolveObject(paraType, paraShortName);
                    paraInjectionList.Add(paraInstance);
                }
                method.Invoke(oInstance, paraInjectionList.ToArray());
            }

            switch (model.Lifetime)
            {
                case LifetimeType.Transient:
                    Console.WriteLine("Transient Do Nothing After ~~");
                    break;
                case LifetimeType.Singleton:
                    model.SingletonInstance = oInstance;
                    break;
                case LifetimeType.Scope:
                    _selfContainerScopeDictionary[key] = oInstance;
                    break;
                case LifetimeType.PerThread:
                    CustomCallContext<object>.SetData($"{key}{Thread.CurrentThread.ManagedThreadId}", oInstance);
                    break;
                default:
                    throw new Exception("出错了");
            }

            return oInstance.AOP(abstractType);
        }

        private string GetShortName(ICustomAttributeProvider provider)
        {
            if (provider.IsDefined(typeof(SelfParameterShortNameAttribute),true))
            {
                var attribute = (SelfParameterShortNameAttribute)provider.GetCustomAttributes(typeof(SelfParameterShortNameAttribute), true)[0];
                return attribute.ShortName;
            }
            return null;

                
        }
        public object Resolve(Type type)
        {
            return ResolveObject(type, null);
        }
    }
}
