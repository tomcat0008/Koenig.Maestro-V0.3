import * as React from "react";

export interface IMenuItemDef { imgName: string, caption:string, itemType:string, width:string, height:string, action:string }

export default class MenuItem extends React.Component<IMenuItemDef>
{

    constructor(props: IMenuItemDef) {
        super(props);
    }

    render() {
        
        let itemBody;

        if (this.props.itemType == "button") {
            var styleSet = { padding: '1px', width: `${this.props.width}`, height: `${this.props.height}` } as React.CSSProperties ;
            itemBody = <div style={styleSet}>

                <a href={this.props.action} title={this.props.caption} className="btn btn-lg btn-primary" style={{ width: '100%', height: '100%', }}>
                    <img src={"../../../img/" + this.props.imgName} alt={this.props.caption} />
                    {this.props.caption} </a>


            </div>;
        }
        else {

        }
        
        return itemBody;
    }


}
