import { NgModule } from "@angular/core";
import { COMMON_IMPORTS } from "../common-imports";
import { ChatPopupComponent } from "./chat-popup/chat-popup.component";
import { LoginPopupComponent } from "./login-popup/login-popup.component";
import { PopupComponent } from "./popup/popup.component";

const EXPORTS = [
    ChatPopupComponent,
    LoginPopupComponent,
    PopupComponent
]

@NgModule({
    declarations: [
        ...EXPORTS
    ],
    exports: [
        ...EXPORTS
    ],
    imports: [
        ...COMMON_IMPORTS
    ],
    providers: []
})
export class ComponentsModule { }