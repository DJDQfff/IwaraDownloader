using System;
using System.Threading.Tasks;

using IwaraDatabase.Operation;
using IwaraDownloader.Helper;
using IwaraClient;
using Windows.Storage;

namespace IwaraDownloader.Models
{
    /// <summary> 改名任务 </summary>
    public class ReNameWork
    {
        private string hash;
        public StorageFile File { private set; get; }
        public string OldName { set; get; }//带扩展名
        public string Newname { set; get; }//带扩展名

        /// <summary>
        /// 类工厂，若本地数据库中含有hash则正常生成，否则返回null
        /// </summary>
        /// <param name="storageFile"> </param>
        /// <returns> </returns>
        public static ReNameWork ChangeNameFactory (StorageFile storageFile)
        {
            try
            {
                ReNameWork changeName = new ReNameWork(storageFile);
                return changeName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //TODO: 改为从源站改名的功能
        /// <summary> 重命名,从本地数据库获取改名信息 </summary>
        /// <returns> </returns>
        public async Task Rename ()
        {
            await File.RenameAsync(Newname, NameCollisionOption.GenerateUniqueName);
        }

        private ReNameWork (StorageFile storageFile)
        {
            File = storageFile;
            OldName = storageFile.Name;
            Newname = SetNewname(storageFile.DisplayName);
        }

        /// <summary> 设置新名称，暂时只能从本地数据库获取title </summary>
        /// <param name="displayname"> 现有文件名（不带扩展名） </param>
        /// <returns> 新文件名（带扩展名，且已去除文件名字符 </returns>
        private string SetNewname (string displayname)
        {
            hash = MMDHelper.GetHashFromOfficialVideoName(displayname);
            string title = hash.GetTitle();
            string newnam = title.RemoveIllegalCharsAsFileName() + ".mp4";
            return newnam;
        }
    }
}