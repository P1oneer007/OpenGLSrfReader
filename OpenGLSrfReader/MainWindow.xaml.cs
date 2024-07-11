using Microsoft.Win32;
using OpenGLSrfReader.Controls;
using OpenGLSrfReader.Services;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XRay;

namespace OpenGLSrfReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private ushort[,,] _currentImage;
        private ushort[] _Image;

        private SrfFileData imageData1;
        private SrfFileData imageData2;
        private void OnLoadImageClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SRF Files (*.srf)|*.srf|RAW Files (*.raw)|*.raw";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                SrfFileReader reader = new SrfFileReader(filePath);
                imageData1 = reader.ReadSrfFile();
                imageData2 = new SrfFileData
                {
                    PixelData = (ushort[])imageData1.PixelData.Clone(),
                    FrameHeight = imageData1.FrameHeight,
                    FrameWidth = imageData1.FrameWidth,
                };
               
                //NormalizeImage(imageData1);
                DisplayImage(imageData1, glControl);
               //NormalizeImage(imageData2);
                DisplayImage(imageData2, glControl2);
            }

        }
        
        private void NormalizeImage_Click(object sender, RoutedEventArgs e)
        {
            NormalizeImage(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void NormalizeImage_Click2(object sender, RoutedEventArgs e)
        {
            NormalizeImage(imageData2);
            DisplayImage(imageData2, glControl2);
        }
        private void NormalizeImage(SrfFileData data)
        {
            if (data == null) return;            
            ushort[,,] _currentImage = Tools.ArrayToTensor(data.PixelData, data.FrameHeight, data.FrameWidth);
            ushort[,,] _NormalizeData = Tools.Normalize(_currentImage);
            data.PixelData = Tools.Flatten(_NormalizeData);            
        }

        private void InvertImage_Click(object sender, RoutedEventArgs e)
        {
            InvertImage(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void InvertImage_Click2(object sender, RoutedEventArgs e)
        {
            InvertImage(imageData2);
            DisplayImage(imageData2, glControl2);
        }
        private void InvertImage(SrfFileData data)
        {
            if (data == null) return;
            ushort[,,] _currentImage = Tools.ArrayToTensor(data.PixelData, data.FrameHeight, data.FrameWidth);
            ushort[,,] _InvertImage = Tools.Inverted(_currentImage);
            data.PixelData = Tools.Flatten(_InvertImage);
        }

        private void HistogramEqualization_Click(object sender, RoutedEventArgs e)
        {
            HistogramEqualization(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void HistogramEqualization_Click2(object sender, RoutedEventArgs e)
        {
            HistogramEqualization(imageData2);
            DisplayImage(imageData2, glControl2);
        }
        private void HistogramEqualization(SrfFileData data)
        {
            if (data == null) return;
            ushort[,,] _currentImage = Tools.ArrayToTensor(data.PixelData, data.FrameHeight, data.FrameWidth);
            ushort[,,] _HistEqualization = Tools.HistEqualization(_currentImage);
            data.PixelData = Tools.Flatten(_HistEqualization);
        }

        private void MedianFilter_Click(object sender, RoutedEventArgs e)
        {
            MedianFilter(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void MedianFilter_Click2(object sender, RoutedEventArgs e)
        {
            MedianFilter(imageData2);
            DisplayImage(imageData2, glControl2);
        }
        private void MedianFilter(SrfFileData data)
        {
            if (data == null) return;
            ushort[,,] _currentImage = Tools.ArrayToTensor(data.PixelData, data.FrameHeight, data.FrameWidth);
            ushort[,,] _MedianFilter = Tools.MedianFilter(_currentImage, 5);
            data.PixelData = Tools.Flatten(_MedianFilter);
        }

        private void BoxFilter_Click(object sender, RoutedEventArgs e)
        {
            BoxFilter(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void BoxFilter_Click2(object sender, RoutedEventArgs e)
        {
            BoxFilter(imageData2);
            DisplayImage(imageData2, glControl2);
        }
        private void BoxFilter(SrfFileData data)
        {
            if (data == null) return;
            ushort[,,] _currentImage = Tools.ArrayToTensor(data.PixelData, data.FrameHeight, data.FrameWidth);
            ushort[,,] _BoxFilter = Tools.BoxFilter(_currentImage, 5);
            data.PixelData = Tools.Flatten(_BoxFilter);
        }

        private void BilateralFilter_Click(object sender, RoutedEventArgs e)
        {
            BilateralFilter(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void BilateralFilter_Click2(object sender, RoutedEventArgs e)
        {
            BilateralFilter(imageData2);
            DisplayImage(imageData2, glControl2);
        }
        private void BilateralFilter(SrfFileData data)
        {
            if (data == null) return;
            ushort[,,] _currentImage = Tools.ArrayToTensor(data.PixelData, data.FrameHeight, data.FrameWidth);
            ushort[,,] _BilateralFilter = Tools.BilateralFilter(_currentImage, 3, 10);
            data.PixelData = Tools.Flatten(_BilateralFilter);
        }

        private void GaussianFilter_Click(object sender, RoutedEventArgs e)
        {
            GaussianFilter(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void GaussianFilter_Click2(object sender, RoutedEventArgs e)
        {
            GaussianFilter(imageData2);
            DisplayImage(imageData2, glControl2);
        }
        private void GaussianFilter(SrfFileData data)
        {
            if (data == null) return;
            ushort[,,] _currentImage = Tools.ArrayToTensor(data.PixelData, data.FrameHeight, data.FrameWidth);
            ushort[,,] _GaussianFilter = Tools.Gaussian(_currentImage, 5, 0.8);
            data.PixelData = Tools.Flatten(_GaussianFilter);
        }

        private void SharpenFilter_Click(object sender, RoutedEventArgs e)
        {
            SharpenFilter(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void SharpenFilter_Click2(object sender, RoutedEventArgs e)
        {
            SharpenFilter(imageData2);
            DisplayImage(imageData2, glControl2);
        }
        private void SharpenFilter(SrfFileData data)
        {
            if (data == null) return;
            ushort[,,] _currentImage = Tools.ArrayToTensor(data.PixelData, data.FrameHeight, data.FrameWidth);
            ushort[,,] _SharpenFilter = Tools.Sharpen(_currentImage, 1.0, false);
            data.PixelData = Tools.Flatten(_SharpenFilter);
        }

        private void CvieFilter_Click(object sender, RoutedEventArgs e)
        {
            CvieFilter(imageData1);
            DisplayImage(imageData1, glControl);
        }
        private void CvieFilter(SrfFileData data)
        {
            if (data == null) return;
            int width = data.FrameWidth;
            int height = data.FrameHeight;
            int length = width * height;
            IntPtr _datain = Tools1.UshortToIntPtr(data.PixelData);
            IntPtr _dataout = _datain;
            Tools1.ProcessImage(_datain, _dataout, data.FrameHeight, data.FrameWidth);
            ushort[] cviedata = Tools1.IntPtrToUshort(_dataout, length);
            data.PixelData = cviedata;
        }

        private void DisplayImage(SrfFileData data, GLControl controlToDisplay)
        {
            controlToDisplay.RenderImage(data, debugTextBlock);
        }
        
        private void glControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}