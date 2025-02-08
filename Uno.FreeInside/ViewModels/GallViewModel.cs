using System.Collections.ObjectModel;
using dcSniper.API;

namespace Uno.FreeInside.ViewModels;

public partial class GallViewModel : ObservableObject
{
    private readonly IDispatcher _dispatcher;
    private INavigator _navigator;

    private int lockNumber = 2;
    private int page = 1;
    private bool nextPage;
    private string _url;
    private string _recommend;
    private MGallList gallList = new MGallList();
    private List<IDictionary<string, string>> galleryData = new List<IDictionary<string, string>>();
    public ObservableCollection<GalleryItem> ListItem { get; } = new ObservableCollection<GalleryItem>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedItemUrl))]
    private GalleryItem? _selectedItem;

    [ObservableProperty]
    private string? _titleGallery;

    [ObservableProperty]
    private string? _messages;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _isMessages = "Collapsed";

    [ObservableProperty]
    private string? _isLoadingVisibility;

    public GallViewModel(IDispatcher dispatcher, INavigator navigator, Entity entity)
    {
        _dispatcher = dispatcher;
        _navigator = navigator;

        _url = entity.Value!;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await GetGalleryPageData(_url);
    }

    public string SelectedItemUrl => SelectedItem?.Url!;

    async partial void OnSelectedItemChanged(GalleryItem? items)
    {
        if (items != null)
        {
            await _navigator.NavigateViewModelAsync<DetailViewModel>(this, data: new Entity(items.Url!));
            SelectedItem = null;
        }
    }

    private async Task GetGalleryPageData(string url)
    {
        IsLoading = true;
        IsLoadingVisibility = "Visible";
        try
        {
            await gallList.GetGallList(url);
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
            //await _navigator.NavigateBackAsync(this);
            return;
        }

        var  gallData = gallList.Gall_List();

        if (gallData == null) 
        {
            await _dispatcher.ExecuteAsync(() => 
            {
                TitleGallery = gallList.Gall_Title();
                IsLoading = false;  
            });

            return;
        }
      
        await _dispatcher.ExecuteAsync(() =>
        {
            TitleGallery = gallList.Gall_Title();

            foreach (var item in gallData)
            {
                ListItem.Add(new GalleryItem()
                {
                    Title = item["Title"],
                    User = item["User"],
                    Subject = item["Subject"],
                    Time = item["Date"],
                    Recommend = item["Recommend"],
                    Url = item["GallURL"],
                    Reply = item["Reply"],
                    Views = item["Count"],
                });
            }

            IsLoading = false;
            IsLoadingVisibility = "Collapsed";
            lockNumber--;
            nextPage = gallList.NextPage();
        });
    }

    public async Task OnNearEnd()
    {
        if (!nextPage) return;

        if (lockNumber == 1)
        {
            lockNumber++;
            await GetGalleryPageData($"{_url}?page={++page}{_recommend}");
            //Debug.WriteLine("GET List");
        }
    }

    public async Task OnReachedEnd()
    {

        /*if (!nextPage) return;

        if (lockNumber == 1)
        {
            lockNumber++;
            await GetGalleryPageData($"{_url}?page={++page}{_recommend}");
        }*/
    }

    [RelayCommand]
    public async Task HomeList() 
    {
        ListItem.Clear();
        page = 1;
        _recommend = null!;
        lockNumber = 2;
        await GetGalleryPageData($"{_url}?page={page}{_recommend}");
    }

    [RelayCommand]
    public async Task StarList()
    {
        ListItem.Clear();
        page = 1;
        _recommend = "&recommend=1";
        lockNumber = 2;
        await GetGalleryPageData($"{_url}?page={page}{_recommend}");
    }

    [RelayCommand]
    public async Task SearchList()
    {
        await _navigator.NavigateViewModelAsync<GallerySearchViewModel>(this, data: new Entity(_url!));
    }
}
