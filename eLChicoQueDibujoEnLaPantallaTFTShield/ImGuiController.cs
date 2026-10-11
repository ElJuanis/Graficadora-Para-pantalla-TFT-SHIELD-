using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace eLChicoQueDibujoEnLaPantallaTFTShield
{
    public class ImGuiController : IDisposable
    {
        private bool _frameBegun;
        private int _vertexArray;
        private int _vertexBuffer;
        private int _vertexBufferSize;
        private int _indexBuffer;
        private int _indexBufferSize;
        private int _fontTexture;
        private int _shader;
        private int _shaderFontTextureLocation;
        private int _shaderProjectionMatrixLocation;
        private int _windowWidth;
        private int _windowHeight;
        public void PressChar(char keyChar)
        {
            ImGui.GetIO().AddInputCharacter(keyChar);
        }
        public ImGuiController(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;

            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.Fonts.AddFontDefault();
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            CreateDeviceObjects();
            SetPerFrameImGuiData(1f / 60f);
            ImGui.NewFrame();
            _frameBegun = true;
        }

        public void WindowResized(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;
        }

        public void Render()
        {
            if (_frameBegun)
            {
                _frameBegun = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());
            }
        }

        public void Update(GameWindow wnd, float deltaTime)
        {
            if (_frameBegun) ImGui.Render();

            SetPerFrameImGuiData(deltaTime);
            UpdateImGuiInput(wnd);

            _frameBegun = true;
            ImGui.NewFrame();
        }

        private void SetPerFrameImGuiData(float deltaTime)
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(_windowWidth, _windowHeight);
            io.DisplayFramebufferScale = System.Numerics.Vector2.One;
            io.DeltaTime = deltaTime;
        }

        private static void UpdateImGuiInput(GameWindow wnd)
        {
            var io = ImGui.GetIO();
            var mouse = wnd.MouseState;
            var keyboard = wnd.KeyboardState;
            io.MouseDown[0] = mouse.IsButtonDown(MouseButton.Left);
            io.MouseDown[1] = mouse.IsButtonDown(MouseButton.Right);
            io.MouseDown[2] = mouse.IsButtonDown(MouseButton.Middle);
            io.MousePos = new System.Numerics.Vector2(mouse.X, mouse.Y);
            io.MouseWheel = mouse.ScrollDelta.Y;

            io.AddKeyEvent(ImGuiKey.ModCtrl, keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl));
            io.AddKeyEvent(ImGuiKey.ModShift, keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift));
            io.AddKeyEvent(ImGuiKey.ModAlt, keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt));
        }

        private void CreateDeviceObjects()
        {
            _vertexBufferSize = 10000;
            _indexBufferSize = 2000;

            _vertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArray);

            _vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            _indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            RecreateFontDeviceTexture();

            string vertexSource = @"
                #version 330 core
                layout(location = 0) in vec2 in_position;
                layout(location = 1) in vec2 in_texCoord;
                layout(location = 2) in vec4 in_color;
                uniform mat4 projection_matrix;
                out vec2 texCoord;
                out vec4 color;
                void main()
                {
                    gl_Position = projection_matrix * vec4(in_position, 0, 1);
                    texCoord = in_texCoord;
                    color = in_color;
                }";

            string fragmentSource = @"
                #version 330 core
                in vec2 texCoord;
                in vec4 color;
                uniform sampler2D in_fontTexture;
                out vec4 outputColor;
                void main()
                {
                    outputColor = color * texture(in_fontTexture, texCoord);
                }";

            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertexSource);
            GL.CompileShader(vs);

            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragmentSource);
            GL.CompileShader(fs);

            _shader = GL.CreateProgram();
            GL.AttachShader(_shader, vs);
            GL.AttachShader(_shader, fs);
            GL.LinkProgram(_shader);
            GL.DetachShader(_shader, vs);
            GL.DetachShader(_shader, fs);
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            _shaderProjectionMatrixLocation = GL.GetUniformLocation(_shader, "projection_matrix");
            _shaderFontTextureLocation = GL.GetUniformLocation(_shader, "in_fontTexture");

            int stride = 20;
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 8);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void RecreateFontDeviceTexture()
        {
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

            _fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            io.Fonts.SetTexID((IntPtr)_fontTexture);
            io.Fonts.ClearTexData();
        }

        private void RenderImDrawData(ImDrawDataPtr drawData)
        {
            if (drawData.CmdListsCount == 0) return;

            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);

            var io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(0, io.DisplaySize.X, io.DisplaySize.Y, 0, -1, 1);

            GL.UseProgram(_shader);
            GL.Uniform1(_shaderFontTextureLocation, 0);
            GL.UniformMatrix4(_shaderProjectionMatrixLocation, false, ref mvp);
            GL.BindVertexArray(_vertexArray);

            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            for (int i = 0; i < drawData.CmdListsCount; i++)
            {
                var cmdList = drawData.CmdLists[i];

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer,
                    cmdList.VtxBuffer.Size * 20,
                    cmdList.VtxBuffer.Data, BufferUsageHint.StreamDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
                GL.BufferData(BufferTarget.ElementArrayBuffer,
                    cmdList.IdxBuffer.Size * sizeof(ushort),
                    cmdList.IdxBuffer.Data, BufferUsageHint.StreamDraw);

                for (int j = 0; j < cmdList.CmdBuffer.Size; j++)
                {
                    var cmd = cmdList.CmdBuffer[j];
                    GL.BindTexture(TextureTarget.Texture2D, (int)cmd.TextureId);
                    GL.Scissor((int)cmd.ClipRect.X,
                        _windowHeight - (int)cmd.ClipRect.W,
                        (int)(cmd.ClipRect.Z - cmd.ClipRect.X),
                        (int)(cmd.ClipRect.W - cmd.ClipRect.Y));
                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles,
                        (int)cmd.ElemCount, DrawElementsType.UnsignedShort,
                        (IntPtr)(cmd.IdxOffset * sizeof(ushort)),
                        (int)cmd.VtxOffset);
                }
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vertexArray);
            GL.DeleteBuffer(_vertexBuffer);
            GL.DeleteBuffer(_indexBuffer);
            GL.DeleteTexture(_fontTexture);
            GL.DeleteProgram(_shader);
        }
    }
}