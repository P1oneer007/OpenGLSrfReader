// using ImageMagick;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using static System.Console;

namespace XRay
{

class Ext()
{
    [DllImport("interface.dll")]
    public static extern int display(ushort[,,] arr, int height, int width, int channels);
    
    [DllImport("interface.dll")]
    public static extern IntPtr readImage(string path);
    
    [DllImport("interface.dll")]
    public static extern int writeImage(string path, ushort[,,] arr, int height, int width, int channels);

    [DllImport("interface.dll")]
    public static extern int releaseMemory(IntPtr ptr, bool grayscale=false);
        
    [DllImport("interface.dll")]
    public static extern IntPtr histEqualization(ushort[,,] arr, int height, int width, int channels);
    
    [DllImport("interface.dll")]
    public static extern int gammaCorrection_16bit(ushort[,,] arr, int height, int width, int channels, double gamma);
    
    [DllImport("interface.dll")]
    public static extern IntPtr medianFilter(ushort[,,] arr, int height, int width, int channels, int kernel_size);

    [DllImport("interface.dll")]
    public static extern IntPtr box(ushort[,,] arr, int height, int width, int channels, int kernel_size);

    [DllImport("interface.dll")]
    public static extern IntPtr bilateral(ushort[,,] arr, int height, int width, int channels, int d, double sigma);

    [DllImport("interface.dll")]
    public static extern IntPtr sharpen(ushort[,,] arr, int height, int width, int channels, double strength, Boolean diag);

    [DllImport("interface.dll")]
    public static extern IntPtr gaussian(ushort[,,] arr, int height, int width, int channels, int kernel_size, double sigma);

    [DllImport("interface.dll")]
    public static extern IntPtr laplacianOfGaussian(ushort[,,] arr, int height, int width, int channels, int kernel_size, double sigma);
    
    [DllImport("interface.dll")]
    public static extern IntPtr thresholding(ushort[,,] arr, int height, int width, int channels, double threshold1, double threshold2, Boolean norm);
    
    [DllImport("interface.dll")]
    public static extern IntPtr normalize_16bit(ushort[,,] arr, int height, int width, int channels);
    
    [DllImport("interface.dll")]
    public static extern IntPtr sobel(ushort[,,] arr, int height, int width, int channels, int kernel_size, double sigma);
    
    [DllImport("interface.dll")]
    public static extern IntPtr sobelx(ushort[,,] arr, int height, int width, int channels, int kernel_size, double sigma);
    
    [DllImport("interface.dll")]
    public static extern IntPtr sobely(ushort[,,] arr, int height, int width, int channels, int kernel_size, double sigma);
    
    [DllImport("interface.dll")]
    public static extern IntPtr canny(ushort[,,] arr, int height, int width, int channels, int threshold1, int threshold2, int kernel_size, double sigma);
}

class Tools() 
{
    public const ushort MAX_ITENSITY_16BIT = 65535;
    public const int MAX_ITENSITY_8BIT = 255;
    
    static ushort[,,] FoldTensor(IntPtr ptr) 
    {
        int rank = Marshal.ReadInt32(ptr);
        int[] shape = new int[rank];
        IntPtr start = IntPtr.Add(ptr, sizeof(int));
        Marshal.Copy(start, shape, 0, rank);
        int size = shape[0] * shape[1] * shape[2];
        int[] values = new int[size];
        start = IntPtr.Add(ptr, sizeof(int)*(rank+1));
        Marshal.Copy(start, values, 0, size);
        ushort[] tensor_linearized = Array.ConvertAll(values, x => (ushort) x);
        ushort[,,] tensor = new ushort[shape[0], shape[1], shape[2]];
        Buffer.BlockCopy(tensor_linearized, 0, tensor, 0, Buffer.ByteLength(tensor_linearized));
        Ext.releaseMemory(ptr);
        return tensor;
    }

    public static void DisplayTensor(ushort[,,] tensor)
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        Ext.display(tensor, height, width, channels);
    }
        
    public static ushort[,,] ReadImageByOpenCV(string path)
    {
        IntPtr ptr = Ext.readImage(path);
        return FoldTensor(ptr);
    }

    public static int WriteImageByOpenCV(string directory, string fileName, ushort[,,] tensor) 
    {
        string path = directory + fileName + ".png";
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        return Ext.writeImage(path, tensor, height, width, channels);
    }

    public static ushort[,,] ReadRaw(string path, int height=3008, int width=3072)
    {
        byte[] data = File.ReadAllBytes(path);
        ushort[,,] tensor = new ushort[height, width, 1];
        Buffer.BlockCopy(data, 0, tensor, 0, data.Length);
        return tensor;
    }

    public static ushort[,,] Normalize(ushort[,,] tensor) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.normalize_16bit(tensor, height, width, channels);
        return FoldTensor(ptr); 
    }

    public static void GammaCorrection(ushort[,,] tensor, double gamma=1.2)
    {   

        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        
        Ext.gammaCorrection_16bit(tensor, height, width, channels, gamma);
    }

    public static void GammaCorrectionAlt(ushort[,,] tensor, double gamma=1.2) {
        // make LUT
        const int length = MAX_ITENSITY_16BIT+1;
        ushort[] gammaLUT = new ushort[length];
        double power = 1.0 / gamma;
        for (int i = 0; i < length; i++) 
        {
            double itensity = (double)i / MAX_ITENSITY_16BIT;
            gammaLUT[i] = (ushort)(MAX_ITENSITY_16BIT * Math.Pow(itensity, power));
        }
        // apply LUT
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        for (int i = 0; i < height; i++) 
        {
            for (int j = 0; j < width; j++) 
            {
                tensor[i, j, 0] = gammaLUT[tensor[i, j, 0]];
            }
        }
    }

    public static void Invert(ushort[,,] tensor) 
    {        
        for (int y = 0; y < tensor.GetLength(0); y++) {
            for (int x = 0; x < tensor.GetLength(1); x++) {
                tensor [y, x, 0] = (ushort)(MAX_ITENSITY_16BIT - tensor[y, x, 0]);
            }
        }
    }
    public static ushort[,,] Inverted(ushort[,,] tensor) 
    {        
        ushort[,,] newTensor = (ushort[,,])tensor.Clone();
        Invert(newTensor);
        return newTensor;
    }
    
    public static ushort[,,] HistEqualization(ushort[,,] tensor)
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);

        IntPtr ptr = Ext.histEqualization(tensor, height, width, channels);
        return FoldTensor(ptr);
    }

    // Smooth & Noise supression
    // -------------------------

    public static ushort[,,] MedianFilter(ushort[,,] tensor, int kernel_size=5)
    {
        // kernel size must be equal 1, 3 or 5
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.medianFilter(tensor, height, width, channels, kernel_size);
        return FoldTensor(ptr);
    }

    public static ushort[,,] BoxFilter(ushort[,,] tensor, int kernel_size=5)
    {
        // kernel size must be equal 1, 3 or 5 
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.box(tensor, height, width, channels, kernel_size);
        return FoldTensor(ptr);
    }

    public static ushort[,,] BilateralFilter(ushort[,,] tensor, int d=3, double sigma=10)
    {
            // kernel size must be equal 1, 3 or 5
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.bilateral(tensor, height, width, channels, d, sigma);
        return FoldTensor(ptr);
    }

    public static ushort[,,] Gaussian(ushort[,,] tensor, int kernel_size=5, double sigma=0.8) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.gaussian(tensor, height, width, channels, kernel_size, sigma);
        return FoldTensor(ptr);
    }

    // Sharpen
    // -------

    public static ushort[,,] Sharpen(ushort[,,] tensor, double strength=1.0, Boolean diag=false) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.sharpen(tensor, height, width, channels, strength, diag);
        return FoldTensor(ptr);
    }

    // Edge detection
    // --------------
    
    // Laplacian of Gaussian:
    public static ushort[,,] LoG(ushort[,,] tensor, int kernel_size=7, double sigma=2.5) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.laplacianOfGaussian(tensor, height, width, channels, kernel_size, sigma);
        return FoldTensor(ptr);        
    }

    public static ushort[,,] Sobel(ushort[,,] tensor, int kernel_size=7, double sigma=2.5) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.sobel(tensor, height, width, channels, kernel_size, sigma);
        return FoldTensor(ptr);        
    }
    public static ushort[,,] SobelX(ushort[,,] tensor, int kernel_size=7, double sigma=2.5) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.sobelx(tensor, height, width, channels, kernel_size, sigma);
        return FoldTensor(ptr);        
    }
    public static ushort[,,] SobelY(ushort[,,] tensor, int kernel_size=7, double sigma=2.5) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.sobely(tensor, height, width, channels, kernel_size, sigma);
        return FoldTensor(ptr);        
    }
    public static ushort[,,] Canny(ushort[,,] tensor, int threshold1, int threshold2, int kernel_size=7, double sigma=2.5) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.canny(tensor, height, width, channels, threshold1, threshold2, kernel_size, sigma);
        return FoldTensor(ptr);        
    }

    public static ushort[,,] Thresholding(ushort[,,] tensor, double threshold1=0, double threshold2=1, bool norm=true) 
    {
        int height = tensor.GetLength(0);
        int width = tensor.GetLength(1);
        int channels = tensor.GetLength(2);
        IntPtr ptr = Ext.thresholding(tensor, height, width, channels, threshold1, threshold2, norm);
        return FoldTensor(ptr); 
    }
    // для преобразования из одномерного массива в тензор
    public static ushort[,,] ArrayToTensor(ushort[] array, int height = 3008, int width = 3072)
    {
             ushort[,,] tensor = new ushort[height, width, 1];
             int length = array.Length * sizeof(ushort);
             System.Buffer.BlockCopy(array, 0, tensor, 0, length);
             return tensor;
        }
    // для преобразования из тензора в одномерный массив
    public static ushort[] Flatten(ushort[,,] tensor)
    {
             int height = tensor.GetLength(0);
             int width = tensor.GetLength(1);
             int channels = tensor.GetLength(2);
             int length = height * width * channels;
             ushort[] array = new ushort[length];
             Buffer.BlockCopy(tensor, 0, array, 0, length * sizeof(ushort));
             return array;
        }

    }
} 