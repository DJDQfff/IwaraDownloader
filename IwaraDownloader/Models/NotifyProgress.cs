using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IwaraDownloader.Models
{
    /// <summary> 下载进度，用于在进度条上显示进度和状态信息 </summary>
    public class NotifyProgress : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _hash;

        public string Hash
        {
            get => _hash;

            set
            {
                _hash = value;
                NotifyPropertyChanged();
            }
        }

        private double _progress;

        public double Progress
        {
            get => _progress;

            set
            {
                _progress = value;
                NotifyPropertyChanged();
            }
        }

        private string _description;

        public string Description
        {
            get => _description;

            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }

        public NotifyProgress ()
        {
            _progress = default;
            _description = default;
            _hash = default;
        }

        private void NotifyPropertyChanged ([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}