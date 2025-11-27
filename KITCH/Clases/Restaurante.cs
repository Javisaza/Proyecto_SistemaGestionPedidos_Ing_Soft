using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KITCH.Modelos
{
    // Clase simple para contener los datos del restaurante seleccionado.
    public class Restaurante
    {
        public int IdRestaurante { get; set; }
        public string Nombre { get; set; }
        // Se pueden añadir más propiedades según lo requiera la base de datos:
        // public string Direccion { get; set; }
        // public string Telefono { get; set; }
        // public int CapacidadMesas { get; set; }
    }
}