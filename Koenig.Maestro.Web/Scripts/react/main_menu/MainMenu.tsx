import * as React from 'react';
import MenuItem from './MenuItem';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap';

export default class MainMenu extends React.Component {
    render() {

        return (
            <div>

                <div style={{ display: 'flex', }}>
                <MenuItem imgName="order_new.png" action="NewOrder"
                    caption="New order" itemType="button"
                    height="70px" width="40%" />
                    <MenuItem imgName="icon-order.png" action="MaestroList/Orders"
                    caption="Orders" itemType="button"
                        height="70px" width="60%" />
                </div>
                <div style={{ display: 'flex', }}>
                <MenuItem imgName="invoice_export.png" action="QbInvoiceExport"
                    caption="Export Invoices to Quickbooks" itemType="button"
                    height="70px" width="100%" />
                </div>

                <div style={{ display: 'flex', }}>
                    <MenuItem imgName="cake.png" action="MaestroList/Products"
                    caption="Products" itemType="button"
                    height="70px" width="40%" />
                    <MenuItem imgName="clients.png" action="MaestroList/Customers"
                    caption="Customers" itemType="button"
                        height="70px" width="40%" />
                    <MenuItem imgName="measure.png" action="MaestroList/Units"
                    caption="Units" itemType="button"
                    height="70px" width="20%" />
                </div>


                <div style={{ display: 'flex', }}>
                    <MenuItem imgName="map.png" action="MaestroList/Regions"
                        caption="Regions" itemType="button"
                        height="70px" width="40%" />
                    <MenuItem imgName="units.png" action="MaestroList/UnitTypes"
                        caption="Unit Types" itemType="button"
                        height="70px" width="40%" />
                    <MenuItem imgName="cup.png" action="MaestroList/CustomerProductUnits"
                        caption="C.P.U." itemType="button"
                        height="70px" width="20%" />

                </div>

                <div style={{ display: 'flex', }}>
                    <MenuItem imgName="qb_customers.png" action="QbImportCustomers"
                        caption="Import Customers" itemType="button"
                        height="70px" width="33%" />
                    <MenuItem imgName="qb_products.png" action="QbImportItems"
                        caption="Import Products" itemType="button"
                        height="70px" width="33%" />
                    <MenuItem imgName="qb_invoices.png" action="QbImportInvoices"
                        caption="Import Invoices" itemType="button"
                        height="70px" width="34%" />

                </div>
            </div>
        );

    }


}