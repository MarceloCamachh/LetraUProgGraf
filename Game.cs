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
        private Parte parteSeleccionada = null;                                     //MOVER OBJETO 
        private List<Parte> partesDisponibles = new List<Parte>();
        private int indiceParte = 0;
        private bool moverTodo = false; // cambia con la tecla T

        private bool moverObjeto = false;
        private List<Objeto> objetosDisponibles = new List<Objeto>();               //MOVER OBJETO
        private int indiceObjeto = 0;
        private Objeto objetoSeleccionado = null;


        public static Parte ParteSeleccionadaGlobal { get; private set; }
        public static bool ModoEscenarioActivo { get; private set; }

        public static bool ModoObjetoActivo { get; private set; }
        public static Objeto ObjetoSeleccionadoGlobal { get; private set; }

        public Game(int width, int height)
            : base(width, height, GraphicsMode.Default, "Tarea Letra U")
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.0f, 0.0f, 0.5f, 1.0f);
            var objetos = new Dictionary<string, Objeto>();
            var objU = CargarObjetoDesdeJSON("letraU.json", new Vector3(-1f, 0f, 0f));
            objetos.Add("U1", objU);
            var objU2 = CargarObjetoDesdeJSON("letraU.json", new Vector3(1f, 0f, 0f)); // misma forma, diferente posición
            objetos.Add("U2", objU2);
           
            escenario = new Escenario(objetos, new Vector3(0, 0, 0));

            partesDisponibles = new List<Parte>();
            // Recorremos todos los objetos y agregamos sus partes a la lista
            foreach (var obj in escenario.GetObjetos())
            {
                partesDisponibles.AddRange(obj.listaDePartes.Values);
            }

            parteSeleccionada = partesDisponibles[0];
            Console.WriteLine("Parte seleccionada: 0");


            objetosDisponibles = new List<Objeto>(escenario.GetObjetos());
            objetoSeleccionado = objetosDisponibles[0];
            Console.WriteLine("Objeto seleccionado: 0");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = Keyboard.GetState();
            ParteSeleccionadaGlobal = parteSeleccionada;
            ModoEscenarioActivo = moverTodo;
            ModoObjetoActivo = moverObjeto;
            ObjetoSeleccionadoGlobal = objetoSeleccionado;
            // Cambiar modo entre "Mover Todo" o "Parte Seleccionada"
            if (input.IsKeyDown(Key.T))
            {
                moverTodo = !moverTodo;
                Console.WriteLine(moverTodo ? "🔁 Modo ESCENARIO activado" : "🎯 Modo PARTE seleccionada");
                System.Threading.Thread.Sleep(200);
            }

            // Cambiar la parte seleccionada solo si estás en MODO PARTE
            if (!moverTodo && input.IsKeyDown(Key.P))
            {
                indiceParte = (indiceParte + 1) % partesDisponibles.Count;
                parteSeleccionada = partesDisponibles[indiceParte];
                Console.WriteLine($"Parte seleccionada: {indiceParte}");
                System.Threading.Thread.Sleep(200);
            }

            if (moverTodo)
            {
                // 🔹 Transformar TODOS los polígonos de todos los objetos
                foreach (var obj in escenario.GetObjetos())
                {
                    foreach (var parte in obj.listaDePartes.Values)
                    {
                        foreach (var poligono in parte.listaDePoligonos.Values)
                        {
                            AplicarTransformaciones(poligono, input);
                        }
                    }
                }
            }
            else if (moverObjeto)
            {
                if (objetoSeleccionado != null)
                {
                    foreach (var parte in objetoSeleccionado.listaDePartes.Values)
                    {
                        foreach (var poligono in parte.listaDePoligonos.Values)
                        {
                            AplicarTransformaciones(poligono, input);
                        }
                    }
                }
            } else
            {
                // 🔹 Transformar SOLO la parte seleccionada
                if (parteSeleccionada != null)
                {
                    foreach (var poligono in parteSeleccionada.listaDePoligonos.Values)
                    {
                        AplicarTransformaciones(poligono, input);
                    }
                }
            }

            //eliminar parte seleccionada
            if (!moverTodo && input.IsKeyDown(Key.Delete))
            {
                foreach (var obj in escenario.GetObjetos())
                {
                    if (obj.listaDePartes.ContainsValue(parteSeleccionada))
                    {
                        string clave = null;
                        foreach (var kvp in obj.listaDePartes)
                        {
                            if (kvp.Value == parteSeleccionada)
                            {
                                clave = kvp.Key;
                                break;
                            }
                        }
                        if (clave != null)
                        {
                            obj.listaDePartes.Remove(clave);
                            partesDisponibles.Remove(parteSeleccionada);
                            Console.WriteLine($"❌ Parte '{clave}' eliminada");
                            indiceParte = 0;
                            parteSeleccionada = partesDisponibles.Count > 0 ? partesDisponibles[0] : null;
                        }
                        break;
                    }
                }
                System.Threading.Thread.Sleep(200);
            }

            //guardar objeto transformado
            if (input.IsKeyDown(Key.G))
            {
                GuardarEscenario("escenario_guardado.json");
                System.Threading.Thread.Sleep(300);
            }

            // Cambiar a modo Objeto
            if (input.IsKeyDown(Key.O))  
            {
                moverObjeto = !moverObjeto;
                moverTodo = false;
                Console.WriteLine(moverObjeto ? "🟥 Modo OBJETO activado" : "🎯 Modo PARTE activado");
                System.Threading.Thread.Sleep(200);
            }

            if (moverObjeto && input.IsKeyDown(Key.M))
            {
                indiceObjeto = (indiceObjeto + 1) % objetosDisponibles.Count;
                objetoSeleccionado = objetosDisponibles[indiceObjeto];
                Console.WriteLine($"🟦 Objeto seleccionado: {indiceObjeto}");
                System.Threading.Thread.Sleep(200);
            }
        }
        private void AplicarTransformaciones(Poligono poligono, KeyboardState input)
        {
            // Movimiento
            var pos = poligono.Posicion;
            if (input.IsKeyDown(Key.Left))
                pos.X -= 0.05f;
            if (input.IsKeyDown(Key.Right))
                pos.X += 0.05f;
            if (input.IsKeyDown(Key.Up))
                pos.Y += 0.05f;
            if (input.IsKeyDown(Key.Down))
                pos.Y -= 0.05f;
            if (input.IsKeyDown(Key.W))
                pos.Z -= 0.05f;
            if (input.IsKeyDown(Key.S))
                pos.Z += 0.05f;
            poligono.Posicion = pos;

            // Rotaciones
            if (input.IsKeyDown(Key.R))
                poligono.RotacionY += 2f;
            if (input.IsKeyDown(Key.X))
                poligono.RotacionX += 2f;
            if (input.IsKeyDown(Key.Z))
                poligono.RotacionZ += 2f;



            // Escalado
            if (input.IsKeyDown(Key.Plus) || input.IsKeyDown(Key.KeypadPlus))
                poligono.Escala *= 1.05f;

            if (input.IsKeyDown(Key.Minus) || input.IsKeyDown(Key.KeypadMinus))
                poligono.Escala *= 0.95f;


            // Resetear transformaciones
            if (input.IsKeyDown(Key.L))
            {
                poligono.Posicion = Vector3.Zero;
                poligono.RotacionX = 0f;
                poligono.RotacionY = 0f;
                poligono.RotacionZ = 0f;
                poligono.Escala = Vector3.One;
                poligono.color = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
                //Console.WriteLine("🔄 Transformaciones reseteadas");
                // System.Threading.Thread.Sleep(200);
            }

            // Cambiar color
            if (input.IsKeyDown(Key.C))
            {
                Random rnd = new Random();
                poligono.SetColor(new Color4(
                    (float)rnd.NextDouble(),
                    (float)rnd.NextDouble(),
                    (float)rnd.NextDouble(),
                    1.0f
                ));
                Console.WriteLine("🎨 Color cambiado aleatoriamente");
                //System.Threading.Thread.Sleep(200);
            }
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
        private void GuardarEscenario(string ruta)
        {
            var exportData = new Dictionary<string, List<PoligonoDataExport>>();

            foreach (var objeto in escenario.GetObjetos())
            {
                foreach (var parte in objeto.listaDePartes.Values)
                {
                    foreach (var poligono in parte.listaDePoligonos.Values)
                    {
                        var poligonoExport = new PoligonoDataExport
                        {
                            Vertices = poligono.listaDeVertices.ConvertAll(v => new Vector3Serializable(v)),
                            Posicion = new Vector3Serializable(poligono.Posicion),
                            Escala = new Vector3Serializable(poligono.Escala),
                            RotacionX = poligono.RotacionX,
                            RotacionY = poligono.RotacionY,
                            RotacionZ = poligono.RotacionZ,
                            Color = $"{poligono.color.R},{poligono.color.G},{poligono.color.B},{poligono.color.A}"
                        };

                        string claveParte = parte.GetHashCode().ToString();  // O usa el nombre si tienes

                        if (!exportData.ContainsKey(claveParte))
                            exportData[claveParte] = new List<PoligonoDataExport>();

                        exportData[claveParte].Add(poligonoExport);
                    }
                }
            }

            string json = JsonConvert.SerializeObject(exportData, Formatting.Indented);
            File.WriteAllText(ruta, json);
            Console.WriteLine($"✅ Escenario guardado en: {ruta}");
        }


    }
}
