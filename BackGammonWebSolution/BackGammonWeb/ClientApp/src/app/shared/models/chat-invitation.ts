export interface IChatInvitation {

  inviterID: number;

  inviterName:string;

  groupName: string;

  message: string;

  error: string;
}

export class ChatInvitation implements IChatInvitation {

  inviterID: number;

  inviterName:string;

  groupName: string;

  message: string;

  error: string;
}

