using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class Salida
    {
        public int IdSalida { get; set; }
        public int? IdProducto { get; set; }
        public int? Cantidad { get; set; }
        public byte[]? ArchivoPdf { get; set; }

        public virtual Producto? IdProductoNavigation { get; set; }
    }
}
