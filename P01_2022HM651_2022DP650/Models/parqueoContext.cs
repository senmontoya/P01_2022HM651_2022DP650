﻿using Microsoft.EntityFrameworkCore;
namespace P01_2022HM651_2022DP650.Models
{
    public class parqueoContext : DbContext
    {
        public parqueoContext(DbContextOptions<parqueoContext> options) : base(options)
        {

        }

        public DbSet<usuario> usuarios { get; set; }
        public DbSet<sucursal> sucursales { get; set; }
        public DbSet<espacioparqueo> espaciosparqueo { get; set; }
        public DbSet<reserva> reservas { get; set; }
    }
}
