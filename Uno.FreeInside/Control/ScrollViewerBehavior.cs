namespace Uno.FreeInside.Control;

public static class ScrollViewerHelper
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.RegisterAttached("ViewModel", typeof(GallViewModel), typeof(ScrollViewerHelper), new PropertyMetadata(null));

    public static GallViewModel GetViewModel(DependencyObject obj)
    {
        return (GallViewModel)obj.GetValue(ViewModelProperty);
    }

    public static void SetViewModel(DependencyObject obj, GallViewModel value)
    {
        obj.SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty IsAttachedProperty =
        DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(ScrollViewerHelper), new PropertyMetadata(false, OnIsAttachedChanged));

    public static bool GetIsAttached(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsAttachedProperty);
    }

    public static void SetIsAttached(DependencyObject obj, bool value)
    {
        obj.SetValue(IsAttachedProperty, value);
    }

    private static void OnIsAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ScrollViewer scrollViewer)
        {
            if ((bool)e.NewValue)
            {
                scrollViewer.ViewChanged += ScrollViewer_ViewChanged!;
            }
            else
            {
                scrollViewer.ViewChanged -= ScrollViewer_ViewChanged!;
            }
        }
    }

    private static async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            var viewModel = GetViewModel(scrollViewer);
            if (viewModel == null) return;

            double verticalOffset = scrollViewer.VerticalOffset;
            double scrollableHeight = scrollViewer.ScrollableHeight;

            if (Math.Abs(scrollableHeight - verticalOffset) < 1)
            {
                // 완전히 끝에 도달
                await viewModel.OnReachedEnd();
            }
            else if (scrollableHeight - verticalOffset < 150) // 끝에서 n픽셀 이내를 "근처"로 정의
            {
                // 끝 근처에 도달
                await viewModel.OnNearEnd();
            }
            else
            {
                // 그 외의 경우
                //await viewModel.OnNormalScroll();
            }
        }
    }
}
