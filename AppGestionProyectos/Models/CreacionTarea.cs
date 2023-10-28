using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace AppGestionProyectos.Models
{
    public class CreacionTarea
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El nombre de la tarea es obligatorio.")]
        public string NombreTarea { get; set; }

        [DisplayName("Descripción")]
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; }

        [DisplayName("Fecha de Inicio")]
        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de Vencimiento")]
        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El creador es obligatorio.")]
        public int Creador { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio.")]
        public string Comentario { get; set; }

        public string Asignado { get; set; }

    }
}