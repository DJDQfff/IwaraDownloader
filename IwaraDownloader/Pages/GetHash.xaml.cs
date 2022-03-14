using System;

using IwaraDownloader.ContentDialogs;
using IwaraDownloader.Helper;
using IwaraDownloader.Models;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using IwaraClient;
using IwaraDatabase.Operation;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace IwaraDownloader.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class GetHash : Page
    {
        private int c;
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

        public GetHash ()
        {
            this.InitializeComponent();
            SetDatePicker();
            button.Click += (sender, e) => ApplicationData.Current.LocalSettings.Values["DownloadingYearMonth"] = datePicker.SelectedDate;
        }

        private async void Button_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            button.IsEnabled = false;
            DateTimeOffset dateTimeOffset = datePicker.Date;
            HashDownloader iwaraClient = new HashDownloader(dateTimeOffset, (IwaraType) comboBox.SelectedIndex);
            HtmlParser.AddedEvent += AddCount;
            // TODO：加一个progressring 开始下载就显示，停止就不显示
            try
            {
                await iwaraClient.GetAllHashes();
                Tools.AddWithoutRepeat(iwaraClient.ThisMonth);
            }
            catch (ArgumentException)
            {
                //HTML网页源码发生了变更，原解析方法无法使用
                await new HtmlChanged().ShowAsync();
            }
            catch (Exception)
            {
                //沙比微软，Windows.web.http类无法使用更好的异常类
                //发生这个一般都是网络请求出了问题
                string message = resourceLoader.GetString("GetHash/ExceptionMessage");
                textBlock.Text += $"\n\n{message}";
            }
            finally
            {
                string finished = resourceLoader.GetString("GetHash/ExceptionMessage");
                textBlock.Text += finished;

                HtmlParser.AddedEvent -= AddCount;
                button.IsEnabled = true;
            }
        }

        private void AddCount ()
        {
            c += 1;
            count.Text = c.ToString();
        }

        /// <summary> 日期选择器初始化，月份不能为当前月 </summary>
        private void SetDatePicker ()
        {
            datePicker.MinYear = new DateTimeOffset(new DateTime(2014, 1, 1));
            datePicker.MaxYear = DateTimeOffset.Now;

            if (ApplicationData.Current.LocalSettings.Values["DownloadingYearMonth"] == null)
            {
                datePicker.SelectedDate = DateTimeOffset.Now.AddMonths(-1);
            }
            else
            {
                datePicker.SelectedDate = ApplicationData.Current.LocalSettings.Values["DownloadingYearMonth"] as DateTimeOffset?;
            }
        }
    }
}