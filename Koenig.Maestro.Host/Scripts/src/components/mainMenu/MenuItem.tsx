import * as React from 'react';

export interface IMenuItemDef {
    imgName: string,
    caption: string,
    itemType: string,
    width: string,
    height: string,
    action: string, 
    tranCode: string,
    msgExtension: { [key: string]: string },
    eventHandler(mi: MenuItem): void,
    buttonList: string[],
    listSelect:boolean,
}

export default class MenuItem extends React.Component<IMenuItemDef>
{

    constructor(props: IMenuItemDef) {
        super(props);
    }

    render() {

        var styleSet = { padding: '1px',  height: `${this.props.height}` } as React.CSSProperties;
        
        let itemBody = <div style={styleSet} > <a onClick={() => { this.props.eventHandler(this) } } data-toggle="modal" data-target="#myModal" title={this.props.caption} className="btn btn-lg btn-primary" style={{ width: '100%', height: '100%', }}>
        <img src={"../../../img/" + this.props.imgName} alt={this.props.caption} />
        {this.props.caption} </a></div>


        return itemBody;
    }




}
