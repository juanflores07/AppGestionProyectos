using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppGestionProyectos.Models
{
    public class DetalleTarea
    {
        public int TareaID { get; set; }

        public bool Realizada { get; set; }

        public int TiempoInvertido { get; set; }

        public DateTime FechaInicio { get; set; }
    }
}