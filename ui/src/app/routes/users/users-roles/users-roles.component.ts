import { Component, ViewChild } from '@angular/core';
import { PopupInstance, PopupService, PopupComponent, PermCheck } from 'src/app/components';
import {  } from 'src/app/components/popup/popup.component';
import { AuthService, BuddyRole, RolesService } from 'src/app/services';

@Component({
    templateUrl: './users-roles.component.html',
    styleUrls: ['./users-roles.component.scss']
})
export class UsersRolesComponent extends PermCheck {
    private _editInst?: PopupInstance;

    roles$ = this._roles.roles$;
    perms$ = this._roles.perms$;

    role?: BuddyRole;
    roleLoading: boolean = false;

    repeat = Array(10);

    @ViewChild('editrole') editPopup!: PopupComponent;
    @ViewChild('deleteconfirm') deletePopup!: PopupComponent;

    constructor(
        private _roles: RolesService,
        private _popup: PopupService,
        private _auth: AuthService
    ) { super(_auth); }

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
            color: '#ffffff',
            id: -1
        };
        this._editInst = this._popup.show(this.editPopup);
    }

    startDelete(role: BuddyRole) {
        this.role = role;
        this._editInst = this._popup.show(this.deletePopup);
    }

    save() {
        if (!this.role) return;

        this.roleLoading = true;
        if (this.role.id === -1) {
            this._roles
                .post(this.role)
                .subscribe(t => {
                    this._editInst?.cancel();
                    this.roleLoading = false;
                });
            return;
        }

        this._roles
            .put(this.role)
            .subscribe(t => {
                this._editInst?.cancel();
                this.roleLoading = false;
            });
    }

    toggleCheckbox(perm: string, role: BuddyRole) {
        const i = role.permissions.indexOf(perm);
        if (i === -1) {
            role.permissions.push(perm);
            return;
        }

        role.permissions.splice(i, 1);
    }

    cancelDelete() { this._editInst?.cancel(); }
    commitDelete() {
        if (!this.role) return;

        this.roleLoading = true;
        this._roles
            .delete(this.role)
            .subscribe(t => {
                this._editInst?.cancel();
                this.roleLoading = false;
            })
    }
}
