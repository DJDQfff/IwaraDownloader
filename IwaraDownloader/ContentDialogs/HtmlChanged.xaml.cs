using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace IwaraDownloader.ContentDialogs
{
    public sealed partial class HtmlChanged : ContentDialog
    {
        public HtmlChanged ()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick (ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick (ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}