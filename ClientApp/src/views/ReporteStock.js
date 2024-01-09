import React, { useEffect, useState } from 'react';

const StockReport = () => {
  const [productos, setProductos] = useState([]);
  const [totalStocks, setTotalStocks] = useState(0);

  useEffect(() => {
    // Aquí puedes realizar una solicitud a tu API para obtener los datos del informe de stock
    // Puedes usar la función fetch o axios, por ejemplo.
    // Ejemplo usando fetch:
    fetch('/api/stock/report')
      .then(response => response.json())
      .then(data => {
        setProductos(data.productos);
        setTotalStocks(data.totalStocks);
      })
      .catch(error => console.error('Error al obtener datos del informe de stock', error));
  }, []);

  return (
    <div>
      <h1>Reporte de Stock</h1>
      <table>
        <thead>
          <tr>
            <th>Producto</th>
            <th>Stock</th>
          </tr>
        </thead>
        <tbody>
          {productos.map(producto => (
            <tr key={producto.idProducto}>
              <td>{producto.descripcion}</td>
              <td>{producto.stock}</td>
            </tr>
          ))}
        </tbody>
      </table>
      <p>Total de Stocks: {totalStocks}</p>
    </div>
  );
};

export default StockReport;
