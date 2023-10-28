using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppGestionProyectos.Models
{
    public class EditarTarea
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo ID es obligatorio.")]
        public int id { get; set; }
        public string nuevaDescripcion { get; set; }
        public DateTime nuevaFechaInicio { get; set; }
        public DateTime nuevaFechaVencimiento { get; set; }
        public int nuevoCreador { get; set; }
        public string nuevoComentario { get; set; }

    }
}