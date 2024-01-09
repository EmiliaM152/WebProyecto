using System;
using System.Collections.Generic;

namespace ReactVentas.Models
{
    public partial class DetalleVenta
    {
        public int IdDetalleVenta { get; set; }
        public int? IdVenta { get; set; }
        public int? IdProducto { get; set; }
        public int? Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Total { get; set; }

        // Propiedad de navegación para el producto asociado al detalle de venta
        public virtual Producto? IdProductoNavigation { get; set; }

        // Propiedad de navegación para la venta asociada al detalle de venta
        public virtual Venta? IdVentaNavigation { get; set; }
    }
}