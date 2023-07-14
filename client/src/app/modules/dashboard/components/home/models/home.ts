import { AuthDetails } from "src/app/shared/models/auth";

export interface Home {
    id: number;
    drawName: string;
    drawDateStartDate: Date;
    drawDateEndDate: Date;
    sequenceNo: string;
    rafflePrice: number;
    authDto: AuthDetails
}