rm -rf /usr/apps/crawler/
dotnet publish /usr/svn/Crawler/CrawlerConsole/CrawlerConsole.csproj -o /usr/apps/crawler
ln -s /usr/svn/Crawler/chromedriver /usr/apps/crawler/chromedriver
systemctl restart crawler.service