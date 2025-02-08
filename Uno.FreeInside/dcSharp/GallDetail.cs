using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web;

namespace dcSniper.API;

internal class GallDetail
{

    private HtmlNodeCollection? htmlNodes;
    private HtmlNodeCollection? contentsData;
    private HtmlNodeCollection? replynumData;
    private HtmlNodeCollection? mediaFileData;

    //private ImageRendering imageRequest = new ImageRendering();

    public async Task GetGallDetail(string URL)
    {

        var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; U; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.5832.202 Safari/537.36");

        try
        {
            var response = await httpClient.GetAsync(URL);
            response.EnsureSuccessStatusCode();
            var gallhtml = await response.Content.ReadAsStringAsync();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpUtility.HtmlDecode(gallhtml));

            htmlNodes = doc.DocumentNode.SelectNodes("//div[@class='writing_view_box']/div[@class='write_div']");
            contentsData = doc.DocumentNode.SelectNodes("//div[@class='gallview_head clear ub-content']");
            replynumData = doc.DocumentNode.SelectNodes("//div[@class='btn_recommend_box recomuse_y morebox']/div[@class='inner_box'] " +
                "| //div[@class='btn_recommend_box recomuse_n morebox']/div[@class='inner_box']");
            mediaFileData = doc.DocumentNode.SelectNodes("//div[@class='appending_file_box']/ul[@class='appending_file']/li/*");

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

    public async Task<List<IDictionary<string, string>>> DetailData()
    {
        if (htmlNodes == null) return null!;

        string? all_html = null;
        List<IDictionary<string, string>> dataList = new List<IDictionary<string, string>>();

        foreach (var Nodes in htmlNodes!)
        {
            all_html = Nodes.OuterHtml;
        }

        //int fileNumber = 0;
        string modifiedHtml = Regex.Replace(all_html!, @"<img\b([^>]*)>(.*?)", "&#&<img$1>$2&#&"); //이미지
        modifiedHtml = Regex.Replace(modifiedHtml, @"<video\b([^>]*)>(.*?)</video>", "&#&<video$1>$2</video>&#&"); //이미지 또는 미디어
        modifiedHtml = Regex.Replace(modifiedHtml, @"<iframe\b([^>]*)>(.*?)</iframe>", "&#&<iframe$1>$2</iframe>&#&"); //박스
        modifiedHtml = Regex.Replace(modifiedHtml, @"<embed\b(.*?)>", "&#&<embed$1></embed>&#&"); //유튜브 임베드

        modifiedHtml = modifiedHtml.Replace("style=\"overflow:hidden;width:900px;\"", ""); //정리

        // string modifiedHtml = Regex.Replace(html, @"<(.*?)\b([^>]*)>(.*?)</(.*?)>", "<div$1>$2</div>,");
        List<string> stringList = new List<string>(modifiedHtml.Split("&#&"));

        foreach (var item in stringList)
        {
            IDictionary<string, string> gallData = new Dictionary<string, string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(item);
            var imgHTML = doc.DocumentNode.SelectSingleNode("//img");
            var gifHTML = doc.DocumentNode.SelectSingleNode("//p/video | //p/div/video | //video[@playsinline='']");
            var embedHTML = doc.DocumentNode.SelectSingleNode("//embed");
            var iframeVideo = doc.DocumentNode.SelectSingleNode("//iframe[@style]");
            var iframeAudio = doc.DocumentNode.SelectSingleNode("//iframe[@width='280px' and  @height='54px']");
            var linkBox = doc.DocumentNode.SelectSingleNode("//div/div[@class='og-div']/a[@href]");

            //randomString = $"{randomString}-{fileNumber++}";

            if (imgHTML != null)
            {
                HtmlAttribute attImg = imgHTML.Attributes["src"];
                gallData.Add("Image", attImg.Value);
            }
            else if (gifHTML != null)
            {
                HtmlAttribute attgif = gifHTML.Attributes["data-src"];
                gallData.Add("GIF", attgif.Value);
            }
            else if (embedHTML != null)
            {
                HtmlAttribute attembed = embedHTML.Attributes["src"];
                gallData.Add("Embed", attembed.Value);
            }
            else if (iframeVideo != null)
            {
                HtmlAttribute attvid = iframeVideo.Attributes["src"];
                VideoHTML videoHTML = new VideoHTML(); ;
                await videoHTML.HTML(attvid.Value);
                gallData.Add("Video", videoHTML.VideoURL());
                gallData.Add("Video-Width", videoHTML.Width().ToString());
                gallData.Add("Video-Height", videoHTML.Height().ToString());
            }
            else if (iframeAudio != null)
            {
                HtmlAttribute attaud = iframeAudio.Attributes["src"];
                AudioHTML audioHTML = new AudioHTML();
                await audioHTML.HTML(attaud.Value);
                gallData.Add("Audio", audioHTML.AudioURL());
            }
            else if (linkBox != null)
            {
                HtmlAttribute attLink = linkBox.Attributes["href"];
                //AudioHTML LinkBoxHref = new AudioHTML();
                gallData.Add("LinkBox", attLink.Value);
            }
            else
            {
                gallData.Add("Html", item);
            }
            dataList.Add(gallData);
        }

        return dataList;
    }


    public IDictionary<string, string> GallUserData()
    {
        if (contentsData == null) { return null!; }

        IDictionary<string, string> UserDataText = new Dictionary<string, string>();

        foreach (var Nodes in contentsData!)
        {

            string titleHeadtext = Nodes.SelectSingleNode(".//span[@class='title_headtext']").InnerText; //머리말
            titleHeadtext = titleHeadtext.Remove(0, 1);
            titleHeadtext = titleHeadtext.Remove(titleHeadtext.Length - 1, 1);

            var titleSubject = Nodes.SelectSingleNode(".//span[@class='title_subject']"); //제목

            UserDataText.Add("TitleHeadt", titleHeadtext);
            UserDataText.Add("Title", titleSubject.InnerText);

            var nickname_in = Nodes.SelectSingleNode(".//span[@class='nickname in']"); //고닉,반고닉
            var nickname = Nodes.SelectSingleNode(".//span[@class='nickname']"); //통피
            var nickname_ip = Nodes.SelectSingleNode(".//span[@class='ip']"); //ip

            if (nickname_in != null)
            {
                UserDataText.Add("User", nickname_in.InnerText);
                if (nickname_in.SelectSingleNode("//div[@class='fl']/a[@class='writer_nikcon ']/img[@src='https://nstatic.dcinside.com/dc/w/images/fix_nik.gif'] | " +
                    "//div[@class='fl']/a[@class='writer_nikcon ']/img[@src='https://nstatic.dcinside.com/dc/w/images/fix_managernik.gif'] | " +
                    "//div[@class='fl']/a[@class='writer_nikcon ']/img[@src='https://nstatic.dcinside.com/dc/w/images/fix_sub_managernik.gif']") != null)
                {
                    UserDataText.Add("Member", "True"); //고닉
                }
                else if (nickname_in.SelectSingleNode("//div[@class='fl']/a[@class='writer_nikcon ']/img[@src='https://nstatic.dcinside.com/dc/w/images/nik.gif'] | " +
                    "//div[@class='fl']/a[@class='writer_nikcon ']/img[@src='https://nstatic.dcinside.com/dc/w/images/managernik.gif']" +
                    "//div[@class='fl']/a[@class='writer_nikcon ']/img[@src='https://nstatic.dcinside.com/dc/w/images/sub_managernik.gif']") != null)
                {
                    UserDataText.Add("Member", "False"); //반고닉
                }

            }
            else if (nickname != null && nickname_ip != null)
            {
                UserDataText.Add("User", nickname.InnerText + nickname_ip.InnerText);
            }
            else if (nickname_ip == null)
            {
                UserDataText.Add("User", nickname!.InnerText);
            }
            else
            {
                UserDataText.Add("User", "-");
            }

            var dateText = Nodes.SelectSingleNode(".//span[@class='gall_date']");

            if (dateText != null)
            {
                HtmlAttribute attdate = dateText.Attributes["title"];
                UserDataText.Add("Date", attdate.Value);
            }
            else
            {
                UserDataText.Add("Date", "-");
            }

            var gall_count = Nodes.SelectSingleNode(".//span[@class='gall_count']");

            if (gall_count != null)
            {
                UserDataText.Add("Count", gall_count.InnerText);
            }

            var gall_reply_num = Nodes.SelectSingleNode(".//span[@class='gall_reply_num']");

            if (gall_reply_num != null)
            {
                UserDataText.Add("Replynum", gall_reply_num.InnerText);
            }

            var gall_comment = Nodes.SelectSingleNode(".//span[@class='gall_comment']");

            if (gall_comment != null)
            {
                UserDataText.Add("Comment", gall_comment.InnerText);
            }
        }

        return UserDataText;
    }

    public IDictionary<string, string> RecommendBox()
    {
        if (replynumData == null) { return null!; }

        IDictionary<string, string> RecommendText = new Dictionary<string, string>();

        foreach (var Nodes in replynumData)
        {
            var up_num = Nodes.SelectSingleNode(".//div[@class='inner']/div[@class='up_num_box']/p[@class='up_num font_red']");
            var down_num = Nodes.SelectSingleNode(".//div[@class='inner']/div[@class='down_num_box']/p[@class='down_num']");

            if (up_num != null)
            {
                RecommendText.Add("Up", up_num.InnerText);
            }
            else
            {
                RecommendText.Add("Up", null!);
            }

            if (down_num != null)
            {
                RecommendText.Add("Down", down_num.InnerText);
            }
            else
            {
                RecommendText.Add("Down", null!);
            }
        }

        return RecommendText;
    }

    public List<IDictionary<string, string>> MediaFile()
    {
        if (mediaFileData == null) return null!; 
        var fileData = new List<IDictionary<string, string>>();
        foreach (var itemMedia in mediaFileData)
        {
            var fileMetadata = new Dictionary<string, string>
            {
                { itemMedia.InnerText, itemMedia.Attributes["href"].Value }
            };
            fileData.Add(fileMetadata);
        }
        return fileData;
    }

    public string GetQueryStringValue(string urls, string key)
    {
        var queryParams = HttpUtility.ParseQueryString(urls);
        return queryParams[key]!;
    }

}
