import { Component } from '@angular/core';
import { AuthService } from 'src/app/Services/auth.service';
import { UserdataService } from 'src/app/Services/userdata.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  loggedIn:boolean=this.auth.isLoggedIn();
  userRole:string="";

  constructor(private auth:AuthService, private userData:UserdataService){
    this.loggedIn=this.auth.isLoggedIn();
  }

  ngOnInit(){
    this.userData.getRoleFromStore().subscribe(val=>{
      let userRoleFromToken=this.auth.getRoleFromToken();
      this.userRole=val || userRoleFromToken;
    });
  }

  logout(){
    this.loggedIn=false;
    this.auth.signOut();
  }
}
