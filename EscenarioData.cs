using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetraU
{
    public class EscenarioData
    {
        public string nombre { get; set; }
        public Dictionary<string, ParteData> partes { get; set; }
    }

    public class ParteData
    {
        public List<List<List<float>>> poligonos { get; set; }
    }
}
