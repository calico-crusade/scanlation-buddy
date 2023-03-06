import { Component, ViewChild } from '@angular/core';
import { PopupInstance, PopupService } from 'src/app/components';
import { PopupComponent } from 'src/app/components/popup/popup.component';
import { BuddyRole, RolesService } from 'src/app/services';

@Component({
    templateUrl: './settings-roles.component.html',
    styleUrls: ['./settings-roles.component.scss']
})
export class SettingsRolesComponent {

    private _editInst?: PopupInstance;

    roles$ = this._roles.roles$;
    perms$ = this._roles.perms$;

    role?: BuddyRole;

    @ViewChild('editrole') editPopup!: PopupComponent;

    constructor(
        private _roles: RolesService,
        private _popup: PopupService
    ) { }

    selectRole(role: BuddyRole) {
        this.role = JSON.parse(JSON.stringify(role));
        this._editInst = this._popup.show(this.editPopup);
    }

    newRole() {
        this.role = {
            name: 'New Role',
            description: 'Fancy things are happening!',
            permissions: [],
            creatorId: -1,
            color: '#fff',
            id: -1
        };
        this._editInst = this._popup.show(this.editPopup);
    }

    save() {
        if (!this.role) return;

        if (this.role.id === -1) {
            this._roles
                .post(this.role)
                .subscribe(t => {
                    this._editInst?.cancel();
                });
            return;
        }

        this._roles
            .put(this.role)
            .subscribe(t => {
                this._editInst?.cancel();
            });
    }
}
