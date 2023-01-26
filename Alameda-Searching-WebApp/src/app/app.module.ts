import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {AngularMatsModule} from './angular-mats/angular-mats.module';
import { PersonalLinksComponent } from './components/personal-links/personal-links.component';
import { HeaderComponent } from './components/header/header.component';
import { TextEntryComponent } from './components/text-entry/text-entry.component';
import { BackgroundImageComponent } from './components/background-image/background-image.component';
import { HttpClientModule } from '@angular/common/http';


@NgModule({
  declarations: [
    AppComponent,
    PersonalLinksComponent,
    HeaderComponent,
    TextEntryComponent,
    BackgroundImageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    AngularMatsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
