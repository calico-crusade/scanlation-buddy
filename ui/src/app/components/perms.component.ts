import { AuthService } from "../services";

export abstract class PermCheck {

    constructor(
        private _serv: AuthService
    ) { }

    get hasSiteAccess() { return this._serv.hasPerm('Access Site'); }
    get hasAdminGrantRoles() { return this._serv.hasPerm('Admin - Grant Roles'); }
    get hasAdminEditRoles() { return this._serv.hasPerm('Admin - Edit Roles'); }
    get hasAdminEditConfig() { return this._serv.hasPerm('Admin - Edit Config'); }
    get hasCreateProject() { return this._serv.hasPerm('Project - Create'); }
}