import React, { useState } from "react";
import { Card, CardBody, CardHeader, Col, FormGroup, Input, Label, Row, Button } from "reactstrap";
import DataTable from 'react-data-table-component';
import Swal from 'sweetalert2';
import * as XLSX from "xlsx";

const modeloInicio = [{
    producto: "",
    stock: "",
}];

const ReporteVenta = () => {
    const [stockMin, setStockMin] = useState("");
    const [stockMax, setStockMax] = useState("");
    const [pendiente, setPendiente] = useState(false);
    const [ventas, setVentas] = useState(modeloInicio);

           const buscar = () => {
            setPendiente(true);
            let apiUrl = `api/producto/Reporte?`;

            if (stockMin !== "" && stockMax !== "") {
                apiUrl += `stockMinimo=${stockMin}&stockMaximo=${stockMax}`;
            }

            fetch(apiUrl)
                .then((response) => {
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then((dataJson) => {
                    var data = dataJson;
                    setPendiente(false);

                    if (data.length < 1) {
                        Swal.fire(
                            'Opps!',
                            'No se encontraron resultados',
                            'warning'
                        );
                    }

                    setVentas(data);
                })
                .catch((error) => {
                    setVentas([]);
                    Swal.fire(
                        'Opps!',
                        'No se pudo encontrar información',
                        'error'
                    );
                });
        };
    const columns = [
        {
            name: 'Producto',
            sortable: true,
            grow: 2,
            maxWidth: '600px',
            selector: row => row.producto,
        },
        {
            name: 'Stock',
            selector: row => row.stock,
        },
    ];

    const customStyles = {
        headCells: {
            style: {
                fontSize: '13px',
                fontWeight: 800,
            },
        },
        headRow: {
            style: {
                backgroundColor: "#eee",
            }
        }
    };

    const exportarExcel = () => {
        var wb = XLSX.utils.book_new();
        var ws = XLSX.utils.json_to_sheet(ventas);

        XLSX.utils.book_append_sheet(wb, ws, "Reporte");
        XLSX.writeFile(wb, "ReporteVentas.xlsx");
    };

    return (
        <>
            <Row>
                <Col sm={12}>
                    <Card>
                        <CardHeader style={{ backgroundColor: '#4e73df', color: "white" }}>
                            Reporte de Stock
                        </CardHeader>
                        <CardBody>
                            <Row className="align-items-end">
                                <Col sm={2}>
                                    <FormGroup>
                                        <label>Stock Mínimo:</label>
                                        <Input
                                            type="text"
                                            value={stockMin}
                                            onChange={(e) => setStockMin(e.target.value)}
                                        />
                                    </FormGroup>
                                </Col>
                                <Col sm={2}>
                                    <FormGroup>
                                        <label>Stock Máximo:</label>
                                        <Input
                                            type="text"
                                            value={stockMax}
                                            onChange={(e) => setStockMax(e.target.value)}
                                        />
                                    </FormGroup>
                                </Col>
                                <Col sm={3}>
                                    <FormGroup>
                                        <Button color="primary" size="sm" block onClick={buscar}>
                                            <i className="fa fa-search" aria-hidden="true"></i> Buscar
                                        </Button>
                                    </FormGroup>
                                </Col>
                                <Col sm={3}>
                                    <FormGroup>
                                        <Button color="success" size="sm" block onClick={exportarExcel}>
                                            <i className="fa fa-file-excel" aria-hidden="true"></i> Exportar
                                        </Button>
                                    </FormGroup>
                                </Col>
                            </Row>
                            <hr></hr>
                            <Row>
                                <Col sm="12">
                                    <DataTable
                                        progressPending={pendiente}
                                        columns={columns}
                                        data={ventas}
                                        customStyles={customStyles}
                                    />
                                </Col>
                            </Row>
                        </CardBody>
                    </Card>
                </Col>
            </Row>
        </>
    );
};

export default ReporteVenta;
