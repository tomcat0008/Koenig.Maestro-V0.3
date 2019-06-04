var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import MaestroCustomer from './dbEntities/IMaestroCustomer';
import MaestroRegion from './dbEntities/IMaestroRegion';
import AxiosAgent from './AxiosAgent';
import OrderMaster from './dbEntities/IOrderMaster';
import MaestroProduct from './dbEntities/IMaestroProduct';
import CustomerProductUnit from './dbEntities/ICustomerProductUnit';
import ErrorInfo from './ErrorInfo';
import MaestroUnit from './dbEntities/IMaestroUnit';
import MaestroProductGroup from './dbEntities/IProductGroup';
import MaestroUnitType from './dbEntities/IMaestroUnitType';
import QbInvoiceLog from './dbEntities/IQbInvoiceLog';
import CustomerAddress from './dbEntities/ICustomerAddress';
import { MaestroReportDefinition } from './dbEntities/IReportDefinition';
export default class EntityAgent {
    static GetFirstSelecItem(tranCode) {
        let result;
        let selectText = "--Please select--";
        switch (tranCode) {
            case "CUSTOMER":
                let customer = new MaestroCustomer(-1);
                customer.Name = selectText;
                result = customer;
                break;
            case "PRODUCT":
                let product = new MaestroProduct(-1);
                product.Name = selectText;
                result = product;
                break;
            case "UNIT":
                let unit = new MaestroUnit(-1);
                unit.Name = selectText;
                result = unit;
                break;
            case "PRODUCT_GROUP":
                let pg = new MaestroProductGroup(-1);
                pg.Name = selectText;
                result = pg;
                break;
            case "REGION":
                let region = new MaestroRegion(-1);
                region.Name = selectText;
                result = region;
                break;
            case "UNIT_TYPE":
                let unitType = new MaestroUnitType(-1);
                unitType.Name = selectText;
                result = unitType;
                break;
            case "ADDRESS":
                let address = new CustomerAddress(-1);
                address.AddressCode = selectText;
                result = address;
                break;
            case "REPORT":
                let reportDef = new MaestroReportDefinition(-1);
                reportDef.Description = selectText;
                reportDef.ReportCode = "";
                result = reportDef;
                break;
        }
        return result;
    }
    static FactoryCreate(tranCode) {
        let result;
        switch (tranCode) {
            case "CUSTOMER":
                result = new MaestroCustomer(0);
                break;
            case "PRODUCT":
                result = new MaestroProduct(0);
                break;
            case "ORDER":
                result = new OrderMaster(0);
                result.IsNew = true;
                break;
            case "CUSTOMER_PRODUCT_UNIT":
                result = new CustomerProductUnit(0);
                result.IsNew = true;
                break;
            case "PRODUCT_GROUP":
                result = new MaestroProductGroup(0);
                result.IsNew = true;
                break;
            case "REGION":
                result = new MaestroRegion(0);
                break;
            case "UNIT":
                result = new MaestroUnit(0);
                break;
            case "UNIT_TYPE":
                result = new MaestroUnitType(0);
                break;
            case "QUICKBOOKS_INVOICE":
                result = new QbInvoiceLog(0);
                break;
        }
        return result;
    }
    static GetNewOrderId() {
        return __awaiter(this, void 0, void 0, function* () {
            let response = yield new AxiosAgent().getNewOrderId();
            return parseInt(response.TransactionResult);
        });
    }
    UpdateItem(tran, item) {
        return __awaiter(this, void 0, void 0, function* () {
            let response;
            let ax = new AxiosAgent();
            response = yield ax.updateItem(tran, item);
            return response;
        });
    }
    CreateItem(tran, item, mde) {
        return __awaiter(this, void 0, void 0, function* () {
            let response;
            let ax = new AxiosAgent();
            response = yield ax.createItem(tran, item, mde);
            return response;
        });
    }
    SaveOrder(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let result;
            if (!item.IsNew)
                result = yield this.UpdateItem("ORDER", item);
            else {
                let mde = {
                    ["REQUEST_TYPE"]: "InsertNewOrder",
                    ["CREATE_INVOICE"]: (item.CreateInvoiceOnQb ? "true" : "false")
                };
                result = yield this.CreateItem("ORDER", item, mde);
            }
            return result;
        });
    }
    ExportOrder(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let ax = new AxiosAgent();
            let result = yield ax.exportItemQb("ORDER", [item]);
            return result;
        });
    }
    CancelOrder(order) {
        return __awaiter(this, void 0, void 0, function* () {
            let ax = new AxiosAgent();
            let result = yield ax.cancelItem("ORDER", order);
            return result;
        });
    }
    CreateInvoices(invoiceList) {
        return __awaiter(this, void 0, void 0, function* () {
            let ax = new AxiosAgent();
            let result = yield ax.createInvoice(invoiceList);
            return result;
        });
    }
    SaveCustomerProductUnit(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let result;
            if (item.Id > 0)
                result = yield this.UpdateItem("CUSTOMER_PRODUCT_UNIT", item);
            else
                result = yield this.CreateItem("CUSTOMER_PRODUCT_UNIT", item);
            return result;
        });
    }
    SaveUnitType(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let result;
            if (item.Id > 0)
                result = yield this.UpdateItem("UNIT_TYPE", item);
            else
                result = yield this.CreateItem("UNIT_TYPE", item);
            return result;
        });
    }
    SaveUnit(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let result;
            if (item.Id > 0)
                result = yield this.UpdateItem("UNIT", item);
            else
                result = yield this.CreateItem("UNIT", item);
            return result;
        });
    }
    SaveRegion(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let result;
            if (item.Id > 0)
                result = yield this.UpdateItem("REGION", item);
            else
                result = yield this.CreateItem("REGION", item);
            return result;
        });
    }
    SaveCustomer(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let result;
            if (item.Id > 0)
                result = yield this.UpdateItem("CUSTOMER", item);
            else
                result = yield this.CreateItem("CUSTOMER", item);
            return result;
        });
    }
    GetReportFilterDisplay() {
        return __awaiter(this, void 0, void 0, function* () {
            let reportDefs;
            let ax = new AxiosAgent();
            let response = yield ax.getList("REPORT_DEFINITION", {});
            if (response.TransactionStatus != "ERROR")
                reportDefs = response.TransactionResult;
            let result = {
                Init: true,
                ErrorInfo: response.ErrorInfo,
                StartDate: new Date(),
                EndDate: new Date(),
                ReportCode: "",
                ReportDefinitions: reportDefs
            };
            return result;
        });
    }
    GetCustomerProductUnitDisplay() {
        return __awaiter(this, void 0, void 0, function* () {
            let cusList;
            let productList;
            let unitList;
            let ax = new AxiosAgent();
            let response = yield ax.getList("CUSTOMER", {});
            if (response.TransactionStatus != "ERROR") {
                cusList = response.TransactionResult;
                response = yield ax.getList("PRODUCT", {});
            }
            if (response.TransactionStatus != "ERROR") {
                productList = response.TransactionResult;
                response = yield ax.getList("UNIT", {});
            }
            if (response.TransactionStatus != "ERROR")
                unitList = response.TransactionResult;
            let cpu = new CustomerProductUnit(0);
            cpu.Actions = new Array("Save");
            let result = {
                Init: true,
                Customers: cusList,
                ErrorInfo: response.ErrorInfo,
                Products: productList,
                Units: unitList,
                ProductId: 0,
                Entity: cpu,
                UnitTypeId: 0
            };
            return result;
        });
    }
    GetOrderDisplay() {
        return __awaiter(this, void 0, void 0, function* () {
            let cusList;
            let productList;
            let productMapList;
            let customerProductUnits;
            let productGroups;
            let units;
            let ax = new AxiosAgent();
            let response = yield ax.getList("CUSTOMER", {});
            if (response.TransactionStatus != "ERROR") {
                cusList = response.TransactionResult;
                response = yield ax.getList("QB_PRODUCT_MAP", {});
            }
            if (response.TransactionStatus != "ERROR") {
                productMapList = response.TransactionResult;
                response = yield ax.getList("PRODUCT", {});
            }
            if (response.TransactionStatus != "ERROR") {
                productList = response.TransactionResult;
                response = yield ax.getList("CUSTOMER_PRODUCT_UNIT", {});
            }
            if (response.TransactionStatus != "ERROR") {
                customerProductUnits = response.TransactionResult;
                response = yield ax.getList("UNIT", {});
            }
            if (response.TransactionStatus != "ERROR") {
                units = response.TransactionResult;
                response = yield ax.getList("PRODUCT_GROUP", {});
            }
            if (response.TransactionStatus != "ERROR")
                productGroups = response.TransactionResult;
            let result = { DeliveryDate: new Date(), OrderDate: new Date(), Units: units,
                Entity: null, Customers: cusList, ErrorInfo: response.ErrorInfo, ProductGroups: productGroups, SummaryDisplay: { display: "block" },
                Products: productList, ProductMaps: productMapList, CustomerProductUnits: customerProductUnits, Init: true
            };
            return result;
        });
    }
    GetQbInvoiceLogDisplay(id) {
        return __awaiter(this, void 0, void 0, function* () {
            let log;
            let response;
            let errorInfo;
            let ax = new AxiosAgent();
            response = yield ax.getItem(id, "QUICKBOOKS_INVOICE");
            log = response.TransactionResult;
            errorInfo = response.ErrorInfo;
            log.Actions = new Array();
            let result = { ErrorInfo: errorInfo, InvoiceLog: log, Init: false };
            return result;
        });
    }
    GetRegionDisplay(id) {
        return __awaiter(this, void 0, void 0, function* () {
            let region;
            let response;
            let errorInfo;
            let ax = new AxiosAgent();
            if (id > 0) {
                response = yield ax.getItem(id, "REGION");
                region = response.TransactionResult;
                errorInfo = response.ErrorInfo;
            }
            else {
                errorInfo = new ErrorInfo();
                region = new MaestroRegion(0);
            }
            region.Actions = new Array("Save");
            let result = { ErrorInfo: errorInfo, Region: region, Init: false };
            return result;
        });
    }
    GetUnitTypeDisplay(id) {
        return __awaiter(this, void 0, void 0, function* () {
            let unitType;
            let response;
            let errorInfo;
            let ax = new AxiosAgent();
            if (id > 0) {
                response = yield ax.getItem(id, "UNIT_TYPE");
                unitType = response.TransactionResult;
                errorInfo = response.ErrorInfo;
            }
            else {
                unitType = new MaestroUnitType(0);
                errorInfo = new ErrorInfo();
            }
            unitType.Actions = new Array("Save");
            let result = { ErrorInfo: errorInfo, UnitType: unitType, Init: false };
            return result;
        });
    }
    GetInvoiceMergeDisplay() {
        return __awaiter(this, void 0, void 0, function* () {
            let ax = new AxiosAgent();
            let response = yield ax.getList("CUSTOMER", { ["LIST_CODE"]: "MERGE_INVOICE" });
            let cusList;
            let orderList;
            if (response.TransactionStatus != "ERROR")
                cusList = response.TransactionResult;
            let result = { Invoice: new QbInvoiceLog(0), Init: true, ErrorInfo: response.ErrorInfo, Customers: cusList, Orders: orderList, Templates: [""], OrdersToMerge: [] };
            return result;
        });
    }
    GetUnitDisplay(id) {
        return __awaiter(this, void 0, void 0, function* () {
            let unit;
            let unitTypeList;
            let response;
            let ax = new AxiosAgent();
            if (id > 0) {
                response = yield ax.getItem(id, "UNIT");
                unit = response.TransactionResult;
            }
            else {
                unit = new MaestroUnit(0);
            }
            unit.Actions = new Array("Save");
            response = yield ax.getList("UNIT_TYPE", {});
            unitTypeList = response.TransactionResult;
            let result = { ErrorInfo: response.ErrorInfo, Unit: unit, UnitTypes: unitTypeList, Init: false };
            return result;
        });
    }
    GetCustomerDisplay(id) {
        return __awaiter(this, void 0, void 0, function* () {
            let cust;
            let regionList;
            let response;
            let ax = new AxiosAgent();
            if (id > 0) {
                response = yield ax.getItem(id, "CUSTOMER");
                cust = response.TransactionResult;
            }
            else {
                cust = new MaestroCustomer(0);
            }
            response = yield ax.getList("REGION", {});
            regionList = response.TransactionResult;
            cust.Actions = new Array();
            cust.Actions.push("Save");
            let result = { ErrorInfo: response.ErrorInfo, Customer: cust, Regions: regionList, Init: false };
            return result;
        });
    }
}
//# sourceMappingURL=EntityAgent.js.map