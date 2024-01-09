using System;
using System.Collections.Generic;

// Se establecio el espacio de nombres
namespace ReactVentas.Models
{
    // Se establecio la clase Categoria
    public partial class Categoria
    {
        // Constructor de la clase Categoria
        public Categoria()
        {
            // Inicializa la colección de productos relacionados con esta categoría
            Productos = new HashSet<Producto>();
        }
        public int IdCategoria { get; set; }
        public string? Descripcion { get; set; }
        public bool? EsActivo { get; set; }
        public DateTime? FechaRegistro { get; set; }

        // Propiedad de navegación que representa la colección de productos asociados a esta categoría
        public virtual ICollection<Producto> Productos { get; set; }
    }
}