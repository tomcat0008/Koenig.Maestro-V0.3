import MaestroCustomer, { IMaestroCustomer } from './dbEntities/IMaestroCustomer';
import { IMaestroRegion } from './dbEntities/IMaestroRegion';
import { IResponseMessage } from './ResponseMessage';
import AxiosAgent from './AxiosAgent';
import { IQbProductMap } from './dbEntities/IQbProductMap';
import OrderMaster, { IOrderMaster } from './dbEntities/IOrderMaster';
import MaestroProduct, { IMaestroProduct } from './dbEntities/IMaestroProduct';
import { DbEntityBase } from './dbEntities/DbEntityBase';
import CustomerProductUnit, { ICustomerProductUnit } from './dbEntities/ICustomerProductUnit';
import { IErrorInfo } from './ErrorInfo';
import MaestroUnit, { IMaestroUnit } from './dbEntities/IMaestroUnit';
import MaestroProductGroup, { IMaestroProductGroup } from './dbEntities/IProductGroup';

interface IDisplayBase {
    Init: boolean;
    ErrorInfo: IErrorInfo;
}

export interface ICustomerDisplay extends IDisplayBase {
    Regions: IMaestroRegion[];
    Customer: IMaestroCustomer;
    
}

export interface IOrderDisplay extends IDisplayBase {
    Customers: IMaestroCustomer[];
    ProductMaps: IQbProductMap[];
    Products: IMaestroProduct[];
    CustomerProductUnits: ICustomerProductUnit[];
    ProductGroups: IMaestroProductGroup[];
    Units: IMaestroUnit[];
    Entity: IOrderMaster;
    OrderDate: Date;
    DeliveryDate: Date;
    SummaryDisplay: any;

}

export interface ICustomerProductUnitDisplay extends IDisplayBase {
    Customers: IMaestroCustomer[];
    Products: IMaestroProduct[];
    Units: IMaestroUnit[];
    ProductId: number;
    Entity: ICustomerProductUnit;
    UnitTypeId: number;
    //CustomerProductUnit: ICustomerProductUnit;
}


export default class EntityAgent {

    static GetFirstSelecItem(tranCode: string): DbEntityBase {
        let result: DbEntityBase;
        let selectText = "--Please select--";
        switch (tranCode) {
            case "CUSTOMER":
                let customer: MaestroCustomer = new MaestroCustomer(-1);
                customer.Name = selectText;
                result = customer;
                break;
            case "PRODUCT":
                let product: MaestroProduct = new MaestroProduct(-1);
                product.Name = selectText;
                result = product;
                break;
            case "UNIT":
                let unit: MaestroUnit = new MaestroUnit(-1);
                unit.Name = selectText;
                result = unit;
                break;
            case "PRODUCT_GROUP":
                let pg: MaestroProductGroup = new MaestroProductGroup(-1);
                pg.Name = selectText;
                result = pg;
                break;

        }
        return result;
    }
    static FactoryCreate(tranCode: string): DbEntityBase
    {
        let result: DbEntityBase;
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


    static async GetNewOrderId(): Promise<number> {
        let response: IResponseMessage = await new AxiosAgent().getNewOrderId();
        return parseInt(response.TransactionResult as string);
    }

    private async UpdateItem(tran: string, item: DbEntityBase):Promise<IResponseMessage> {
        let response: IResponseMessage;
        let ax: AxiosAgent = new AxiosAgent();
        response = await ax.updateItem(tran, item);
        return response;
    }

    private async CreateItem(tran: string, item: DbEntityBase, mde?: { [key: string]: string }): Promise<IResponseMessage> {
        let response: IResponseMessage;
        let ax: AxiosAgent = new AxiosAgent();
        response = await ax.createItem(tran, item, mde);
        return response;
    }

    async SaveOrder(item: IOrderMaster): Promise<IResponseMessage> {
        let result: IResponseMessage;
        if (!item.IsNew)
            result = await this.UpdateItem("ORDER", item);
        else {
            let mde: { [key: string]: string } = {
                ["REQUEST_TYPE"]: "InsertNewOrder",
                ["CREATE_INVOICE"]:(item.CreateInvoiceOnQb ? "true":"false")
            };
            result = await this.CreateItem("ORDER", item, mde);
        }
        return result;
        
    }

    async SaveCustomerProductUnit(item: ICustomerProductUnit): Promise<IResponseMessage> {
        let result: IResponseMessage;
        if (item.Id > 0)
            result = await this.UpdateItem("CUSTOMER_PRODUCT_UNIT", item);
        else
            result = await this.CreateItem("CUSTOMER_PRODUCT_UNIT", item);
        return result;
    }


    async SaveCustomer(item: IMaestroCustomer): Promise<IResponseMessage> {
        let result: IResponseMessage;
        if (item.Id > 0)
            result = await this.UpdateItem("CUSTOMER", item);
        else
            result = await this.CreateItem("CUSTOMER", item);
        return result;
    }

    async GetCustomerProductUnitDisplay(): Promise<ICustomerProductUnitDisplay> {
        let cusList: IMaestroCustomer[];
        let productList: IMaestroProduct[];
        let unitList: IMaestroUnit[];

        let ax: AxiosAgent = new AxiosAgent();
        let response: IResponseMessage = await ax.getList("CUSTOMER", {});

        if (response.TransactionStatus != "ERROR") {
            cusList = response.TransactionResult as IMaestroCustomer[];

            response = await ax.getList("PRODUCT", {});
        }
        if (response.TransactionStatus != "ERROR") {
            productList = response.TransactionResult as IMaestroProduct[];

            response = await ax.getList("UNIT", {});
        }

        if (response.TransactionStatus != "ERROR")
            unitList = response.TransactionResult as IMaestroUnit[];

        let result: ICustomerProductUnitDisplay = {
            Init: true,
            Customers: cusList,
            ErrorInfo: response.ErrorInfo,
            Products: productList,
            Units: unitList,
            ProductId: 0,
            Entity: new CustomerProductUnit(0),
            UnitTypeId:0
            
        };
        return result;

    }

    async GetOrderDisplay(): Promise<IOrderDisplay> {

        let cusList: IMaestroCustomer[];
        let productList: IMaestroProduct[];
        let productMapList: IQbProductMap[];
        let customerProductUnits: ICustomerProductUnit[];
        let productGroups: IMaestroProductGroup[];
        let units: IMaestroUnit[];
        let ax: AxiosAgent = new AxiosAgent();
        let response: IResponseMessage = await ax.getList("CUSTOMER", {});

        if (response.TransactionStatus != "ERROR") {
            cusList = response.TransactionResult as IMaestroCustomer[];

            response = await ax.getList("QB_PRODUCT_MAP", {});
        }

        if (response.TransactionStatus != "ERROR") {
            productMapList = response.TransactionResult as IQbProductMap[];

            response = await ax.getList("PRODUCT", {});
        }

        if (response.TransactionStatus != "ERROR") {
            productList = response.TransactionResult as IMaestroProduct[];

            response = await ax.getList("CUSTOMER_PRODUCT_UNIT", {});
        }

        if (response.TransactionStatus != "ERROR") {
            customerProductUnits = response.TransactionResult as ICustomerProductUnit[];
            response = await ax.getList("UNIT", {});
        }

        if (response.TransactionStatus != "ERROR") {
            units = response.TransactionResult as IMaestroUnit[];
            response = await ax.getList("PRODUCT_GROUP", {});
        }

        
        if (response.TransactionStatus != "ERROR")
            productGroups = response.TransactionResult as IMaestroProductGroup[];

        let result: IOrderDisplay = { DeliveryDate: new Date(), OrderDate: new Date(),Units:units,
            Entity: null, Customers: cusList, ErrorInfo: response.ErrorInfo, ProductGroups: productGroups, SummaryDisplay: { display: "block" },
            Products: productList, ProductMaps: productMapList, CustomerProductUnits: customerProductUnits, Init: true
        };
        return result;
    }

    async GetCustomerDisplay(id:number):Promise<ICustomerDisplay> {


        let cust: IMaestroCustomer;
        let regionList: IMaestroRegion[]

        let response: IResponseMessage;

        let ax: AxiosAgent = new AxiosAgent();
        if (id > 0) {
            response = await ax.getItem(id, "CUSTOMER");
            cust = response.TransactionResult;
        }
        else {
            cust = new MaestroCustomer(0);
        }
        response = await ax.getList("REGION", {});
        regionList = response.TransactionResult;

        let result: ICustomerDisplay = { ErrorInfo:response.ErrorInfo, Customer:cust, Regions:regionList, Init:false };
        return result;
    }
    /*
    async GetOrderEntryObjects(id:number, executeGet:boolean): Promise<IOrderDisplay> {

        let response: IResponseMessage;

        let cusList: IMaestroCustomer[];
        let prodList: IQbProductMap[];
        


        let ax: AxiosAgent = new AxiosAgent();
        response = await ax.getList("CUSTOMER", {});
        cusList = response.TransactionResult;
        response = await ax.getList("QB_PRODUCT_MAP", {});
        prodList = response.TransactionResult;

        if (executeGet) {
            response = await ax.getItem(id, "ORDER");
            order = response.TransactionResult;
        }
        else {
            order = new OrderMaster(id);
        }

        let result: IOrderDisplay = { Customers:cusList, Products:prodList, Order:order };

        return result;


    }
    */
    



}