import { AuthDetails } from "@shared/models/auth"

export interface GenerateUpi {
    authDto: AuthDetails | null
    orderNo: string
    total: string
}

export interface UpiResponse {
    qr: string
}