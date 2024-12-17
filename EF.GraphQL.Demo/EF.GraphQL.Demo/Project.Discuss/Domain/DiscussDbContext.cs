using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Project.Discuss.Models;

namespace Project.Discuss.Domain;

public partial class DiscussDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DiscussDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public virtual DbSet<BbsArticle> BbsArticles { get; set; }

    public virtual DbSet<BbsArticleCoin> BbsArticleCoins { get; set; }

    public virtual DbSet<BbsArticleContent> BbsArticleContents { get; set; }

    public virtual DbSet<BbsArticleLike> BbsArticleLikes { get; set; }

    public virtual DbSet<BbsArticleTopic> BbsArticleTopics { get; set; }

    public virtual DbSet<BbsArticlesPush> BbsArticlesPushes { get; set; }

    public virtual DbSet<BbsArticlesRelay> BbsArticlesRelays { get; set; }

    public virtual DbSet<BbsCollection> BbsCollections { get; set; }

    public virtual DbSet<BbsComment> BbsComments { get; set; }

    public virtual DbSet<BbsCommentLike> BbsCommentLikes { get; set; }

    public virtual DbSet<BbsFollow> BbsFollows { get; set; }

    public virtual DbSet<BbsMutedetail> BbsMutedetails { get; set; }

    public virtual DbSet<BbsRakingUserinfo> BbsRakingUserinfos { get; set; }

    public virtual DbSet<BbsReportdetail> BbsReportdetails { get; set; }

    public virtual DbSet<BbsSharedetail> BbsSharedetails { get; set; }

    public virtual DbSet<BbsTopic> BbsTopics { get; set; }

    public virtual DbSet<BbsUserNotification> BbsUserNotifications { get; set; }

    public virtual DbSet<BbsUserTopic> BbsUserTopics { get; set; }

    public virtual DbSet<BbsUserblock> BbsUserblocks { get; set; }

    public virtual DbSet<BbsUserinfo> BbsUserinfos { get; set; }

    public virtual DbSet<BbsUserinfoAppeal> BbsUserinfoAppeals { get; set; }

    public virtual DbSet<BbsUserinfoRecommend> BbsUserinfoRecommends { get; set; }

    public virtual DbSet<BbsViewRecord> BbsViewRecords { get; set; }

    public virtual DbSet<I18n> I18ns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.CommandTimeout(30)); // 设置命令超时时间为 30 秒

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BbsArticle>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PRIMARY");

            entity.ToTable("bbs_articles", tb => tb.HasComment("用户发布文章"));

            entity.HasIndex(e => new { e.ContentMd5, e.UserId }, "ContentMd5");

            entity.HasIndex(e => e.ForbiddenState, "ForbiddenState");

            entity.HasIndex(e => new { e.Hot, e.Status }, "Hot");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.HasIndex(e => e.UserType, "UserType");

            entity.HasIndex(e => new { e.Status, e.ArticleId }, "idx_articles_status_articleid");

            entity.HasIndex(e => new { e.Status, e.Articletype, e.Hot, e.ArticleId }, "idx_status_articletype_hot");

            entity.HasIndex(e => new { e.Status, e.PublishDateTime, e.ArticleId }, "idx_status_publishdatetime");

            entity.HasIndex(e => new { e.Status, e.UserId, e.Articletype, e.TopStartTime, e.PublishDateTime, e.ArticleId }, "idx_status_userid_articletype");

            entity.Property(e => e.ArticleId)
                .HasComment("文章ID")
                .HasColumnName("ArticleID");
            entity.Property(e => e.ArticleContent)
                .HasComment("文章详情")
                .HasColumnType("text");
            entity.Property(e => e.ArticleTitle)
                .HasMaxLength(160)
                .HasComment("文章标题");
            entity.Property(e => e.Articletype).HasComment("文章类型，0：短文，1：长文");
            entity.Property(e => e.AuditTime)
                .HasComment("审核时间")
                .HasColumnType("datetime");
            entity.Property(e => e.CoinCode)
                .HasMaxLength(180)
                .HasComment("币种code");
            entity.Property(e => e.CollectionCount).HasComment("收藏数量");
            entity.Property(e => e.CommentUserCount).HasComment("评论用户数");
            entity.Property(e => e.Comments).HasComment("评论数");
            entity.Property(e => e.ContentMd5)
                .HasMaxLength(36)
                .HasComment("文章内容m5");
            entity.Property(e => e.CreatedTime).HasComment("创建时间");
            entity.Property(e => e.EditTime).HasComment("修改时间");
            entity.Property(e => e.EditUserId).HasComment("审核管理ID");
            entity.Property(e => e.ForbiddenState).HasComment("禁言处理状态，0：待处理，1：无需处理");
            entity.Property(e => e.ForwardingFid)
                .HasComment("转发父id")
                .HasColumnName("ForwardingFId");
            entity.Property(e => e.Forwardings).HasComment("转发量");
            entity.Property(e => e.Freshness)
                .HasDefaultValueSql("'1.0000000000'")
                .HasComment("内容新鲜度(每天更新一次)");
            entity.Property(e => e.Hot).HasComment("文章热度，默认0");
            entity.Property(e => e.ImageAddress)
                .HasMaxLength(3000)
                .HasComment("附图地址,JSON格式存储");
            entity.Property(e => e.IsTop)
                .HasDefaultValueSql("b'0'")
                .HasComment("是否置顶，默认0")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Likes).HasComment("点赞数");
            entity.Property(e => e.NewsId)
                .HasComment("对应的资讯id")
                .HasColumnName("NewsID");
            entity.Property(e => e.PublishDateTime).HasComment("发布时间");
            entity.Property(e => e.PublishTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("发布时间")
                .HasColumnType("datetime");
            entity.Property(e => e.PublishingMode).HasComment("发布方式,0:手动，1：自动");
            entity.Property(e => e.Reply)
                .HasComment("回复内容冗余数据（3条，JSON格式保存）")
                .HasColumnType("text");
            entity.Property(e => e.Reports).HasComment("举报数");
            entity.Property(e => e.Shares).HasComment("分享数");
            entity.Property(e => e.ShelfReason)
                .HasMaxLength(255)
                .HasComment("下架理由")
                .HasColumnName("Shelf_Reason");
            entity.Property(e => e.Sort).HasComment("排序(likes+comments)");
            entity.Property(e => e.SourceType).HasComment("发布来源");
            entity.Property(e => e.StandPoint)
                .HasDefaultValueSql("'-1'")
                .HasComment("观点类型(空,多,中立)-1：默认，0:空，1:多，2：中立");
            entity.Property(e => e.Status).HasComment("文章状态(-1-草稿,0-待审核，1-审核通过，2-下架，3-删除)");
            entity.Property(e => e.SubscribeType).HasComment("datacenter库中push_tag_subscribe 表的订阅id");
            entity.Property(e => e.TopArea).HasComment("置顶显示位置：0-全部，1-最新，2-热门，3-关注，4-最新+热门");
            entity.Property(e => e.TopEndTime)
                .HasDefaultValueSql("'00000000000000000000'")
                .HasComment("置顶结束时间")
                .HasColumnType("bigint(20) unsigned zerofill");
            entity.Property(e => e.TopStartTime)
                .HasDefaultValueSql("'0'")
                .HasComment("置顶开始时间");
            entity.Property(e => e.UserId)
                .HasComment("用户ID")
                .HasColumnName("UserID");
            entity.Property(e => e.UserNickname)
                .HasMaxLength(25)
                .HasDefaultValueSql("''")
                .HasComment("发布用户的昵称");
            entity.Property(e => e.UserType).HasComment("用户类型(普通用户，媒体，项目方等)，32：非小号官方");
            entity.Property(e => e.View).HasComment("浏览量");
            entity.Property(e => e.ViewDuration).HasComment("浏览时长  单位：秒");
        });

        modelBuilder.Entity<BbsArticleCoin>(entity =>
        {
            entity.HasKey(e => new { e.ArticleId, e.CoinCode }).HasName("PRIMARY");

            entity.ToTable("bbs_article_coins");

            entity.HasIndex(e => new { e.ArticleId, e.CoinCode }, "idex_articleid_coincode").IsUnique();

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.CoinCode).HasMaxLength(63);
            entity.Property(e => e.CoinName).HasMaxLength(63);
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Sort)
                .HasDefaultValueSql("'0'")
                .HasComment("币种顺序，升序");

            entity.HasOne(d => d.Article).WithMany(p => p.BbsArticleCoins)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bbs_article_coins_ibfk_1");
        });

        modelBuilder.Entity<BbsArticleContent>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PRIMARY");

            entity.ToTable("bbs_article_contents");

            entity.HasIndex(e => e.ArticleId, "ArticleID").IsUnique();

            entity.Property(e => e.ArticleId)
                .HasComment("文章id")
                .HasColumnName("ArticleID");
            entity.Property(e => e.ContentHtml)
                .HasComment("文章内容带html")
                .HasColumnType("mediumtext");
        });

        modelBuilder.Entity<BbsArticleLike>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PRIMARY");

            entity.ToTable("bbs_article_likes", tb => tb.HasComment("文章点赞"));

            entity.HasIndex(e => new { e.ArticleId, e.LikeUserId }, "ArticleID").IsUnique();

            entity.HasIndex(e => new { e.LikeUserId, e.LikeStatus, e.ArticleId }, "idx_articleid_likeuser_id");

            entity.HasIndex(e => new { e.LikeUserId, e.LikeStatus, e.LikeTime, e.ArticleId }, "idx_likeuser_id_status_liketime_articleid");

            entity.HasIndex(e => new { e.LikeTime, e.UnRead }, "index_liketime_unread");

            entity.Property(e => e.LikeId).HasColumnName("LikeID");
            entity.Property(e => e.ArticleId)
                .HasComment("文章ID")
                .HasColumnName("ArticleID");
            entity.Property(e => e.LikeStatus)
                .HasDefaultValueSql("b'1'")
                .HasComment("点赞状态(1-点赞，0-取消点赞)")
                .HasColumnType("bit(1)");
            entity.Property(e => e.LikeTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("点赞时间")
                .HasColumnType("datetime");
            entity.Property(e => e.LikeUserId)
                .HasComment("点赞用户ID")
                .HasColumnName("LikeUserID");
            entity.Property(e => e.UnLikeTime)
                .HasComment("取消点赞时间")
                .HasColumnType("datetime");
            entity.Property(e => e.UnRead)
                .HasDefaultValueSql("b'1'")
                .HasComment("是否未读：1-是，0-不是")
                .HasColumnType("bit(1)");
        });

        modelBuilder.Entity<BbsArticleTopic>(entity =>
        {
            entity.HasKey(e => new { e.ArticleId, e.TopicId }).HasName("PRIMARY");

            entity.ToTable("bbs_article_topics", tb => tb.HasComment("文章参与话题列表"));

            entity.Property(e => e.ArticleId)
                .HasComment("文章ID")
                .HasColumnName("ArticleID");
            entity.Property(e => e.TopicId)
                .HasComment("话题ID")
                .HasColumnName("TopicID");
            entity.Property(e => e.ArticleStatus)
                .HasDefaultValueSql("'0'")
                .HasComment("文章状态(-1-草稿,0-待审核，1-审核通过，2-下架，3-删除)");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Sort)
                .HasDefaultValueSql("'0.0000000000'")
                .HasComment("热度");
        });

        modelBuilder.Entity<BbsArticlesPush>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PRIMARY");

            entity.ToTable("bbs_articles_push", tb => tb.HasComment("早报推送记录"));

            entity.Property(e => e.ArticleId)
                .HasComment("文章ID")
                .HasColumnName("ArticleID");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("添加时间")
                .HasColumnType("datetime");
            entity.Property(e => e.IsPush).HasComment("1:已推送，0：待推送");
            entity.Property(e => e.UpdateTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<BbsArticlesRelay>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bbs_articles_relay", tb => tb.HasComment("文章转发关联表"));

            entity.HasIndex(e => e.ArticleId, "ArticleID_INDEX");

            entity.Property(e => e.ArticleId)
                .HasComment("文章id")
                .HasColumnName("ArticleID");
            entity.Property(e => e.CreateTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("转发时间")
                .HasColumnType("datetime");
            entity.Property(e => e.ParentArticleId)
                .HasComment("被转发文章的id")
                .HasColumnName("ParentArticleID");
            entity.Property(e => e.ParentArticleUserId)
                .HasComment("被转发文章的用户id")
                .HasColumnName("ParentArticleUserID");
            entity.Property(e => e.UserId)
                .HasComment("用户的id")
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<BbsCollection>(entity =>
        {
            entity.HasKey(e => e.CollectionId).HasName("PRIMARY");

            entity.ToTable("bbs_collection", tb => tb.HasComment("用户收藏的文章"));

            entity.HasIndex(e => e.ArticleId, "articleid_index");

            entity.HasIndex(e => new { e.UserId, e.ArticleId }, "idx_articleid_userid").IsUnique();

            entity.HasIndex(e => new { e.UserId, e.CreateTime, e.ArticleId }, "idx_userid_createtime");

            entity.Property(e => e.CollectionId)
                .HasComment("主键")
                .HasColumnName("CollectionID");
            entity.Property(e => e.ArticleId)
                .HasComment("文章id")
                .HasColumnName("ArticleID");
            entity.Property(e => e.CreateTime)
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasComment("用户id")
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<BbsComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PRIMARY");

            entity.ToTable("bbs_comments", tb => tb.HasComment("文章评论"));

            entity.Property(e => e.CommentId)
                .HasComment("评论ID")
                .HasColumnName("CommentID");
            entity.Property(e => e.ArticleId)
                .HasComment("文章ID")
                .HasColumnName("ArticleID");
            entity.Property(e => e.CommentContent)
                .HasComment("评论内容")
                .HasColumnType("text");
            entity.Property(e => e.CommentStatus).HasComment("评论状态（0-待审核，1-已审核，2-已删除）");
            entity.Property(e => e.CommentTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("评论时间")
                .HasColumnType("datetime");
            entity.Property(e => e.CommentTimespan).HasComment("评论时间戳");
            entity.Property(e => e.CommentUserId)
                .HasComment("评论者ID")
                .HasColumnName("CommentUserID");
            entity.Property(e => e.Comments).HasComment("评论数");
            entity.Property(e => e.Hot)
                .HasDefaultValueSql("'0.0000000000'")
                .HasComment("评论热度，默认0");
            entity.Property(e => e.ImageAddress)
                .HasMaxLength(1000)
                .HasComment("附图地址");
            entity.Property(e => e.IsCommentArticle)
                .HasDefaultValueSql("b'0'")
                .HasComment("是否针对文章评论")
                .HasColumnType("bit(1)");
            entity.Property(e => e.IsShow)
                .HasDefaultValueSql("b'1'")
                .HasComment("是否展示默认1")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Likes).HasComment("点赞数");
            entity.Property(e => e.ParentCommentId)
                .HasComment("父评论ID")
                .HasColumnName("ParentCommentID");
            entity.Property(e => e.Reports).HasComment("举报数");
            entity.Property(e => e.Shares).HasComment("分享数");
            entity.Property(e => e.SourceType).HasComment("发布来源");
        });

        modelBuilder.Entity<BbsCommentLike>(entity =>
        {
            entity.HasKey(e => new { e.CommentId, e.LikeUserId }).HasName("PRIMARY");

            entity.ToTable("bbs_comment_likes", tb => tb.HasComment("文章点赞"));

            entity.HasIndex(e => new { e.LikeTime, e.UnRead }, "index_liketime_unread");

            entity.Property(e => e.CommentId)
                .HasComment("文章ID")
                .HasColumnName("CommentID");
            entity.Property(e => e.LikeUserId)
                .HasComment("点赞用户ID")
                .HasColumnName("LikeUserID");
            entity.Property(e => e.LikeStatus)
                .HasDefaultValueSql("b'1'")
                .HasComment("点赞状态(1-点赞，0-取消点赞)")
                .HasColumnType("bit(1)");
            entity.Property(e => e.LikeTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("点赞时间")
                .HasColumnType("datetime");
            entity.Property(e => e.LikeTimespan).HasComment("点赞时间戳");
            entity.Property(e => e.UnLikeTime)
                .HasComment("取消点赞时间")
                .HasColumnType("datetime");
            entity.Property(e => e.UnLikeTimespan).HasComment("取消赞时间戳");
            entity.Property(e => e.UnRead)
                .HasDefaultValueSql("b'1'")
                .HasComment("是否未读：1-是，0-不是")
                .HasColumnType("bit(1)");
        });

        modelBuilder.Entity<BbsFollow>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.FollowerUserId }).HasName("PRIMARY");

            entity.ToTable("bbs_follows", tb => tb.HasComment("用户关注"));

            entity.HasIndex(e => e.FollowerUserId, "FollowerUserID");

            entity.HasIndex(e => new { e.FollowerUserId, e.UserId }, "idx_followers");

            entity.Property(e => e.UserId)
                .HasComment("用户ID")
                .HasColumnName("UserID");
            entity.Property(e => e.FollowerUserId)
                .HasComment("关注者用户ID")
                .HasColumnName("FollowerUserID");
            entity.Property(e => e.FollowTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("关注时间")
                .HasColumnType("datetime");
            entity.Property(e => e.IsFriend)
                .HasComment("是否互相关注")
                .HasColumnType("bit(1)");
        });

        modelBuilder.Entity<BbsMutedetail>(entity =>
        {
            entity.HasKey(e => e.MuteDetailId).HasName("PRIMARY");

            entity.ToTable("bbs_mutedetails", tb => tb.HasComment("用户禁言明细"));

            entity.Property(e => e.MuteDetailId)
                .HasComment("禁言明细ID")
                .HasColumnName("MuteDetailID");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasComment("禁言说明");
            entity.Property(e => e.MuteReason)
                .HasMaxLength(1000)
                .HasComment("禁言原因");
            entity.Property(e => e.MuteTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("禁言时间")
                .HasColumnType("datetime");
            entity.Property(e => e.MuteUserId)
                .HasComment("用户ID")
                .HasColumnName("MuteUserID");
            entity.Property(e => e.ReleaseMuteTime)
                .HasComment("解除禁言时间")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<BbsRakingUserinfo>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("bbs_raking_userinfo", tb => tb.HasComment("一周明星用户表"));

            entity.HasIndex(e => e.RankNo, "RankNo");

            entity.Property(e => e.UserId).HasComment("用户id");
            entity.Property(e => e.ArticleCount).HasComment("上周观点数");
            entity.Property(e => e.AvgLike).HasComment("上周平均点赞数");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("timestamp");
            entity.Property(e => e.CustomRankNo)
                .HasDefaultValueSql("'999999'")
                .HasComment("人工插入排名,默认排名999999");
            entity.Property(e => e.CustomRankNoExpireTime)
                .HasDefaultValueSql("'1989-12-31 16:00:00'")
                .HasComment("人工插入排名过期时间")
                .HasColumnType("timestamp");
            entity.Property(e => e.Followcount).HasComment("粉丝数");
            entity.Property(e => e.ModifyTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("更新时间")
                .HasColumnType("timestamp");
            entity.Property(e => e.RankNo)
                .HasDefaultValueSql("'999999'")
                .HasComment("排名,默认排名999999");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasComment("用户昵称");
        });

        modelBuilder.Entity<BbsReportdetail>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PRIMARY");

            entity.ToTable("bbs_reportdetails", tb => tb.HasComment("文章举报明细"));

            entity.Property(e => e.ReportId)
                .HasComment("举报明细ID")
                .HasColumnName("ReportID");
            entity.Property(e => e.ArticleId)
                .HasComment("文章ID")
                .HasColumnName("ArticleID");
            entity.Property(e => e.ReportDescription)
                .HasMaxLength(1000)
                .HasComment("举报说明");
            entity.Property(e => e.ReportResult).HasComment("举报处理结果");
            entity.Property(e => e.ReportResultDescription)
                .HasMaxLength(1000)
                .HasComment("举报处理结果说明");
            entity.Property(e => e.ReportStatus)
                .HasDefaultValueSql("'1'")
                .HasComment("举报状态（1-待确认，2-已确认，3-已处理）");
            entity.Property(e => e.ReportTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("举报时间")
                .HasColumnType("datetime");
            entity.Property(e => e.ReportType).HasComment("举报理由，0：未设置，1：色情，2：广告，4：政治，8：其他");
            entity.Property(e => e.ReportUserId)
                .HasComment("举报人")
                .HasColumnName("ReportUserID");
        });

        modelBuilder.Entity<BbsSharedetail>(entity =>
        {
            entity.HasKey(e => new { e.ArticleId, e.ShareUserId }).HasName("PRIMARY");

            entity.ToTable("bbs_sharedetails", tb => tb.HasComment("文章分享"));

            entity.Property(e => e.ArticleId)
                .HasComment("文章ID")
                .HasColumnName("ArticleID");
            entity.Property(e => e.ShareUserId)
                .HasComment("分享用户ID")
                .HasColumnName("ShareUserID");
            entity.Property(e => e.ShareTarget).HasComment("分享目标");
            entity.Property(e => e.ShareTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("分享时间")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<BbsTopic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PRIMARY");

            entity.ToTable("bbs_topics", tb => tb.HasComment("话题"));

            entity.Property(e => e.TopicId)
                .HasComment("话题ID")
                .HasColumnName("TopicID");
            entity.Property(e => e.Comments).HasComment("评论数");
            entity.Property(e => e.CreateTime).HasComment("创建时间");
            entity.Property(e => e.CreaterUserId)
                .HasComment("话题创建者")
                .HasColumnName("CreaterUserID");
            entity.Property(e => e.Hot).HasComment("话题热度");
            entity.Property(e => e.ImageAddress)
                .HasMaxLength(1000)
                .HasComment("附图地址,多个图片用【$$$$】分开");
            entity.Property(e => e.IsTop)
                .HasDefaultValueSql("b'0'")
                .HasComment("是否置顶（0-否，1-是）")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Likes).HasComment("点赞数");
            entity.Property(e => e.Participants).HasComment("参与人数");
            entity.Property(e => e.Sort)
                .HasDefaultValueSql("'999999'")
                .HasComment("排序（默认999999）");
            entity.Property(e => e.TopicContent)
                .HasMaxLength(1000)
                .HasComment("话题内容");
            entity.Property(e => e.TopicStatus).HasComment("话题状态（1-激活，2-删除，0-待激活）");
            entity.Property(e => e.TopicTitle)
                .HasMaxLength(1000)
                .HasComment("话题标题");
            entity.Property(e => e.UpdateTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("更新时间 ")
                .HasColumnType("datetime");
            entity.Property(e => e.View)
                .HasDefaultValueSql("'0'")
                .HasComment("浏览量  ");
        });

        modelBuilder.Entity<BbsUserNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity.ToTable("bbs_user_notification");

            entity.HasIndex(e => e.SenderId, "SenderID");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.NotificationId)
                .HasComment("通知id")
                .HasColumnName("NotificationID");
            entity.Property(e => e.Category).HasComment("内容类型1:文章 2:评论 3:话题 4:用户");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasComment("通知的内容（例如“用户A评论了你的文章”）");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedTimespan).HasComment("创建时间戳");
            entity.Property(e => e.IsRead)
                .HasDefaultValueSql("'0'")
                .HasComment("是否已读（1-已读，0-未读）");
            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .HasComment("跳转链接，指向具体的资源，如文章、评论、用户主页等");
            entity.Property(e => e.SenderId)
                .HasComment("发送通知的用户ID（点赞者、评论者、回复者等）")
                .HasColumnName("SenderID");
            entity.Property(e => e.Type).HasComment("通知类型（ 0-全部 1-点赞 2-评论 3-关注 4-提及 5-回复 6-收藏  )");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("更新时间，更新为已读时使用")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedTimespan).HasComment("修改时间戳");
            entity.Property(e => e.UserId)
                .HasComment("接收通知的用户ID")
                .HasColumnName("UserID");

            entity.HasOne(d => d.Sender).WithMany(p => p.BbsUserNotificationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bbs_user_notification_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.BbsUserNotificationUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bbs_user_notification_ibfk_1");
        });

        modelBuilder.Entity<BbsUserTopic>(entity =>
        {
            entity.HasKey(e => new { e.TopicId, e.UserId }).HasName("PRIMARY");

            entity.ToTable("bbs_user_topic", tb => tb.HasComment("用户参与的话题"));

            entity.Property(e => e.TopicId)
                .ValueGeneratedOnAdd()
                .HasComment("话题ID")
                .HasColumnName("TopicID");
            entity.Property(e => e.UserId)
                .HasComment("用户ID")
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<BbsUserblock>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BlockedUserId }).HasName("PRIMARY");

            entity.ToTable("bbs_userblocks", tb => tb.HasComment("用户屏蔽列表"));

            entity.Property(e => e.UserId)
                .HasComment("用户ID")
                .HasColumnName("UserID");
            entity.Property(e => e.BlockedUserId)
                .HasComment("被屏蔽用户ID")
                .HasColumnName("BlockedUserID");
            entity.Property(e => e.BlockReason)
                .HasMaxLength(256)
                .HasComment("屏蔽原因");
            entity.Property(e => e.BlockStatus)
                .HasDefaultValueSql("'1'")
                .HasComment("屏蔽状态（1-开启，0-关闭）");
            entity.Property(e => e.BlockTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("屏蔽时间")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<BbsUserinfo>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("bbs_userinfo", tb => tb.HasComment("社区用户信息"));

            entity.Property(e => e.UserId)
                .HasComment("用户ID")
                .HasColumnName("UserID");
            entity.Property(e => e.Articles).HasComment("用户文章数");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasComment("头像");
            entity.Property(e => e.BackgroundImage)
                .HasMaxLength(255)
                .HasComment("用户信息背景图");
            entity.Property(e => e.Comments).HasComment("用户评论数");
            entity.Property(e => e.ContryAreaCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("''")
                .HasComment("手机区号")
                .HasColumnName("contry_area_code");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasComment("邮箱");
            entity.Property(e => e.Followers).HasComment("用户粉丝数");
            entity.Property(e => e.Follows).HasComment("用户关注数");
            entity.Property(e => e.Forwardings)
                .HasDefaultValueSql("'0'")
                .HasComment("转发量");
            entity.Property(e => e.Introduction)
                .HasMaxLength(400)
                .HasDefaultValueSql("''")
                .HasComment("简介");
            entity.Property(e => e.Likes)
                .HasDefaultValueSql("'0'")
                .HasComment("点赞数");
            entity.Property(e => e.ModifyTime)
                .HasComment("数据同步时间")
                .HasColumnType("datetime");
            entity.Property(e => e.MuteTime)
                .HasDefaultValueSql("'1900-01-01 00:00:00'")
                .HasComment("禁言时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasDefaultValueSql("''")
                .HasComment("手机号码");
            entity.Property(e => e.Power).HasComment("权限，0：无权限，1：有发文权限，2：有发观点评论权限 3:有全部权限");
            entity.Property(e => e.ReleaseMuteTime)
                .HasDefaultValueSql("'1900-01-01 00:00:00'")
                .HasComment("解除禁言时间")
                .HasColumnType("datetime");
            entity.Property(e => e.ReportComments).HasComment("用户被举报评论数");
            entity.Property(e => e.ReportCount).HasComment("被举报文章数");
            entity.Property(e => e.Shares)
                .HasDefaultValueSql("'0'")
                .HasComment("分享数");
            entity.Property(e => e.ShowV)
                .HasDefaultValueSql("'1'")
                .HasComment("是否显示V标志：1-是，0-不是");
            entity.Property(e => e.UserCertType).HasComment("用户认证类型");
            entity.Property(e => e.UserLevel).HasComment("用户等级");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasComment("用户昵称");
            entity.Property(e => e.UserStatus)
                .HasDefaultValueSql("'1'")
                .HasComment("用户状态（1-激活，0-关闭）");
            entity.Property(e => e.UserType).HasComment("用户类型(0：普通用户，1：项目方，2：交易所，4：媒体，6：微博等,7:推特 ， 8：自媒体，16：个人，32:官方,64:钱包)");
        });

        modelBuilder.Entity<BbsUserinfoAppeal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bbs_userinfo_appeal", tb => tb.HasComment("用户申诉明细"));

            entity.HasIndex(e => new { e.ModifyTime, e.State, e.UserId }, "ModifyTime");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.Content)
                .HasMaxLength(300)
                .HasDefaultValueSql("''")
                .HasComment("申诉内容");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Images)
                .HasComment("图片url数组")
                .HasColumnType("text");
            entity.Property(e => e.ModifyTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.State).HasComment("处理状态，0:待处理，1：已处理,2:已禁言");
            entity.Property(e => e.UserId)
                .HasComment("用户id")
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<BbsUserinfoRecommend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bbs_userinfo_recommend", tb => tb.HasComment("推荐用户列表"));

            entity.Property(e => e.Id)
                .HasComment("推荐ID")
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Sort).HasComment("排序");
            entity.Property(e => e.UserId)
                .HasComment("推荐用户ID")
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<BbsViewRecord>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("PRIMARY");

            entity.ToTable("bbs_view_record", tb => tb.HasComment("用户浏览记录表"));

            entity.HasIndex(e => e.ContentType, "idx_content_type");

            entity.HasIndex(e => new { e.UserId, e.ContentId }, "idx_user_content");

            entity.Property(e => e.ViewId)
                .HasComment("浏览记录 ID")
                .HasColumnName("ViewID");
            entity.Property(e => e.ContentId)
                .HasComment("浏览内容 ID")
                .HasColumnName("ContentID");
            entity.Property(e => e.ContentType).HasComment("内容类型（1-文章，2-评论，3-其他）");
            entity.Property(e => e.Duration).HasComment("浏览时长（秒）");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .HasComment("用户 IP 地址")
                .HasColumnName("IPAddress");
            entity.Property(e => e.Referrer)
                .HasMaxLength(500)
                .HasComment("来源链接");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(255)
                .HasComment("用户设备信息");
            entity.Property(e => e.UserId)
                .HasComment("用户 ID，关联用户表")
                .HasColumnName("UserID");
            entity.Property(e => e.ViewTime)
                .HasComment("浏览时间")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<I18n>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("i18n", tb => tb.HasComment("国际化表"));

            entity.HasIndex(e => e.MsgKey, "msg_key").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("id")
                .HasColumnName("id");
            entity.Property(e => e.Cn)
                .HasMaxLength(255)
                .HasColumnName("cn");
            entity.Property(e => e.Hk)
                .HasMaxLength(255)
                .HasColumnName("hk");
            entity.Property(e => e.Jp)
                .HasMaxLength(255)
                .HasColumnName("jp");
            entity.Property(e => e.MsgKey)
                .HasComment("消息模板 key")
                .HasColumnName("msg_key");
            entity.Property(e => e.Tw)
                .HasMaxLength(255)
                .HasColumnName("tw");
            entity.Property(e => e.Us)
                .HasMaxLength(255)
                .HasColumnName("us");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
