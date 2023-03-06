import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';

@Component({
    selector: 'app-chat-popup',
    templateUrl: './chat-popup.component.html',
    styleUrls: ['./chat-popup.component.scss']
})
export class ChatPopupComponent {
    showPopup: boolean = false;

    tabs: { title: string; icon: string; }[] = [
        { title: 'alerts', icon: 'notifications_active' },
        { title: 'chats', icon: 'group' },
        { title: 'settings', icon: 'settings' }
    ];

    tab: number = 0;

    constructor() { }

}
