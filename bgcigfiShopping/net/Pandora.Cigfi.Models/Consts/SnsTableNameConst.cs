using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Models.Consts
{
    /// <summary>
    /// sns库表名
    /// </summary>
    public class SnsTableNameConst
    {
        /// <summary>
        /// 观点
        /// </summary>

        public const string SNS_ARTICLES = "bbs.bbs_articles";
        /// <summary>
        /// 观点
        /// </summary>

        public const string BBS_ARTICLE_TOPICS = "bbs.bbs_article_topics";
        /// <summary>
        /// 用户举报明细
        /// </summary>

        public const string SNS_REPORTDETAILS = "sns_reportdetails";
        /// <summary>
        /// 用户举报明细
        /// </summary>

        public const string BASE_COMMENT_COMPLAINT = "datacenter.base_comment_complaint";
        /// <summary>
        /// 用户信息
        /// </summary>

        public const string SNS_USERINFO = "bbs.bbs_userinfo";
        /// <summary>
        /// 推荐用户
        /// </summary>

        public const string BBS_USERINFO_RECOMMEND = "bbs.bbs_userinfo_recommend";
        /// <summary>
        /// 观点评论
        /// </summary>
        public const string SNS_COMMENTDETAILS= "sns_commentdetails";
        /// <summary>
        /// 观点评论的用户号举报明细
        /// </summary>
        public static string SNS_COMMENTREPOTDETAILS= "sns_commentrepotdetails";
        /// <summary>
        /// 用户申诉
        /// </summary>
        public static string SNS_USERINFO_APPEAL= "sns_userinfo_appeal";

        /// <summary>
        /// 钱包用户评论
        /// </summary>
        public static string SNS_COMMENTS = "sns_comments";
    }
}
