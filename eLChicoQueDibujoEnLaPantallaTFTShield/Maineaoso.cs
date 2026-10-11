using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.IO;

// ESPERO QUE ESTA WEBADA SEA UTIL PORQUE ME MATE A PAJAS HACIENDOLO.lower() ):

namespace eLChicoQueDibujoEnLaPantallaTFTShield
{
    public class Maineaoso : GameWindow
    {
        Vector4 my_color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        int frafoseado1 = -1;
        Vector2 posincialdelfrafoso;
        Vector2 posicionFinalFRado;
        Vector2 Debugeano;
        Vector2 Debugeano2;
        bool asd = false;
        bool dsa = false; 
        bool mostrarExportar = false;
        bool Utilidades = true;
        bool gapa = true;
        string nigga = "";
        string nigga2 = "";
        bool fifas = false;
        int selecio = -1;
        private List<DibujosRicolinos> dibujo = new List<DibujosRicolinos>();
        private ImGuiController _controller;

        public Maineaoso(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        protected override void OnLoad()
        {
            base.OnLoad();
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            //dibujo.Add(new DibujosRicolinos("Rectangulo", new Vector2(100f, 100f), new Vector2(100f, 100f), 5, new Vector4(0f, 0f, 0f, 0f)));
            //esto es el pasado oiste viejo
            Console.WriteLine("Para Exportar se presiona la tecla E");
            Console.WriteLine("Los numeros del teclado sirven como atajo para el Cuadrado, Circulo (no funciona), Pincel, Goma");
            Console.WriteLine("Todo se exporta al Escritorio");
            Console.WriteLine("Al Seleccionar un objecto con el circulo verde se puede mover y con el rojo escalar");
            Console.WriteLine("En propiedades puedes cambiar las capas, esto tambien se ve reflejado en la pantalla cuando se exporta");
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            ImGui.GetIO().AddInputCharacter((uint)e.Unicode);
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backspace)
            {
                if (selecio != -1)
                {
                    dibujo.RemoveAt(selecio - 1);
                    selecio = -1;
                }
            }
            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space)
            {
                fifas = false;
                selecio = -1;
            }
            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.E)
            {
                mostrarExportar = true;
            }
            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.D1)
            {
                frafoseado1 = frafoseado1 == 1 ? -1 : 1;
            }
            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.D2)
            {
                frafoseado1 = frafoseado1 == 2 ? -1 : 2;
            }
            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.D3)
            {
                frafoseado1 = frafoseado1 == 3 ? -1 : 3;
            }
            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.D4)
            {
                frafoseado1 = frafoseado1 == 4 ? -1 : 4;
            }
            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.D5)
            {
                frafoseado1 = frafoseado1 == 5 ? -1 : 5;
            }
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);
            base.OnRenderFrame(e);
            GL.ClearColor(0.1f, 0.1f, 0.11f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);
            Vector2 screenSize = ImGui.GetIO().DisplaySize; //no me acordaba que habia que declarar estas cosas :v
            _controller.Update(this, (float)e.Time);
            ImGui.SetNextWindowBgAlpha(0.85f);
            //------------------------------------Exportar----------------------------------------------

            if (mostrarExportar)
            {
                ImGui.Begin("Exportar", ref mostrarExportar, ImGuiWindowFlags.AlwaysAutoResize);
                if (ImGui.InputText("Nombre del dibujito (nombre de la funcion)", ref nigga, 100)) 
                {
                    //Console.WriteLine(nigga);
                }
                if (ImGui.InputText("Nombre de la pantalla", ref nigga2, 100))
                {
                    //Console.WriteLine(nigga2);
                }
                if (ImGui.Button("  EXPORTAR  "))
                {
                    Exportar(nigga, nigga2);
                    mostrarExportar = false;
                }
                ImGui.End();
            }
            //---------------------------CAPAS----------------------------------------------- este es el menu de capas, entendieron el menu de capas capas capas capas capas

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(screenSize.X - 310f, 50f));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(300f, 800));
            ImGui.Begin("Capas", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
            int i = 0;
            ListaEnOrdenDeCapas();
            foreach (var manuel in dibujo)
            {
                i++;
                bool tempselecio = selecio == i;
                if (ImGui.Selectable(manuel.capa + ". " + manuel.Forma + ":" + manuel.color, tempselecio))
                {
                    selecio = selecio == i ? -1 : i;
                }
            }
            ImGui.End();

            //---------------------------CAJA DE HERRAMIENTAS----------------------------------------------- es el menu donde seleccionas la herramienta que vas a usar entendierion es un menu con herramientas menu con herramientas menu con herramientas

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(400, 30f));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(screenSize.X - 800f, 40f));
            ImGui.Begin("heramientas", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.MenuBar);
            if (ImGui.BeginMenuBar())
            {
                ImGui.SetWindowFontScale(2.5f);
                ImGui.Dummy(new System.Numerics.Vector2(80f, 5f));
                if (ImGui.Selectable("mouse", frafoseado1 == 1, ImGuiSelectableFlags.None, new Vector2(80f, 60f)))
                {
                    frafoseado1 = frafoseado1 == 1 ? -1 : 1;
                }
                ImGui.Dummy(new System.Numerics.Vector2(100f, 5f));
                ImGui.Spacing();
                if (ImGui.Selectable("[]", frafoseado1 == 2, ImGuiSelectableFlags.None, new Vector2(40f, 40f)))
                {
                    frafoseado1 = frafoseado1 == 2 ? -1 : 2;
                }
                ImGui.Spacing();
                if (ImGui.Selectable("O", frafoseado1 == 3, ImGuiSelectableFlags.None, new Vector2(40f, 40f)))
                {
                    frafoseado1 = frafoseado1 == 3 ? -1 : 3;
                }
                ImGui.Dummy(new System.Numerics.Vector2(100f, 5f));
                Utilidades = true;
                if (ImGui.ColorEdit4("Color", ref my_color, ImGuiColorEditFlags.NoAlpha | ImGuiColorEditFlags.NoInputs)) //xd saque el ejemplo de la doc oficial de c++
                {
                    frafoseado1 = 1;
                    if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                    {
                        gapa = true;
                    }
                    //Console.WriteLine("negro de Sprinfield"); no funciona xddd
                }else 
                {
                    gapa = false;
                    //Console.WriteLine("blancos de Sprinfield");
                }
                ImGui.Dummy(new System.Numerics.Vector2(25f, 5f));
                if (ImGui.Selectable("pincel", frafoseado1 == 4, ImGuiSelectableFlags.None, new Vector2(40f, 40f))) 
                {
                    frafoseado1 = frafoseado1 == 4 ? -1 : 4;
                }
                ImGui.Dummy(new System.Numerics.Vector2(25f, 5f));
                if (ImGui.Selectable("Gm", frafoseado1 == 5, ImGuiSelectableFlags.None, new Vector2(40f, 40f)))
                {
                    frafoseado1 = frafoseado1 == 5 ? -1 : 5;
                }
                ImGui.Dummy(new System.Numerics.Vector2(100f, 5f));
                ImGui.EndMenuBar();
            }
            ImGui.End();
            //----------------------------LIENZO-------------------------------------------- No voy a seguir con el chiste
            ImGui.SetNextWindowPos(new System.Numerics.Vector2((screenSize.X - 480 * 2) / 2, (screenSize.Y - 320 * 2) / 2));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(480 * 2, 320 * 2));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new System.Numerics.Vector4(0.073f * 4, 0.079f * 4, 0.179f * 4, 1f));
            ImGui.PushStyleColor(ImGuiCol.Border, new System.Numerics.Vector4(0.9f, 0.9f, 0.9f, 1f));
            ImGui.Begin("Lienzo", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
            var dibuha = ImGui.GetWindowDrawList();
            foreach (var manuel in dibujo.OrderBy(d => d.capa).ToList())
            {
                if (manuel == null) continue;
                if (manuel.Forma == "Rectangulo")
                {
                    dibuha.AddRectFilled(manuel.posicioninicial, manuel.posicioninicial + manuel.tamano, ImGui.ColorConvertFloat4ToU32(manuel.color));
                    if (selecio == dibujo.IndexOf(manuel) + 1)
                    {
                        dibuha.AddRect(manuel.posicioninicial, manuel.posicioninicial + manuel.tamano, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)), 0f, ImDrawFlags.None, 3f);
                        dibuha.AddRectFilled(manuel.posicioninicial, manuel.posicioninicial + manuel.tamano, ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 1f, 0.3f)));
                        if(asd)
                        {
                            dibuha.AddRect(Debugeano2 - manuel.tamano / 2, Debugeano2 + manuel.tamano - manuel.tamano / 2, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)), 0f, ImDrawFlags.None, 3f);
                        }
                        if (dsa) 
                        {
                            dibuha.AddRect(manuel.posicioninicial, Debugeano, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)), 0f, ImDrawFlags.None, 3f);
                        }
                    }
                }
                else if (manuel.Forma == "Circulo")
                {
                    dibuha.AddCircleFilled(manuel.posicioninicial, manuel.tamano.X / 2, ImGui.ColorConvertFloat4ToU32(manuel.color));
                }

                if (frafoseado1 == 1)
                {
                    Vector2 mou = new Vector2(ImGui.GetMousePos().X, ImGui.GetMousePos().Y);
                    if (selecio != -1) 
                    {
                        if (asd)
                        {
                            Debugeano2 = mou;
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                            {
                                dibujo[selecio - 1].posicioninicial = Debugeano2 - manuel.tamano / 2;
                            }
                        }
                        else if (dsa) 
                        {
                            Debugeano = mou;
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                            {
                                dibujo[selecio - 1].tamano = mou - dibujo[selecio - 1].posicioninicial;
                                if (dibujo[selecio - 1].tamano.X <= 0) 
                                {
                                    dibujo[selecio - 1].posicioninicial.X += dibujo[selecio - 1].tamano.X;
                                    dibujo[selecio - 1].tamano.X = -dibujo[selecio - 1].tamano.X;
                                    dsa = false;
                                }
                                if (dibujo[selecio - 1].tamano.Y <= 0)
                                {
                                    dibujo[selecio - 1].posicioninicial.Y += dibujo[selecio - 1].tamano.Y;
                                    dibujo[selecio - 1].tamano.Y = -dibujo[selecio - 1].tamano.Y;
                                    dsa = false;
                                }
                            }
                        }
                        dibuha.AddCircleFilled(new Vector2(dibujo[selecio - 1].posicioninicial.X + dibujo[selecio - 1].tamano.X / 2, dibujo[selecio - 1].posicioninicial.Y + dibujo[selecio - 1].tamano.Y / 2),12f, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 1, 0, 1)));
                        dibuha.AddCircleFilled(new Vector2(dibujo[selecio - 1].posicioninicial.X + dibujo[selecio - 1].tamano.X, dibujo[selecio - 1].posicioninicial.Y + dibujo[selecio - 1].tamano.Y), 8f, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0, 0, 1)));
                        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                        {
                            Vector2 pelotita = new Vector2(dibujo[selecio - 1].posicioninicial.X + dibujo[selecio - 1].tamano.X / 2, dibujo[selecio - 1].posicioninicial.Y + dibujo[selecio - 1].tamano.Y / 2);
                            Vector2 pelotitaroja = new Vector2(dibujo[selecio - 1].posicioninicial.X + dibujo[selecio - 1].tamano.X, dibujo[selecio - 1].posicioninicial.Y + dibujo[selecio - 1].tamano.Y);
                            if (mou.X > pelotita.X - 12f && mou.X < pelotita.X + 12 && mou.Y > pelotita.Y - 12 && mou.Y < pelotita.Y + 12) 
                            {
                                //Console.WriteLine("negro de Springfield");
                                asd = !asd;
                            }else if (mou.X > pelotitaroja.X - 12f && mou.X < pelotitaroja.X + 12 && mou.Y > pelotitaroja.Y - 12 && mou.Y < pelotitaroja.Y + 12) 
                            {
                                //Console.WriteLine("blanco de Springfield");
                                dsa = !dsa;
                            }

                        } 
                    }
                }
            }
            if (fifas)
            {
                dibuha.AddRect(posincialdelfrafoso, Debugeano, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)));
            }
            ImGui.End();
            ImGui.PopStyleColor(2);
            //---------------------------PROPIEDADES-----------------------------------------------  
            if (selecio != -1)
            {
                ImGui.SetNextWindowPos(new System.Numerics.Vector2(50f, 50f));
                ImGui.SetNextWindowSize(new System.Numerics.Vector2(300f, 800));
                ImGui.Begin("Propiedades", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
                var manuel = dibujo[selecio - 1];
                if (manuel.Forma == "Rectangulo")
                {
                    ImGui.Spacing();
                    TextoCentrado("posicionIncial");
                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.SliderFloat("Posicion X", ref manuel.posicioninicial.X, 300, 1500);
                    ImGui.SliderFloat("Posicion Y", ref manuel.posicioninicial.Y, 0, 1000);
                    ImGui.Separator();
                    ImGui.Spacing();
                    TextoCentrado("tamaño");
                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.SliderFloat("Posicion X##tamano", ref manuel.tamano.X, 0, 1000);
                    ImGui.SliderFloat("Posicion Y##tamano", ref manuel.tamano.Y, 0, 1000);
                    ImGui.Separator();
                    ImGui.Spacing();
                    ImGui.InputInt("capa", ref manuel.capa, -1000, 1000);
                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                    if (ImGui.ColorEdit4("Color", ref manuel.color, ImGuiColorEditFlags.NoAlpha)) 
                    {
                        gapa = true;
                    }else 
                    {
                        gapa = false;
                    }
                    dibujo[selecio - 1] = manuel;
                }
                ImGui.End();
            }


            if (frafoseado1 == 2)
            {
                var mouPositio = new Vector2(ImGui.GetMousePos().X, ImGui.GetMousePos().Y);

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && Utilidades)
                {
                    if (mouPositio.X > (screenSize.X - 480 * 2) / 2 && mouPositio.X < ((screenSize.X - 480 * 2) / 2) + 480 * 2 && mouPositio.Y > (screenSize.Y - 320 * 2) / 2 && mouPositio.Y < ((screenSize.Y - 320 * 2) / 2) + 320 * 2)
                    {
                        fifas = !fifas;
                        //Console.WriteLine(fifas);

                        if (fifas)
                        {
                            posincialdelfrafoso = mouPositio;
                        }
                        else
                        {
                            posicionFinalFRado = mouPositio;
                            Vector2 esquinaReal = Vector2.Min(posincialdelfrafoso, posicionFinalFRado);
                            Vector2 tamanoReal = Vector2.Max(posincialdelfrafoso, posicionFinalFRado) - esquinaReal;
                            dibujo.Add(new DibujosRicolinos("Rectangulo", esquinaReal, tamanoReal, dibujo.Count + 1, my_color));
                        }
                    }
                }

                if (fifas)
                {
                    Debugeano = mouPositio;
                }
            }
            if (frafoseado1 == 1)
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !gapa)
                {
                    foreach (var manuel in dibujo)
                    {
                        if (manuel.Forma == "Rectangulo")
                        {
                            if (ImGui.GetMousePos().X > manuel.posicioninicial.X && ImGui.GetMousePos().X < manuel.posicioninicial.X + manuel.tamano.X && ImGui.GetMousePos().Y > manuel.posicioninicial.Y && ImGui.GetMousePos().Y < manuel.posicioninicial.Y + manuel.tamano.Y)
                            {
                                selecio = dibujo.IndexOf(manuel) + 1;
                                break;
                            }
                        }
                    }
                }
            }
            if (frafoseado1 == 4) 
            {
                if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && !gapa) 
                {
                    Vector2 mouPositio = ImGui.GetMousePos();
                    if (mouPositio.X > (screenSize.X - 480 * 2) / 2 && mouPositio.X < ((screenSize.X - 480 * 2) / 2) + 480 * 2 && mouPositio.Y > (screenSize.Y - 320 * 2) / 2 && mouPositio.Y < ((screenSize.Y - 320 * 2) / 2) + 320 * 2) 
                    {
                        dibujo.Add(new DibujosRicolinos("Rectangulo", new Vector2 (mouPositio.X - 5f, mouPositio.Y - 5f), new Vector2(10f, 10f), dibujo.Count + 1, my_color));
                        Thread.Sleep(5);
                    }
                }
            }
            if (frafoseado1 == 5) 
            {
                if(ImGui.IsMouseDown(ImGuiMouseButton.Left) && !gapa)
                {
                    foreach (var manuel in dibujo)
                    {
                        if (manuel.Forma == "Rectangulo")
                        {
                            if (ImGui.GetMousePos().X > manuel.posicioninicial.X && ImGui.GetMousePos().X < manuel.posicioninicial.X + manuel.tamano.X && ImGui.GetMousePos().Y > manuel.posicioninicial.Y && ImGui.GetMousePos().Y < manuel.posicioninicial.Y + manuel.tamano.Y)
                            {
                                int dsad = dibujo.IndexOf(manuel);
                                dibujo.Remove(dibujo[dsad]);
                                break;
                            }
                        }
                    }
                }
            }
            _controller.Render();
            SwapBuffers();
        }
        void ListaEnOrdenDeCapas()
        {
            DibujosRicolinos objSeleccionado = (selecio != -1 && selecio <= dibujo.Count) ? dibujo[selecio - 1] : null;
            dibujo = dibujo.OrderByDescending(d => d.capa).ToList();
                int i = 0;
                foreach (var manuel in dibujo)
                {
                    i++;
                    manuel.capa = dibujo.Count() - i + 1;
                }
            if (objSeleccionado != null)
            {
                selecio = dibujo.IndexOf(objSeleccionado) + 1;
            }
            else
            {
                selecio = -1;
            }
        }
        void Exportar(string funcio, string pantalla)
        {
            if (string.IsNullOrEmpty(funcio))
            {
                funcio = (Random.Shared.Next(1, 100)).ToString();
            }
            StringBuilder cv = new StringBuilder();
            cv.AppendLine("// Hola que rico es comer mortadela los lunes");
            cv.AppendLine("// copia esto deberia funcionar :v");
            cv.AppendLine("#include <Arduino.h>");
            cv.AppendLine("#include <Adafruit_GFX.h>");
            cv.AppendLine("#include <MCUFRIEND_kbv.h>");
            cv.AppendLine("");

            int i = 0;
            foreach (var manuel in dibujo)
            {
                i++;
                cv.AppendLine("#define " + i + "color " + Vector4ToRGB565Hex(manuel.color));
            }
            cv.AppendLine("void " + funcio + "() {");
            i = 0;
            foreach (var manuel in dibujo.OrderBy(d => d.capa).ToList())
            {
                i++;
                Vector2 screenSize = ImGui.GetIO().DisplaySize;
                float offsetX = (screenSize.X - 480 * 2) / 2f;
                float offsetY = (screenSize.Y - 320 * 2) / 2f;
                float xMapeada = (manuel.posicioninicial.X - offsetX) / 2f;
                float yMapeada = (manuel.posicioninicial.Y - offsetY) / 2f;
                float wMapeada = manuel.tamano.X / 2f;
                float hMapeada = manuel.tamano.Y / 2f;

                if (manuel.Forma == "Rectangulo")
                {
                    cv.AppendLine(pantalla + ".fillRect(" + (int)xMapeada + "," + (int)yMapeada + "," + (int)wMapeada + "," + (int)hMapeada + "," + i + "color" + ");");
                }
            }
            cv.AppendLine("}");
            string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string NueveDeOnce = Path.Combine(escritorio, funcio + ".cpp");
            File.WriteAllText(NueveDeOnce, cv.ToString());
        }

        public static string Vector4ToRGB565Hex(Vector4 color) // GEMINI NO SERVISTE PARA NADA HACE ALGO DECENTE CON ESTO (ayudo tantito)
        {
            int r = (int)(Math.Clamp(color.X, 0.0f, 1.0f) * 31.0f);
            int g = (int)(Math.Clamp(color.Y, 0.0f, 1.0f) * 63.0f);
            int b = (int)(Math.Clamp(color.Z, 0.0f, 1.0f) * 31.0f);

            ushort rgb565 = (ushort)((r << 11) | (g << 5) | b);
            return $"0x{rgb565:X4}";
        }
        void TextoCentrado(string texto)
        {
            float anchoTexto = ImGui.CalcTextSize(texto).X;
            ImGui.SetCursorPosX((ImGui.GetWindowSize().X - anchoTexto) / 2);
            ImGui.Text(texto);
        }
    }
}