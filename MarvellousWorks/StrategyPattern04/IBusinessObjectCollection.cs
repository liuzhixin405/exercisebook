using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern04
{
    public interface IBusinessObjectCollection
    {
        IComparer<IBusinessObjct> Comparer { get; set; }
        void Add(IBusinessObjct objct);
        IEnumerable<IBusinessObjct> GetAll();
    }
    public class BusinessObjectCollection : IBusinessObjectCollection
    {
        IList<IBusinessObjct> businessObjcts = new List<IBusinessObjct>();
        IComparer<IBusinessObjct> comparer;
        public IComparer<IBusinessObjct> Comparer { get => comparer; set => comparer =value; }

        public void Add(IBusinessObjct objct)
        {
            businessObjcts.Add(objct);
        }

        public IEnumerable<IBusinessObjct> GetAll()
        {
            if (businessObjcts.Count == 0) yield break;
            if(businessObjcts.Count == 1)
            {
                yield return businessObjcts[0];
                yield break;
            }

            IBusinessObjct[] array = new IBusinessObjct[businessObjcts.Count];
            businessObjcts.CopyTo(array, 0);
            Array.Sort(array, comparer);
            for (int i = 0; i < array.Length; i++)
            {
                yield return array[i];
            }
        }
    }
}
