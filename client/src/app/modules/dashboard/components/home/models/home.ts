import { AuthDetails } from "src/app/shared/models/auth";

export interface Home {
    id: number
    raffleDate: string
    startOn: string
    endOn: string
    custStatus: number
    ticketNo: number
    raffleName: string
    rafflePrice: number
    authDto: AuthDetails
    drawCount: number
}