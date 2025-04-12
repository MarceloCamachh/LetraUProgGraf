using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK;

namespace LetraU
{
    public class Parte
    {
        public Dictionary<string, Poligono> listaDePoligonos;
        private Vector3 centro;
        public Color4 color;

        public Parte()
        {
            this.listaDePoligonos = new Dictionary<string, Poligono>();
            this.color = new Color4(0, 0, 0, 0);
        }

        public void Add(string nombre, Poligono p)
        {
            this.listaDePoligonos.Add(nombre, p);
        }

        public Poligono GetPoligono(string nombre)
        {
            return this.listaDePoligonos[nombre];
        }

        public Vector3 GetCentro()
        {
            return this.centro;
        }

        public void SetCentro(Vector3 centro)
        {
            this.centro = centro;
            foreach (Poligono poligono in listaDePoligonos.Values)
            {
                poligono.SetCentro(centro);
            }
        }

        public void SetColor(string nombre, Color4 color)
        {
            this.color = color;
            listaDePoligonos[nombre].SetColor(this.color);
        }

        public Vector3 CalcularCentroMasa()
        {
            Vector3 suma = Vector3.Zero;
            foreach (var poligono in listaDePoligonos.Values)
            {
                suma += poligono.CalcularCentroMasa();
            }

            if (listaDePoligonos.Count > 0)
                suma /= listaDePoligonos.Count;

            return suma;
        }

        public void Draw()
        {
            foreach (var poligono in this.listaDePoligonos.Values)
            {
                poligono.Draw();
            }
        }

        public void Rotar(float grados, Vector3 eje)
        {
            throw new NotImplementedException("Rotación no implementada en Parte.");
        }
    }
}
