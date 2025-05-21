using Pandora.Cigfi.IServices.Sys;
using Pandora.Cigfi.Models.Sys;
using FXH.Common.DapperService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Services.Sys
{
    public class ReviewLogService : BaseRepository<Sys_ReviewLogModel>, IReviewLogService
    {
        public ReviewLogService(IDapperRepository context) : base(context)
        {


        }

        public Task<int> CountAsync(Hashtable queryHt)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Sys_ReviewLogModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            throw new NotImplementedException();
        }
    }
}
