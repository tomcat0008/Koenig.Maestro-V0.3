import * as React from 'react';
import MaestroCustomerComponent from './MaestroCustomerComponent';
export default class GridRowExpandDisplay extends React.Component {
    constructor(props) {
        super(props);
        this.pr = props;
    }
    render() {
        if (this.pr.data.TypeName == "MaestroCustomer") {
            return (React.createElement(MaestroCustomerComponent, Object.assign({}, this.pr.data)));
        }
        else
            return (React.createElement("p", null, "not ready yet"));
    }
}
//# sourceMappingURL=GridRowExpandDisplay.js.map