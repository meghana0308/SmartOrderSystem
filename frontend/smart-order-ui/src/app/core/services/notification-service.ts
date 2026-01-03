import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'
import { NotificationDto } from '../../models/notification.model'

@Injectable({ providedIn: 'root' })
export class NotificationService {

  private baseUrl='https://localhost:7267/api/notifications'

  constructor(private http:HttpClient){}

  getMyNotifications():Observable<NotificationDto[]>{
    return this.http.get<NotificationDto[]>(`${this.baseUrl}`)
  }

  markAsRead(id:number){
    return this.http.put(`${this.baseUrl}/${id}/read`,{})
  }
}
