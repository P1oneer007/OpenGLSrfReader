using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static OpenGLSrfReader.Services.CvieLibrary;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenGLSrfReader.Services
{
    internal class CvieLibrary
    {
        public enum ECVIE
        {
            CVIE_E_OK = 0,
            CVIE_E_NOT_OK = 1,
            CVIE_E_CREATE_HANDLE = 2,
            CVIE_E_BAD_HANDLE = 3,
            CVIE_E_USER_ABORTED = 4,
            CVIE_E_FATAL_ERROR = 5,
            CVIE_E_FILEIO_ERROR = 6,
            CVIE_E_ILLEGAL_COMMAND = 7,
            CVIE_E_INVALID_INPUT = 8,
            CVIE_E_INVALID_PARAMETER_FILE = 9,
            CVIE_E_LICENSE_ERROR = 10,
            CVIE_E_NOT_SUPPORTED = 11,
            CVIE_E_SETTING_NOT_READY = 12,
            CVIE_E_SERVER_TIMEOUT = 13,
            CVIE_E_SERVER_LAUNCH_FAILURE = 14,
            CVIE_E_UNKNOWN_ERROR = 15
        }
       /* public class HCVIE
        {
            
        }*/
         public class HCVIE : IDisposable
         {
             public IntPtr Handle { get; private set; }
             public HCVIE()
             {
                 Handle = IntPtr.Zero;
             }

             public void Dispose()
             {
                /* if (Handle != IntPtr.Zero)
                 {
                     CVIEDestroy(ref Handle);
                 }*/
             }
         }

        public enum DIPAR
        {
            DI_STRENGTH = 1,
            DI_DETAIL = 2,
            DI_NOISE = 3,
            DI_EDGE = 4,
            DI_MASKEXPANSION = 5
        }

        public enum CVIE_DATA_FLAGS
        {
            CVIE_DATA_U16 = 0x00000001,
            CVIE_DATA_S16 = 0x00000002,
            CVIE_DATA_U8 = 0x00000004,
            CVIE_DATA_F32 = 0x00000008,
            CVIE_DATA_CUDA_FLAG = 0x00010000
        }

        public enum CVIE_CREATE_FLAGS
        {
            CVIE_CREATE_DEFAULT = 0,
            CVIE_CREATE_TRACE = 16,
            CVIE_CREATE_SERVER = 32,
            CVIE_CREATE_CUDA = 64
        }

        public struct TCVIEImageStatistics
        {
            public float low3Percentile;
            public float high97Percentile;
            public float medianPixelValue;
            public float meanPixelValue;
            public float minPixelValue;
            public float maxPixelValue;
        }

        public struct TCVIEPoint
        {
            public float x;
            public float y;
        }

        public struct TCVIECurve
        {
            public int count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public TCVIEPoint[] points;
        }

        public enum CVIE_SETTINGS
        {
            SETTING_HISTOGRAM_OPTIMIZATION = 0x100,
            SETTING_SKINLINE_WEIGHT = 0x101,
            SETTING_LATITUDE_COMPRESSION = 0x102,
            SETTING_CONTRAST_ENHANCEMENT = 0x103,
            SETTING_EDGE_ENHANCEMENT = 0x104,
            SETTING_NOISE_SUPPRESSION = 0x105,
            SETTING_OUT_MASK = 0x106,
            OBJECT_MEAN = 0x110,
            OBJECT_MEDIAN = 0x111,
            ALUT_ORG_HIST = 0x112,
            ALUT_LUT1_HIST = 0x113,
            ALUT_LUT2_HIST = 0x114,
            ALUT_LIN_CURVE = 0x115,
            ALUT_EXP_CURVE = 0x116,
            ALUT_IGAMMA_CURVE = 0x117,
            ALUT_FINAL_CURVE = 0x118
        }

        public enum CVIE_PARAMETER_TYPES
        {
            PARAMETER_DESCRIPTION = 1,
            OPERATION_DESCRIPTION = 2,
            ALGORITHM_DESCRIPTION = 3,
            TEMP_DIRECTORY = 4,
            CUDA_CONTEXT = 5,
            TIMER_DUMP = 6,
            ENABLE_IMAGE_STATISTICS = 7,
            IMAGE_STATISTICS_MARGIN = 8,
            IMAGE_STATISTICS = 9,
            MAMMO_WINDOW_CENTER = 10,
            MAMMO_WINDOW_WIDTH = 11
        }
        /* Functions */
     /*   [DllImport("cvie.dll")]
        public static extern ECVIE CVIECreate(ref HCVIE handle, int flags);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIESetParameterFile(HCVIE handle, string fileName, ref int settings);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIESetParameterBuffer(HCVIE handle, string buffer, ref int settings);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIEEnhanceSetup(HCVIE handle, int width, int height, int flags, int setting, byte[] mask);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIEEnhanceNext(HCVIE handle, IntPtr inImage, IntPtr outImage, int setting);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIESetProgressCallback(HCVIE handle, IntPtr callBackFunction);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIEDestroy(ref HCVIE handle);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIESetThreads(HCVIE handle, int threads);

        /* Doctor's Interface */
      /*  [DllImport("cvie.dll")]
        public static extern ECVIE CVIEGetDIParameter(HCVIE handle, int setting, DIPAR parameter, ref double value);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIESetDIParameter(HCVIE handle, int setting, DIPAR parameter, double value);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIEGetParameterValue(HCVIE handle, int setting, int parameter, IntPtr value);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVIESetParameterValue(HCVIE handle, int setting, int parameter, IntPtr value);

        /* License handling */
     /*   [DllImport("cvie.dll")]
        public static extern ECVIE CVLMCheckLicense(HCVIE handle, int moduleIndex);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVLMGetHostId(HCVIE handle, int hostTypeIndex, ref uint hostId);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVLMGetPossibleHostTypes(HCVIE handle, ref IntPtr hostTypes);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVLMGetPossibleModules(HCVIE handle, ref IntPtr modules);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVLMGetRegisteredModules(HCVIE handle, ref int moduleIndexes, ref int hostTypeIndexes, IntPtr info);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVLMSetKey(HCVIE handle, int moduleIndex, string key);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVLMSetParameterValue(int parameter, IntPtr value);

        [DllImport("cvie.dll")]
        public static extern ECVIE CVLMUninstall(HCVIE handle, int moduleIndex, int hostTypeIndex);

        /* Error handling */
      /*  [DllImport("cvie.dll")]
        public static extern IntPtr CVEMGetLastError(HCVIE handle, IntPtr errstr, int len);

        /* Static LUT */
      /*  [DllImport("cvie.dll")]
        public static extern ECVIE CVSLImageLut(HCVIE handle, IntPtr inImage, IntPtr outImage, int height, int width, int flags, string fileName);

        /* Deprecated functions. Should not be used in new code */

        /* DEPRECATED. Use CVIEEnhanceSetup and CVIEEnhanceNext instead, which results in better
           performance when processing multiple images. */
      /*  [DllImport("cvie.dll")]
        public static extern ECVIE CVIEEnhance(HCVIE handle, IntPtr inImage, IntPtr outImage, int width, int height, int flags, int setting, byte[] mask);*/
    
    }
    public static class CvieInterop
    {
        private const string DllName = "C:\\Users\\root\\source\\repos\\OpenGLSrfReader\\OpenGLSrfReader\\bin\\Debug\\net8.0-windows\\cvie.dll";

        [DllImport(DllName, EntryPoint = "_CVIECreate@8", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIECreate(out IntPtr handle, int flags);

        [DllImport(DllName, EntryPoint = "_CVIESetParameterFile@12", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIESetParameterFile(IntPtr handle, string fileName, out int settings);

        [DllImport(DllName, EntryPoint = "_CVIESetParameterBuffer@12", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIESetParameterBuffer(IntPtr handle, string buffer, out int settings);

        [DllImport(DllName, EntryPoint = "_CVIEEnhanceSetup@24", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIEEnhanceSetup(IntPtr handle, int width, int height, int flags, int setting, IntPtr mask);

        [DllImport(DllName, EntryPoint = "_CVIEEnhanceNext@16", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIEEnhanceNext(IntPtr handle, IntPtr inImage, IntPtr outImage, int setting);

        [DllImport(DllName, EntryPoint = "_CVIESetProgressCallback@8", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIESetProgressCallback(IntPtr handle, ProgressCallback callback);

        [DllImport(DllName, EntryPoint = "_CVIEDestroy@4", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIEDestroy(ref IntPtr handle);

        [DllImport(DllName, EntryPoint = "_CVIESetThreads@8", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIESetThreads(IntPtr handle, int threads);

        public delegate int ProgressCallback(float percent);

        [DllImport(DllName, EntryPoint = "_CVIEGetDIParameter@16", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIEGetDIParameter(IntPtr handle, int setting, int parameter, out double value);

        [DllImport(DllName, EntryPoint = "_CVIESetDIParameter@20", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIESetDIParameter(IntPtr handle, int setting, int parameter, double value);

        [DllImport(DllName, EntryPoint = "_CVIEGetParameterValue@16", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIEGetParameterValue(IntPtr handle, int setting, int parameter, IntPtr value);

        [DllImport(DllName, EntryPoint = "_CVIESetParameterValue@16", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIESetParameterValue(IntPtr handle, int setting, int parameter, IntPtr value);

        [DllImport(DllName, EntryPoint = "_CVLMCheckLicense@8", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVLMCheckLicense(IntPtr handle, int moduleIndex);

        [DllImport(DllName, EntryPoint = "_CVLMGetHostId@12", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVLMGetHostId(IntPtr handle, int hostTypeIndex, out ulong hostId);

        [DllImport(DllName, EntryPoint = "_CVLMGetPossibleHostTypes@8", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVLMGetPossibleHostTypes(IntPtr handle, out IntPtr hostTypes);

        [DllImport(DllName, EntryPoint = "_CVLMGetPossibleModules@8", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVLMGetPossibleModules(IntPtr handle, out IntPtr modules);

        [DllImport(DllName, EntryPoint = "_CVLMGetRegisteredModules@16", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVLMGetRegisteredModules(IntPtr handle, IntPtr moduleIndexes, IntPtr hostTypeIndexes, IntPtr info);

        [DllImport(DllName, EntryPoint = "_CVLMSetKey@12", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVLMSetKey(IntPtr handle, int moduleIndex, string key);

        [DllImport(DllName, EntryPoint = "_CVLMSetParameterValue@8", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVLMSetParameterValue(int parameter, IntPtr value);

        [DllImport(DllName, EntryPoint = "_CVLMUninstall@12", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVLMUninstall(IntPtr handle, int moduleIndex, int hostTypeIndex);

        [DllImport(DllName, EntryPoint = "_CVEMGetLastError@12", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CVEMGetLastError(IntPtr handle, IntPtr errstr, int len);

        [DllImport(DllName, EntryPoint = "_CVSLImageLut@28", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVSLImageLut(IntPtr handle, IntPtr inImage, IntPtr outImage, int height, int width, int flags, string fileName);

        // Deprecated function
        [DllImport(DllName, EntryPoint = "_CVIEEnhance@32", CallingConvention = CallingConvention.StdCall)]
        public static extern int CVIEEnhance(IntPtr handle, IntPtr inImage, IntPtr outImage, int width, int height, int flags, int setting, IntPtr mask);

        
    }
    class Tools1
    {
        public static IntPtr UshortToIntPtr(ushort[] value)
        {
            /* unsafe
             {
                 return new IntPtr(&value);
             }*/

            int length = value.Length;
            IntPtr ptr = Marshal.AllocHGlobal(length * sizeof(ushort));
            unsafe
            {
                ushort* arrayPtr = (ushort*)ptr;
                for (int i = 0; i < length; i++)
                {
                    arrayPtr[i] = value[i];
                }
            }
            return ptr;
        }
        public static ushort[] IntPtrToUshort(IntPtr ptr, int length)
        {
            /* unsafe
             {
                 return *(ushort[]*)ptr;
             }*/

            ushort[] array = new ushort[length];
            unsafe
            {
                ushort* arrayPtr = (ushort*)ptr;
                for (int i = 0; i < length; i++)
                {
                    array[i] = arrayPtr[i];
                }
            }
            return array;
        }

        public static int ProcessImage(IntPtr in_data, IntPtr out_data, int width, int height)
        {
            ECVIE result;
           // HCVIE handle = new HCVIE();
            int numSettings = 0;
             
            result = (ECVIE)CvieInterop.CVIECreate(out IntPtr handle, (int)CVIE_CREATE_FLAGS.CVIE_CREATE_DEFAULT);
            if (result != ECVIE.CVIE_E_OK)
            {
                Console.WriteLine("Error in CVIECreate()");
                return -1;
            }

            result = (ECVIE)CvieInterop.CVIESetParameterFile(handle, "par\\default.alut", out numSettings);
            if (result != ECVIE.CVIE_E_OK)
            {
                Console.WriteLine("Error in CVIESetParameterFile()");
                CvieInterop.CVIEDestroy(ref handle);
                return -1;
            }

           result = (ECVIE)CvieInterop.CVIESetParameterFile(handle, "par\\default.gop", out numSettings);
            if (result != ECVIE.CVIE_E_OK)
            {
                Console.WriteLine("Error in CVIESetParameterFile()");
                CvieInterop.CVIEDestroy(ref handle);
                return -1;
            }

            result = (ECVIE)CvieInterop.CVIEEnhanceSetup(handle, width, height, (int)CVIE_DATA_FLAGS.CVIE_DATA_U16, 0, 0);
            if (result != ECVIE.CVIE_E_OK)
            {
                Console.WriteLine("Error in CVIEEnhanceSetup()");
                CvieInterop.CVIEDestroy(ref handle);
                return -1;
            }

            result = (ECVIE)CvieInterop.CVIEEnhanceNext(handle, in_data, out_data, numSettings);
            if (result != ECVIE.CVIE_E_OK)
            {
                Console.WriteLine("Error in CVIEEnhanceNext()");
                CvieInterop.CVIEDestroy(ref handle);
                return -1;
            }

            result = (ECVIE)CvieInterop.CVIEDestroy(ref handle);
            if (result != ECVIE.CVIE_E_OK)
            {
                Console.WriteLine("Error in CVIEDestroy()");
                return -1;
            }
           
            return 0;
        }
    }
}
