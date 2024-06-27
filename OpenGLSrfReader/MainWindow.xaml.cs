using Microsoft.Win32;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Wpf;
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
        
        private void OnLoadImageClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SRF Files (*.srf)|*.srf|RAW Files (*.raw)|*.raw";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                SrfFileReader reader = new SrfFileReader(filePath);
                SrfFileData data = reader.ReadSrfFile();
                SrfFileData data2 = reader.ReadSrfFile();
                DisplayImage(data);
                DisplayImage2(data2);
            }

        }
        
        private void DisplayImage(SrfFileData data)
        {
            glControl.RenderImage(data, debugTextBlock);
        }
        private void DisplayImage2(SrfFileData data2)
        {
            glControl2.RenderImage2(data2, debugTextBlock2);
        }
        private void glControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

    }
}