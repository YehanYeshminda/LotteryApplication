import { AuthDetails } from "@shared/models/auth";

export interface EasyDrawResponse {
    result: number[];
}

export interface BuyEasyDraw {
    raffleId: string
    ticketNo: string
    authDto: AuthDetails | null
}

export interface Root<T> {
    result: T;
    isSuccess: boolean;
    message: string;
}

export interface GetDrawResult {
    id: number
    raffleId: number
    ticketNo: string
    userId: number
    raffleUniqueId: string
    addOn: string
    lotteryReferenceId: string
}

export interface FullEasyDraw {
    id: number
    raffleDate: string
    startOn: string
    endOn: string
    custStatus: number
    ticketNo: string
    raffleName: string
    rafflePrice: number
    drawCount: number
    uniqueRaffleId: string
    winAmount: number
}