import MaestroCustomer, { IMaestroCustomer } from './dbEntities/IMaestroCustomer';
import { IMaestroRegion } from './dbEntities/IMaestroRegion';
import { IResponseMessage } from './ResponseMessage';
import AxiosAgent from './AxiosAgent';
import { IQbProductMap } from './dbEntities/IQbProductMap';
import OrderMaster, { IOrderMaster } from './dbEntities/IOrderMaster';
import MaestroProduct from './dbEntities/IMaestroProduct';
import { DbEntityBase } from './dbEntities/DbEntityBase';


export interface ICustomerDisplay {
    Regions: IMaestroRegion[];
    Customer: IMaestroCustomer;
    init: boolean;
}

export interface IOrderDisplay {
    Customers: IMaestroCustomer[];
    Products: IQbProductMap[];
    Order: IOrderMaster;
}

export default class EntityAgent {


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

    private async CreateItem(tran: string, item: DbEntityBase): Promise<IResponseMessage> {
        let response: IResponseMessage;
        let ax: AxiosAgent = new AxiosAgent();
        response = await ax.createItem(tran, item);
        return response;
    }

    async SaveOrder(item: IOrderMaster): Promise<IResponseMessage> {
        let result: IResponseMessage;
        if (item.IsNew)
            result = await this.UpdateItem("ORDER", item);
        else
            result = await this.CreateItem("ORDER", item);
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

        let result: ICustomerDisplay = { Customer:cust, Regions:regionList, init:false };
        return result;
    }

    async GetOrderEntryObjects(id:number, executeGet:boolean): Promise<IOrderDisplay> {

        let response: IResponseMessage;

        let cusList: IMaestroCustomer[];
        let prodList: IQbProductMap[];
        let order: IOrderMaster;


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

    



}