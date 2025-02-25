using System.ComponentModel.DataAnnotations;

namespace P01_2022HM651_2022DP650.Models
{
    public class espacioparqueo
    {
        [Key]
        public int Id { get; set; }
        public int SucursalId { get; set; }
        public string Numero { get; set; }
        public string Ubicacion { get; set; }
        public decimal CostoPorHora { get; set; }
        public string Estado { get; set; }

        public sucursal Sucursal { get; set; }
    }
}
