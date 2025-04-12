using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetraU;

namespace TareaU
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Game(800, 640)) // Crea una instancia de la clase Game
            {
                game.Run(60.0); // Ejecuta la aplicación de juego
            }
        }
    }
}
