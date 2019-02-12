import { Injectable } from '@angular/core';
import { Utilities } from './utilities';
import { environment } from '../../../../environments/environment';

@Injectable()
export class ConfigurationService {
    get apiBaseUrl(){return environment.apiUrl || Utilities.baseUrl()}
    get loginEndpoint(){return  environment.loginEndpoint;}
    get contentUrl(){return environment.contentUrl}
    get hubUrl(){return environment.hubUrl}
}
