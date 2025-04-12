using LetraU.LetraU;
using LetraU;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace LetraU
{
    public class Game : GameWindow
    {
        private Escenario escenario;

        public Game(int width, int height)
            : base(width, height, GraphicsMode.Default, "Tarea Letra U")
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.0f, 0.0f, 0.5f, 1.0f);
            var objetos = new Dictionary<string, Objeto>();
            var objU = CargarObjetoDesdeJSON("letraU.json", new Vector3(-2f, 0f, 0f));
            objetos.Add("U1", objU);
            escenario = new Escenario(objetos, new Vector3(0, 0, 0));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            float aspectRatio = (float)Width / Height;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f), aspectRatio, 0.1f, 100.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            Matrix4 modelview = Matrix4.LookAt(
                new Vector3(1.5f, 2f, 3.5f),  // Cámara fija
                new Vector3(0.0f, 0.1f, 0.0f), // Mira al origen
                Vector3.UnitY);
            GL.LoadMatrix(ref modelview);

            escenario.Draw();

            DibujarEjes();
            SwapBuffers();
        }

        private Dictionary<string, Objeto> GenerarEscenario()
        {
            var objetos = new Dictionary<string, Objeto>();
            var objetoU = GenerarLetraU(new Vector3(-2f, 0f, 0f));
            objetos.Add("U1", objetoU);
            return objetos;
        }

        private Objeto GenerarLetraU(Vector3 posicion)
        {
            var objeto = new Objeto(new Dictionary<string, Parte>(), posicion);
            Color4 color = new Color4(0.5f, 0.5f, 0.5f, 1f);

            objeto.AddParte("izquierda", CrearCaja(-0.8f, -0.8f, 0.3f, -0.4f, 1.2f, -0.3f, color));
            objeto.AddParte("derecha", CrearCaja(0.4f, -0.8f, 0.3f, 0.8f, 1.2f, -0.3f, color));
            objeto.AddParte("base", CrearCaja(-0.8f, -1.2f, 0.3f, 0.8f, -0.8f, -0.3f, color));

            return objeto;
        }

        private Parte CrearCaja(float x1, float y1, float z1, float x2, float y2, float z2, Color4 color)
        {
            Parte parte = new Parte();

            Vector3[] v = new Vector3[8];
            v[0] = new Vector3(x1, y2, z1);
            v[1] = new Vector3(x2, y2, z1);
            v[2] = new Vector3(x2, y1, z1);
            v[3] = new Vector3(x1, y1, z1);
            v[4] = new Vector3(x1, y2, z2);
            v[5] = new Vector3(x2, y2, z2);
            v[6] = new Vector3(x2, y1, z2);
            v[7] = new Vector3(x1, y1, z2);

            parte.Add("frente", CrearCara(v[0], v[1], v[2], v[3], color));
            parte.Add("atras", CrearCara(v[4], v[5], v[6], v[7], color));
            parte.Add("izquierda", CrearCara(v[0], v[4], v[7], v[3], color));
            parte.Add("derecha", CrearCara(v[1], v[5], v[6], v[2], color));
            parte.Add("superior", CrearCara(v[0], v[1], v[5], v[4], color));
            parte.Add("inferior", CrearCara(v[3], v[2], v[6], v[7], color));

            return parte;
        }

        private Poligono CrearCara(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color4 color)
        {
            var cara = new Poligono(color);
            cara.Add(a);
            cara.Add(b);
            cara.Add(c);
            cara.Add(d);
            return cara;
        }

        private void DibujarEjes()
        {
            GL.Begin(PrimitiveType.Lines);

            GL.Color3(1.0f, 0.0f, 0.0f); // X
            GL.Vertex3(-2.0f, 0.0f, 0.0f);
            GL.Vertex3(2.0f, 0.0f, 0.0f);

            GL.Color3(0.0f, 1.0f, 0.0f); // Y
            GL.Vertex3(0.0f, -2.0f, 0.0f);
            GL.Vertex3(0.0f, 2.0f, 0.0f);

            GL.Color3(0.0f, 0.0f, 1.0f); // Z
            GL.Vertex3(0.0f, 0.0f, -2.0f);
            GL.Vertex3(0.0f, 0.0f, 2.0f);

            GL.End();
        }
     private Objeto CargarObjetoDesdeJSON(string path, Vector3 posicion)
        {
            try
            {
                Console.WriteLine("Buscando archivo en: " + Path.GetFullPath(path));
                string json = File.ReadAllText(path);
                EscenarioData data = JsonConvert.DeserializeObject<EscenarioData>(json);

                var objeto = new Objeto(new Dictionary<string, Parte>(), posicion);
            Color4 color = new Color4(0.5f, 0.5f, 0.5f, 1.0f);

            foreach (var parteKV in data.partes)
            {
                Parte parte = new Parte();
                int index = 0;
                foreach (var poligono in parteKV.Value.poligonos)
                {
                    Poligono cara = new Poligono(color);
                    foreach (var punto in poligono)
                    {
                        cara.Add(new Vector3(punto[0], punto[1], punto[2]));
                    }
                    parte.Add($"p{index++}", cara);
                }
                objeto.AddParte(parteKV.Key, parte);
            }

            return objeto;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"❌ Archivo no encontrado: {path}");
                return new Objeto(new Dictionary<string, Parte>(), posicion);
            }
        }
    } 
}
