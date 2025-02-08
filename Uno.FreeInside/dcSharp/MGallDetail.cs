using HtmlAgilityPack;
using System.Web;
using Uno.FreeInside.Data;

namespace dcSniper.API;

internal class MGallDetail
{
    private HtmlNode? htmlData;
    private HtmlNode? contentData;
    private HtmlNode? replynumData;

    public async Task GetGallDetail(string URL)
    {

        var httpClient = new HttpClient();
        string UA = RandomUA.UserAgent();

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UA);
        httpClient.DefaultRequestHeaders.Referrer = new Uri(URL);

        try
        {
            var response = await httpClient.GetAsync(URL);
            response.EnsureSuccessStatusCode();
            var gallhtml = await response.Content.ReadAsStringAsync();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpUtility.HtmlDecode(gallhtml));

            htmlData = doc.DocumentNode.SelectSingleNode("//div[@class='thum-txt']/div[@class='thum-txtin']");
            contentData = doc.DocumentNode.SelectSingleNode("//div[@class]/section[@class='grid ']");
            replynumData = doc.DocumentNode.SelectSingleNode("//div[@class]/div[@class='reco-area four']/div[@class='reco-circle']/ul[@class='reco-cicle-lst']| " +
                "//div[@class]/div[@class='reco-area four']/div[@class='reco-circle one-reco']/ul[@class='reco-cicle-lst']");

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

    public string DetailData()
    {
        if (htmlData == null) return null!;
   
        var imgNodes = htmlData!.SelectNodes("//img");

        if (imgNodes != null)
        {
            foreach (var imgNode in imgNodes)
            {
                // data-gif 속성 값 추출
                var dataGif = imgNode.GetAttributeValue("data-gif", string.Empty);

                // data-gif 속성이 존재하고 값이 비어 있지 않을 경우
                if (!string.IsNullOrEmpty(dataGif))
                {
                    // data-gif 속성을 src 속성으로 설정
                    imgNode.SetAttributeValue("src", dataGif);
                }
                else
                {
                    // data-gif 속성이 없거나 값이 비어 있는 경우 data-original 속성 값 추출
                    var dataOriginal = imgNode.GetAttributeValue("data-original", string.Empty);

                    // data-original 속성 값이 존재할 경우 src 속성 값으로 설정
                    if (!string.IsNullOrEmpty(dataOriginal))
                    {
                        imgNode.SetAttributeValue("src", dataOriginal);
                    }
                }

                // class 속성 추가
                imgNode.SetAttributeValue("class", "img-fluid lazy");
            }
        }

        //스포일러제거
        var spoilerNodes = htmlData.SelectSingleNode("//div[@id='spoiler']");
        if (spoilerNodes != null)
        {
            // 부모 노드에서 spoilerNodes 제거
            spoilerNodes.ParentNode.RemoveChild(spoilerNodes);
        }

        //광고제거
        var advNodes = htmlData.SelectSingleNode("//div[contains(@class, 'adv-groupin')] | //div[@class='adv-groupno']");
        if (advNodes != null)
        {
            // 부모 노드에서 advNode 제거
            advNodes.ParentNode.RemoveChild(advNodes);
        }

        return htmlData!.OuterHtml;
    }


    public IDictionary<string, string> GallUserData()
    {
        if (contentData == null) return null!; 

        IDictionary<string, string> UserDataText = new Dictionary<string, string>();

        //string titleHeadtext = contentData.SelectSingleNode(".//span[@class='title_headtext']").InnerText; //머리말
        //titleHeadtext = titleHeadtext.Remove(0, 1);
        //titleHeadtext = titleHeadtext.Remove(titleHeadtext.Length - 1, 1);
        //var titleSubject = contentData!.SelectSingleNode(".//div[@class='gallview-tit-box\r\n\t\t\t\r\n\t\t\t']/span[@class='tit']"); //제목
        var titleSubject = contentData!.SelectSingleNode("//input[@id='subject']").Attributes["value"]; //제목
        UserDataText.Add("TitleHeadt", "-");
        UserDataText.Add("Title", titleSubject.Value.Trim());

        var member = contentData.SelectSingleNode(".//div[@class]/div[@class='btm']/ul[@class]/li/a[@href]/span[@class='sp-nick gonick']"); //고닉,반고닉
        var userName = contentData.SelectSingleNode(".//div[@class]/div[@class='btm']/ul[@class]/li"); //통피
        //var nickname_ip = Nodes.SelectSingleNode(".//span[@class='ip']"); //ip

        if (member != null)
        {
            UserDataText.Add("Member", "True"); //고닉
        }
        else {
            UserDataText.Add("Member", "False"); //반고닉
        }
        
        if (userName != null)
        {
            UserDataText.Add("User", userName.InnerText);
        }
        else
        {
            UserDataText.Add("User", "-");
        }

        var dateText = contentData.SelectSingleNode(".//div[@class]/div[@class='btm']/ul[@class]/li[2]");

        if (dateText != null)
        {
            UserDataText.Add("Date", dateText.InnerText);
        }
        else
        {
            UserDataText.Add("Date", "-");
        }
  
        var gall_count = contentData.SelectSingleNode(".//div[@class='gall-thum-btm ']/div[@class]/ul[@class='ginfo2']/li[1]");

        if (gall_count != null)
        {
            UserDataText.Add("Count", gall_count.InnerText.Replace("조회수", "").Trim());
        }
        else 
        {
            UserDataText.Add("Count", "-");
        }

        var gall_reply_num = contentData.SelectSingleNode(".//div[@class='gall-thum-btm ']/div[@class]/ul[@class='ginfo2']/li[2]");

        if (gall_reply_num != null)
        {
            UserDataText.Add("Replynum", gall_reply_num.InnerText.Replace("추천", "").Trim());
        }
        
        var gall_comment = contentData.SelectSingleNode(".//div[@class='gall-thum-btm ']/div[@class]/ul[@class='ginfo2']/li[3]");

        if (gall_comment != null)
        {
            UserDataText.Add("Comment", gall_comment.InnerText.Replace("댓글", "").Trim());
        }
        
        return UserDataText;
    }

    
    public IDictionary<string, string> RecommendBox()
    {
        if (replynumData == null) return null!;

        IDictionary<string, string> RecommendText = new Dictionary<string, string>();

        var up_num = replynumData.SelectSingleNode(".//li[@class='reco-up']/div[@class]/span[@id='recomm_btn']");
        var down_num = replynumData.SelectSingleNode(".//li[@class='reco-down']/div[@class]");

        if (up_num != null)
        {
            RecommendText.Add("Up", up_num.InnerText.Trim());
        }
        else
        {
            RecommendText.Add("Up", "null"!);
        }
        
        if (down_num != null)
        {
            RecommendText.Add("Down", down_num.InnerText.Trim());
        }
        else
        {
            RecommendText.Add("Down", null!);
        }
        
        return RecommendText;
    }

    
}
