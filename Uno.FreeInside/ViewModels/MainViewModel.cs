namespace Uno.FreeInside.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private INavigator _navigator;

    [ObservableProperty]
    private string? _value;

    public MainViewModel(INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
    }
    public string? Title { get; }

    [RelayCommand]
    private async Task GoToSearch()
    {
        if (Value == null | Value == "") 
        {
            _ = _navigator.ShowMessageDialogAsync(this, title: "정보", content: "검색어를 입력해주세요.");
            return;
        } 

        await _navigator.NavigateViewModelAsync<SearchViewModel>(this, data: new Entity(Value!));
    }

}
