using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class Solicitante
    {
        public Solicitante()
        {
            SolicitudDePrestamos = new HashSet<SolicitudDePrestamo>();
        }

        public int IdSolicitante { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Cedula { get; set; }
        public string? Dirrecion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        public virtual ICollection<SolicitudDePrestamo> SolicitudDePrestamos { get; set; }
    }
}
