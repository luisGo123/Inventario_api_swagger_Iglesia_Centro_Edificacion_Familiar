using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class AprovacionDeSolicitudPrestamo
    {
        public AprovacionDeSolicitudPrestamo()
        {
            DeVolucions = new HashSet<DeVolucion>();
        }

        public int IdAprovacionDeSolicitudPrestamo { get; set; }
        public int? IdSolicitudDePrestamo { get; set; }
        public DateTime? FechaAprovacion { get; set; }
        public int? Cantidad { get; set; }
        public string? Observacion { get; set; }

        public virtual SolicitudDePrestamo? IdSolicitudDePrestamoNavigation { get; set; }
        public virtual ICollection<DeVolucion> DeVolucions { get; set; }
    }
}
