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

export interface ComboData {
    id: number
    value: string
}

export interface ExistingBankDetails {
    benificiaryAccountNo: string
    benificiaryIfscCode: string
    benificiaryName: string
    upiid: string
}

export interface MakeRequestToForBankDetials {
    authDto: AuthDetails
    id: number
}

export interface EditExistingBankDetails {
    benificiaryAccountNo: string
    benificiaryIfscCode: string
    benificiaryName: string
    amount: number
    upiId: string
    authDto: AuthDetails
}