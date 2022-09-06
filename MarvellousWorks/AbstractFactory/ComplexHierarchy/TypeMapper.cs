using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory.ComplexHierarchy
{
    public abstract class TypeMapperBase : Dictionary<Type, Type> { } //存放接口和实现的容器
    public class TypeMapperDictionary : Dictionary<Type, TypeMapperBase> { }//存放接口和实现的字典
    public interface IAbstractFactoryWithTypeMapper : IAbstractFactory    //T Create<T>()  
    {
        TypeMapperBase Mapper { get; set; } 
    }
    public abstract class AbstractFactoryBase : IAbstractFactoryWithTypeMapper
    {
        protected TypeMapperBase mapper;
        public virtual TypeMapperBase Mapper { get { return mapper; } set { mapper = value; } }
        public virtual T Create<T>()
        {
            Type targetType = mapper[typeof(T)];
            return (T)Activator.CreateInstance(targetType);
        }
    }
    #region TypeMapperBase实例
    public class ConcreteXTypeMapper : TypeMapperBase
    {
        public ConcreteXTypeMapper()
        {
            base.Add(typeof(IProductXA), typeof(ProductXA2));
            base.Add(typeof(IProductXB), typeof(ProductXB1));
        }
    }
    public class ConcreteYTypeMapper : TypeMapperBase
    {
        public ConcreteYTypeMapper()
        {
            base.Add(typeof(IProductYA), typeof(ProductYA1));
            base.Add(typeof(IProductYB), typeof(ProductYB1));
            base.Add(typeof(IProductYC), typeof(ProductYC1));
        }
    }
    #endregion
    #region AbstractFactoryBase实例
    public class ConcreteFactoryX : AbstractFactoryBase
    {

    }
    public class ConcreteFactoryY : AbstractFactoryBase
    {

    } 
    #endregion
    /// <summary>
    /// 加载相关TypeMapper/IAbstractFactory的对应信息实际执行的时候可以通过访问配置完成
    /// </summary>
    public static class AssemblyMechanism
    {
        private static TypeMapperDictionary dictionary = new TypeMapperDictionary();
        static AssemblyMechanism()
        {
            //AbstractFactoryBase实例和TypeBase实例
            dictionary.Add(typeof(ConcreteFactoryX), new ConcreteXTypeMapper());  
            dictionary.Add(typeof(ConcreteFactoryY), new ConcreteYTypeMapper());
        }
        /// <summary>
        /// 为AbstractFactory找到他的TypeMapper,并注入
        /// </summary>
        /// <param name="factory">AbstractFactory</param>

        public static void Assembly(IAbstractFactoryWithTypeMapper factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            TypeMapperBase mapper = dictionary[factory.GetType()];
            factory.Mapper = mapper;
        }
    }
}
