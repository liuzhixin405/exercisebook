using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;

namespace CqrsWithEs.Dragons
{
    public class PrivateReflectionDynamicObject : DynamicObject
    {
        private static IDictionary<Type, IDictionary<string, IProperty>> _propertiesOnType = new ConcurrentDictionary<Type, IDictionary<string, IProperty>>();

        interface IProperty
        {
            string Name { get; }
            object GetValue(object obj, object[] index);
            void SetValue(object obj, object val, object[] index);
        }

        class Property : IProperty
        {
            internal PropertyInfo PropertyInfo { get; set; }
            public string Name => PropertyInfo.Name;

            public object GetValue(object obj, object[] index)
            {
                return PropertyInfo.GetValue(obj, index);
            }

            public void SetValue(object obj, object val, object[] index)
            {
                PropertyInfo.SetValue(obj, val, index);
            }
        }

        class Field : IProperty
        {
            internal FieldInfo FieldInfo { get; set; }
            public string Name => FieldInfo.Name;

            public object GetValue(object obj, object[] index)
            {
                return FieldInfo.GetValue(obj);
            }

            public void SetValue(object obj, object val, object[] index)
            {
                FieldInfo.SetValue(obj, val);
            }
        }
        private object RealObject { get; set; }

        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        internal static object WrapObjectIfNeeded(object o)
        {
            if (o == null || o.GetType().IsPrimitive || o is string)
                return o;
            return new PrivateReflectionDynamicObject() { RealObject = o };
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            IProperty prop = GetProperty(binder.Name);
            result = prop.GetValue(RealObject, index: null);
            result = WrapObjectIfNeeded(result);

            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            IProperty property = GetProperty(binder.Name);
            property.SetValue(RealObject, value, index: null);
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            // The indexed property is always named "Item" in C#
            IProperty prop = GetIndexProperty();
            result = prop.GetValue(RealObject, indexes);

            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapObjectIfNeeded(result);

            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            // The indexed property is always named "Item" in C#
            IProperty prop = GetIndexProperty();
            prop.SetValue(RealObject, value, indexes);
            return true;
        }

        // Called when a method is called
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = InvokeMemberOnType(RealObject.GetType(), RealObject, binder.Name, args);

            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapObjectIfNeeded(result);

            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Convert.ChangeType(RealObject, binder.Type);
            return true;
        }

        public override string ToString()
        {
            return RealObject.ToString();
        }

        #region private
        private IProperty GetIndexProperty()
        {
            // The index property is always named "Item" in C#
            return GetProperty("Item");
        }

        private IProperty GetProperty(string propertyName)
        {
            IDictionary<string, IProperty> typePropertoes = GetTypeProperties(RealObject.GetType());
            IProperty property;
            if(typePropertoes.TryGetValue(propertyName, out property)) 
                return property;
            var propNames = typePropertoes.Keys.Where(name => name[0] != '<').OrderBy(name => name);
            throw new ArgumentException(string.Format("The property {0} doesn't exist on type {1}. Supported properties are: {2}",
                propertyName, RealObject.GetType(), String.Join(", ", propNames)));
        }

        private static IDictionary<string, IProperty> GetTypeProperties(Type type)
        {
            IDictionary<string, IProperty> typeProperties;
            if (_propertiesOnType.TryGetValue(type, out typeProperties))
            {
                return typeProperties;
            }
            typeProperties = new Dictionary<string, IProperty>();
            foreach (PropertyInfo prop in type.GetProperties(bindingFlags).Where(p => p.DeclaringType == type))
            {
                typeProperties[prop.Name] = new Property() { PropertyInfo = prop };
            }
            foreach (var field in type.GetFields(bindingFlags).Where(p => p.DeclaringType == type))
            {
                typeProperties[field.Name] = new Field() { FieldInfo = field };
            }
            if (type.BaseType != null)
            {
                foreach (Property prop in GetTypeProperties(type.BaseType).Values)
                {
                    typeProperties[prop.Name] = prop;
                }
            }
            _propertiesOnType[type] = typeProperties;
            return typeProperties;
        }

        private static object InvokeMemberOnType(Type type, object target, string name, object[] args)
        {
            try
            {
                return type.InvokeMember(
                    name,
                    BindingFlags.InvokeMethod | bindingFlags,
                    null,
                    target,
                    args);
            }
            catch(MissingMethodException)
            {
                if(type.BaseType != null)
                    return InvokeMemberOnType((Type)type.BaseType, target, name, args);
                throw;
            }
        }
        #endregion
    }
    public static class PrivateReflectionDynamicObjectExtensions
    {
        public static dynamic AsDynamic(this object o)
            => PrivateReflectionDynamicObject.WrapObjectIfNeeded(o);
    }
}
