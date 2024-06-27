using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using System.Windows;
using System.Windows.Controls;
using XRay;
namespace OpenGLSrfReader.Controls
{
    public partial class GLControl2 : UserControl
    {
        private int _textureId2;
        private int _program2;
        private int _vertexArray2;
        private int _textureWidth2;
        private int _textureHeight2;

        public GLControl2()
        {
            InitializeComponent();
        }

        private void OnLoaded2(object sender, RoutedEventArgs e)
        {
            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 3
            };
            openTkControl2.Start(settings);
            InitializeShaders2();
            InitializeTexture2();
        }

        private void OnUnloaded2(object sender, RoutedEventArgs e)
        {
            GL.DeleteTexture(_textureId2);
            GL.DeleteProgram(_program2);
            openTkControl2.Dispose();
        }
        private void OnRender2(TimeSpan delta)
        {
            Clear();
            DrawTexture2((int)openTkControl2.ActualWidth, (int)openTkControl2.ActualHeight, _textureId2);
        }

        private void InitializeTexture2()
        {
            _textureId2 = GL.GenTexture();
        }

        private void Clear()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        ushort[] _data2;
        ushort[,,] _Data2;
        public void RenderImage2(SrfFileData data2, TextBox debugTextBlock2)
        {
            string debugInfo = $"Pixel Data: {string.Join(", ", data2.PixelData.Take(10))}";
            debugTextBlock2.Text = debugInfo;

            _Data2 = Tools.ArrayToTensor(data2.PixelData, data2.FrameHeight, data2.FrameWidth);
            ushort[,,] _NormalizeData = Tools.Normalize(_Data2);
            //ushort[,,] _Invert = Tools.Inverted(_NormalizeData);
            _data2 = Tools.Flatten(_NormalizeData);

            // Гармонизируем изображение
            // ushort[] harmonizedData = ImageProcessing.HarmonizeImage(data.PixelData);
            // ushort[] BCData = ImageProcessing.ApplyBrightnessAndContrast(harmonizedData, 0, 0, true);

            _textureWidth2 = data2.FrameWidth;
            _textureHeight2 = data2.FrameHeight;

            GL.BindTexture(TextureTarget.Texture2D, _textureId2);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R16, _textureWidth2, _textureHeight2, 0,
                PixelFormat.Red, PixelType.UnsignedShort, _data2);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            openTkControl2.InvalidateVisual();
        }

        private void DrawTexture2(int width, int height, int textureId)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(_program2);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.BindVertexArray(_vertexArray2);

            // GL.Viewport(0, 0, width, height);

            float textureAspectRatio = (float)_textureWidth2 / _textureHeight2;
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

        private void InitializeShaders2()
        {
            string vertexShaderSource2 = @"
                #version 330 core
                layout(location = 0) in vec2 aPos;
                layout(location = 1) in vec2 aTexCoord2;

                out vec2 TexCoord2;

                void main()
                {
                    TexCoord2 = aTexCoord2;
                    gl_Position = vec4(aPos, 0.0, 1.0);
                }";

            string fragmentShaderSource2 = @"
                #version 330 core
                out vec4 FragColor;

                in vec2 TexCoord2;

                uniform sampler2D texture2;

                void main()
                {
                    float value = texture(texture2, TexCoord2).r;
                    FragColor = vec4(value, value, value, 1.0);
                }";

            int vertexShader2 = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader2, vertexShaderSource2);
            GL.CompileShader(vertexShader2);
            CheckShaderCompileStatus(vertexShader2);

            int fragmentShader2 = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader2, fragmentShaderSource2);
            GL.CompileShader(fragmentShader2);
            CheckShaderCompileStatus(fragmentShader2);

            _program2 = GL.CreateProgram();
            GL.AttachShader(_program2, vertexShader2);
            GL.AttachShader(_program2, fragmentShader2);
            GL.LinkProgram(_program2);
            CheckProgramLinkStatus(_program2);

            GL.DeleteShader(vertexShader2);
            GL.DeleteShader(fragmentShader2);

            float[] vertices = {
                // positions   // tex coords
                -1.0f,  1.0f,  0.0f, 0.0f,
                -1.0f, -1.0f,  0.0f, 1.0f,
                 1.0f,  1.0f,  1.0f, 0.0f,
                 1.0f, -1.0f,  1.0f, 1.0f,
            };

            int vertexBuffer;
            GL.GenVertexArrays(1, out _vertexArray2);
            GL.GenBuffers(1, out vertexBuffer);

            GL.BindVertexArray(_vertexArray2);

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
