using System.ComponentModel.DataAnnotations;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Base_Dato_DTO
{
    public class SalidaDTO
    {
        [Required]
        public int? IdProducto { get; set; }

        [Required]
        public int? Cantidad { get; set; }

        // Los campos opcionales, pueden ser nulos
        public string Color { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public string TipoProducto { get; set; }
        public string NombreProducto { get; set; }

        public IFormFile ArchivoPdf { get; set; }
    }

}
