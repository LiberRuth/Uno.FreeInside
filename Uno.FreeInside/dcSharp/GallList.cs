using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace dcSniper.API;

internal class GallList
{
    private HtmlNodeCollection? htmlNodes_list;
    private HtmlNodeCollection? htmlNodes_title;
    private HtmlNodeCollection? htmlNodes_PageNumber;
    private HtmlNodeCollection? htmlNodes_PageSearchNumber;
    private HtmlNodeCollection? htmlNodes_NewPageSearch;
    private HtmlNodeCollection? htmlNodes_typeNetPageEnd;
    private HtmlNodeCollection? htmlNodes_typeSearchNetPageEnd;

    public async Task GetGallList(string URL)
    {

        var httpClient = new HttpClient();

        try
        {

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 11.0; Win64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5824.213 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.9");
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://gall.dcinside.com/");
            var response = await httpClient.GetAsync(URL);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            htmlNodes_list = doc.DocumentNode.SelectNodes("//tbody/tr[@class='ub-content us-post'] | //tbody/tr[@class='ub-content us-post thum']");
            htmlNodes_title = doc.DocumentNode.SelectNodes("//div/div[@class='tab_menubox']");
            htmlNodes_PageNumber = doc.DocumentNode.SelectNodes("//div[@class='bottom_paging_wrap']/div[@class='bottom_paging_box iconpaging']/*");
            htmlNodes_PageSearchNumber = doc.DocumentNode.SelectNodes("//div[@class='bottom_paging_wrap re']/div[@class='bottom_paging_box iconpaging']/*");
            htmlNodes_NewPageSearch = doc.DocumentNode.SelectNodes("//div/div[@class='bottom_paging_box iconpaging']/a[@class='search_next']");
            htmlNodes_typeNetPageEnd = doc.DocumentNode.SelectNodes("//div/a[@class='sp_pagingicon page_end']");
            htmlNodes_typeSearchNetPageEnd = doc.DocumentNode.SelectNodes("//div/div[@class='bottom_paging_box iconpaging']/a[@class='search_next']");

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

        string dataTitle = "";
        foreach (var Nodes in htmlNodes_title!)
        {
            dataTitle = Nodes.SelectSingleNode(".//p[@class='gallname']").InnerText;
        }
        return dataTitle;
    }


    public List<IDictionary<string, string>> Gall_List()
    {
        if (htmlNodes_list == null) return null!;

        List<IDictionary<string, string>>? dataList = new List<IDictionary<string, string>>();

        foreach (var Nodes in htmlNodes_list!)
        {

            IDictionary<string, string> dataIDictionary = new Dictionary<string, string>();

            var gallNum = Nodes.SelectSingleNode(".//td[@class='gall_num']");
            dataIDictionary.Add("Num", gallNum.InnerText);

            var gallSubject_inner = Nodes.SelectSingleNode(".//td[@class='gall_subject']/p[@class='subject_inner']");
            var gallSubject = Nodes.SelectSingleNode(".//td[@class='gall_subject']");

            if (gallSubject_inner != null)
            {
                dataIDictionary.Add("Subject", gallSubject_inner.InnerText);
            }
            else if (gallSubject != null) 
            {
                dataIDictionary.Add("Subject", gallSubject.InnerText);
            }
            else
            {
                dataIDictionary.Add("Subject", null!);
            }

            var gallTitle = Nodes.SelectSingleNode(".//td[@class]/a[@view-msg]");
            dataIDictionary.Add("Title", gallTitle.InnerText.Trim());

            var replyNumbox = Nodes.SelectSingleNode(".//td[@class='gall_tit ub-word']/a[@class='reply_numbox']/span");
            if (replyNumbox != null)
            {
                dataIDictionary.Add("Reply", replyNumbox.InnerText);
            }
            else
            {
                dataIDictionary.Add("Reply", " ");
            }

            var pageURL = Nodes.SelectSingleNode(".//td[@class='gall_tit ub-word'] | .//td[@class='gall_tit ub-word voice_tit']");
            if (pageURL != null)
            {
                var galllink = pageURL.SelectSingleNode(".//a[@href]").Attributes["href"];
                dataIDictionary.Add("GallURL", galllink.Value);
            }

            var userName_member = Nodes.SelectNodes(".//td[@class='gall_writer ub-writer']/span[@class='nickname in']");
            var userName_Nonmembers = Nodes.SelectNodes(".//td[@class='gall_writer ub-writer']/span[@class='nickname']");
            var userIP = Nodes.SelectNodes(".//td[@class='gall_writer ub-writer']/span[@class='ip']");
            var userName_notification = Nodes.SelectNodes(".//td[@class='gall_writer ub-writer']/b");

            if (userName_member != null)
            {
                foreach (var user in userName_member)
                {
                    dataIDictionary.Add("User", user.InnerText);
                }
            }
            else if (userName_notification != null)
            {
                foreach (var user in userName_notification)
                {
                    dataIDictionary.Add("User", user.InnerText);
                }

            }
            else
            {
                string IP_plus = "";
                foreach (var user in userName_Nonmembers)
                {
                    IP_plus = user.InnerText;
                }
                if (userIP != null)
                {
                    foreach (var txtIP in userIP)
                    {
                        IP_plus += txtIP.InnerText;
                    }
                }
                dataIDictionary.Add("User", IP_plus);

            }

            var userNikcon = Nodes.SelectSingleNode(".//td[@class='gall_writer ub-writer']/a[@class='writer_nikcon '] | " +
                ".//td[@class='gall_writer ub-writer']/a[@class='writer_nikcon'] | " +
                ".//td[@class='gall_writer ub-writer']/b/a[@class='writer_nikcon ']");
            if (userNikcon != null)
            {
                var nikconImg = userNikcon.SelectSingleNode(".//img[@src]").Attributes["src"];
                var match = Regex.Match(nikconImg.Value, @"([^\/]+)(?=\.[^.\/]+$)");
                dataIDictionary.Add("UserNikcon", match.Groups[1].Value);
            }

            var gallDate = Nodes.SelectSingleNode(".//td[@class='gall_date']");
            dataIDictionary.Add("Date", gallDate.InnerText);

            var gallCount = Nodes.SelectSingleNode(".//td[@class='gall_count']");
            dataIDictionary.Add("Count", gallCount.InnerText);

            var gallRecommend = Nodes.SelectSingleNode(".//td[@class='gall_recommend']");
            dataIDictionary.Add("Recommend", gallRecommend.InnerText);

            dataList.Add(dataIDictionary);

        }

        return dataList;
    }

    public int MaxPaging()
    {
        if (htmlNodes_typeNetPageEnd != null)
        {
            foreach (var Nodes in htmlNodes_typeNetPageEnd)
            {
                string nextURL = Nodes.Attributes["href"].Value;
                string pattern = @"page=(\d+)";
                Match match = Regex.Match(nextURL, pattern);
                string pageValue = match.Groups[1].Value;
                int pageNumber = int.Parse(pageValue);
                return pageNumber;
            }
        }
        else
        {
            int pageNumber = 0;
            if (htmlNodes_PageNumber != null)
            {
                foreach (var Nodes in htmlNodes_PageNumber!)
                {
                    pageNumber++;
                }
            }
            return pageNumber;
        }
        return 1;
    }

    public int MaxSearchPaging()
    {
        if (htmlNodes_PageSearchNumber == null) return 0;
        int pageNumber = 0;
        foreach (var Nodes in htmlNodes_PageSearchNumber)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Nodes.OuterHtml);
            var next_paging = htmlDocument.DocumentNode.Descendants("a")
                .FirstOrDefault(node => node.GetAttributeValue("class", "") == "sp_pagingicon page_end");
            var paging_number = htmlDocument.DocumentNode.Descendants("a")
                .FirstOrDefault(node => node.GetAttributeValue("class", "") == "search_next");
            if (next_paging != null)
            {
                string nextURL = next_paging.Attributes["href"].Value;
                string pattern = @"page=(\d+)";
                Match match = Regex.Match(nextURL, pattern);
                string pageValue = match.Groups[1].Value;
                return int.Parse(pageValue);
            }
            if (paging_number == null) pageNumber++;
        }
        return pageNumber;
    }

    public string NewPageSearch()
    {
        if (htmlNodes_NewPageSearch == null) return null!;
        string? newNextURL = "";
        foreach (var Nodes in htmlNodes_NewPageSearch!)
        {
            newNextURL = $"https://gall.dcinside.com{Nodes.Attributes["href"].Value}";
        }
        return newNextURL;
    }

    public bool TypeNextPageEnd() => htmlNodes_typeNetPageEnd != null;

    public bool TypeSearchNextPageEnd() => (htmlNodes_typeSearchNetPageEnd != null);

}
