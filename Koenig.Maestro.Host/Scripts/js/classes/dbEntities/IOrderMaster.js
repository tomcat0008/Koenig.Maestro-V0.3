export default class OrderMaster {
    constructor(id) {
        this.Id = id;
        this.OrderDate = new Date();
        this.DeliveryDate = new Date();
        this.DeliveryDate.setDate(this.OrderDate.getDate() + 1);
    }
}
//# sourceMappingURL=IOrderMaster.js.map