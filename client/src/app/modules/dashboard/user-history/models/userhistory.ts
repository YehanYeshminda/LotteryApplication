import { AuthDetails } from "src/app/shared/models/auth"

export interface UserHistoryResponse {
    raffleId: number
    ticketNumber: string
    uniqueRaffleId: string
    orderedOn: string
    referenceId: string
    isWin: boolean
}

export interface PagedList {
    pageNumber: number
    pageSize: number
    authDto: AuthDetails
}
