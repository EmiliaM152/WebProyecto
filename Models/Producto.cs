using System;
using System.Collections.Generic;

namespace ReactVentas.Models
{
    public partial class Producto
    {
        public Producto()
        {
            DetalleVenta = new HashSet<DetalleVenta>();
        }

        public int IdProducto { get; set; }
        public string? Codigo { get; set; }
        public string? Marca { get; set; }
        public string? Descripcion { get; set; }
        public int? IdCategoria { get; set; }
        public int? Stock { get; set; }
        public decimal? Precio { get; set; }
        public bool? EsActivo { get; set; }
        public DateTime? FechaRegistro { get; set; }

        // Propiedad de navegación para la categoría asociada al producto
        public virtual Categoria? IdCategoriaNavigation { get; set; }

        // Propiedad para la colección de detalles de venta del producto
        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; }
    }
}