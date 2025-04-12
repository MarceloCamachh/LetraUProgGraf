using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetraU
{
    using System;
    using System.Collections.Generic;
    using OpenTK;

    namespace LetraU
    {
        public class Escenario
        {
            private Vector3 centro;
            private Dictionary<string, Objeto> listaDeObjetos;

            public Escenario(Dictionary<string, Objeto> list, Vector3 centro)
            {
                this.listaDeObjetos = list;
                this.centro = centro;
            }

            public void AddObjeto(string nombre, Objeto nuevoObjeto)
            {
                this.listaDeObjetos.Add(nombre, nuevoObjeto);
            }

            public Objeto GetObjeto(string nombre)
            {
                return this.listaDeObjetos[nombre];
            }

            public Vector3 GetCentro()
            {
                return this.centro;
            }

            public void SetCentro(Vector3 centro)
            {
                this.centro = centro;
                foreach (var objeto in listaDeObjetos.Values)
                {
                    objeto.SetCentro(centro);
                }
            }

            public void Draw()
            {
                foreach (var objeto in this.listaDeObjetos.Values)
                {
                    objeto.Draw();
                }
            }

            public Vector3 CalcularCentroMasa()
            {
                Vector3 sumaCentros = Vector3.Zero;
                foreach (var objeto in listaDeObjetos.Values)
                {
                    sumaCentros += objeto.CalcularCentroMasa();
                }

                if (listaDeObjetos.Count > 0)
                    sumaCentros /= listaDeObjetos.Count;

                return sumaCentros;
            }

           
        }
    }

}
