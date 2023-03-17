import { Injectable } from "@angular/core";
import { PermCheck } from "../components";
import { AuthService } from "./auth.service";

@Injectable({ providedIn: 'root' })
export class Guards extends PermCheck {
    constructor(auth: AuthService) { super(auth); }
}