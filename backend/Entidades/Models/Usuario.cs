﻿using Entidades.Interfaces;

namespace Entidades.Models
{
    public partial class Usuario : IEntity
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public long UsuarioImagem { get; set; }

        public virtual UsuarioImagem IdNavigation { get; set; }
    }
}