import { AuthDetails } from "src/app/shared/models/auth";

export interface MegaDrawResponse {
  arr: number[];
  authDto: AuthDetails;
}

export interface FullMegaDraw {
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
