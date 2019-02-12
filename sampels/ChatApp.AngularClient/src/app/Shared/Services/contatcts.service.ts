import { Injectable, Injector } from '@angular/core';
import { EndpointFactory } from './Helpers/endpoint-factory.service';
import { HttpClient } from '@angular/common/http';
import { ConfigurationService } from './Helpers/configuration.service';
import { Observable } from 'rxjs';
import { Contact } from '../Models/ChatAppModels';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ContatctsService extends EndpointFactory{
  //#region Api EndPoints  
  private get contactsUrl() { return this.apiBaseUrl + "/ChatApp/Contacts"; }
  private get removecontactUrl() { return this.apiBaseUrl + "/ChatApp/Contacts/Remove"; }
  //#endregion
 
  constructor(http:HttpClient,configurations:ConfigurationService,injector:Injector) {
    super(http,configurations,injector);
  }

  getContactIdById(contactId:string):Observable<Contact>{
    let endpoint = `${this.contactsUrl}/${contactId}`;
    return this.http.get(endpoint,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.getContactIdById(contactId)))
    );
  }
  
  getAllContacts():Observable<Contact[]>{
    return this.http.get(this.contactsUrl,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=> this.getAllContacts()))
    );
  }

  removeContact(contactId:string):Observable<Contact>{
    let endpoint = `${this.removecontactUrl}/${contactId}`;
    return this.http.delete(endpoint,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.removeContact(contactId)))
    );
  }
}
