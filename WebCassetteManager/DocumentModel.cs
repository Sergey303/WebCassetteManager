using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using WebCassetteManager.Annotations;

namespace WebCassetteManager
{
    class DocumentModel : INotifyPropertyChanged
    {
        public string Uri
        {
            get { return uri; }
            set
            {
                if (value == uri) return;
                uri = value;
                OnPropertyChanged(nameof(Uri));
            }
        }

        public string Id { get; set; }

        private string binaryData;
        private string uri;

        public string BinaryData
        {
            get { return binaryData; }
            set
            {
                if (Equals(value, binaryData)) return;
                binaryData = value;
                OnPropertyChanged(nameof(BinaryData));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}