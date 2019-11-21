

export interface IUser {
  userID: number;
  token: string;
  userName: string;
  password: string;
  firstName: string;
  lastName: string;
  isOnline:boolean;
  avatar:any;
  haveNewMessage:boolean;
  haveNewPrivateChat:boolean;
  haveGame:boolean,
  groupName:string;
}

