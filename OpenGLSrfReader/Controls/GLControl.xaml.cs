using OpenGLSrfReader.Services;
using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using XRay;
using System.Reflection;
namespace OpenGLSrfReader.Controls
{
    public partial class GLControl : UserControl
    {
        private int _textureId;
        private int _program;
        private int _vertexArray;
        private int _textureWidth;
        private int _textureHeight;

        public GLControl()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 3
            };
            
            openTkControl.Start(settings);
            InitializeShaders();
            InitializeTexture();
                       
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            GL.DeleteTexture(_textureId);
            GL.DeleteProgram(_program);
            openTkControl.Dispose();
        }
        
        private void OnRender(TimeSpan delta)
        {
            Clear();
            DrawTexture((int)openTkControl.ActualWidth, (int)openTkControl.ActualHeight, _textureId);
        }
      
        private void InitializeTexture()
        {
            _textureId = GL.GenTexture();
        }
        
        private void Clear()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        ushort[] _data;
        ushort[,,] _Data;
        
        public void RenderImage(SrfFileData data, TextBox debugTextBlock)
        {
            string debugInfo = $"Pixel Data: {string.Join(", ", data.PixelData.Take(10))}";
            debugTextBlock.Text = debugInfo;
           
           // Гармонизируем изображение
           // ushort[] harmonizedData = ImageProcessing.HarmonizeImage(data.PixelData);
           // ushort[] BCData = ImageProcessing.ApplyBrightnessAndContrast(harmonizedData, 0, 0, true);
           
            _textureWidth = data.FrameWidth;
            _textureHeight = data.FrameHeight;

           // ushort[] dataToRender = (ushort[])data.PixelData.Clone();

            GL.BindTexture(TextureTarget.Texture2D, _textureId);GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R16, _textureWidth, _textureHeight, 0,
               PixelFormat.Red, PixelType.UnsignedShort, data.PixelData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            openTkControl.InvalidateVisual();
        }
       
        private void DrawTexture(int width, int height, int textureId)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(_program);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.BindVertexArray(_vertexArray);

           // GL.Viewport(0, 0, width, height);

            float textureAspectRatio = (float)_textureWidth / _textureHeight;
            float windowAspectRatio = (float)width / height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
           // double viewport_width, viewport_height;
            if (windowAspectRatio > textureAspectRatio)
            {
               // viewport_height = Height;
               // viewport_width = (int)(Height * textureAspectRatio);

                float adjustedWidth = textureAspectRatio / windowAspectRatio;
                GL.Frustum(-adjustedWidth, adjustedWidth, -1.0f, 1.0f, -1.0f, 1.0f);
            }
             else
             {
               // viewport_width = Width;
              //  viewport_height = (int)(Width / textureAspectRatio);
               
                float adjustedHeight = windowAspectRatio / textureAspectRatio;
                GL.Ortho(-1.0f, 1.0f, -adjustedHeight, adjustedHeight, -1.0f, 1.0f);
             }
         // int x = (int)((Width - viewport_width) / 2);
         // double y = (Height - viewport_height) / 2;
         // GL.Viewport(x, (int)y, (int)viewport_width, (int)viewport_height);
           
            /* double ww = width;
             double hh = height;
             if (width > height)
             {
                 GL.Ortho(-2.0 * (ww / hh), 2.0 * (ww / hh), -2.0f, 2.0f, -1.0f, 1.0f);
             }
             else
             {
                 GL.Ortho(-2.0 * (ww / hh), 2.0 * (ww / hh), -2.0f, 2.0f, -1.0f, 1.0f);
             }*/

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            GL.BindVertexArray(0);
            GL.UseProgram(0);

        }
        
        private void InitializeShaders()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec2 aPos;
                layout(location = 1) in vec2 aTexCoord;

                out vec2 TexCoord;

                void main()
                {
                    TexCoord = aTexCoord;
                    gl_Position = vec4(aPos, 0.0, 1.0);
                }";

            string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;

                in vec2 TexCoord;

                uniform sampler2D texture1;

                void main()
                {
                    float value = texture(texture1, TexCoord).r;
                    FragColor = vec4(value, value, value, 1.0);
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompileStatus(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompileStatus(fragmentShader);

            _program = GL.CreateProgram();
            GL.AttachShader(_program, vertexShader);
            GL.AttachShader(_program, fragmentShader);
            GL.LinkProgram(_program);
            CheckProgramLinkStatus(_program);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            float[] vertices = {
                // positions   // tex coords
                -1.0f,  1.0f,  0.0f, 0.0f,
                -1.0f, -1.0f,  0.0f, 1.0f,
                 1.0f,  1.0f,  1.0f, 0.0f,
                 1.0f, -1.0f,  1.0f, 1.0f,
            };

            int vertexBuffer;
            GL.GenVertexArrays(1, out _vertexArray);
            GL.GenBuffers(1, out vertexBuffer);

            GL.BindVertexArray(_vertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void CheckShaderCompileStatus(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling shader: {infoLog}");
            }
        }

        private void CheckProgramLinkStatus(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Error linking program: {infoLog}");
            }
        }
    }
}