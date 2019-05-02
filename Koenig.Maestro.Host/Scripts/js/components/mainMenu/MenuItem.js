import * as React from 'react';
export default class MenuItem extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        var styleSet = { padding: '1px', height: `${this.props.height}` };
        let itemBody = React.createElement("div", { style: styleSet },
            " ",
            React.createElement("a", { onClick: () => { this.props.eventHandler(this); }, "data-toggle": "modal", "data-target": "#myModal", title: this.props.caption, className: "btn btn-lg btn-primary", style: { width: '100%', height: '100%', } },
                React.createElement("img", { src: "/Maestro/img/" + this.props.imgName, alt: this.props.caption }),
                this.props.caption,
                " "));
        return itemBody;
    }
}
//# sourceMappingURL=MenuItem.js.map