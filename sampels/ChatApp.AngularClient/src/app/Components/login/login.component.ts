import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserLogin } from '../../Shared/Models/user-login.model';
import { AlertService, MessageSeverity } from '../../Shared/Services/Helpers/alert.service';
import { AuthService } from '../../Shared/Services/auth.service';
import { Utilities } from '../../Shared/Services/Helpers/utilities';
import { Subscription } from 'rxjs';
import { map, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { Validators, FormControl, FormGroup } from '@angular/forms';
import { AccountService } from '../../Shared/Services/account.service';
import { UserRegister } from '../../Shared/Models/user-register.model';
import { SignalRService } from 'src/app/Shared/Services/signalR.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
  export class LoginComponent implements OnInit, OnDestroy {

    private _registerSubscription:Subscription;
    private _passwordSubscription:Subscription;
    private _loginSubscription:Subscription;
    
    userLogin = new UserLogin();
    isLoadding = false;
    
    hidePassword = true;
    hideConfirm = true;

    loginFormGroup = new FormGroup({
      userName: new FormControl('', [
        Validators.required,
        Validators.minLength(5),
        Validators.maxLength(50),
        Validators.pattern('[a-zA-Z0-9@$_~]*')]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(6)]),
      rememberMe:new FormControl()
    });

    registerFormGroup = new FormGroup({
      email: new FormControl('', [
        Validators.required,
        Validators.email]),
      userName: new FormControl('', [
        Validators.required,
        Validators.minLength(5),
        Validators.maxLength(50),
        Validators.pattern('[a-zA-Z0-9@$]*')]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(6)]),
      confirmPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(6)]),
      rememberMe:new FormControl()
    });

    constructor(private alertService: AlertService,
       private authService: AuthService,
       private accountService:AccountService,
       private signalRService:SignalRService) {
      this.registerFormGroup.get('password').valueChanges.subscribe(
        v=>
      this.registerFormGroup.get('confirmPassword').setValidators([
        Validators.pattern(v||" "),
        Validators.required,
        Validators.minLength(6)])
      );
    }
  
  
    ngOnInit() {
      this.loginFormGroup.get('rememberMe').setValue(this.authService.rememberMe);
      
      this._passwordSubscription = this.registerFormGroup.get('userName').valueChanges.pipe(
        map(v=>v.trim()),
        debounceTime(750),
        distinctUntilChanged()).subscribe(v=>{
          if(this.registerFormGroup.get('userName').valid){
            console.log(v)
          }
        });

    }
  
  
    ngOnDestroy() {
      if(this._registerSubscription) this._registerSubscription.unsubscribe();
      if(this._passwordSubscription) this._passwordSubscription.unsubscribe();
      if(this._loginSubscription) this._loginSubscription.unsubscribe();
    }
  
    showErrorAlert(caption: string, message: string) {
      this.alertService.showMessage(caption, message, MessageSeverity.error);
    }
  
    // isUnique(event:Event){
    //   if(!this.userNameObs)
    //   {
    //     this.userNameObs = fromEvent((<FromEventTarget<Event>>event.target),"input")
    //           .pipe(
    //             map(e=>
    //                 (<HTMLInputElement>e.target).value
    //               ),
    //             debounceTime(800),
    //             distinctUntilChanged()
    //             );
    //     this._userNameSubscription = this.userNameObs.subscribe(v=>{
    //       if(v.trim()==="") return;
    //       //search logic
    //       console.log(v)
    //     });
    //   }
    // }
  
  
    login(byReg?:{userName:string,password:string,rememberMe:boolean}) {
      if(!byReg){
        this.userLogin = this.loginFormGroup.value;
        if(!this.loginFormGroup.get('userName').valid ) this.showErrorAlert('Email is required', 'Please enter a valid email');
        if(!this.loginFormGroup.get('password').valid ) this.showErrorAlert('Password is required', 'Please enter a valid password');
        if(this.loginFormGroup.invalid) return;
      }
      else{
        this.userLogin.userName = byReg.userName;
        this.userLogin.password = byReg.password;
        this.userLogin.rememberMe = byReg.rememberMe;
      }
      this.isLoadding = true;
      this.alertService.startLoadingMessage("", "Attempting login...");
  
      this._loginSubscription = this.authService.login(this.userLogin.userName, this.userLogin.password, this.userLogin.rememberMe)
      .subscribe(
        user => {
          this.signalRService.connect();
          setTimeout(() => {
            this.isLoadding = false;
            this.alertService.stopLoadingMessage();
             
            this.alertService.showMessage("Login", `Session for ${user.userName} restored!`, MessageSeverity.success);
            setTimeout(() => {
              this.alertService.showStickyMessage("Session Restored", "Please try your last operation again", MessageSeverity.default);
            }, 500);

          }, 500);
        },
        error => {

          this.alertService.stopLoadingMessage();

          if (Utilities.checkNoNetwork(error)) {
            this.alertService.showStickyMessage(Utilities.noNetworkMessageCaption, Utilities.noNetworkMessageDetail, MessageSeverity.error, error);
          
          }
          else {
            let errorMessage = Utilities.findHttpResponseMessage("error_description", error);

            if (errorMessage)
              this.alertService.showStickyMessage("Unable to login", errorMessage, MessageSeverity.error, error);
            else
              this.alertService.showStickyMessage("Unable to login", "An error occured whilst logging in, please try again later.\nError: " + Utilities.getResponseBody(error), MessageSeverity.error, error);
          }

          setTimeout(() => {
            this.isLoadding = false;
          }, 500);
        });
    }

    register(){
      this.isLoadding = true
      let user:UserRegister = this.registerFormGroup.value;
      user.rememberMe = user.rememberMe ? true : false;//important to avoid the null value
      this._registerSubscription = this.accountService.registerAppUser(user)
      .pipe(
        switchMap(user=>{
          return this.accountService.registerChatUser(<UserRegister>user);
        }))
      .subscribe(
        (x)=>{
          this.login(user);
        },
        (err)=>{
          this.isLoadding =false;
          console.log(err);
        }
      );
    }
  }