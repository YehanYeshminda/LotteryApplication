import { AuthDetails } from "@shared/models/auth"

export interface WithDrawal {
    id: number
    bankId: number
    amount: number
    userId: number
    requestUniqueId: string
    longitude: string
    latitude: string
    status: string
}

export interface MakeWithdrawalRequest {
    benificiaryAccountNo: string
    benificiaryIfscCode: string
    benificiaryName: string
    amount: number
    longitude: string
    latitude: string
    authDto: AuthDetails
}