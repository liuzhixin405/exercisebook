
{
    //var tasks = new List<Task>();

    //for(int i = 0; i < 5; i++)
    //{
    //    tasks.Add(new Task(() =>
    //    {
    //        Console.WriteLine("Task {0} is finished", Task.CurrentId);
    //    }));
    //}

    //// await Task.WhenAny(taskList);
    //Task.WaitAll(tasks.ToArray()); //将不会正常运行，会一直无限等待，因为 new Task 这样创建出来的 Task 不会自动运行，需要手动调用 Task.Start
    //Console.WriteLine("exit");
}


{
    //var tasks = new List<Task>();

    //for (int i = 0; i < 5; i++)
    //{
    //    tasks.Add(Task.Run(() =>
    //    {
    //        Console.WriteLine("Task {0} is finished", Task.CurrentId);
    //    }));
    //}

    //// await Task.WhenAny(taskList);
    //Task.WaitAll(tasks.ToArray());   //正常
    //Console.WriteLine("exit");
}




{
    //var tasks = new List<Task>();

    //for (int i = 0; i < 5; i++)
    //{
    //    tasks.Add(Task.Factory.StartNew(async () =>
    //    {
    //        await Task.Delay(3000);
    //        Console.WriteLine("Task {0} is finished", Task.CurrentId);
    //    }));
    //}

    // await Task.WhenAny(tasks);  //直接退出，不会等待任务执行完毕
    //Console.WriteLine("exit");
}


{
    var tasks = new List<Task>();

    for (int i = 0; i < 5; i++)
    {
        tasks.Add(Task.Factory.StartNew(async () =>
        {
            await Task.Delay(3000);
            Console.WriteLine("Task {0} is finished", Task.CurrentId);
        }).Unwrap()); //解除包装 UnWrap    这样等价于Task.Run()
    }

    await Task.WhenAny(tasks);
    Console.WriteLine("exit");
}