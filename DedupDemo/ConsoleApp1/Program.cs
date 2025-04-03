using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.Json.Serialization;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 读取文本文件为字符串
            string filePath = "C:\\Users\\victor\\Desktop\\xxx.txt";
            string fileContent = File.ReadAllText(filePath);

            var jObject = JObject.Parse(fileContent);
            var instructions = jObject["result"]?["timeline"]?["instructions"];
            var posts = new List<TwitterPostDto>();
            foreach (var instruction in instructions)
            {
                var entries = instruction["entries"];
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        var post = ParseTweetEntryFromJToken(entry); // 你可以写一个支持JToken的解析函数
                        if (post != null)
                            posts.Add(post);
                    }
                }
                else if (instruction["entry"] != null)
                {
                    var post = ParseTweetEntryFromJToken(instruction["entry"]);
                    if (post != null)
                        posts.Add(post);
                }
            }
        }

public static TwitterPostDto ParseTweetEntryFromJToken(JToken entry)
    {
        var tweet = entry["content"]?["itemContent"]?["tweet_results"]?["result"];
        if (tweet == null)
            return null;

        string entryIdStr = entry["entryId"]?.ToString()?.Replace("tweet-", "");
        if (string.IsNullOrEmpty(entryIdStr))
            return null;

        // 获取文本内容：优先用 note_tweet -> text，其次 fallback 到 legacy.full_text
        string text = tweet["note_tweet"]?["note_tweet_results"]?["result"]?["text"]?.ToString()
                      ?? tweet["legacy"]?["full_text"]?.ToString()
                      ?? "";

        var post = new TwitterPostDto
        {
            Id = long.TryParse(entryIdStr, out var id) ? id : 0,
            Text = text,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Media = new List<MediaDto>()
        };

        var mediaItems = tweet["legacy"]?["extended_entities"]?["media"];
        if (mediaItems != null)
        {
            foreach (var media in mediaItems)
            {
                post.Media.Add(new MediaDto
                {
                    Type = "photo", // 固定为 photo
                    Url = media["expanded_url"]?.ToString() ?? ""
                });
            }
        }

        return post;
    }

    static TwitterPostDto ParseTweetEntry(dynamic entry)
        {
            var tweet = entry.content?.itemContent?.tweet_results?.result;
            if (tweet == null) return null;

            var noteTweetText = tweet.note_tweet?.note_tweet_results?.result?.text;
            string text = noteTweetText != null ? (string)noteTweetText : (string)tweet.legacy?.full_text;

            var post = new TwitterPostDto
            {
                Id = long.Parse(((string)entry.entryId).Replace("tweet-", "")),
                Text = text,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Media = new List<MediaDto>()
            };

            var mediaItems = tweet.legacy?.extended_entities?.media;
            if (mediaItems != null)
            {
                foreach (var m in mediaItems)
                {
                    post.Media.Add(new MediaDto
                    {
                        Type = "photo", // 固定类型
                        Url = m.expanded_url != null ? (string)m.expanded_url : ""
                    });
                }
            }

            return post;
        }


    }


    public class TwitterPostDto
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string CreatedAt { get; set; }
        public List<MediaDto> Media { get; set; }
    }

    public class MediaDto
    {
        public string Type { get; set; }
        public string Url { get; set; }
    }


}
