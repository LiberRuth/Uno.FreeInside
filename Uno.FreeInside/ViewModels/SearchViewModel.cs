using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Web;
using dcSharp.API;

namespace Uno.FreeInside.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly IDispatcher _dispatcher;
    private INavigator _navigator;

    private Search search = new Search();
    private List<IDictionary<string, string>> searchData = new List<IDictionary<string, string>>();
    public ObservableCollection<SearchItem> ListItem { get; } = new ObservableCollection<SearchItem>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedItemUrl))]
    private SearchItem? _selectedItem;

    [ObservableProperty]
    private string? _messages;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _isMessages = "Collapsed";

    public SearchViewModel(IDispatcher dispatcher, INavigator navigator, Entity entity) 
    {
        _dispatcher = dispatcher;
        _navigator = navigator;

        InitializeAsync($"https://search.dcinside.com/gallery/q/{UrlConversion(entity?.Value!)}");      
    }

    private async void InitializeAsync(string url)
    {
        await GetSearchPageData(url);
    }

    public string SelectedItemUrl => SelectedItem?.Url!;

    async partial void OnSelectedItemChanged(SearchItem? items)
    {
        if (items != null)
        {
            await _navigator.NavigateViewModelAsync<GallViewModel>(this, data: new Entity(UrlGalleryConversion(items.Url!)));
            SelectedItem = null;
        }
    }

    private async Task GetSearchPageData(string url)
    {
        IsLoading = true;
        try 
        {
            await search.GetSearch(url);
        }
        catch (HttpRequestException e)
        {
            await _dispatcher.ExecuteAsync(() =>
            {
                IsLoading = false;
                IsMessages = "Visible";
                Messages = e.Message;
            });
            //_ = _navigator.ShowMessageDialogAsync(this, title: "Error", content: e.Message);
            //Messages = e.Message;
            //await _navigator.NavigateBackAsync(this);
            return;
        }
        catch (Exception e)
        {
            await _dispatcher.ExecuteAsync(() => 
            {
                IsLoading = false;
                IsMessages = "Visible";
                Messages = e.Message;
            });
            //_ = _navigator.ShowMessageDialogAsync(this, title: "Error", content: e.Message);
            //Messages = e.Message;
            //await _navigator.NavigateBackAsync(this);
            return;
        }

        searchData = search.Gallery();

        await _dispatcher.ExecuteAsync(() =>
        {

            foreach (var item in searchData)
            {
                ListItem.Add(new SearchItem()
                {
                    Title = item["Title"],
                    Text = item["Text"],
                    Url = item["Url"],
                });
            }
            IsLoading = false;
        });
 
    }

    private string UrlConversion(string originalText)
    {
        string encodedText = HttpUtility.UrlEncode(originalText);
        encodedText = encodedText.Replace("%", ".");
        encodedText = encodedText.Replace("+", ".20");
        return encodedText;
    }

    private string UrlParameter(string url, string par) 
    {
        Uri uri = new Uri(url);
        string query = uri.Query;

        var queryParameters = HttpUtility.ParseQueryString(query);
        string idValue = queryParameters[par]!;

        return idValue;
    }

    private string UrlGalleryConversion(string url) 
    {
        if (url.Contains("gall.dcinside.com/mgallery/board/") | url.Contains("gall.dcinside.com/board/"))
        {
            url = $"https://m.dcinside.com/board/{UrlParameter(url, "id")}";
        }
        else if (url.Contains("gall.dcinside.com/person/board/"))
        {
            url = $"https://m.dcinside.com/person/{UrlParameter(url, "id")}";
        }
        else
        {
            url = $"https://m.dcinside.com/mini/{UrlParameter(url, "id")}";
        }

        return url;
    }
}
