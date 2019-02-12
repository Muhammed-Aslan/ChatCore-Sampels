import { Gender } from '../Enums/Gender';
import { Status } from '../Enums/Status';

export class User {
    id: string;
    accountId: string;
    userName: string;
    email: string;
    phone: string;
    isLockedOut: boolean;
    image: string;
    statusMessage: string;
    status: Status;
    gender: Gender;
}