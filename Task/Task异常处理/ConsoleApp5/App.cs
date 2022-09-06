using ConsoleApp5;
using System.Drawing;

var res = await Test._cache["https://www.baidu.com"];
Console.WriteLine(res);

var urls = new string[3];
List<Task<Bitmap>> imageTasks = (from imageUrl in urls select GetBitmapAsync(imageUrl)).ToList();

Task<Bitmap> GetBitmapAsync(string imageUrl)
{
    throw new NotImplementedException();
}

while (imageTasks.Count > 0)
{
    try
    {
        Task<Bitmap> imageTask = await Task.WhenAny(imageTasks);
        imageTasks.Remove(imageTask);
        Bitmap bitmap = await imageTask;
        //TODO
    }
    catch
    {

    }
}

AsyncProducerConsumerCollection<string> collection = new AsyncProducerConsumerCollection<string>();
collection.Add("hello");
collection.Add("world");

Console.WriteLine(await collection.Take());
Console.WriteLine(await collection.Take());
