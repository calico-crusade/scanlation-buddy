import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-chat-popup',
    templateUrl: './chat-popup.component.html',
    styleUrls: ['./chat-popup.component.scss']
})
export class ChatPopupComponent {
    @Input('show') showPopup: boolean = false;

    tabs: { title: string; icon: string; }[] = [
        { title: 'alerts', icon: 'notifications_active' },
        { title: 'chats', icon: 'group' },
        { title: 'settings', icon: 'settings' }
    ];

    tab: number = 0;

    constructor() { }

}
