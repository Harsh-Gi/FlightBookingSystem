import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/Services/auth.service';
import { UserdataService } from 'src/app/Services/userdata.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent {
  userName: string="";
  age: string="";
  gender: string="";
  ticketType: string="";
  journeyId: number=0;
  userId:string="";
  totalCost:number=0;

  passengerArray:any[]=[];
  resultArray!:any[];
  row:any;

  private routeSubscription!: Subscription;
  constructor(private route:ActivatedRoute, private auth:AuthService, private router:Router, private userData:UserdataService){}

  ngOnInit() {
    this.userData.getUserIdFromStore().subscribe(val=>{
      let userIdFromToken=this.auth.getUserIdFromToken();
      this.userId=val || userIdFromToken;
    });
    // alert(this.userId);

    this.routeSubscription = this.route.queryParamMap.subscribe(params => {
      this.journeyId = Number(params.get('id'));
    });
      // alert(this.journeyId);
      this.auth.getJourneyById(this.journeyId).subscribe({
        next:(res)=>{
          this.row=res;
        },
        error:(err)=>{
          this.row=null;
          Swal.fire({
            title: 'Error!',
            text: err?.error.message,
            icon: 'error',
            confirmButtonText: 'Ok'
          });
        }
      });
  }

  addPassenger(){
    if(this.userName!="" && this.age!="" && this.gender!="" && this.ticketType!="" && this.journeyId!=null){
      
        this.passengerArray.push({
          userId:this.userId,
          userName: this.userName,
          age: this.age,
          gender: this.gender,
          ticketType: this.ticketType,
          journeyId: this.journeyId
        });
        if(this.ticketType=="Economy"){
          this.totalCost=this.totalCost+this.row.eClassPrice;
        }
        else{
          this.totalCost=this.totalCost+this.row.bClassPrice;
        }
        this.userName="";
        this.age="";
        this.gender="";
        this.ticketType="";
        document.getElementById("addPassengerModalClose")?.click();
    }
    else{
      Swal.fire("Error!", "Please Enter All Details!", "error");
    }
  }

  clickAddPassenger(){
    if(this.passengerArray.length>3){
      Swal.fire("Error!", "Max 4 Tickets Can Be Booked At a Time!", "error");
    }
    else{
      document.getElementById("addPassengerBtn")?.click();
    }
  }

  onSubmit(){
    console.log(this.userName+" "+this.age+" "+this.gender+" "+this.ticketType+" "+this.journeyId);
    if(this.passengerArray.length>0){
      this.auth.paymentAPI({cost:this.totalCost}).subscribe({
        next:(paymentRes)=>{
          this.auth.bookTicket(this.passengerArray).subscribe({
            next:(res)=>{
              Swal.fire({
                title: 'Success!',
                text: "Your Ticket is Booked Successfully! And Confirmation Email is Sent To Registered Mail ID.",
                icon: 'success',
                confirmButtonText: 'Ok'
              });
              this.totalCost=0;
              this.passengerArray=[];
              this.resultArray=res;
              // this.router.navigate(["home"]);
            },
            error:(err)=>{
              Swal.fire({
                title: 'Error!',
                text: err?.error.message,
                icon: 'error',
                confirmButtonText: 'Ok'
              });
            }
          });
        },
        error:(paymentErr)=>{
          Swal.fire({
            title: paymentErr.title,
            // title: 'Payment Failed!',
            // text: "Payment Failed! Please Try Again...",
            text: paymentErr.message,
            icon: 'error',
            confirmButtonText: 'Ok'
          });
        }
      });
    }
    else{
      Swal.fire("Error!", "Please Add Atleast One Passenger!", "error");
    }
  }
}