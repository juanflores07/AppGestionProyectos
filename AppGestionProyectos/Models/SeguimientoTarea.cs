using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppGestionProyectos.Models
{
    public class SeguimientoTarea
    {
        public int TareaID { get; set; }
        public int TiempoInvertido { get; set; }
        public DateTime FechaInicio { get; set; }
        public string Descripcion { get; set; }
        public bool Realizada { get; set; }
        public string Nombre { get; set; }
    }
}