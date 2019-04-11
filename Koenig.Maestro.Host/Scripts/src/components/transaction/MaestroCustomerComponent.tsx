import * as React from 'react';
import { Form, Col, Dropdown, FormCheck, InputGroup } from 'react-bootstrap';
import { Row } from 'react-bootstrap';
import MaestroCustomer, { IMaestroCustomer } from '../../classes/dbEntities/IMaestroCustomer';
import EntityAgent, { ICustomerDisplay } from '../../classes/EntityAgent';
import MaestroRegion, { IMaestroRegion } from '../../classes/dbEntities/IMaestroRegion';
import { IResponseMessage } from '../../classes/ResponseMessage';
import { ICrudComponent } from '../ICrudComponent';
import ErrorInfo from '../../classes/ErrorInfo';
import { ITranComponentProp } from '../../classes/ITranComponentProp';

export default class MaestroCustomerComponent extends React.Component<ITranComponentProp, ICustomerDisplay> implements ICrudComponent {

    state = { Customer: null, Regions: [], Init: true, ErrorInfo: new ErrorInfo() }

    //item:IMaestroCustomer;
    constructor(props: ITranComponentProp) {
        super(props);
        //this.setState({ Customer: props.Entity as IMaestroCustomer, Regions: [], Init: true, ErrorInfo: new ErrorInfo()  });
    }

    async componentDidMount() {
        let ea: EntityAgent = new EntityAgent();
        let cd: ICustomerDisplay = await ea.GetCustomerDisplay(this.props.Entity.Id);
        
        this.Save = this.Save.bind(this);
        this.setState(cd);
        if(this.props.Entity.IsNew)
            (document.getElementById("customerId") as HTMLInputElement).value = '';

        this.props.ButtonSetMethod(cd.Customer.Actions);
    }

    async Cancel(): Promise<IResponseMessage> {
        return null;
    }

    async Integrate(): Promise<IResponseMessage> {
        return null;
    }

    async Save(): Promise<IResponseMessage> {
        let ea: EntityAgent = new EntityAgent();
        let cust = new MaestroCustomer(0);
        cust.Id = this.state.Customer.Id;
        cust.Address = (document.getElementById("customerAddressId") as HTMLInputElement).value;
        cust.Email = (document.getElementById("customerEmailId") as HTMLInputElement).value;
        
        cust.Name = (document.getElementById("customerNameId") as HTMLInputElement).value;
        cust.Phone = (document.getElementById("customerPhoneId") as HTMLInputElement).value;
        cust.Title = (document.getElementById("customerTitleId") as HTMLInputElement).value;
        cust.QuickBooksId = (document.getElementById("qbListId") as HTMLInputElement).value;
        cust.QuickBoosCompany = (document.getElementById("qbCompanyId") as HTMLInputElement).value;
        cust.DefaultPaymentType = this.state.Customer.DefaultPaymentType == null ? "" : this.state.Customer.DefaultPaymentType;
        cust.MaestroRegion = new MaestroRegion(parseInt((document.getElementById("customerRegionId") as HTMLInputElement).value));
        cust.DefaultPaymentType = (document.getElementById("defaultPaymentId") as HTMLInputElement).checked ? "COD" : "";
        cust.CustomerGroup = (document.getElementById("customerGroupId") as HTMLInputElement).value;
        cust.ReportGroup = (document.getElementById("reportGroupId") as HTMLInputElement).value;
        let result: IResponseMessage = await ea.SaveCustomer(cust);
        this.DisableEnable(true);
        if (result.ErrorInfo != null) {
            this.DisableEnable(false);
            throw result.ErrorInfo;
            //this.props.ExceptionMethod(result.ErrorInfo);
        }
        else {
            if (cust.Id <= 0) {
                cust.Id = ((result.TransactionResult as MaestroCustomer).Id);
                (document.getElementById("customerIdId") as HTMLInputElement).value = cust.Id.toString();
            }
            cust.IsNew = false;

            this.DisableEnable(true);
            return result;
        }
    }


    DisableEnable(disable:boolean) {
        (document.getElementById("customerIdId") as HTMLInputElement).disabled = disable;
        (document.getElementById("qbCompanyId") as HTMLInputElement).disabled = disable;
        (document.getElementById("customerNameId") as HTMLInputElement).disabled = disable;
        (document.getElementById("qbListId") as HTMLInputElement).disabled = disable;
        (document.getElementById("customerTitleId") as HTMLInputElement).disabled = disable;
        (document.getElementById("customerRegionId") as HTMLInputElement).disabled = disable;
        (document.getElementById("customerAddressId") as HTMLInputElement).disabled = disable;
        (document.getElementById("customerEmailId") as HTMLInputElement).disabled = disable;
        (document.getElementById("customerPhoneId") as HTMLInputElement).disabled = disable;
        (document.getElementById("defaultPaymentId") as HTMLInputElement).disabled = disable;
        (document.getElementById("customerGroupId") as HTMLInputElement).disabled = disable;
        (document.getElementById("reportGroupId") as HTMLInputElement).disabled = disable;
        
    }

    render() {
        let cus: IMaestroCustomer = this.state.Customer;
        let regions: IMaestroRegion[] = this.state.Regions;
        if (regions.find(c => c.Id == -1) == undefined)
            regions.unshift(EntityAgent.GetFirstSelecItem("REGION") as IMaestroRegion);

        if (this.state.Init) {
            return (<p></p>);
        }
        else {
            let qbCompany, qbId;
            if (this.props.Entity.IsNew) {
                qbCompany = <Form.Control id="qbCompanyId" type="input" />
                qbId = <Form.Control id="qbListId" type="input" />
            }
            else {
                qbCompany = <Form.Control id="qbCompanyId" plaintext readOnly className="modal-disabled" defaultValue={cus.QuickBoosCompany} />;
                qbId = <Form.Control id="qbListId" plaintext readOnly className="modal-disabled" defaultValue={cus.QuickBooksId} />
            }
            return (
                <Form>

                        <Form.Row>
                            <Form.Group as={Col} controlId="customerId">
                            <Form.Label >Customer Id</Form.Label>
                            <Form.Control className="modal-disabled" id="customerIdId" plaintext readOnly defaultValue={cus.IsNew ? "New customer" : cus.Id.toString()} />
                            </Form.Group>
                        </Form.Row>
                        <Form.Row>


                        <Form.Group as={Col} controlId="customerName">
                            <Form.Label>Customer Name</Form.Label>
                            <Form.Control id="customerNameId" type="input" defaultValue={cus.Name} />
                        </Form.Group>

                        <Form.Group as={Col} controlId="customerTitle">
                            <Form.Label>Title</Form.Label>
                            <Form.Control id="customerTitleId" type="input" defaultValue={cus.Title} />
                        </Form.Group>

                        
                    </Form.Row>
                    <Form.Row>
                        <Form.Group as={Col} controlId="customerGroup">
                            <Form.Label>Customer Group</Form.Label>
                            <Form.Control id="customerGroupId" type="input" defaultValue={cus.CustomerGroup} />
                        </Form.Group>
                        <Form.Group as={Col} controlId="customerReport">
                            <Form.Label>Report Group</Form.Label>
                            <Form.Control id="reportGroupId" type="input" defaultValue={cus.ReportGroup} />
                        </Form.Group>
                    </Form.Row>
                    <Form.Row>
                        <Form.Group as={Col} controlId="customerEmail">
                            <Form.Label>Email</Form.Label>
                            <Form.Control type="input" id="customerEmailId" defaultValue={cus.Email} />
                        </Form.Group>
                        <Form.Group as={Col} controlId="customerPhone">
                            <Form.Label>Phone</Form.Label>
                            <Form.Control type="input" id="customerPhoneId" defaultValue={cus.Phone} />
                        </Form.Group>
                        <Form.Group as={Col} controlId="customerRegion2">
                            <Form.Label>Region</Form.Label>
                            <Form.Control as="select" id="customerRegionId">
                                {
                                    regions.map(rg => <option selected={rg.Id == cus.RegionId} value={rg.Id}>{rg.Name + " (" + rg.PostalCode + ")"} </option>)
                                }
                            </Form.Control>
                        </Form.Group>

                    </Form.Row>
                    <Form.Row>
                        <Form.Group as={Col} controlId="customerAddress">
                            <Form.Label>Address</Form.Label>
                            <Form.Control as="textarea" id="customerAddressId" rows="3">{cus.Address}</Form.Control>
                        </Form.Group>
                    </Form.Row>
                    <Form.Row>
                        <Form.Group as={Col} controlId="customerQbCompany">
                            <Form.Label>QB Company</Form.Label>
                            {qbCompany}
                        </Form.Group>
                        <Form.Group as={Col} controlId="customerQbId">
                            <Form.Label>QB Object Id</Form.Label>
                            {qbId}
                        </Form.Group>
                    </Form.Row>
                    <Form.Group id="defaultPayment">
                        <Form.Check type="checkbox" id="defaultPaymentId" label="C.O.D." defaultChecked={cus.DefaultPaymentType=="COD"} />
                    </Form.Group>

                </Form>
                
            );
        }
    }


}