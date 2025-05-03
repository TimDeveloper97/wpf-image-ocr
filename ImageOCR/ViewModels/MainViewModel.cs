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
using System.Diagnostics;

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
        public ICommand OCRCommand { get; set; }

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
            //// handle import image and init image
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

            //// handle OCR image
            OCRCommand = new RelayCommand<object>(p => Image != null && !string.IsNullOrEmpty(Image.Path), p =>
            {
                try
                {
                    /// train data: https://github.com/tesseract-ocr/tessdata
                    /// download and import to _tessdata folder
                    using (var img = Tesseract.Pix.LoadFromFile(Image.Path))
                    {
                        // Tesseract engine ENG
                        using (var engine = new Tesseract.TesseractEngine(@"./_tessdata", "eng", Tesseract.EngineMode.Default))
                        {
                            // OCR
                            using (var page = engine.Process(img))
                            {
                                // get text from image
                                Image.TextEng = page.GetText();
                            }
                        }

                        // Tesseract engine Vi
                        using (var engine = new Tesseract.TesseractEngine(@"./_tessdata", "vie", Tesseract.EngineMode.Default))
                        {
                            // OCR
                            using (var page = engine.Process(img))
                            {
                                // get text from image
                                Image.TextVi = page.GetText();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during OCR: {ex.Message}");
                }
            });

        }
    }
}
