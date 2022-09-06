using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericesLib
{
    public class BinaryTreeNode:ObjectWithName
    {
        private string name;
        public BinaryTreeNode(string name):base(name)
        {

        }

        public BinaryTreeNode Left = null;
        public BinaryTreeNode Right = null;
        public IEnumerator GetEnumerator()
        {
            yield return this;
            if(Left!=null)
                foreach (ObjectWithName item in Left)
                {
                    yield return item;
                }
            if(Right!=null)
                foreach (ObjectWithName item in Right)
                {
                    yield return item;
                }
        }
    }
}
