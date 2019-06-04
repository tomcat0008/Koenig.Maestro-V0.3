export interface IDashboardMenuItem {
    caption: string,
    action: string,
    tranCode: string,
    msgExtension: { [key: string]: string },
    buttonList: string[],
    listSelect: boolean,
}

export default class DashboardMenuItem implements IDashboardMenuItem {

    caption: string;
    action: string;
    tranCode: string;
    msgExtension: { [key: string]: string; };
    buttonList: string[];
    listSelect: boolean;


}