import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ContactsListComponent } from './Components/contacts-list/contacts-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { LeftNavbarComponent } from './Components/left-navbar/left-navbar.component';
import { LeftProfileComponent } from './Components/left-navbar/left-profile/left-profile.component';
import { AddFriendDialogComponent } from './Components/add-friend-dialog/add-friend-dialog.component';
import { LeftMainComponent } from './Components/left-navbar/left-main/left-main.component';
import { ChatWindowComponent } from './Components/chat-window/chat-window.component';
import { LoginComponent } from './Components/login/login.component';
import { ChatsListComponent } from './Components/chats-list/chats-list.component';
import { ChatRoomsListComponent } from './Components/chat-rooms-list/chat-rooms-list.component';
import { LocalStoreManager } from './Shared/Services/Helpers/local-store-manager.service';
import { AuthService } from './Shared/Services/auth.service';
import { ConfigurationService } from './Shared/Services/Helpers/configuration.service';
import { EndpointFactory } from './Shared/Services/Helpers/endpoint-factory.service';
import { AlertService } from './Shared/Services/Helpers/alert.service';
import { AccountService } from './Shared/Services/account.service';
import{ 
  MatSidenavModule,
  MatToolbarModule, 
  MatButtonModule, 
  MatListModule, 
  MatMenuModule, 
  MatIconModule, 
  MatTabsModule, 
  MatSelectModule, 
  MatTooltipModule, 
  MatDialogModule, 
  MatFormFieldModule,
  MatInputModule,
  MatProgressSpinnerModule,
  MatCheckboxModule,
  MatSnackBarModule,
  MatDividerModule
} from '@angular/material';
import { ChatAppManagerService } from './Shared/Services/chat-app-manager.service';
import { ContatctsService } from './Shared/Services/contatcts.service';
import { FriendRequestsService } from './Shared/Services/friend-requests.service';
import { ChatService } from './Shared/Services/chat.service';
import { MessagesService } from './Shared/Services/messages.service';
import { SignalRService } from './Shared/Services/signalR.service';
import { RightNavbarComponent } from './Components/right-navbar/right-navbar.component';

@NgModule({
  declarations: [
    AppComponent,
    ContactsListComponent,
    LeftNavbarComponent,
    LeftProfileComponent,
    AddFriendDialogComponent,
    LeftMainComponent,
    ChatWindowComponent,
    ChatsListComponent,
    ChatRoomsListComponent,
    LoginComponent,
    RightNavbarComponent,

  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatListModule,
    MatMenuModule,
    MatIconModule,
    MatTabsModule,
    MatSelectModule,
    MatTooltipModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatSnackBarModule,
    MatDividerModule,
    

  ],
  providers: [
    AlertService,
    ConfigurationService,

    EndpointFactory,
    LocalStoreManager,
    AccountService,
    AuthService,
    ChatAppManagerService,
    ChatService,
    ContatctsService,
    FriendRequestsService,
    MessagesService,
    SignalRService
    
  ],
  bootstrap: [AppComponent],
  entryComponents:[AddFriendDialogComponent]
})
export class AppModule { }
