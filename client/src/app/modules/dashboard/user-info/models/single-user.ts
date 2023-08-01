import { AuthDetails } from "src/app/shared/models/auth"

export interface SingleUserInfo {
    id: number
    custName: string
    nic: string
    email: string
    custAddress: string
    mobile: string
    alternatePhone: string
    contactNo: string
    otp: string
    custPassword: string
    addOn: string
    photo: string
    custStatus: number
    accountBalance: any
    hash: string
}

export interface UpdateSingleUserInfo {
    custName: string
    nic: string
    email: string
    custAddress: string
    mobile: string
    alternatePhone: string
    contactNo: string
    authDto?: AuthDetails | null
}