using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web;
using Uno.FreeInside.Data;

namespace dcSniper.API;

internal class MGallList
{
    private HtmlNodeCollection? htmlNodes_list;
    private HtmlNodeCollection? htmlNodes_title;
    private HtmlNodeCollection? htmlNodes_detailbox;
    private HtmlNodeCollection? htmlNodes_next;
    private HtmlNode? htmlNodes_newNewt;

    public async Task GetGallList(string URL)
    {

        var httpClient = new HttpClient();
        string UA = RandomUA.UserAgent();

        try
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UA);
            //httpClient.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Cookie", "list_count=30");
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://m.dcinside.com/");
            var response = await httpClient.GetAsync(URL);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();

            HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(html);
            doc.LoadHtml(HttpUtility.HtmlDecode(html));
            htmlNodes_list = doc.DocumentNode.SelectNodes("//section/ul[@class='gall-detail-lst ']/li | //section/ul[@class='gall-detail-lst thum']/li");
            htmlNodes_title = doc.DocumentNode.SelectNodes("//div/div[@class='brick-wid']/section/div/h3/a[@class]");
            htmlNodes_detailbox = doc.DocumentNode.SelectNodes("//section[@class='gall-lst-group grid ']/div[@class='mal-sw-wrap']");
            htmlNodes_next = doc.DocumentNode.SelectNodes("//div[@class='sec-wrap-sub']/div[@class]/section/a[@id='listMore']");
            htmlNodes_newNewt = doc.DocumentNode.SelectSingleNode("//section[@class='gall-lst-group grid ']/div[@id='pagination_div']/a[@class='next']");
        }
        catch (HttpRequestException e)
        {
            throw new Exception(e.Message, e);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public string Gall_Title() 
    {
        if (htmlNodes_title == null) return null!;

        string gallTitle = "";

        foreach (var itemTitle in htmlNodes_title)
        {
            gallTitle = itemTitle.InnerText.Trim();
        }

        return gallTitle;
    }

    public List<IDictionary<string, string>> Gall_List() 
    {
        if (htmlNodes_list == null) return null!;

        List<IDictionary<string, string>>? dataList = new List<IDictionary<string, string>>();

        foreach (var itemNodes in htmlNodes_list)
        {
            IDictionary<string, string> dataIDictionary = new Dictionary<string, string>();

            var gallTitle = itemNodes.SelectSingleNode(".//div/a[@class='lt']/span/span[@class='subjectin']");
            if (gallTitle != null)
            {
                dataIDictionary.Add("Title", gallTitle.InnerText);

                dataList.Add(dataIDictionary);
            }

            var userName = itemNodes.SelectSingleNode("./span[@data-name]");
            var userInfo = itemNodes.SelectSingleNode("./span[@data-info]");
            if (userName != null)
            {
                var userIP = userName.Attributes["data-info"].Value;
                if (Regex.IsMatch(userIP, @"^(\d{1,3})\.(\d{1,3})$"))
                {
                    dataIDictionary.Add("User", $"{userName.Attributes["data-name"].Value}({userIP})");
                }
                else
                {
                    dataIDictionary.Add("User", userName.Attributes["data-name"].Value);
                }

            }

            int tagindex = 1;
            var gallSubject = itemNodes.SelectSingleNode($".//div/a[@class='lt']/ul[@class='ginfo ']/li[{tagindex}]");
            if (htmlNodes_detailbox != null) 
            {
                if (gallSubject != null)
                {
                    //string subject = gallSubject.InnerText;
                    //subject = subject.Remove(0, 1);
                    //subject = subject.Remove(subject.Length - 1, 1);
                    dataIDictionary.Add("Subject", gallSubject.InnerHtml);
               
                    tagindex++;
                }
            }
            else
            {
                dataIDictionary.Add("Subject", " ");
            }
            var userIcon = itemNodes.SelectSingleNode($".//div/a[@class='lt']/ul[@class='ginfo ']/li[{tagindex}]");
            if (userIcon != null)
            {
                var nikconImg = userIcon.SelectSingleNode(".//span");
                var nikconImgBest = userIcon.SelectSingleNode(".//img");
                if (nikconImg != null)
                {
                    //dataIDictionary.Add("UserNikcon", nikconImg.Attributes["class"].Value.Replace(" ", "_"));
                    dataIDictionary.Add("UserNikcon","-");
                }
                else if (nikconImgBest != null)
                {
                    var match = Regex.Match(nikconImgBest.Attributes["src"].Value, @"([^\/]+)(?=\.[^.\/]+$)");
                    //dataIDictionary.Add("UserNikcon", match.Groups[1].Value);
                    dataIDictionary.Add("UserNikcon", "-");
                }

                //dataIDictionary.Add("User", userName.InnerText);
                tagindex++;
            }

            var gallDate = itemNodes.SelectSingleNode($".//div/a[@class='lt']/ul[@class='ginfo ']/li[{tagindex}]");
            if (gallDate != null)
            {
                dataIDictionary.Add("Date", gallDate.InnerText);
                tagindex++;
            }

            var gallCount = itemNodes.SelectSingleNode($".//div/a[@class='lt']/ul[@class='ginfo ']/li[{tagindex}]");
            if (gallCount != null)
            {
                dataIDictionary.Add("Count", gallCount.InnerText.Replace("조회", "").Trim());
                tagindex++;
            }

            var gallRecommend = itemNodes.SelectSingleNode($".//div/a[@class='lt']/ul[@class='ginfo ']/li[{tagindex}]/span");
            if (gallRecommend != null)
            {
                dataIDictionary.Add("Recommend", gallRecommend.InnerText);
            }

            var gallUrl = itemNodes.SelectNodes(".//div");
            foreach (var itemUrl in gallUrl)
            {
                var itemLink = itemUrl.SelectSingleNode(".//a");
                if (itemLink != null)
                {
                    Match match = Regex.Match(itemLink.Attributes["href"].Value, @"\/(\w+)\/(\w+)\/(\d+)");

                    string gallType = match.Groups[1].Value;
                    string gallId = match.Groups[2].Value;
                    string number = match.Groups[3].Value;

                    dataIDictionary.Add("Num", number);
                    //dataIDictionary.Add("GallURL", $"https://m.dcinside.com/{gallType}/view/?id={gallId}&no={number}");
                    dataIDictionary.Add("GallURL", itemLink.Attributes["href"].Value);
                }
            }

            var gallReply = itemNodes.SelectSingleNode(".//div/a[@class='rt']/span[@class]");
            if (gallReply != null)
            {
                if (gallReply.InnerText.ToString() != "0")
                {
                    dataIDictionary.Add("Reply", gallReply.InnerText.ToString());
                }
                else
                {
                    dataIDictionary.Add("Reply", " ");
                }
            }
        }

        return dataList;
    }

    public bool NextPage()
    {
        return htmlNodes_next != null;
    }

    public string NextNewPage()
    {
        if (htmlNodes_newNewt == null) return null!;

        string nextPage = htmlNodes_newNewt.Attributes["href"].Value;
        nextPage = nextPage.Replace("&amp;", "&");

        Uri uri = new Uri(nextPage);
        string query = uri.Query;
        var queryParameters = HttpUtility.ParseQueryString(query);
        string idValue = queryParameters["s_pos"]!;

        return idValue;
    }
}
