namespace Iglesia_Bautista_Centro_Familiar_NET.CORE
{
    public class ProductoService  // Asegúrate de que está dentro de una clase
    {
        // Método que convierte el estado de string a bool?
        public bool? ConvertirEstado(string estado)
        {
            switch (estado.ToLower())
            {
                case "activo":
                    return true;  // Activo se convierte a true
                case "inactivo":
                    return false; // Inactivo se convierte a false
                case "en proceso":
                    return null;  // En Proceso se convierte a null
                default:
                    return null;  // Si no coincide, lo dejamos en null
            }
        }
    }
}
