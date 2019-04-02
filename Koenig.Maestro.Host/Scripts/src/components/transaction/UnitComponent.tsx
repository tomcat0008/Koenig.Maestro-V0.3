import * as React from "react";
import { ITranComponentProp } from "../../classes/ITranComponentProp";
import EntityAgent, { IUnitDisplay } from "../../classes/EntityAgent";
import { ICrudComponent } from "../ICrudComponent";
import ErrorInfo from "../../classes/ErrorInfo";
import { IResponseMessage } from "../../classes/ResponseMessage";
import { Form, Row, Col } from "react-bootstrap";
import MaestroUnit, { IMaestroUnit } from "../../classes/dbEntities/IMaestroUnit";
import { IMaestroUnitType } from "../../classes/dbEntities/IMaestroUnitType";

export default class UnitComponent extends React.Component<ITranComponentProp, IUnitDisplay> implements ICrudComponent {

    state = { Unit: null, UnitTypes:[], Init: true, ErrorInfo: new ErrorInfo() }

    constructor(props: ITranComponentProp) {
        super(props);
        this.state = { Unit: props.Entity as IMaestroUnit, UnitTypes: [], Init: true, ErrorInfo: new ErrorInfo() }
    }

    async componentDidMount() {
        let ea: EntityAgent = new EntityAgent();
        let display: IUnitDisplay = await ea.GetUnitDisplay(this.props.Entity.Id);

        this.Save = this.Save.bind(this);
        this.setState(display);
        this.props.ButtonSetMethod(display.Unit.Actions);
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
        let unit: IMaestroUnit = this.state.Unit;
        unit.UnitTypeId = parseInt((document.getElementById("unitTypeId") as HTMLSelectElement).value);
        unit.Name = (document.getElementById("unitNameId") as HTMLInputElement).value;
        unit.QuickBooksUnit = (document.getElementById("qbUnitId") as HTMLInputElement).value;
        
        let result: IResponseMessage = await ea.SaveUnit(unit);

        if (result.ErrorInfo != null) {
            this.DisableEnable(false);
            throw result.ErrorInfo;
        }
        else {
            if (unit.Id <= 0) {
                unit.Id = ((result.TransactionResult as MaestroUnit).Id);
                (document.getElementById("unitId") as HTMLInputElement).value = unit.Id.toString();
            }
            unit.IsNew = false;
        }


        return result;

    }

    DisableEnable(disable: boolean) {
        (document.getElementById("unitNameId") as HTMLInputElement).disabled = disable;
        (document.getElementById("unitTypeId") as HTMLSelectElement).disabled = disable;
        (document.getElementById("qbUnitId") as HTMLInputElement).disabled = disable;
    }

    render() {

        let unit: IMaestroUnit = this.state.Unit;
        let unitTypes: IMaestroUnitType[] = (this.state.UnitTypes as IMaestroUnitType[]).filter(u => u.CanHaveUnits).sort((a, b) => a.Name.localeCompare(b.Name));
        if (unitTypes.find(c => c.Id == -1) == undefined)
            unitTypes.unshift(EntityAgent.GetFirstSelecItem("UNIT_TYPE") as IMaestroUnitType);


        if (this.state.Init) {
            return (<p></p>);
        }
        else {

            return (
                <div className="container">
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Unit Id</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="unitId" plaintext readOnly defaultValue={unit.IsNew ? "New Region" : unit.Id.toString()} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Unit Name</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="unitNameId" type="input" defaultValue={unit.Name} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Unit Type</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control as="select" id="unitTypeId">
                            {
                                unitTypes.map(ut => <option selected={ut.Id == unit.UnitTypeId} value={ut.Id}>{ut.Name} </option>)
                            }
                            </Form.Control>
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Quickbooks Unit</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control id="qbUnitId" type="input" defaultValue={unit.QuickBooksUnit} />
                        </Col>
                    </Row>

                </div>
            );
        }
    }

}