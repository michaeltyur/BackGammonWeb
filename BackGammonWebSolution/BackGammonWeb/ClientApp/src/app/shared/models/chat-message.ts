import { User } from './user';

export class ChatMessage {
  type: string;
  text: string;
  reply: boolean;
  sender: string;
  date: Date;
  files: Array<any>;
  quote: string;
  latitude: number;
  longitude: number;
  avatar: string;
  user: any;
  constructor() {
    this.user = {
      name: "",
      avatar: ""
    }
  }
}


