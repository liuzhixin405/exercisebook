using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.Data
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
