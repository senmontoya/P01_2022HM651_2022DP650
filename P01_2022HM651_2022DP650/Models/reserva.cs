namespace P01_2022HM651_2022DP650.Models
{
    public class reserva
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EspacioParqueoId { get; set; }
        public DateTime FechaReserva { get; set; }
        public int CantidadHoras { get; set; }
        public string Estado { get; set; }
        public decimal CostoTotal { get; set; }
        public usuario Usuario { get; set; }
        public espacioparqueo EspacioParqueo { get; set; }
    }
}
