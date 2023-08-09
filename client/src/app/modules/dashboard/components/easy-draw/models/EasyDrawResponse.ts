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