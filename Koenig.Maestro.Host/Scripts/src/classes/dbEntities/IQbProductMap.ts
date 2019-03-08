export interface IQbProductMap {
    ProductId: number;
    ProductName: string;
    Price: number;
    FullName: string;
    QuickBooksDescription: string;
    UnitId: number;
    UnitName: string;

}

export class QbProductMap implements IQbProductMap {

    ProductId: number;    ProductName: string;
    Price: number;
    FullName: string;
    QuickBooksDescription: string;
    UnitId: number;
    UnitName: string;


}