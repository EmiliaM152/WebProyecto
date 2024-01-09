using System;
using System.Collections.Generic;

namespace ReactVentas.Models
{
    public partial class Rol
    {
        public Rol()
        {
            Usuarios = new HashSet<Usuario>();
        }

        public int IdRol { get; set; }
        public string? Descripcion { get; set; }
        public bool? EsActivo { get; set; }
        public DateTime? FechaRegistro { get; set; }

        // Obtiene o establece la colección de usuarios asociados al rol.
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}