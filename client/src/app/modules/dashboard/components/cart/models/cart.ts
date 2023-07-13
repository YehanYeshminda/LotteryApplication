import { AuthDetails } from "src/app/shared/models/auth";

export interface Cart {
    authDto: AuthDetails | null;
    cartNumbers: number[];
    raffleId: string;
    price: number;
    name: string;
}

export interface CartReponse {
    id?: number
    raffleNo: string
    raffleId: string
    userId: number
    addOn: string
    price: number
    paid: number
    lotteryStatus: number
    name: string
    cartNumbers: number[]
    authDto?: AuthDetails | null;
}
