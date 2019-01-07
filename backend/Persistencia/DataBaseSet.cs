﻿using Entidades.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistencia
{
    public partial class DataBaseContext : DbContext
    {
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<UsuarioImagem> UsuarioImagem { get; set; }
    }
}