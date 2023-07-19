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

export interface OldRafflesReponse {
    id: number | undefined
    lotteryId: number | undefined
    drawDate: string | undefined
    sequence: string | undefined
}