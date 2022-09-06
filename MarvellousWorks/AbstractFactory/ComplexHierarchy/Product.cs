using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory.ComplexHierarchy
{
    #region Product接口
    public interface IProductXA { }
    public interface IProductXB { }
    public interface IProductYA { }
    public interface IProductYB { }
    public interface IProductYC { }

    #endregion
    #region 实现
    public class ProductXA1 : IProductXA { }
    public class ProductXA2 : IProductXA { }
    public class ProductXA3 : IProductXA { }
    public class ProductXB1 : IProductXB { }
    public class ProductYA1 : IProductYA { }
    public class ProductYB1 : IProductYB { }
    public class ProductYB2 : IProductYB { }
    public class ProductYC1 : IProductYC { } 
    #endregion
    /// <summary>
    /// 实现接口
    /// </summary>
    public interface IAbstractFactory
    {
        T Create<T>();
    }
}
