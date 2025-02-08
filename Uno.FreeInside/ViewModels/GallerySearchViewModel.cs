using dcSniper.API;
using System.Collections.ObjectModel;

namespace Uno.FreeInside.ViewModels;

public partial class GallerySearchViewModel : ObservableObject
{
    private readonly IDispatcher _dispatcher;
    private INavigator _navigator;

    private string _url;
    private string recommend;
    private string spos;
    private int page = 1;
    private MGallList gallList = new MGallList();
    private List<IDictionary<string, string>> galleryData = new List<IDictionary<string, string>>();
    public ObservableCollection<GalleryItem> ListItem { get; } = new ObservableCollection<GalleryItem>();

    public List<CombosBoxItem> Options { get; } = new List<CombosBoxItem>
    {
        new CombosBoxItem { Text = "제목 + 내용", Data = "subject_m" },
        new CombosBoxItem { Text = "제목", Data = "subject" },
        new CombosBoxItem { Text = "내용", Data = "memo" },
        new CombosBoxItem { Text = "유저명", Data = "name" }
    };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedItemUrl))]
    private GalleryItem? _selectedItem;

    [ObservableProperty]
    private CombosBoxItem _selectedOption;

    [ObservableProperty]
    private string? _messages;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _textBoxValue;

    [ObservableProperty]
    private bool? _isChecked = false;

    [ObservableProperty]
    private string? _isMessages = "Collapsed";

    [ObservableProperty]
    private string _isAdd = "Collapsed";

    [ObservableProperty]
    private string? _isLoadingVisibility = "Collapsed";

    public GallerySearchViewModel(IDispatcher dispatcher, INavigator navigator, Entity entity)
    {
        _dispatcher = dispatcher;
        _navigator = navigator;

        _url = entity.Value;
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

    [RelayCommand]
    private async Task GallListSearch()
    {
        if (SelectedOption != null)
        {
            IsAdd = "Collapsed";
            IsLoadingVisibility = "Collapsed";
            ListItem.Clear();
            page = 1;

            recommend = (bool)IsChecked! ? "1" : null!;

            await GetGalleryPageData($"{_url}?s_type={SelectedOption.Data}&serval={TextBoxValue}&recommend={recommend}&headid=");

            IsAdd = "Visible";
        }
    }

    [RelayCommand]
    private async Task ListAdd() 
    {
        if (!gallList.NextPage())
        {
            page = 0;
            spos = gallList.NextNewPage();

            if (gallList.NextNewPage() is null)
            {
                //page = 1;
                IsAdd = "Collapsed";
                return;
            }
        }

        if (SelectedOption != null)
        {
            await GetGalleryPageData($"{_url}?s_type={SelectedOption.Data}&serval={TextBoxValue}&recommend={recommend}&page={++page}&s_pos={spos}");
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
                --page;
            });
            _ = _navigator.ShowMessageDialogAsync(this, title: "Error", content: e.Message);
            return;
        }
        catch (Exception e)
        {
            await _dispatcher.ExecuteAsync(() =>
            {
                IsLoading = false;
                --page;
            });
            _ = _navigator.ShowMessageDialogAsync(this, title: "Error", content: e.Message);
            return;
        }

        var gallData = gallList.Gall_List();

        if (gallData == null)
        {
            await _dispatcher.ExecuteAsync(() =>
            {
                IsLoading = false;
            });

            return;
        }

        await _dispatcher.ExecuteAsync(() =>
        {

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
        });
    }
}
