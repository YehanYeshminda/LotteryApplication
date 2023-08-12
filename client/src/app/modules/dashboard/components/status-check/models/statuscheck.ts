import { AuthDetails } from "@shared/models/auth";

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

export interface MakeRequestToCheckUpdate {
    authDto: AuthDetails | null
    transactionId: string
}