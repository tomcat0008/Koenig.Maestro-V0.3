import * as React from 'react';
import { ITranComponentProp } from '../../classes/ITranComponentProp';
import CustomerProductUnit, { ICustomerProductUnit } from '../../classes/dbEntities/ICustomerProductUnit';
import EntityAgent, { ICustomerProductUnitDisplay } from '../../classes/EntityAgent';
import { ICrudComponent } from '../ICrudComponent';
import { IResponseMessage } from '../../classes/ResponseMessage';
import { Form, Col } from 'react-bootstrap';
import { IMaestroCustomer } from '../../classes/dbEntities/IMaestroCustomer';
import { IMaestroProduct } from '../../classes/dbEntities/IMaestroProduct';
import { IMaestroUnit } from '../../classes/dbEntities/IMaestroUnit';
import ErrorInfo from '../../classes/ErrorInfo';

export default class CustomerProductUnitsComponent extends React.Component<ITranComponentProp, ICustomerProductUnitDisplay> implements ICrudComponent  {

    state = { Customers: [], Products: [], Units: [], Init: true, ErrorInfo: new ErrorInfo(), ProductId: 0, Entity:new CustomerProductUnit(0), UnitTypeId:0 };

    constructor(props: ITranComponentProp) {
        super(props);
        this.state = {
            ProductId: props.Entity.Id, Customers: [], Products: [], Units: [], Init: true, ErrorInfo: new ErrorInfo(),
            Entity: this.props.Entity as ICustomerProductUnit, UnitTypeId: (this.props.Entity as ICustomerProductUnit).UnitTypeId
        };
        //props.Entity.IsNew = props.Entity.Id > 0;

    }

    async Cancel(): Promise<IResponseMessage> {
        return null;
    }

    async Integrate(): Promise<IResponseMessage> {
        return null;
    }

    async Save(): Promise<IResponseMessage> {

        let cpu: ICustomerProductUnit = new CustomerProductUnit(0);



        let ea: EntityAgent = new EntityAgent();
        cpu.Id = this.props.Entity.Id;
        cpu.ProductId = parseInt((document.getElementById("cpuProductId") as HTMLSelectElement).value);
        cpu.UnitId = parseInt((document.getElementById("cpuUnitId") as HTMLSelectElement).value);
        cpu.CustomerId = parseInt((document.getElementById("cpuCustomerId") as HTMLSelectElement).value);

        this.DisableEnable(true);
        let result: IResponseMessage = await ea.SaveCustomerProductUnit(cpu);
        if (result.ErrorInfo != null) {
            this.DisableEnable(false);
            throw result.ErrorInfo;
            //this.props.ExceptionMethod(result.ErrorInfo);
        }
            
        else {
            if (cpu.Id <= 0) {
                cpu.Id = ((result.TransactionResult as CustomerProductUnit).Id);
                (document.getElementById("cpuId") as HTMLInputElement).value = cpu.Id.toString();
            }
            cpu.IsNew = false;

            
            return result;
        }
    }

    DisableEnable(disable:boolean): void {
        (document.getElementById("cpuProductId") as HTMLSelectElement).disabled = disable;
        (document.getElementById("cpuUnitId") as HTMLSelectElement).disabled = disable;
        (document.getElementById("cpuCustomerId") as HTMLSelectElement).disabled = disable;
    }

    async componentDidMount() {
        
        let ea: EntityAgent = new EntityAgent();

        let cd: ICustomerProductUnitDisplay = await ea.GetCustomerProductUnitDisplay();
        if (cd.ErrorInfo != null) {
            if (cd.ErrorInfo.UserFriendlyMessage != "") {
                this.props.ExceptionMethod(cd.ErrorInfo);
            }
            else {
                cd.Init = false;
                if (this.props.Entity.Id > 0) {
                    cd.Entity = this.props.Entity as ICustomerProductUnit;
                    cd.ProductId = (this.props.Entity as ICustomerProductUnit).ProductId;
                    cd.UnitTypeId = (this.props.Entity as ICustomerProductUnit).UnitTypeId;
                }
                this.setState(cd);
            }
        }
        else {
            cd.Init = false;
            if (this.props.Entity.Id > 0) {
                cd.Entity = this.props.Entity as ICustomerProductUnit;
                cd.ProductId = (this.props.Entity as ICustomerProductUnit).ProductId;
                cd.UnitTypeId = (this.props.Entity as ICustomerProductUnit).UnitTypeId;
            }
            this.setState(cd);
        }

        this.props.ButtonSetMethod(cd.Entity.Actions);
    }

    

    onProductChange = (evt) => {
        let product: IMaestroProduct = (this.state.Products as IMaestroProduct[]).find(p => p.Id == parseInt(evt.target.value)) ;
        this.setState({ ProductId: evt.target.value, UnitTypeId: product.UnitTypeId });
        (document.getElementById("cpuUnitId") as HTMLSelectElement).disabled = false;
    }

    render() {

        //units.filter(u => u.UnitTypeId == products.find(p => p.Id == this.state.ProductId).UnitTypeId).map(unit => <option value={unit.Id}>{unit.Name} </option>)

        if (!this.state.Init) {

            let customers: IMaestroCustomer[] = this.state.Customers;
            let products: IMaestroProduct[] = (this.state.Products as IMaestroProduct[]).filter(p => p.UnitTypeCanHaveUnits);
            let units: IMaestroUnit[] = this.state.Units;
            let cpu: ICustomerProductUnit = this.state.Entity as ICustomerProductUnit

            if (customers.find(c => c.Id == -1) == undefined)
                customers.unshift(EntityAgent.GetFirstSelecItem("CUSTOMER") as IMaestroCustomer);
            if (products.find(c => c.Id == -1) == undefined)
                products.unshift(EntityAgent.GetFirstSelecItem("PRODUCT") as IMaestroProduct);
            if (units.find(c => c.Id == -1) == undefined)
                units.unshift(EntityAgent.GetFirstSelecItem("UNIT") as IMaestroUnit);

            products.sort((a, b) => { return a.Name.localeCompare(b.Name); });
            customers.sort((a, b) => { return a.Name.localeCompare(b.Name); });
            units.sort((a, b) => { return a.Name.localeCompare(b.Name); });

            

            let options;
            if (this.state.UnitTypeId > 0) {
                //(document.getElementById("cpuUnitId") as HTMLSelectElement).disabled = false;
                options = units.filter(u => u.UnitTypeId == this.state.UnitTypeId || u.Id == -1).map(unit => <option selected={unit.Id == cpu.UnitId} value={unit.Id}>{unit.Name} </option>);
            }


            return (
                <div className="container">
                    <Form>

                        <Form.Group as={Col} controlId="itemId">
                            <Form.Label >Customer Product Unit Id</Form.Label>
                            <Form.Control className="modal-disabled" id="cpuId" plaintext readOnly defaultValue={cpu.IsNew ? "New record" : cpu.Id.toString()} />
                        </Form.Group>
                        <Form.Group as={Col} controlId="customerDropDownId">
                            <Form.Label>Customer</Form.Label>
                            <Form.Control as="select" id="cpuCustomerId" >
                                {
                                    customers.map(customer => <option selected={customer.Id == cpu.CustomerId} value={customer.Id}>{customer.Name + " (" + customer.QuickBoosCompany  + ")"} </option>)
                                }
                            </Form.Control>
                        </Form.Group>
                        <Form.Group as={Col} controlId="productDropDownId">
                            <Form.Label>Product</Form.Label>
                            <Form.Control as="select" id="cpuProductId" onChange={this.onProductChange}>
                                {
                                    products.map(product => <option selected={product.Id == cpu.ProductId} value={product.Id}>{product.Name} </option>)
                                }
                            </Form.Control>
                        </Form.Group>
                        <Form.Group as={Col} controlId="unitDropDownId">
                            <Form.Label>Unit</Form.Label>
                            <Form.Control as="select" id="cpuUnitId" disabled={cpu.IsNew}>
                                {options}
                            </Form.Control>
                        </Form.Group>


                    </Form>
                    
                </div>);
        }
        else {
            return (<div></div>);
        }
    }

}