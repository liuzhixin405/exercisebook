using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStruct
{
    public class BinaryTreeHelper<T>
    {
        private TreeNode<T> root;

        public BinaryTreeHelper(TreeNode<T> root)
        {
            this.root = root;
        }

        /// <summary>
        /// 前序遍历
        /// </summary>
        /// <param name="node"></param>
        public void PreOrderTraversal(TreeNode<T> node)
        {
            if (node == null)
            {
                return;
            }
            Console.Write(node.Data + " ");
            PreOrderTraversal(node.LeftChild);
            PreOrderTraversal(node.RightChild);
        }

        /// <summary>
        /// 中序遍历
        /// </summary>
        /// <param name="node"></param>
        public void InOrderTraversal(TreeNode<T> node)
        {
            if (node == null)
            {
                return;
            }
            InOrderTraversal(node.LeftChild);
            Console.Write(node.Data + " ");
            InOrderTraversal(node.RightChild);
        }

        /// <summary>
        /// 后序遍历
        /// </summary>
        /// <param name="node"></param>
        public void PostOrderTraversal(TreeNode<T> node)
        {
            if (node == null)
            {
                return;
            }
            PostOrderTraversal(node.LeftChild);
            PostOrderTraversal(node.RightChild);
            Console.Write(node.Data + " ");
        }
    }

    public class TreeNode<T>
    {
        public T Data;
        public TreeNode<T> LeftChild;
        public TreeNode<T> RightChild;
        public TreeNode(T data)
        {
            Data = data;
            LeftChild = null;
            RightChild = null;
        }
    }
}
