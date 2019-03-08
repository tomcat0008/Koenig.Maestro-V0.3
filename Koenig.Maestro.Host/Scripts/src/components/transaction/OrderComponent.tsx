import * as React from 'react';
import { Form, Col } from 'react-bootstrap';
import { Row } from 'react-bootstrap';
import OrderMaster, { IOrderMaster } from '../../classes/dbEntities/IOrderMaster';
import ResponseMessage, { IResponseMessage } from '../../classes/ResponseMessage';
import AxiosAgent from '../../classes/AxiosAgent';
import { ICrudComponent } from '../ICrudComponent';
import EntityAgent from '../../classes/EntityAgent';

interface IOrderState { order: IOrderMaster, response: IResponseMessage, init:boolean  }

export default class OrderComponent extends React.Component<IOrderMaster, IOrderState> implements ICrudComponent {

    state = { response: new ResponseMessage(), init: true, order: new OrderMaster(0) };

    constructor(props: IOrderMaster) {
        super(props);
        this.setState({ order: props });
    }

    async Save(): Promise<IResponseMessage> {
        

        let order: OrderMaster = new OrderMaster(0);
        

/*
        let cust = new MaestroCustomer(0);
        cust.Id = this.state.Customer.Id;
        cust.Address = (document.getElementById("customerAddressId") as HTMLInputElement).value;
        cust.Email = (document.getElementById("customerEmailId") as HTMLInputElement).value;

        cust.Name = (document.getElementById("customerNameId") as HTMLInputElement).value;
        cust.Phone = (document.getElementById("customerPhoneId") as HTMLInputElement).value;
        cust.Title = (document.getElementById("customerTitleId") as HTMLInputElement).value;
        cust.QuickBooksId = this.state.Customer.QuickBooksId;
        cust.QuickBoosCompany = this.state.Customer.QuickBoosCompany;
        cust.DefaultPaymentType = this.state.Customer.DefaultPaymentType;
        cust.MaestroRegion = new MaestroRegion(parseInt((document.getElementById("customerRegionId") as HTMLInputElement).value));*/

        let ea: EntityAgent = new EntityAgent();
        let result: IResponseMessage = await ea.SaveOrder(order);
        return result;
    }

    async componentDidMount() {

        const exception = (msg: string) => {
            $('#errorDiv').show();
            $('#errorInfo').text(msg);
        };

        try {
            let response: IResponseMessage;
            let order: IOrderMaster;
            
            if (this.props.Id == 0) {
                response = await new AxiosAgent().getNewOrderId();
                order = new OrderMaster(response.TransactionResult);
            }
            else {
                response = await new AxiosAgent().getItem(this.props.Id, "ORDER");
                order = response.TransactionResult
            }

            this.setState({ response: response, init: false, order:order  });
            console.log(response);
        }
        catch(err)
        {
            exception(err);
        }


        $('#wait').hide();
    }


    render() {
        if (!this.state.init) {
            return (
                <div className="container">
                <Form>
                    <Form.Group as={Row} controlId="orderId">
                        <Col className="col-form-label" sm={3} as={Form.Label} >Order Id</Col>
                        <Col sm="6">
                            <Form.Control plaintext readOnly defaultValue={this.state.order.Id.toString()} />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row} controlId="customerId">
                        <Col className="col-form-label" sm={3} as={Form.Label}>Customer</Col>
                        <Col sm="6">
                            <Form.Control type="input" defaultValue={this.state.order.CustomerName} />
                        </Col>
                    </Form.Group>
                    </Form>
                    </div>
            );
        }
        else
            return(<div></div>)
    }

}