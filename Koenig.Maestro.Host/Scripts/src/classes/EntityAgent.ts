import MaestroCustomer, { IMaestroCustomer } from './dbEntities/IMaestroCustomer';
import MaestroRegion, { IMaestroRegion } from './dbEntities/IMaestroRegion';
import { IResponseMessage } from './ResponseMessage';
import AxiosAgent from './AxiosAgent';
import { IQbProductMap } from './dbEntities/IQbProductMap';
import OrderMaster, { IOrderMaster } from './dbEntities/IOrderMaster';
import MaestroProduct, { IMaestroProduct } from './dbEntities/IMaestroProduct';
import { DbEntityBase } from './dbEntities/DbEntityBase';
import CustomerProductUnit, { ICustomerProductUnit } from './dbEntities/ICustomerProductUnit';
import ErrorInfo, { IErrorInfo } from './ErrorInfo';
import MaestroUnit, { IMaestroUnit } from './dbEntities/IMaestroUnit';
import MaestroProductGroup, { IMaestroProductGroup } from './dbEntities/IProductGroup';
import MaestroUnitType, { IMaestroUnitType } from './dbEntities/IMaestroUnitType';
import QbInvoiceLog, { IQbInvoiceLog } from './dbEntities/IQbInvoiceLog';
import CustomerAddress from './dbEntities/ICustomerAddress';

interface IDisplayBase {
    Init: boolean;
    ErrorInfo: IErrorInfo;
}

export interface ICustomerDisplay extends IDisplayBase {
    Regions: IMaestroRegion[];
    Customer: IMaestroCustomer;
    
}

export interface IQbInvoiceLogDisplay extends IDisplayBase {
    InvoiceLog: IQbInvoiceLog;
}

export interface IRegionDisplay extends IDisplayBase {
    Region: IMaestroRegion;
}

export interface IUnitDisplay extends IDisplayBase {
    Unit: IMaestroUnit;
    UnitTypes: IMaestroUnitType[];
}

export interface IUnitTypeDisplay extends IDisplayBase {
    UnitType: IMaestroUnitType;
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
            case "REGION":
                let region: MaestroRegion = new MaestroRegion(-1);
                region.Name = selectText;
                result = region;
                break;
            case "UNIT_TYPE":
                let unitType: MaestroUnitType = new MaestroUnitType(-1);
                unitType.Name = selectText;
                result = unitType;
                break;
            case "ADDRESS":
                let address: CustomerAddress = new CustomerAddress(-1);
                address.AddressCode = selectText;
                result = address;
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

    async ExportOrder(item: IOrderMaster): Promise<IResponseMessage> {

        
        let ax: AxiosAgent = new AxiosAgent();
        let result: IResponseMessage = await ax.exportItemQb("ORDER", [item]);
        return result;

    }

    async CancelOrder(order: IOrderMaster): Promise<IResponseMessage> {
        let ax: AxiosAgent = new AxiosAgent();
        let result: IResponseMessage = await ax.cancelItem("ORDER", order);
        return result;
    }

    async CreateInvoices(invoiceList:number[]): Promise<IResponseMessage> {
        let ax: AxiosAgent = new AxiosAgent();

        let result: IResponseMessage = await ax.createInvoice(invoiceList);
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

    async SaveUnitType(item: IMaestroUnitType): Promise<IResponseMessage> {
        let result: IResponseMessage;
        if (item.Id > 0)
            result = await this.UpdateItem("UNIT_TYPE", item);
        else
            result = await this.CreateItem("UNIT_TYPE", item);
        return result;
    }

    async SaveUnit(item: IMaestroUnit): Promise<IResponseMessage> {
        let result: IResponseMessage;
        if (item.Id > 0)
            result = await this.UpdateItem("UNIT", item);
        else
            result = await this.CreateItem("UNIT", item);
        return result;
    }

    async SaveRegion(item: IMaestroRegion): Promise<IResponseMessage> {
        let result: IResponseMessage;
        if (item.Id > 0)
            result = await this.UpdateItem("REGION", item);
        else
            result = await this.CreateItem("REGION", item);
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

        let cpu: ICustomerProductUnit = new CustomerProductUnit(0);
        cpu.Actions = new Array<string>("Save");
        let result: ICustomerProductUnitDisplay = {
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

    async GetQbInvoiceLogDisplay(id: number): Promise<IQbInvoiceLogDisplay> {
        let log: IQbInvoiceLog;
        let response: IResponseMessage;
        let errorInfo: IErrorInfo;
        let ax: AxiosAgent = new AxiosAgent();

        response = await ax.getItem(id, "QUICKBOOKS_INVOICE");
        log = response.TransactionResult;
        errorInfo = response.ErrorInfo;
        log.Actions = new Array<string>();

        let result: IQbInvoiceLogDisplay = { ErrorInfo: errorInfo, InvoiceLog: log, Init: false };
        return result;
    }


    async GetRegionDisplay(id: number): Promise<IRegionDisplay> {
        let region: IMaestroRegion;
        let response: IResponseMessage;
        let errorInfo: IErrorInfo;
        let ax: AxiosAgent = new AxiosAgent();
        if (id > 0) {
            response = await ax.getItem(id, "REGION");
            region = response.TransactionResult;
            errorInfo = response.ErrorInfo;
        }
        else {
            errorInfo = new ErrorInfo();
            region = new MaestroRegion(0);
        }
        region.Actions = new Array<string>("Save");
        let result: IRegionDisplay = { ErrorInfo: errorInfo, Region: region, Init: false };
        return result;
    }

    async GetUnitTypeDisplay(id: number): Promise<IUnitTypeDisplay> {
        let unitType: IMaestroUnitType;
        let response: IResponseMessage;
        let errorInfo: IErrorInfo;
        let ax: AxiosAgent = new AxiosAgent();
        if (id > 0) {
            response = await ax.getItem(id, "UNIT_TYPE");
            unitType = response.TransactionResult;
            errorInfo = response.ErrorInfo;
        }
        else {
            unitType = new MaestroUnitType(0);
            errorInfo = new ErrorInfo();
        }
        unitType.Actions = new Array<string>("Save");
        let result: IUnitTypeDisplay = { ErrorInfo: errorInfo, UnitType: unitType, Init: false };
        return result;
    }

    async GetUnitDisplay(id: number): Promise<IUnitDisplay> {


        let unit: IMaestroUnit;
        let unitTypeList: IMaestroUnitType[]

        let response: IResponseMessage;

        let ax: AxiosAgent = new AxiosAgent();
        if (id > 0) {
            response = await ax.getItem(id, "UNIT");
            unit= response.TransactionResult;
        }
        else {
            unit = new MaestroUnit(0);
        }
        unit.Actions = new Array<string>("Save");
        response = await ax.getList("UNIT_TYPE", {});
        unitTypeList = response.TransactionResult;
        

        let result: IUnitDisplay = { ErrorInfo: response.ErrorInfo, Unit: unit, UnitTypes: unitTypeList, Init: false };
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

        cust.Actions = new Array<string>();
        cust.Actions.push("Save");
        
        let result: ICustomerDisplay = { ErrorInfo:response.ErrorInfo, Customer:cust, Regions:regionList, Init:false };
        return result;
    }

}