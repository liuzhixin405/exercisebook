using GenericesLib;

Stack<ObjectWithName> stack = new Stack<ObjectWithName>();
stack.Push(new ObjectWithName("2"));
stack.Push(new ObjectWithName("3"));
Queue<ObjectWithName> queue = new Queue<ObjectWithName>();
queue.Enqueue(new ObjectWithName("4"));
queue.Enqueue(new ObjectWithName("5"));

ObjectWithName[] array = new ObjectWithName[3];
array[0] = new ObjectWithName("6");
array[1] = new ObjectWithName("7");
array[2] = new ObjectWithName("8");

BinaryTreeNode root = new BinaryTreeNode("9");
root.Left = new BinaryTreeNode("10");
root.Right = new BinaryTreeNode("11");

root.Right.Left = new BinaryTreeNode("13");
root.Right.Left.Left = new BinaryTreeNode("15");
root.Right.Left.Right = new BinaryTreeNode("16");
root.Right.Right = new BinaryTreeNode("17");
root.Right.Right.Right = new BinaryTreeNode("18");
root.Right.Right.Right.Right = new BinaryTreeNode("100");

CompositeIterator iterator = new CompositeIterator();
iterator.Add(stack);
iterator.Add(queue);
iterator.Add(array);
iterator.Add(root);

int count = 0;
foreach (ObjectWithName obj in iterator)
{
    Console.WriteLine($"{++count} ,{obj.ToString()}");
}

