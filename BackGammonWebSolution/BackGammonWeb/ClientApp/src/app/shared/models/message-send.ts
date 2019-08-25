export interface SendMessage {
  type:string;
  message:string;
  reply:boolean;
  sender:string;
  date:Date;
  files:Array<any>;
  quote:string;
  latitude:number;
  longitude:number;
  avatar:string;
  user:any;
}
