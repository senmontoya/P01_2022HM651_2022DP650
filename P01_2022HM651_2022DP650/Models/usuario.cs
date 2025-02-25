using System.ComponentModel.DataAnnotations;

namespace P01_2022HM651_2022DP650.Models
{
    public class usuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }    
        public string Contraseña { get; set; }
        public string rol { get; set; }
    }
}
