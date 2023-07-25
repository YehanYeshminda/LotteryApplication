import { AuthDetails } from "src/app/shared/models/auth"

export interface PaymentDoneModel {
    authDto: AuthDetails
    raffleNo: string
    matchCount: number
    credit: number
    paid: number
}

export interface PaymentResponseModel {
    raffleNo: string
}