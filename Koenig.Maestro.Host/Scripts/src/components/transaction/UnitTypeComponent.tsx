import * as React from "react";
import { ITranComponentProp } from "../../classes/ITranComponentProp";
import EntityAgent, { IUnitTypeDisplay } from "../../classes/EntityAgent";
import { ICrudComponent } from "../ICrudComponent";
import ErrorInfo from "../../classes/ErrorInfo";
import { IResponseMessage } from "../../classes/ResponseMessage";
import MaestroUnitType, { IMaestroUnitType } from "../../classes/dbEntities/IMaestroUnitType";
import { Form, Row, Col } from "react-bootstrap";

export default class UnitTypeComponent extends React.Component<ITranComponentProp, IUnitTypeDisplay> implements ICrudComponent {


    state = { UnitType: null, Init: true, ErrorInfo: new ErrorInfo() }

    constructor(props: ITranComponentProp) {
        super(props);
        this.state = { UnitType: props.Entity as IMaestroUnitType, Init: true, ErrorInfo: new ErrorInfo() }
    }

    async componentDidMount() {
        let ea: EntityAgent = new EntityAgent();
        let display: IUnitTypeDisplay = await ea.GetUnitTypeDisplay(this.props.Entity.Id);

        this.Save = this.Save.bind(this);
        this.setState(display);
        this.props.ButtonSetMethod(display.UnitType.Actions);

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
        let unitType: IMaestroUnitType = this.state.UnitType;

        unitType.Name = (document.getElementById("unitTypeNameId") as HTMLInputElement).value;
        unitType.Description = (document.getElementById("unitTypeDescId") as HTMLInputElement).value;
        unitType.CanHaveUnits = (document.getElementById("chkCanHaveUnits") as HTMLInputElement).checked;

        let result: IResponseMessage = await ea.SaveUnitType(unitType);
        
        if (result.ErrorInfo != null) {
            this.DisableEnable(false);
            throw result.ErrorInfo;
            //this.props.ExceptionMethod(result.ErrorInfo);
        }
        else {
            if (unitType.Id <= 0) {
                unitType.Id = ((result.TransactionResult as MaestroUnitType).Id);
                (document.getElementById("unitTypeId") as HTMLInputElement).value = unitType.Id.toString();
            }
            unitType.IsNew = false;
        }


        return result;

    }

    DisableEnable(disable: boolean) {
        
        (document.getElementById("unitTypeNameId") as HTMLInputElement).disabled = disable;
        (document.getElementById("unitTypeDescId") as HTMLInputElement).disabled = disable;
        (document.getElementById("chkCanHaveUnits") as HTMLInputElement).disabled = disable;
    }

    render() {
        let unitType: IMaestroUnitType = this.state.UnitType;

        if (this.state.Init) {
            return (<p></p>);
        }
        else {

            return (
                <div className="container">
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Unit Type Id</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="unitTypeId" plaintext readOnly defaultValue={unitType.IsNew ? "New Unit Type" : unitType.Id.toString()} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Unit Type Name</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="unitTypeNameId" type="input" defaultValue={unitType.Name} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Unit Type Description</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="unitTypeDescId" type="input" defaultValue={unitType.Description} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Can have units</Col>
                        <Col>
                            <Form.Check aria-label="chkCanHaveUnits" id="chkCanHaveUnits" checked={unitType.CanHaveUnits} />
                        </Col>
                    </Row>

                </div>
            );
        }
        
    }

}