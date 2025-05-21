using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Pandora.Cigfi.Models.Sys
{
    /// <summary>
    /// 审核概览实体类
    /// </summary>
    [Serializable]
    public class ReviewCountDBModel
    {

        [DisplayName("参与审核ID")]
        public int AddOprID { get; set; }
        [DisplayName("编辑ID")]
        public int EditOprID { get; set; }

        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }
        [DisplayName("编辑时间")]
        public DateTime ModifyTime { get; set; }
    }

    /// <summary>
    /// 审核概览实体类
    /// </summary>
    [Serializable]
    public class ReviewCountModel
    {
        [DisplayName("用户ID")]
        public int ID { get; set; }


        [DisplayName("参与审核")]
        public string  AddOprName { get; set; }

        [DisplayName("本月审核")]
        public int MonthCount { get; set; }

        [DisplayName("本月修改数量")]
        public int MonthEditCount { get; set; }

        [DisplayName("本周审核")]
        public int WeekCount { get; set; }

        [DisplayName("本周修改数量")]
        public int WeekEditCount { get; set; }

        [DisplayName("当天审核")]
        public int ToDayCount { get; set; }

        [DisplayName("当天修改数量")]
        public int ToDayEditCount { get; set; }
    }
}
