using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LetraU
{
    public class Objeto
    {
        public Dictionary<string, Parte> listaDePartes;
        private Vector3 centro;
        public Color4 color;
        public Vector3 Posicion { get; set; } = Vector3.Zero;
        public Vector3 Escala { get; set; } = Vector3.One;

        public Objeto(Dictionary<string, Parte> list, Vector3 centro)
        {
            this.listaDePartes = list;
            this.centro = centro;
            this.Posicion = centro;
        }

        public void AddParte(string nombre, Parte nuevaParte)
        {
            listaDePartes.Add(nombre, nuevaParte);
        }

        public Vector3 GetCentro()
        {
            return this.centro;
        }

        public void SetCentro(Vector3 centro)
        {
            this.centro = centro;
            foreach (Parte parteActual in listaDePartes.Values)
            {
                parteActual.SetCentro(centro);
            }
        }

        public void SetColor(string parte, string poligono, Color4 color)
        {
            this.color = color;
            listaDePartes[parte].SetColor(poligono, this.color);
        }

        public Parte GetParte(string nombre)
        {
            return this.listaDePartes[nombre];
        }

        public void Draw()
        {
            GL.PushMatrix();
            GL.Translate(Posicion);

            foreach (var parte in listaDePartes.Values)
            {
                parte.Draw();
            }

            GL.PopMatrix();
        }

        public Vector3 CalcularCentroMasa()
        {
            Vector3 suma = Vector3.Zero;
            foreach (var parte in listaDePartes.Values)
            {
                suma += parte.CalcularCentroMasa();
            }

            if (listaDePartes.Count > 0)
                suma /= listaDePartes.Count;

            return suma;
        }
    }
}
