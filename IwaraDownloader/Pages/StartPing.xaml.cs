using System;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace IwaraDownloader.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class StartPing : Page
    {
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
        public StartPing ()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri("https://ecchi.iwara.tv");
            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = await httpClient.GetAsync(uri);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ShowText.Text = resourceLoader.GetString("Ping/OK");
                }
                else
                {
                    ShowText.Text = resourceLoader.GetString("Ping/Maybe");
                }
            }
            catch (Exception)
            {
                ShowText.Text = resourceLoader.GetString("Ping/Error");
            }
        }
    }
}