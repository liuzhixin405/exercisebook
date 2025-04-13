using Pandora.Pipeline.Tools;
using SimMetricsCore.API;
using SimMetricsCore.Metric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using 内容相似度比较.Tools;
using 内容相似度比较.Tools.Pandora.Pipeline.Tools;

class Program
{
    static void Main()
    {
        // 模拟新闻采集
        //string[] newNewsList = {
        //    "据PandoraTech News显示，芝加哥商品交易所集团 (CME) 2025年第一季度加密货币衍生品日均交易量达198,000份合约，名义价值113亿美元，创历史新高。微型以太坊期货交易量达76,000份，微型比特币期货年增113%，日均交易量达77,000份。微型合约因门槛低、便于风险管理，推动整体交易活跃度飙升。",
        //    "据PandoraTech News显示，据 CoinDesk 报道，芝加哥商品交易所集团 (CME) 2025 年第一季度加密货币衍生品日均交易量达 198,000 份合约，名义价值 113 亿美元，创历史新高。\r\n微型以太坊期货交易量达 76,000 份，微型比特币期货年增 113%，日均交易量达 77,000 份。微型合约因门槛低、便于风险管理，推动整体交易活跃度飙升。"
        //}; //通过测试
        //      string[] newNewsList = {
        //	"据PandoraTech News显示，行情显示，ACT短时跌破0.055美元，现报0.05531美元，24小时跌幅达到43.74%，行情波动较大，请做好风险控制。",
        //	"据PandoraTech News显示，4 月 2 日，据 HTX 行情数据显示，ACT 跌破 0.055 美元，现报价 0.0537 美元，24 小时跌幅 44.7%。"
        //};   //不通过测试
        //string[] newNewsList = {
        //	"据PandoraTech News显示，据 BNB Chain 官方公告，BNB Chain 已宣布首批 1 亿美元流动性激励计划获奖项目名单。KiloEX(KILO)、Mubarak(MUBARAK)、CZ'S Dog(BROCCOLI714)、Tutorial(TUT) 和 Banana For Scale(BANANAS31) 五个项目成为首批获得流动性支持奖励的项目。 此次奖励总额高达 230 万美元，仅限于 3 月 24 日后在 CEX 上市的资产。其中，在币安上市的项目最高可获 50 万美元支持。奖励将于本周内发放。",
        //	"深潮 TechFlow 消息，4 月 2 日，据BNB Chain官方公告，BNB Chain 已宣布首批 1 亿美元流动性激励计划获奖项目名单。KiloEX(KILO)、Mubarak(MUBARAK)、CZ'S Dog(BROCCOLI714)、Tutorial(TUT) 和 Banana For Scale(BANANAS31) 五个项目成为首批获得流动性支持奖励的项目。\r\n此次奖励总额高达 230 万美元，仅限于 3 月 24 日后在 CEX 上市的资产。其中，在币安上市的项目最高可获 50 万美元支持。奖励将于本周内发放。\r\nBNB Chain 表示，已获奖项目仍可通过在更多交易所上市获取额外奖励，未获奖项目仍有机会参与下一批评选。"
        //};//不通过测试
        //string[] newNewsList = {
        //          "据PandoraTech News显示，据官方消息，Binance 宣布开启第二轮投票上币活动。参与本次活动的币种包括：VIRTUAL、BIGTIME、UXLINK、MORPHO、GRASS、ATH、WAL、SAFE、ZETA、IP、ONDO、PLUME。",
        //          "深潮 TechFlow 消息，4 月 2 日，据官方公告，币安开启第二轮投票上币活动，包括VIRTUAL、BIGTIME、UXLINK、MORPHO、GRASS、ATH、WAL、SAFE、ZETA、IP、ONDO、PLUME。"
        //      };  //不通过测试
        //string[] newNewsList = {
        //    "据PandoraTech News显示，据官方公告，日本上市游戏开发商 enish 宣布将比特币纳入其金融战略，计划于 4 月 1 日至 4 日期间购买价值 1 亿日元（约 67 万美元）的比特币。\r\n据悉，该公司将比特币购买定位为其整体金融战略的重要组成部分。",
        //    "深潮 TechFlow 消息，4 月 2 日，据官方公告，日本上市游戏开发商enish宣布将比特币纳入其金融战略，计划于4月1日至4日期间购买价值1亿日元（约67万美元）的比特币。该公司将比特币购买定位为其整体金融战略的重要组成部分。"
        //};
        //string[] newNewsList = {
        //    "据PandoraTech News显示，据PeckShield监测，First Digital Labs在过去24小时内销毁了总计8700万枚FDUSD。这其中包括从与Wintermute相关的一个地址转入First Digital Labs的约4900万枚FDUSD，随后这些资金被销毁。 此外，First Digital Labs向地址0xe79a...1d93发送了约5300万枚FDUSD，该地址随后返还了约4000万枚USDT，并将剩余的1300万枚FDUSD发送到币安。",
        //    "据PandoraTech News显示，据 PeckShieldAlert 监测，First Digital Labs 在过去 24 小时内总计销毁 8700 万枚 FDUSD。其中约 4900 万枚 FDUSD 是从与 Wintermute 相关的地址转移至 First Digital Labs 后被销毁。此外，First Digital Labs 还向一个地址发送了约 5300 万枚 FDUSD。"
        //};
        //string[] newNewsList = {
        //    "数据：First Digital Labs过去24小时内已销毁8700万枚FDUSD",
        //    "First Digital Labs在过去24小时内已销毁8700万枚FDUSD"
        //};
        //string[] newNewsList = {

        //    "美国现货比特币ETF昨日净流入2.181亿美元",
        //    "昨日美国比特币现货ETF净流入2.181亿美元"
        //};
        //string[] newNewsList = {

        //    "Stake Stone公布STO代币经济学：总量10亿枚，空投和未来激励占比7.85%",
        //    "Stake Stone公布STO代币经济学模型，空投和未来激励占比7.85%"
        //};
        //string[] newNewsList = {

        //    "币安将为ARDR，BSW等添加观察标签，并移除JUP，STRK和TON的种子标签",
        //    "Binance将为ARDR、BSW和FLM等代币添加观察标签，并移除JUP、STRK和TON的种子标签"
        //};
        //string[] newNewsList = {

        //    "BNB跌破600美元",
        //    "BNB短时跌破600美元"
        //};
        //string[] newNewsList = {

        //    "F2Pool联创王纯更新其太空飞行记录，马斯克转发",
        //    "F2Pool联创王纯更新其太空飞行记录，Elon Musk转发"
        //};
        //string[] newNewsList = {

        //    "某鲸鱼向交易所存入 500 万美元 USDC 并开启比特币 20 倍杠杆多单",
        //    "某鲸鱼向交易所存入500万美元USDC并开启比特币20倍杠杆多单"
        //};
        string[] newNewsList = {

           "F2Pool联创王纯更新其太空飞行记录，马斯克转发",
           "F2Pool联创王纯更新其太空飞行记录，Elon Musk转发"
        };
        //string[] newNewsList = {

        //    "据PandoraTech News显示，4 月 4 日，F2Pool 联创王纯在 X 平台更新其太空飞行第四日记录：「我早早醒来，在 YouTube 上观看了 Starlink Group 11-13 的发射。不久之后，SpaceX 与我们联系，告知我们在第二阶段脱轨燃烧期间，我们将飞越蒙古。我们打开了发射舱，试图观察这一过程，但没有成功。尽管如此，我们从孟加拉湾一直飞到北极，一路欣赏着这一美景。」此前消息，F2Pool 联创王纯于 3 月 31 日晚，乘坐 SpaceX 的猎鹰 9 号火箭在佛罗里达州卡纳维拉尔角空军基地开始太空飞行之旅。",
        //    "深潮 TechFlow 消息，4 月 4 日，F2Pool 联创王纯在 X 平台发文，“飞行第4天：我早早醒来，在YouTube上观看了Starlink Group 11-13的发射。不久之后，SpaceX联系我们，告知我们在二级火箭进行离轨燃烧时，我们将飞越蒙古上空。我们打开了穹顶观察窗，试图观察这一现象，但并未成功。尽管如此，当我们从孟加拉湾一直飞到北极时，我们都欣赏到了美妙的景色。”\r\nElon Musk 转发引用此推文转发称“我刚刚通过星链（Starlink）和 fram2（@framonauts） 进行了 FaceTime 视频通话！”\r\n此前消息，SpaceX 的载人龙飞船将于 2025 年 4 月 1 日开启第六次载人航天任务（Fram-2），本次任务共有 4 名宇航员参与：王纯、扬妮克·米克尔森、拉贝亚·罗格、安埃里克·菲利普斯。"
        //};
        //var text1 = "数据：First Digital Labs过去24小时内已销毁8700万枚FDUSD";
        //var text2 = "First Digital Labs在过去24小时内已销毁8700万枚FDUSD";
        //var dedup = new TextDeduplicator("127.0.0.1:6379");
        //Console.WriteLine($"{newNewsList[0]}和{newNewsList[1]}比较的相似度为：{dedup.CalculateCharJaccardSimilarity(TextDeduplicator.NormalizeText(newNewsList[0]), TextDeduplicator.NormalizeText(newNewsList[1]))}");
        //var text1 = "F2Pool联创王纯更新其太空飞行记录，马斯克转发";
        //var text2 = "F2Pool联创王纯更新其太空飞行记录，Elon Musk转发";

        //foreach (var news in newNewsList)
        //{
        //    //collector.ProcessNewNews(news);
        //    var result = new SimDeduplicator("127.0.0.1:6379").Deduplicate(news, true, DateTime.Today, 3);
        //    Console.WriteLine($"{news} 是否重复{result}");
        //    //NewsDeduplicator.IsDuplicate(news);       
        //}



        //var text1 = "据PandoraTech News显示，行情显示，ACT短时跌破0.055美元，现报0.05531美元，24小时跌幅达到43.74%，行情波动较大，请做好风险控制。";
        //var text2 = "据PandoraTech News显示，4 月 2 日，据 HTX 行情数据显示，ACT 跌破 0.055 美元，现报价 0.0537 美元，24 小时跌幅 44.7%。";
        ////var text1 = "据PandoraTech News显示，4 月 4 日，F2Pool 联创王纯在 X 平台更新其太空飞行第四日记录：「我早早醒来，在 YouTube 上观看了 Starlink Group 11-13 的发射。不久之后，SpaceX 与我们联系，告知我们在第二阶段脱轨燃烧期间，我们将飞越蒙古。我们打开了发射舱，试图观察这一过程，但没有成功。尽管如此，我们从孟加拉湾一直飞到北极，一路欣赏着这一美景。」此前消息，F2Pool 联创王纯于 3 月 31 日晚，乘坐 SpaceX 的猎鹰 9 号火箭在佛罗里达州卡纳维拉尔角空军基地开始太空飞行之旅。";
        ////var text2 = "深潮 TechFlow 消息，4 月 4 日，F2Pool 联创王纯在 X 平台发文，“飞行第4天：我早早醒来，在YouTube上观看了Starlink Group 11-13的发射。不久之后，SpaceX联系我们，告知我们在二级火箭进行离轨燃烧时，我们将飞越蒙古上空。我们打开了穹顶观察窗，试图观察这一现象，但并未成功。尽管如此，当我们从孟加拉湾一直飞到北极时，我们都欣赏到了美妙的景色。”\r\nElon Musk 转发引用此推文转发称“我刚刚通过星链（Starlink）和 fram2（@framonauts） 进行了 FaceTime 视频通话！”\r\n此前消息，SpaceX 的载人龙飞船将于 2025 年 4 月 1 日开启第六次载人航天任务（Fram-2），本次任务共有 4 名宇航员参与：王纯、扬妮克·米克尔森、拉贝亚·罗格、安埃里克·菲利普斯。";
        //var text1="币安将为ARDR，BSW等添加观察标签，并移除JUP，STRK和TON的种子标签";
        //var text2="Binance将为ARDR、BSW和FLM等代币添加观察标签，并移除JUP、STRK和TON的种子标签";
        // 使用 Jaro-Winkler 算法（适合短文本）
        //IStringMetric metric = new JaroWinkler();

        //double similarity = metric.GetSimilarity(text1, text2);
        //Console.WriteLine($"Jaro-Winkler 相似度: {similarity}");

        //// 使用 Cosine 算法（适合词袋模型、长文本）
        //metric = new CosineSimilarity();
        //similarity = metric.GetSimilarity(text1, text2);
        //Console.WriteLine($"Cosine 相似度: {similarity}");

        //// Jaccard 也可以试试
        //metric = new JaccardSimilarity();
        //similarity = metric.GetSimilarity(text1, text2);
        //Console.WriteLine($"Jaccard 相似度: {similarity}");


        var simhash = new SimpleSimHash();
        var hash1 = simhash.ComputeSimHash("据PandoraTech News显示，行情显示，ACT短时跌破0.055美元，现报0.05531美元，24小时跌幅达到43.74%，行情波动较大，请做好风险控制。");
        var hash2 = simhash.ComputeSimHash("据PandoraTech News显示，4 月 2 日，据 HTX 行情数据显示，ACT 跌破 0.055 美元，现报价 0.0537 美元，24 小时跌幅 44.7%。");

        int distance = simhash.HammingDistance(hash1, hash2);
        Console.WriteLine($"Hamming 距离：{distance}，相似度：{(64 - distance) / 64.0:F2}");
    }
}