export interface User {
    username: string;
    password: string;
    hash: string;
}

export interface MakeLogin {
    username: string;
    password: string;
}

export interface MakeRegisterUser {
    custName: string
    nic: string
    email: string
    custAddress: string
    mobile: string
    alternatePhone: string
    contactNo: string
    otp: string
    custPassword: string
    addOn: string
    photo: string
    custStatus: number
    accountBalance: number
  }