import * as React from 'react';
export default class MenuItem2 extends React.Component {
    constructor(props) {
        super(props);
        this.onClickEventHandler = this.onClickEventHandler.bind(this);
    }
    onClickEventHandler(id) {
        this.props.clickHandler(id);
    }
    render() {
        return (React.createElement("div", null,
            React.createElement("a", { onClick: () => this.onClickEventHandler(this) }, "sdfsfsdfsdfsdfsdfsdf")));
    }
}
//# sourceMappingURL=MenuItem2.js.map