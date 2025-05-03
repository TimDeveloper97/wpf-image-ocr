using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageOCR.Models;
using ImageOCR.Services;

namespace ImageOCR.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        #region private properties
        private string _helloWord;
        private Image _image;
        #endregion

        #region public properties
        public ICommand ImportImageCommand { get; set; }
        public static ObservableCollection<object> List { get; set; }
        public string HelloWord { get => _helloWord; set => SetProperty(ref _helloWord, value); }
        public Image Image { get => _image; set => SetProperty(ref _image, value); }
        #endregion

        [Inject]
        public MainViewModel()
        {
            InitProperties();
            InitCommand();
        }

        /// <summary>
        /// khởi tạo giá trị cho các biến
        /// </summary>
        void InitProperties()
        {
            HelloWord = _helloWorld;
        }

        /// <summary>
        /// khởi tạo giá trị cho các sự kiện button
        /// </summary>
        void InitCommand()
        {
            ImportImageCommand = new RelayCommand<object>(p => { return true; }, p =>
            {
                // Sử dụng OpenFileDialog để chọn file ảnh
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                    Title = "Select an Image File"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Tạo đối tượng Image và gán thông tin file
                    Image = new Image
                    {
                        Name = System.IO.Path.GetFileName(openFileDialog.FileName),
                        Path = openFileDialog.FileName,
                        Capacity = new System.IO.FileInfo(openFileDialog.FileName).Length / 1024.0
                    };
                }
            });

        }
    }
}
