using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorNext
{
    internal interface IRepository_Bak
    {
        int Get();
        int[] List();
        void Write(int id);
        void Update(int id);
    }
    // mongodb  protsql Redis等实现接口IRepository_Bak
    //或者如下  被访问的对象需要固定，否则不适合用次模式
    internal interface IRepository
    {
        void Visit(IOperation operation);
    }
    internal interface IOperation
    {
        void VisitPostgres(Prostgres prostgres);
        void VisitMongoDb(MongoDb md);
        void VisitRedis(Redis redis);
    }

    internal abstract class Get<T> : IOperation
    {
        public T Result { get; set; }
        public abstract void VisitMongoDb(MongoDb md);
        public abstract void VisitPostgres(Prostgres prostgres);

        public abstract void VisitRedis(Redis redis);
    }

    internal class GetInt : Get<int>
    {
        public override void VisitMongoDb(MongoDb md)
        {
            Result = 5;
        }

        public override void VisitPostgres(Prostgres prostgres)
        {
            Result = 6;
        }

        public override void VisitRedis(Redis redis)
        {
            Result = 7;
        }
    }


}
