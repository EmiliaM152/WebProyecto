using System;
using System.Collections.Generic;

namespace ReactVentas.Models
{
    public partial class Venta
    {
        public Venta()
        {
            DetalleVenta = new HashSet<DetalleVenta>();
        }

        public int IdVenta { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuario { get; set; }
        public string? DocumentoCliente { get; set; }
        public string? NombreCliente { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? ImpuestoTotal { get; set; }
        public decimal? Total { get; set; }

        /// Obtiene o establece la navegación al usuario que realizó la venta.
        public virtual Usuario? IdUsuarioNavigation { get; set; }
        /// Obtiene o establece la colección de detalles de venta asociados a la venta.
        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; }
    }
}