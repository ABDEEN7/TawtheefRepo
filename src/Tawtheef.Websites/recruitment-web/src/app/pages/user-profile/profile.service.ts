import { Injectable } from '@angular/core';

export interface ProfileModel {
  fullName?: string;
  fullNameEn?: string;
  qid?: string;
  dob?: string;
  country?: string;
  dialCode?: string;
  phone?: string;
  address?: string;
  email?: string;
  degrees?: any[];
  experiences?: any[];
  courses?: any[];
  attachments?: any[];
  skills?: string[];
  languages?: {name:string,level:string}[];
  naZone?: string;
  naStreet?: string;
  naBuilding?: string;
  naUnit?: string;
  isAvailable?: boolean;
}

const STORE_KEY = 'tawtheef_profile_v3_mod';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  data: ProfileModel = {};
  constructor(){
    const raw = localStorage.getItem(STORE_KEY);
    if(raw){
      try { this.data = JSON.parse(raw); } catch(e){ this.data = {}; }
    }
    if(this.data.degrees == null) this.data.degrees = [];
    if(this.data.experiences == null) this.data.experiences = [];
    if(this.data.courses == null) this.data.courses = [];
    if(this.data.attachments == null) this.data.attachments = [];
    if(this.data.skills == null) this.data.skills = [];
    if(this.data.languages == null) this.data.languages = [];
    this.save();
  }
  save(){ localStorage.setItem(STORE_KEY, JSON.stringify(this.data)); }
  get completion(){
    const keys=['fullName','qid','dob','country','dialCode','phone','address','email'];
    let filled=0; keys.forEach(k=>{ if((this.data as any)[k]) filled++; });
    return Math.round(filled/keys.length*100);
  }
}
