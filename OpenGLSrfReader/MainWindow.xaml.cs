using Microsoft.Win32;
using OpenGLSrfReader.Controls;
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
                imageData2 = new SrfFileData();
                imageData2 = imageData1 as SrfFileData;

                NormalizeImage(imageData1);
                DisplayImage(imageData1, glControl);
                NormalizeImage(imageData2);
                DisplayImage(imageData2, glControl2);
            }

        }
        
        private void NormalizeImage_Click(object sender, RoutedEventArgs e)
        {
            NormalizeImage(imageData1);
            DisplayImage(imageData1, glControl);
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
            _currentImage = Tools.ArrayToTensor(_Image, 3008, 3072);
            ushort[,,] _InvertImage= Tools.Inverted(_currentImage);
            _Image = Tools.Flatten(_InvertImage);

            SrfFileData Data = new SrfFileData
            {
                PixelData = _Image,
                FrameWidth = 3072,
                FrameHeight = 3008
            };
            DisplayImage(Data, glControl);
        }

        private void HistogramEqualization_Click(object sender, RoutedEventArgs e)
        {
            _currentImage = Tools.ArrayToTensor(_Image, 3008, 3072);
            ushort[,,] _HistEqualization = Tools.HistEqualization(_currentImage);
            _Image = Tools.Flatten(_HistEqualization);

            SrfFileData Data = new SrfFileData
            {
                PixelData = _Image,
                FrameWidth = 3072,
                FrameHeight = 3008
            };
            DisplayImage(Data, glControl);
        }

        private void MedianFilter_Click(object sender, RoutedEventArgs e)
        {
            _currentImage = Tools.ArrayToTensor(_Image, 3008, 3072);
            ushort[,,] _MedianFilter = Tools.MedianFilter(_currentImage, 5);
            _Image = Tools.Flatten(_MedianFilter);

            SrfFileData Data = new SrfFileData
            {
                PixelData = _Image,
                FrameWidth = 3072,
                FrameHeight = 3008
            };
            DisplayImage(Data, glControl);
        }

        private void BoxFilter_Click(object sender, RoutedEventArgs e)
        {
            _currentImage = Tools.ArrayToTensor(_Image, 3008, 3072);
            ushort[,,] _BoxFilter = Tools.BoxFilter(_currentImage, 5);
            _Image = Tools.Flatten(_BoxFilter);

            SrfFileData Data = new SrfFileData
            {
                PixelData = _Image,
                FrameWidth = 3072,
                FrameHeight = 3008
            };
            DisplayImage(Data, glControl);
        }

        private void BilateralFilter_Click(object sender, RoutedEventArgs e)
        {
            _currentImage = Tools.ArrayToTensor(_Image, 3008, 3072);
            ushort[,,] _BilateralFilter = Tools.BilateralFilter(_currentImage, 3, 10);
            _Image = Tools.Flatten(_BilateralFilter);

            SrfFileData Data = new SrfFileData
            {
                PixelData = _Image,
                FrameWidth = 3072,
                FrameHeight = 3008
            };
            DisplayImage(Data, glControl);
        }

        private void GaussianFilter_Click(object sender, RoutedEventArgs e)
        {
            _currentImage = Tools.ArrayToTensor(_Image, 3008, 3072);
            ushort[,,] _GaussianFilter = Tools.Gaussian(_currentImage, 5, 0.8);
            _Image = Tools.Flatten(_GaussianFilter);

            SrfFileData Data = new SrfFileData
            {
                PixelData = _Image,
                FrameWidth = 3072,
                FrameHeight = 3008
            };
            DisplayImage(Data   , glControl);
        }

        private void SharpenFilter_Click(object sender, RoutedEventArgs e)
        {
            _currentImage = Tools.ArrayToTensor(_Image, 3008, 3072);
            ushort[,,] _SharpenFilter = Tools.Sharpen(_currentImage, 1.0, false);
            _Image = Tools.Flatten(_SharpenFilter);

            SrfFileData Data = new SrfFileData
            {
                PixelData = _Image,
                FrameWidth = 3072,
                FrameHeight = 3008
            };
            DisplayImage(Data, glControl);
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