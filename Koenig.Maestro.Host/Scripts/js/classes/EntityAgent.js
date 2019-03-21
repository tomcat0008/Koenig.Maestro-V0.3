var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import MaestroCustomer from './dbEntities/IMaestroCustomer';
import AxiosAgent from './AxiosAgent';
import OrderMaster from './dbEntities/IOrderMaster';
import MaestroProduct from './dbEntities/IMaestroProduct';
import CustomerProductUnit from './dbEntities/ICustomerProductUnit';
import MaestroUnit from './dbEntities/IMaestroUnit';
import MaestroProductGroup from './dbEntities/IProductGroup';
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
            let result = {
                Init: true,
                Customers: cusList,
                ErrorInfo: response.ErrorInfo,
                Products: productList,
                Units: unitList,
                ProductId: 0,
                Entity: new CustomerProductUnit(0),
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
            let result = { ErrorInfo: response.ErrorInfo, Customer: cust, Regions: regionList, Init: false };
            return result;
        });
    }
}
//# sourceMappingURL=EntityAgent.js.map