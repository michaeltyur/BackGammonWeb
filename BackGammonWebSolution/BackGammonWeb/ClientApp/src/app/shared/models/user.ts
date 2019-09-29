

export interface User {
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
  groupName:string;
}

