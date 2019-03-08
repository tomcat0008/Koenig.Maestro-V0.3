import * as React from 'react';
import { Form, Col, Dropdown, FormCheck } from 'react-bootstrap';
import { Row } from 'react-bootstrap';
import MaestroCustomer, { IMaestroCustomer } from '../../classes/dbEntities/IMaestroCustomer';
import EntityAgent, { ICustomerDisplay } from '../../classes/EntityAgent';
import MaestroRegion, { IMaestroRegion } from '../../classes/dbEntities/IMaestroRegion';
import { IResponseMessage } from '../../classes/ResponseMessage';
import { ICrudComponent } from '../ICrudComponent';

export default class MaestroCustomerComponent extends React.Component<IMaestroCustomer, ICustomerDisplay> implements ICrudComponent {

    //item:IMaestroCustomer;
    constructor(props:IMaestroCustomer) {
        super(props);
        this.state = { Customer: props, Regions:[], init:true };
    }

    async componentDidMount() {
        let ea: EntityAgent = new EntityAgent();
        let cd: ICustomerDisplay = await ea.GetCustomerDisplay(this.props.Id);
        
        this.Save = this.Save.bind(this);
        this.setState(cd);
        if(this.props.IsNew)
            (document.getElementById("customerRegionId") as HTMLInputElement).value = '';
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
        let result: IResponseMessage = await ea.SaveCustomer(cust);
        return result;
    }

    render() {
        let cus: IMaestroCustomer = this.state.Customer;
        let regions: IMaestroRegion[] = this.state.Regions;

        if (this.state.init) {
            return (<p></p>);
        }
        else {
            let qbCompany, qbId;
            if (this.props.IsNew) {
                qbCompany = <Form.Control id="qbCompanyId" type="input" />
                qbId = <Form.Control id="qbListId" type="input" />
            }
            else {
                qbCompany = <Form.Control id="qbCompanyId" plaintext readOnly defaultValue={cus.QuickBoosCompany} />;
                qbId = <Form.Control id="qbListId" plaintext readOnly defaultValue={cus.QuickBooksId} />
            }
            return (
                <Form>
                    <Form.Group as={Row} controlId="customerId">
                        <Col className="col-form-label" sm={3} as={Form.Label} >Customer Id</Col>
                        <Col sm="6">
                            <Form.Control plaintext readOnly defaultValue={cus.IsNew ? "New customer" : cus.Id.toString()} />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row} controlId="customerName">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Customer Name</Col>
                        <Col sm="6">
                            <Form.Control id="customerNameId" type="input" defaultValue={cus.Name} />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row} controlId="customerTitle">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Title</Col>
                        <Col sm="6">
                            <Form.Control id="customerTitleId" type="input" defaultValue={cus.Title} />
                        </Col>
                    </Form.Group>

                    <Form.Group as={Row} controlId="customerRegion2">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Region</Col>
                        <Col sm="6">
                            <Form.Control as="select" id="customerRegionId">
                            {
                                regions.map(rg => <option selected={rg.Id == cus.RegionId} value={rg.Id}>{rg.Name +" ("+ rg.PostalCode + ")" } </option>)
                            }
                            </Form.Control>
                        </Col>
                    </Form.Group>


                    <Form.Group as={Row} controlId="customerAddress">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Address</Col>
                        <Col sm="6">
                            <Form.Control as="textarea" id="customerAddressId" rows="3">{cus.Address}</Form.Control>
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row} controlId="customerEmail">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Email</Col>
                        <Col sm="6">
                            <Form.Control type="input" id="customerEmailId" defaultValue={cus.Email} />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row} controlId="customerPhone">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Phone</Col>
                        <Col sm="6">
                            <Form.Control type="input" id="customerPhoneId" defaultValue={cus.Phone} />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row} controlId="customerQbCompany">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Qb Company</Col>
                        <Col sm="6">
                            {qbCompany}
                        </Col>
                    </Form.Group>

                    <Form.Group as={Row} controlId="customerQbId">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Qb Object Id</Col>
                        <Col sm="6">
                            {qbId}
                            
                        </Col>
                    </Form.Group>

                    <Form.Group as={Row} controlId="defaultPayment">
                        <Col className="col-form-label" sm={3} as={Form.Label}>C.O.D.</Col>
                        <Col sm="6">
                            <input
                                id="defaultPaymentId"
                                type="checkbox"
                                checked={cus.DefaultPaymentType == "COD"}
                            />

                        </Col>
                        
                    </Form.Group>


                </Form>
                
            );
        }
    }


}