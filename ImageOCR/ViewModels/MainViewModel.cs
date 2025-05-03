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
using ControlzEx.Standard;
using System.Text.RegularExpressions;
using SkiaSharp;

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
                    // resize image path
                    string resizedImagePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "resized_image.png");

                    // processed image path
                    string processedImagePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "processed_image.png");

                    // change scale
                    ResizeImage(Image.Path, resizedImagePath, 4);

                    // change color
                    PreprocessImage(resizedImagePath, processedImagePath);

                    // detect text from image
                    using (var img = Tesseract.Pix.LoadFromFile(processedImagePath))
                    {
                        // Tesseract engine ENG
                        using (var engine = new Tesseract.TesseractEngine(@"./_tessdata_best", "eng", Tesseract.EngineMode.Default))
                        {
                            // OCR
                            using (var page = engine.Process(img))
                            {
                                Image.TextEng = page.GetText();
                            }
                        }

                        // Tesseract engine Vi
                        using (var engine = new Tesseract.TesseractEngine(@"./_tessdata", "vie", Tesseract.EngineMode.Default))
                        {
                            // OCR
                            using (var page = engine.Process(img))
                            {
                                Image.TextVi = page.GetText();
                            }
                        }
                    }

                    // delete temp files
                    if (System.IO.File.Exists(resizedImagePath))
                    {
                        System.IO.File.Delete(resizedImagePath);
                    }
                    if (System.IO.File.Exists(processedImagePath))
                    {
                        System.IO.File.Delete(processedImagePath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during OCR: {ex.Message}");
                }
            });
        }

        void ResizeImage(string inputPath, string outputPath, int scaleFactor)
        {
            using (var input = SKBitmap.Decode(inputPath))
            {
                var resized = input.Resize(new SKImageInfo(input.Width * scaleFactor, input.Height * scaleFactor), SKFilterQuality.High);
                using (var output = System.IO.File.OpenWrite(outputPath))
                {
                    resized.Encode(output, SKEncodedImageFormat.Png, 100);
                }
            }
        }

        void PreprocessImage(string inputPath, string outputPath)
        {
            using (var input = SKBitmap.Decode(inputPath))
            {
                // Chuyển ảnh sang thang độ xám
                using (var grayImage = input.Copy(SKColorType.Gray8))
                {
                    // Đảo ngược màu sắc
                    for (int y = 0; y < grayImage.Height; y++)
                    {
                        for (int x = 0; x < grayImage.Width; x++)
                        {
                            var pixel = grayImage.GetPixel(x, y);
                            var invertedPixel = new SKColor((byte)(255 - pixel.Red), (byte)(255 - pixel.Red), (byte)(255 - pixel.Red));
                            grayImage.SetPixel(x, y, invertedPixel);
                        }
                    }

                    // Lưu ảnh đã xử lý
                    using (var output = System.IO.File.OpenWrite(outputPath))
                    {
                        grayImage.Encode(output, SKEncodedImageFormat.Png, 100);
                    }
                }
            }
        }
    }
}
