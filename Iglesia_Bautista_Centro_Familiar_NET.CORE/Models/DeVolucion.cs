using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class DeVolucion
    {
        public int IdDeVolucion { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public int? IdAprovacionDeSolicitudPrestamo { get; set; }
        public string? Observacion { get; set; }
        public int? Cantidad { get; set; }
        public byte[]? ArchivoPdf { get; set; }

        public virtual AprovacionDeSolicitudPrestamo? IdAprovacionDeSolicitudPrestamoNavigation { get; set; }
    }
}
