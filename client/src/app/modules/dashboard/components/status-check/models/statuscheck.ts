export interface CheckStatusResult {
    status: string;
}

export interface StatusCheckData {
    packageName: string
    packageUniqueId: string
    packagePrice: string
    packageOrderUniqueId: string
    addOn: string
    orderStatus: number
}