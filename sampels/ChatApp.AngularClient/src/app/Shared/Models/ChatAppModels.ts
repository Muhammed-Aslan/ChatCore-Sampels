import { Status} from '../Enums/Status';
import { Gender} from '../Enums/Gender';
import { ChatType} from '../Enums/ChatType';
import { MessageType} from '../Enums/MessageType';
import { AttachmentType} from '../Enums/AttachmentType';

export class Chat {
  constructor(){
    this.messages = new Array<Message>();
    this.users = new Array<Contact>();
  }
  id: string;
  chatType: ChatType;
  name: string;
  image: string;
  description:string;
  unReadMessagesCoun: number;
  createDate: Date;
  createdByUser: Contact;
  modifyDate: Date;
  messages: Message[];
  users: Contact[];

}
export class Contact {
  id: string;
  userName: string;
  image: string;
  statusMessage: string;
  status: Status;
  gender: Gender;

}
export class Message {
  constructor(){
    this.attachments = new Array<Attachment>();
  }
  id: string;
  messageType: MessageType;
  content: string;
  date: Date;
  chatId: string;
  fromUserId: string;
  attachments: Attachment[];
}
export class Attachment {
  id: string;
  attachmentType: AttachmentType;
  uri: string;
}
export class FriendRequest {
  id: string;
  date: Date;
  fromUser: Contact;
  toUser: Contact;
}
