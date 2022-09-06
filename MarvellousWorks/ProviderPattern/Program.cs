using System;

namespace ProviderPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Pattern1
            Console.Write(Message.Insert(new MessageModel("插入", DateTime.Now)));
            Console.Write("<br />");
            Console.Write(Message.Get()[0].Message + " " + Message.Get()[0].PublishTime.ToString()); 
            #endregion

            Console.ReadKey();
        }
    }
}
