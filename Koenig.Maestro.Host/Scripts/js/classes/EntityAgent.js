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
export default class EntityAgent {
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
    CreateItem(tran, item) {
        return __awaiter(this, void 0, void 0, function* () {
            let response;
            let ax = new AxiosAgent();
            response = yield ax.createItem(tran, item);
            return response;
        });
    }
    SaveOrder(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let result;
            if (item.IsNew)
                result = yield this.UpdateItem("ORDER", item);
            else
                result = yield this.CreateItem("ORDER", item);
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
            let result = { Customer: cust, Regions: regionList, init: false };
            return result;
        });
    }
    GetOrderEntryObjects(id, executeGet) {
        return __awaiter(this, void 0, void 0, function* () {
            let response;
            let cusList;
            let prodList;
            let order;
            let ax = new AxiosAgent();
            response = yield ax.getList("CUSTOMER", {});
            cusList = response.TransactionResult;
            response = yield ax.getList("QB_PRODUCT_MAP", {});
            prodList = response.TransactionResult;
            if (executeGet) {
                response = yield ax.getItem(id, "ORDER");
                order = response.TransactionResult;
            }
            else {
                order = new OrderMaster(id);
            }
            let result = { Customers: cusList, Products: prodList, Order: order };
            return result;
        });
    }
}
//# sourceMappingURL=EntityAgent.js.map