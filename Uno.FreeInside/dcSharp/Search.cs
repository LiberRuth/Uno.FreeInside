using HtmlAgilityPack;

namespace dcSharp.API;

internal class Search
{
    private HtmlNodeCollection? htmlNodes_gallery;
    private HtmlNodeCollection? htmlNodes_post;

    public async Task GetSearch(string URL)
    {
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0");

        //string encodeName = HttpUtility.UrlEncode(txt, Encoding.UTF8);
        //encodeName = encodeName.Replace("%", ".").ToUpper();
        //encodeName = encodeName.Replace("+", "").ToUpper();


        try
        {
            var response = await httpClient.GetAsync(URL);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            htmlNodes_gallery = doc.DocumentNode.SelectNodes("//div[@class='integrate_cont gallsch_result_all brd'] | " +
                "//div[@class='integrate_cont integrate_recom'] ");
            htmlNodes_post = doc.DocumentNode.SelectNodes("//div[@class='integrate_cont sch_result result_all ']");
            //htmlNodes_WidthHeight = doc.DocumentNode.SelectNodes("//body[@class='tx-content-container']");
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


    public List<IDictionary<string, string>> Gallery()
    {
        if (htmlNodes_gallery == null) return null!;

        List<IDictionary<string, string>> galleryList = new List<IDictionary<string, string>>();

        foreach (var Noeds in htmlNodes_gallery!)
        {
            var cont_List = Noeds.SelectNodes(".//ul[@class='integrate_cont_list']/li");
            if (cont_List != null)
            {
                foreach (var cont in cont_List)
                {

                    if (cont.SelectSingleNode(".//div[@class]") == null) continue;

                    Dictionary<string, string>? listData = new Dictionary<string, string>();

                    listData!.Add("Title", cont.SelectSingleNode(".//a[@href]").InnerText);
                    listData!.Add("Text", cont.SelectSingleNode(".//div[@class]").InnerText);

                    foreach (HtmlNode gallUrl in cont.SelectNodes(".//a[@href]"))
                    {
                        HtmlAttribute gallUrlText = gallUrl.Attributes["href"];
                        listData!.Add("Url", gallUrlText.Value);
                    }
                    galleryList.Add(listData);
                }
            }
        }

        return galleryList;
    }

    public List<IDictionary<string, string>> Post()
    {
        if(htmlNodes_post == null) return null!;

        List<IDictionary<string, string>> postList = new List<IDictionary<string, string>>();

        foreach (var Noeds in htmlNodes_post!)
        {
            var resultt_List = Noeds.SelectNodes(".//ul[@class='sch_result_list']/li");
            if (resultt_List != null)
            {
                foreach (var result in resultt_List)
                {
                    Dictionary<string, string>? listData = new Dictionary<string, string>();

                    listData!.Add("Title", result.SelectSingleNode(".//a[@href]").InnerText);
                    listData!.Add("GallTitle", result.SelectSingleNode(".//p/a[@class='sub_txt']").InnerText);
                    listData!.Add("Text", result.SelectSingleNode(".//p[@class='link_dsc_txt']").InnerText);
                    listData!.Add("Date", result.SelectSingleNode(".//p[@class='link_dsc_txt dsc_sub']/span[@class='date_time']").InnerText);

                    foreach (HtmlNode resultUrl in result.SelectNodes(".//a[@class='tit_txt']"))
                    {
                        HtmlAttribute resultUrlText = resultUrl.Attributes["href"];
                        listData!.Add("TextUrl", resultUrlText.Value);
                    }

                    postList.Add(listData);
                }
            }
        }

        return postList;
    }
}
