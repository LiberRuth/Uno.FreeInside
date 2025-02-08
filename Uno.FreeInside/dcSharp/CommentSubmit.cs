using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Collections.Specialized;


namespace dcSniper.API;

internal class CommentSubmit
{
    private string? UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";
    //private string? CSRF_TOKEN;

    public string? name { get; set; }
    public string? password { get; set; }
    public string? memo { get; set; }

    static string ToQueryString(Dictionary<string, string> dictionary)
    {
        var list = new List<string>();
        foreach (var pair in dictionary)
        {
            list.Add($"{HttpUtility.UrlEncode(pair.Key)}={HttpUtility.UrlEncode(pair.Value)}");
        }

        return string.Join("&", list);
    }

    private async Task<string> ConKey()
    {
        string blockKey = "";
        HttpClient HttpClient = new HttpClient();

        string url = "https://m.dcinside.com/ajax/access";

        var postData = new Dictionary<string, string>
        {
            { "token_verify", "com_submit" }
        };
        HttpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        HttpClient.DefaultRequestHeaders.Add("User-Accept", UA);
        HttpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        HttpClient.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.9");
        HttpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        HttpClient.DefaultRequestHeaders.Referrer = new Uri("https://m.dcinside.com/");
        var content = new FormUrlEncodedContent(postData);
        var response = await HttpClient.PostAsync(url, content);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic jsonObject = JsonConvert.DeserializeObject(responseContent)!;
            blockKey = jsonObject!.Block_key;
        }
        else
        {
            Console.WriteLine("요청이 실패하였습니다. 상태 코드: " + response.StatusCode);
        }

        return blockKey;
    }


    public async Task<IDictionary<string, string>> CommentSubmitDataAsync(string target_url)
    {
        HttpClient httpClient = new HttpClient();
        Dictionary<string, string> Dictionary_Comment = new Dictionary<string, string>();

        httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        httpClient.DefaultRequestHeaders.Add("User-Agent", UA);
        httpClient.DefaultRequestHeaders.Add("X-Requested-With", "ko-KR,ko;q=0.9");
        httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        httpClient.DefaultRequestHeaders.Referrer = new Uri(target_url);

        var response = await httpClient.GetAsync(target_url);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        HtmlNode vcurtNode = doc.DocumentNode.SelectSingleNode("//input[@name='v_cur_t']");
        string vcurt = vcurtNode.Attributes["value"].Value;

        HtmlNode check6Node = doc.DocumentNode.SelectSingleNode("//input[@name='check_6']");
        string check_6 = check6Node.Attributes["value"].Value;

        HtmlNode check7Node = doc.DocumentNode.SelectSingleNode("//input[@name='check_7']");
        string check_7 = check7Node.Attributes["value"].Value;

        HtmlNode check8Node = doc.DocumentNode.SelectSingleNode("//input[@name='check_8']");
        string check_8 = check8Node.Attributes["value"].Value;

        HtmlNode check9Node = doc.DocumentNode.SelectSingleNode("//input[@name='check_9']");
        string check_9 = check6Node.Attributes["value"].Value;

        HtmlNode check10Node = doc.DocumentNode.SelectSingleNode("//input[@name='check_10']");
        string check_10 = check10Node.Attributes["value"].Value;

        HtmlNode recommendNode = doc.DocumentNode.SelectSingleNode("//input[@id='recommend']");
        string recommend = recommendNode.Attributes["value"].Value;

        HtmlNode serviceCodeNode = doc.DocumentNode.SelectSingleNode("//input[@name='service_code']");
        string serviceCode = serviceCodeNode.Attributes["value"].Value;

        HtmlNode galltypeNode = doc.DocumentNode.SelectSingleNode("//input[@name='_GALLTYPE_']");
        string galltype = galltypeNode.Attributes["value"].Value;

        //CSRF_TOKEN = doc.DocumentNode.SelectSingleNode("//meta[@name='csrf-token']").Attributes["content"].Value;

        UriBuilder uriBuilder = new UriBuilder(target_url);
        NameValueCollection queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);
        string? id = queryParams["id"];
        string? no = queryParams["no"];

        /*
        string postData = $"&id={id!}&no={no!}&name={name!}&password={password!}&memo={memo!}&cur_t={vcurt}" +
            $"&check_6={check_6}&check_7={check_7}&check_8={check_8}&check_9={check_9}&check_10={check_10}" +
            $"&recommend={recommend}&user_ip=undefined&t_vch2=&t_vch2_chk=&c_gall_id={id!}&c_gall_no={no!}" +
            $"service_code={serviceCode}&g-recaptcha-response=&_GALLTYPE_={galltype}&headTail=\"\"";
        */
        
        Dictionary_Comment.Add("id", id!); 
        Dictionary_Comment.Add("no", no!); 
        Dictionary_Comment.Add("name", name!); //디코딩
        Dictionary_Comment.Add("password", password!); //디코딩
        Dictionary_Comment.Add("memo", memo!);
        Dictionary_Comment.Add("cur_t", vcurt);
        Dictionary_Comment.Add("check_6", check_6);
        Dictionary_Comment.Add("check_7", check_7);
        Dictionary_Comment.Add("check_8", check_8);
        Dictionary_Comment.Add("check_9", check_9);
        Dictionary_Comment.Add("check_10", check_10); //디코딩
        Dictionary_Comment.Add("recommend", recommend);
        Dictionary_Comment.Add("user_ip", "undefined");
        Dictionary_Comment.Add("t_vch2", "");
        Dictionary_Comment.Add("t_vch2_chk", "");
        Dictionary_Comment.Add("c_gall_id", id!);
        Dictionary_Comment.Add("c_gall_no", no!);
        Dictionary_Comment.Add("service_code", serviceCode);
        Dictionary_Comment.Add("g-recaptcha-response", "");
        Dictionary_Comment.Add("_GALLTYPE_", galltype);
        Dictionary_Comment.Add("headTail", "");
        

        return Dictionary_Comment;
    }

    private static string Quote(string decoded)
    {
        List<string> arr = new List<string>();
        foreach (char c in decoded)
        {
            string t = ((int)c).ToString("X");
            if (t.Length >= 4)
            {
                arr.Add("%u" + t);
            }
            else
            {
                arr.Add("%" + t);
            }
        }
        return string.Join("", arr);
    }

    public string CookiesStr(string URL)
    {

        string pattern = @"\/board\/(?<boardName>[^\/]+)\/(?<postId>[^\/]+)";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(URL);
        string board_id = match.Groups["boardName"].Value;
        string board_name = match.Groups["postId"].Value;

        Dictionary<string, string> cookies = new Dictionary<string, string>
        {
            { "m_dcinside_" + board_id, board_id },
            { "m_dcinside_lately", Quote(board_id + "|" + board_name + ",") },
            { "_ga", "GA1.2.693521455.1588839880" }
        };

        string Cookies = "";
        foreach (var cookie in cookies)
        {

            Cookies += $"{cookie.Key}: {cookie.Value}, ";
        }

        return Cookies;

    }

    public async Task SubmitAsync(string URL)
    {

        HttpClient client = new HttpClient();

        string submit_url = "https://gall.dcinside.com/board/forms/comment_submit";
        var postDataTxtJSON = await CommentSubmitDataAsync(URL);
        /*
        string postDataTxt = ToQueryString(postDataTxtJSON);
        if (postDataTxt.EndsWith("&"))
        {
            // 문자열 끝의 '&' 제거
            postDataTxt = postDataTxt.Remove(postDataTxt.Length - 1);
        }
        */
        //Mozilla/5.0 (Linux; Android 11; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.91 Mobile Safari/537.36
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("User-Accept", UA);
        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        client.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.9");
        client.DefaultRequestHeaders.Add("Host", "gall.dcinside.com");
        client.DefaultRequestHeaders.Add("Origin", "https://gall.dcinside.com");
        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

        client.DefaultRequestHeaders.Referrer = new Uri(URL);

        Console.WriteLine(postDataTxtJSON);

        //postDataTxt = "&id=dcbest&no=224971&name=ㅇㅇ&password=1234q&memo=%E3%85%8B%E3%85%8B&cur_t=1713614920&check_6=a65f04ac373e782a8c735c6dc65dfdc1cee3b1a203b4fa624219f44e209962dbb34e30e791c2fbeccd745078e09d585a29431feedff8ea&check_7=7d&check_8=ae5837a50035&check_9=7ce58368b4826fe83cee82fb4286&check_10=7fed8272a8826ceb3fe696e7448b756cf1f886&recommend=K&user_ip=undefined&t_vch2=&t_vch2_chk=&c_gall_id=dcbest&c_gall_no=224971&service_code=21ac6d96ad152e8f15a05b7350a2475909d19bcedeba9d4face8115e9bc0fe4801084c58f38338fdbcc7037dfe83c59ab3405c2c4421e01728ab12510e39bfd70f5e2ee8506b370f82360facb3a5d3915749f0ca198f7e8f3d7c9f886badff41394ceb663809a80a33b4207540dd71078e0ff775c6f7ff20b9894694994197bd3e759bf6d0862ce9b12b532f1663f662aa03e8073b4b8a3c870c4ddc98d5ad51fc9910bdd9a1e9e7dabfdf2d04ebc86fe090b53b759ebfd30beffd728c438e31e04ff69dcf19a0&g-recaptcha-response=&_GALLTYPE_=G&headTail=\"\"";

        //StringContent content = new StringContent(postDataTxtJSON, Encoding.UTF8);

        string postDataString = string.Join("&", postDataTxtJSON.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        HttpContent content = new StringContent(postDataString, Encoding.UTF8, "application/x-www-form-urlencoded");
        HttpResponseMessage response = await client.PostAsync(submit_url, content);

        // 응답 결과 확인
        string result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
        //HttpContent content = new StringContent(postDataTxt, Encoding.UTF8);
        //var response = await client.PostAsync(submit_url, new StringContent(postDataTxt, Encoding.UTF8));

        /*
        var response = await client.PostAsync(submit_url, new FormUrlEncodedContent(postDataTxtJSON));
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("응답 내용:");
            Console.WriteLine(responseContent);
        }
        else
        {
            Console.WriteLine("HTTP Error: " + response.StatusCode);
        }
        */

    }
}

