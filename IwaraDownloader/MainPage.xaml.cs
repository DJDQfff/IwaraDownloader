using System;

using IwaraDownloader.Pages;

using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static Windows.UI.Colors;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace IwaraDownloader
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public MainPage ()
        {
            this.InitializeComponent();
            Current = this;
        }

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.SetTitleBar(TitleBar);
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.BackgroundColor = Transparent;
            titlebar.ButtonBackgroundColor = Transparent;
            titlebar.ButtonPressedBackgroundColor = Transparent;
            MainFrame.Navigate(typeof(StartPing));
        }

        private void Navigationview_ItemInvoked (NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Type type = default;
            if (args.IsSettingsInvoked)
                type = typeof(Setting);

            switch (args.InvokedItemContainer.Name)
            {
                case "GetHash":
                    type = typeof(GetHash);
                    break;

                case "TransfromHashToDownloadUrl":
                    type = typeof(TransfromHashToDownloadUrl);
                    break;

                case "DownloadedVideo":
                    type = typeof(DownloadedVideo);
                    break;

                case "TranslateNameFromHashToTitle":
                    type = typeof(TranslateNameFromHashToTitle);
                    break;
            }

            if (MainFrame.CurrentSourcePageType != type)
            {
                MainFrame.Navigate(type);
            }
        }

        private void Navigationtoggle_Click (object sender, RoutedEventArgs e)
        {
            navigationview.IsPaneOpen = !navigationview.IsPaneOpen;
        }
    }
}