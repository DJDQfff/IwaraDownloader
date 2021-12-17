using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using IwaraDownloader.Databases;
using IwaraDownloader.Databases.Entities;
using IwaraDownloader.Databases.Tools;
using IwaraDownloader.Helper;

using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Web;

using static IwaraDownloader.Helper.MMDHelper;

namespace IwaraDownloader.Models
{
    public class VideoDownloader
    {
        public List<DownloadOperation> activeDownloads { get; } = new List<DownloadOperation>();
        public List<string> hashqueue { get; } = new List<string>();//hash队列，表达“下载中”状态

                                                                    //数据库的状态仅表示是否已下载完成，
        public ObservableCollection<NotifyProgress> progressInfos { get; } = new ObservableCollection<NotifyProgress>();

        private CancellationTokenSource cts;

        public event Action FinishDownloadsEvent;

        private int qualityIndex;

        public async Task StartAsync (int index)
        {
            qualityIndex = index;
            cts = new CancellationTokenSource();
            await DiscoverActiveDownloadsAsync();
            await AddActiveDownloadAsync();
            FinishDownloadsEvent?.Invoke();
        }

        public void Pause ()
        {
            foreach (var d in activeDownloads)
            {
                //TODO:判断状态的话，无法正常启动或暂停，下面那个也是
                //if (d.Progress.Status == BackgroundTransferStatus.Running)
                d.Pause();
            }
        }

        public void Resume ()
        {
            foreach (var d in activeDownloads)
            {
                //if (d.Progress.Status == BackgroundTransferStatus.PausedByApplication)
                d.Resume();
            }
        }

        public void Cancel ()
        {
            cts.Cancel();

            if (cts != null)
            {
                cts.Dispose();
                cts = null;
            }
        }

        /// <summary> 应用启动时，发掘仍在队列中的后台下载任务 </summary>
        /// <returns> </returns>
        private async Task DiscoverActiveDownloadsAsync ()
        {
            IReadOnlyList<DownloadOperation> downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            if (downloads.Count > 0)
            {
                List<Task> tasks = new List<Task>();
                foreach (DownloadOperation download in downloads)
                {
                    // Attach progress and completion handlers.
                    tasks.Add(HandleDownloadAsync(download, false));
                }

                // Don't await HandleDownloadAsync() in the
                // foreach loop since we would attach to the
                // second download only when the first one
                // completed; attach to the third download
                // when the second one completes etc. We
                // want to attach to all downloads
                // immediately. If there are actions that
                // need to be taken once downloads complete,
                // await tasks here, outside the loop.
                await Task.WhenAll(tasks);
            }
        }

        private async Task AddActiveDownloadAsync ()
        {
            if (cts == null || cts.IsCancellationRequested == true)
                return;
            int todownloadcount = MaxDownloads.GetCount() - activeDownloads.Count;
            if (todownloadcount > 0)
            {
                Database database = new Database();

                var query = database.MMDInfos
                    .Where(n => n.WhetherDownloaded == false)
                    .Where(n => hashqueue.Contains(n.Hash) == false)
                    .Where(n => n.EyeOpen > 0)
                    .Where(n => n.Heart > 0);

                List<MMDInfo> newmmds = new List<MMDInfo>();
                List<Task> tasks = new List<Task>();

                newmmds = query.Take(todownloadcount).ToList();//Take()参数是最大值，而不是必须返回这么多，
                                                               //若小于或等于零，返回容量为0的Ienumerable
                                                               //可以直接在foreach里遍历
                foreach (var mmd in newmmds)
                {
                    try
                    {
                        Task task = NewDownloadOperation(mmd);
                        tasks.Add(task);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                await Task.WhenAll(tasks);
            }
            async Task NewDownloadOperation (MMDInfo mmd)
            {
                Uri uri = await mmd.DownloadUri(qualityIndex);
                //Uri uri = mmd.TestFtpDownloadUri();

                StorageFolder storageFolder = await SetDownloadsFolder.GetSaveFolderAsync();
                StorageFile storageFile = await storageFolder.CreateFileAsync(mmd.Hash, CreationCollisionOption.ReplaceExisting);

                // TODO：文件写入作者信息，有BUG
                //var a = await storageFile.Properties.GetVideoPropertiesAsync();
                //a.Publisher = "Iwara";
                //a.Title = mmd.Title;
                //a.Producers.Add(mmd.Username);
                //await a.SavePropertiesAsync();

                BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
                DownloadOperation downloadOperation = backgroundDownloader.CreateDownload(uri, storageFile);
                await HandleDownloadAsync(downloadOperation, true);
            }
        }

        /// <summary>
        /// 处理单个后台传输任务 移入、移出活动下载队列 处理任务取消、错误 任务结束时开始新下载
        /// </summary>
        /// <param name="download"> </param>
        /// <param name="start">    为true则开始一个新下载；false则匹配原有下载 </param>
        /// <returns> </returns>
        private async Task HandleDownloadAsync (DownloadOperation download, bool start)
        {
            string hash = download.ResultFile.Name;
            NotifyProgress progressInfo = new NotifyProgress();
            activeDownloads.Add(download);
            hashqueue.Add(hash);
            progressInfo.Hash = hash;
            progressInfos.Add(progressInfo);

            try
            {
                // Store the download so we can pause/resume.

                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                if (start)
                {
                    // Start the download and attach a
                    // progress handler.
                    await download.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The download was already running when
                    // the application started, re-attach
                    // the progress handler.
                    await download.AttachAsync().AsTask(cts.Token, progressCallback);
                }
                string filename = hash.GetTitle().StorageFileName(SaveNameMode.Title);
                await download.ResultFile.RenameAsync(filename, NameCollisionOption.GenerateUniqueName);
                hash.SetWhetherDownloaded(true);
            }
            catch (TaskCanceledException)
            {
                progressInfo.Description = "已取消";
                await download.ResultFile.DeleteAsync();
            }
            catch (Exception ex)
            {
                progressInfo.Description = "下载错误";
                await download.ResultFile.DeleteAsync();

                if (!IsExceptionHandled("Execution error", ex, download))
                {
                    throw;
                }
            }
            finally
            {
                activeDownloads.Remove(download);
                hashqueue.Remove(hash);
                progressInfos.Remove(progressInfo);
                await AddActiveDownloadAsync();
            }

            void DownloadProgress (DownloadOperation downloadOperation)
            {
                // DownloadOperation.Progress is updated in
                // real-time while the operation is ongoing.
                // Therefore, we must make a local copy so
                // that we can have a consistent view of
                // that ever-changing state throughout this
                // method's lifetime.
                BackgroundDownloadProgress currentProgress = downloadOperation.Progress;

                var all = currentProgress.TotalBytesToReceive;
                double percent = default;
                if (all > 0)
                {
                    var received = currentProgress.BytesReceived;
                    percent = received * 100d / all;
                    progressInfo.Progress = percent;
                }
                string showtest = default;
                switch (currentProgress.Status)
                {
                    case BackgroundTransferStatus.Canceled:
                        showtest = "下载已取消";
                        break;

                    case BackgroundTransferStatus.Completed:
                        showtest = "下载完成";
                        break;

                    case BackgroundTransferStatus.Error:
                        showtest = "下载异常";
                        break;

                    case BackgroundTransferStatus.Running:
                        showtest = percent.ToString();
                        break;

                    case BackgroundTransferStatus.PausedByApplication:
                        showtest = "已暂停";
                        break;

                    case BackgroundTransferStatus.PausedNoNetwork:
                        showtest = "无网络";
                        break;
                }
                progressInfo.Description = showtest;

                if (currentProgress.HasResponseChanged)
                {
                    // We have received new response headers
                    // from the server. Be aware that
                    // GetResponseInformation() returns null
                    // for non-HTTP transfers (e.g., FTP).
                    ResponseInformation response = downloadOperation.GetResponseInformation();

                    // If you want to stream the response
                    // data this is a good time to start. download.GetResultStreamAt(0);
                }
            }
        }

        /// <summary> 接管后台下载任务失败，可能为Iwara被墙了，或者下载链接超过一天失效 </summary>
        /// <param name="title">    </param>
        /// <param name="ex">       </param>
        /// <param name="download"> </param>
        /// <returns> </returns>
        private bool IsExceptionHandled (string title, Exception ex, DownloadOperation download = null)
        {
            WebErrorStatus error = BackgroundTransferError.GetStatus(ex.HResult);
            if (error == WebErrorStatus.Unknown)
            {
                return false;
            }

            if (download == null)
            {
            }
            else
            {
            }

            return true;
        }
    }
}