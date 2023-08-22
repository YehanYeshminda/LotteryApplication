export interface UserTransaction {
    id: number
    username: string
    ticketNo: string
    drawDate: string
    raffleId: string
    wonAmount: number
}

export interface UserLosingTransaction {
    id: number
    username: string
    ticketNo: string
    drawDate: string
    raffleId: string
    wonAmount: number
}