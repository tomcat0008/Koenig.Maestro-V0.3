import * as React from "react";
import { ITranComponentProp } from "../../classes/ITranComponentProp";
import EntityAgent, { IRegionDisplay } from "../../classes/EntityAgent";
import { ICrudComponent } from "../ICrudComponent";
import ErrorInfo from "../../classes/ErrorInfo";
import  { IResponseMessage } from "../../classes/ResponseMessage";
import MaestroRegion, { IMaestroRegion } from "../../classes/dbEntities/IMaestroRegion";
import { Form, Row, Col } from "react-bootstrap";

export default class RegionComponent extends React.Component<ITranComponentProp, IRegionDisplay> implements ICrudComponent {

    state = { Region: null, Init: true, ErrorInfo: new ErrorInfo() }

    constructor(props: ITranComponentProp) {
        super(props);
        this.state = { Region: props.Entity as IMaestroRegion, Init: true, ErrorInfo: new ErrorInfo() }
    }

    async componentDidMount() {
        let ea: EntityAgent = new EntityAgent();
        let display: IRegionDisplay = await ea.GetRegionDisplay(this.props.Entity.Id);

        this.Save = this.Save.bind(this);
        this.setState(display);
        this.props.ButtonSetMethod(display.Region.Actions);
    }

    async Cancel(): Promise<IResponseMessage> {
        return null;
    }

    async Integrate(): Promise<IResponseMessage> {
        return null;
    }

    async Save(): Promise<IResponseMessage> {

        this.DisableEnable(true);
        let ea: EntityAgent = new EntityAgent();
        let region: IMaestroRegion = this.state.Region;
        region.Name = (document.getElementById("regionNameId") as HTMLInputElement).value;
        region.Description = (document.getElementById("regionDescId") as HTMLInputElement).value;
        region.PostalCode = (document.getElementById("regionPKId") as HTMLInputElement).value
        region.GreaterRegion = (document.getElementById("greaterRegionId") as HTMLInputElement).value
        
        let result: IResponseMessage = await ea.SaveRegion(region);

        if (result.ErrorInfo != null) {
            this.DisableEnable(false);
            throw result.ErrorInfo;

        }
        else {
            if (region.Id <= 0) {
                region.Id = ((result.TransactionResult as MaestroRegion).Id);
                (document.getElementById("regionId") as HTMLInputElement).value = region.Id.toString();
            }
            region.IsNew = false;
        }


        return result;

    }

    DisableEnable(disable: boolean) {

        (document.getElementById("regionNameId") as HTMLInputElement).disabled = disable;
        (document.getElementById("regionPKId") as HTMLInputElement).disabled = disable;
        (document.getElementById("regionDescId") as HTMLInputElement).disabled = disable;
        (document.getElementById("greaterRegionId") as HTMLInputElement).disabled = disable;
        
    }

    render() {
        let region: IMaestroRegion = this.state.Region;

        if (this.state.Init) {
            return (<p></p>);
        }
        else {

            return (
                <div className="container">
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Region Id</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="regionId" plaintext readOnly defaultValue={region.IsNew ? "New Region" : region.Id.toString()} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Region Name</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="regionNameId" type="input" defaultValue={region.Name} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Postal Code</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="regionPKId" type="input" defaultValue={region.PostalCode} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Region Description</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="regionDescId" type="input" defaultValue={region.Description} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Greater Region</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="greaterRegionId" type="input" defaultValue={region.GreaterRegion} />
                        </Col>
                    </Row>
                </div>
            );
        }
    }

}