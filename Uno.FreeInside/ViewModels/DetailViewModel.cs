using dcSniper.API;
using Windows.System;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;

namespace Uno.FreeInside.ViewModels;

public partial class DetailViewModel : ObservableObject
{
    private string? upBox;
    private string? downBox;

    private readonly IDispatcher _dispatcher;
    private INavigator _navigator;

    private string? _url;
    private MGallDetail? gallDetail = new MGallDetail();

    [ObservableProperty]
    private WebViewSource? _htmlContent;

    [ObservableProperty]
    private string? _titleDetail;

    [ObservableProperty]
    private string? _messages;

    [ObservableProperty]
    private string? _isMessages = "Collapsed";

    [ObservableProperty]
    private string? _detailTitle;

    [ObservableProperty]
    private string? _headerTitle;

    [ObservableProperty]
    private string? _headerUser;

    [ObservableProperty]
    private string? _headerCount;

    [ObservableProperty]
    private string? _headerDate;

    [ObservableProperty]
    private string? _headerReplynum;

    [ObservableProperty]
    private string? _upText;

    [ObservableProperty]
    private string? _downText;

    public DetailViewModel(IDispatcher dispatcher, INavigator navigator, Entity entity)
    {
        _dispatcher = dispatcher;
        _navigator = navigator;

        _url = entity.Value;

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await GetDetailPageData(_url!);
    }

    private async Task GetDetailPageData(string url)
    {
        try
        {
            await gallDetail!.GetGallDetail(url);
        }
        catch (HttpRequestException e)
        {
            await _dispatcher.ExecuteAsync(() =>
            {
                IsMessages = "Visible";
                Messages = e.Message;
            }); ;
            return;
        }
        catch (Exception e)
        {
            await _dispatcher.ExecuteAsync(() =>
            {
                IsMessages = "Visible";
                Messages = e.Message;
            });
            return;
        }

        if (gallDetail.DetailData() == null) return;


        await _dispatcher.ExecuteAsync(() =>
        {
            var metaData = gallDetail.GallUserData();
            var recommendBox = gallDetail.RecommendBox();

            TitleDetail = metaData["Title"];
            DetailTitle = metaData["Title"];

            HeaderTitle = metaData["Title"];
            HeaderUser = metaData["User"];
            HeaderCount = $"조회 {metaData["Count"]}";
            HeaderDate = metaData["Date"];

            //UpText = $"추천 {recommendBox["Up"]}";
            //DownText = $"비추천 {recommendBox["Down"]}";

            if (recommendBox["Up"] != null) upBox = $"<span>추천 {recommendBox["Up"]}</span>";

            if (recommendBox["Down"] != null) downBox = $"<div class=\"vr me-2 ms-2\"></div>\r\n<span>비추천 {recommendBox["Down"]}</span>";

            string content = @$"
                    <!doctype html>
                    <html lang=""ko"">
                      <head>
                        <meta charset=""utf-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                        <title>WebView</title>
                        <link href=""https://gcore.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH"" crossorigin=""anonymous"">
                      </head>
                      <body>
                        <div class=""container"">
                            {gallDetail.DetailData()}
                                <div class=""container text-center mt-3"">
                            	    <div class=""d-inline-flex align-items-center border rounded p-2"">
                                        {upBox} 
                                        {downBox}
	                                </div>
                                </div>
                        </div>
                        <script src=""https://gcore.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"" integrity=""sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz"" crossorigin=""anonymous""></script>
                      </body>
                    </html>";

            HtmlContent = new HtmlWebViewSource
            {
                Html = content
            };
        });

    }

    [RelayCommand]
    public void CopyToClipboard()
    {
        var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
        Uri uri = new Uri(_url!);
        UriBuilder uriBuilder = new UriBuilder(uri)
        {
            Query = string.Empty,
            Port = -1
        };
        string urlWithoutParams = uriBuilder.ToString();
        dataPackage.SetText(urlWithoutParams!);
        Clipboard.SetContent(dataPackage);
    }

    [RelayCommand]
    public void DownloadFiles()
    {
        //DownloadFiles
    }

    [RelayCommand]
    public async Task OpenBrowser()
    {
        Uri uri = new Uri(_url!);
        UriBuilder uriBuilder = new UriBuilder(uri)
        {
            Query = string.Empty,
            Port = -1
        };
        string urlWithoutParams = uriBuilder.ToString();
        await Launcher.LaunchUriAsync(new Uri(urlWithoutParams!));
    }
}
