var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import OrderMaster from '../../classes/dbEntities/IOrderMaster';
import ResponseMessage from '../../classes/ResponseMessage';
import AxiosAgent from '../../classes/AxiosAgent';
import EntityAgent from '../../classes/EntityAgent';
import { Form, Col } from 'react-bootstrap';
import { Row } from 'react-bootstrap';
export default class OrderComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { response: new ResponseMessage(), init: true, order: new OrderMaster(0) };
        this.setState({ order: props });
    }
    Disable() { }
    Save() {
        return __awaiter(this, void 0, void 0, function* () {
            let order = new OrderMaster(0);
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
            let ea = new EntityAgent();
            let result = yield ea.SaveOrder(order);
            return result;
        });
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            const exception = (msg) => {
                $('#errorDiv').show();
                $('#errorInfo').text(msg);
            };
            try {
                let response;
                let order;
                if (this.props.Id == 0) {
                    response = yield new AxiosAgent().getNewOrderId();
                    order = new OrderMaster(response.TransactionResult);
                }
                else {
                    response = yield new AxiosAgent().getItem(this.props.Id, "ORDER");
                    order = response.TransactionResult;
                }
                this.setState({ response: response, init: false, order: order });
                console.log(response);
            }
            catch (err) {
                exception(err);
            }
            $('#wait').hide();
        });
    }
    render() {
        if (!this.state.init) {
            return (React.createElement("div", { className: "container" },
                React.createElement(Form, null,
                    React.createElement(Form.Group, { as: Row, controlId: "orderId" },
                        React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Order Id"),
                        React.createElement(Col, { sm: "6" },
                            React.createElement(Form.Control, { plaintext: true, readOnly: true, defaultValue: this.state.order.Id.toString() }))),
                    React.createElement(Form.Group, { as: Row, controlId: "customerId" },
                        React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Customer"),
                        React.createElement(Col, { sm: "6" },
                            React.createElement(Form.Control, { type: "input", defaultValue: this.state.order.CustomerName }))))));
        }
        else
            return (React.createElement("div", null));
    }
}
//# sourceMappingURL=OrderComponent.js.map