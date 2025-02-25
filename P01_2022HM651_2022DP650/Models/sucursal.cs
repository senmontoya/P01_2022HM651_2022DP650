namespace P01_2022HM651_2022DP650.Models
{
    public class sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public int? AdministradorId { get; set; }
        public int NumEspaciosDisponibles { get; set; }

        public usuario Administrador { get; set; }
    }
}
