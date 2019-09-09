export interface ISendMessage {
  messageId:number;
  userName:string;
  content:string;
  date:Date;
  groupName:string;
}
export class SendMessage implements ISendMessage {
  messageId:number;
  userName:string;
  content:string;
  date:Date;
  groupName:string;
}
