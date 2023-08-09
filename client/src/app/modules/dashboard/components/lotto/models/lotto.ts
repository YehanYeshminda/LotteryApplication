import { AuthDetails } from "@shared/models/auth"

export interface GetLotto {
    lottoNo: string
}

export interface Company {
    id: number
    companyName: string
    companyCode: string
}

export interface BuyLotto {
    authDto: AuthDetails | null
    lottoNumber: string
}