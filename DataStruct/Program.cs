
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace DataStruct;

public class Program
{

    static void Main(string[] args)
    {

        //ArrayTest();s
        //BinaryTreeTest();
        //TreeTest();
        LinkedListHelper<int> linkedList = new LinkedListHelper<int>();
        linkedList.Insert(1);
        linkedList.Insert(2);
        linkedList.Insert(3);
        linkedList.Insert(4);
        linkedList.Insert(5);
        linkedList.Insert(6);
        Console.WriteLine("链表中元素：");
        linkedList.Print();

        Console.WriteLine("反转后的链表为:");
        linkedList.Reverse();
        linkedList.Print();
    }

    private static void TreeTest()
    {
        TreeNode<int> root = new TreeNode<int>(1);
        root.LeftChild = new TreeNode<int>(2);
        root.RightChild = new TreeNode<int>(3);
        root.LeftChild.LeftChild = new TreeNode<int>(4);
        root.LeftChild.RightChild = new TreeNode<int>(5);

        BinaryTreeHelper<int> tree = new BinaryTreeHelper<int>(root);

        Console.WriteLine("前序遍历结果:");
        tree.PreOrderTraversal(root);
        Console.WriteLine();

        Console.WriteLine("中序遍历结果:");
        tree.InOrderTraversal(root);
        Console.WriteLine();

        Console.WriteLine("后序遍历结果:");
        tree.PostOrderTraversal(root);
        Console.WriteLine();
    }

    private static void BinaryTreeTest()
    {
        Console.WriteLine("LinkList test:");
        LinkedListHelper<int> linkedList = new LinkedListHelper<int>();
        linkedList.Insert(1);
        linkedList.Insert(2);
        linkedList.Insert(3);
        linkedList.Remove(2);
        var node = linkedList.Find(3);

        Console.WriteLine($"查找元素 3 后的节点值：{node.Data}");
        Console.WriteLine("链表中元素：");
        linkedList.Print();
    }

    private static void ArrayTest()
    {
        // int[] arr= new int[]{1,2,4,5,6,7,8};
        // Console.WriteLine("operation before:");
        // arr.ToList().ForEach(x=> Console.WriteLine(x));
        //  var newArr = ArrayHelper.AddNew<int>(arr,3,2);
        //     Console.WriteLine("operation after:");
        // newArr.ToList().ForEach(x=> Console.WriteLine(x));
        // 测试用例
        int[] arr = { 1, 2, 4, 5, 6, 7, 8 };

        // 插入到第一个位置
        TestAdd(arr, 0, 0);

        // 插入到中间位置
        TestAdd(arr, 3, 2);

        // 插入到最后一个位置
        TestAdd(arr, 9, arr.Length);
        // 插入到超出范围的位置
        TestAddOutOfRange(arr, 10, arr.Length + 1);

        int[] delArr = new int[] { 1, 2, 3, 4, 5 };
        Console.WriteLine("delete before:");
        delArr.ToList().ForEach(x => Console.WriteLine(x));
        var resultDelArr = ArrayHelper.Delete<int>(delArr, 1);
        Console.WriteLine("delete after:");
        resultDelArr.ToList().ForEach(x => Console.WriteLine(x));
    }

    static void TestAdd<T>(T[] arr, T item, int index)
    {
        try
        {
            var newArr = ArrayHelper.Add(arr, item, index);
            Console.WriteLine($"在索引 {index} 处插入元素 {item} 后的数组：");
            foreach (var element in newArr)
            {
                Console.Write(element + " ");
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"插入元素时发生异常：{ex.Message}");
        }
    }

    static void TestAddOutOfRange<T>(T[] arr, T item, int index)
    {
        try
        {
            var newArr = ArrayHelper.Add(arr, item, index);
            Console.WriteLine($"在索引 {index} 处插入元素 {item} 后的数组：");
            foreach (var element in newArr)
            {
                Console.Write(element + " ");
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"插入元素时发生异常：{ex.Message}");
        }
    }
}