using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class Entrada
    {
        public int IdEntrada { get; set; }
        public int? IdProducto { get; set; }
        public int? Cantidad { get; set; }
        public byte[]? ArchivoPdf { get; set; }

        [JsonIgnore]
        public virtual Producto? IdProductoNavigation { get; set; }
    }
}
