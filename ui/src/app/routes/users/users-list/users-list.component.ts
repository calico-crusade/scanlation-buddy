import { Component, ViewChild } from '@angular/core';
import { switchMap } from 'rxjs';
import { PermCheck, PopupComponent, PopupInstance, PopupService } from 'src/app/components';
import { AuthService, BuddyRole, BuddyUserRoles, RolesService, UserFilters, UserService } from 'src/app/services';

@Component({
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent extends PermCheck {

    users$ = this._user.users$;
    roles$ = this._role.roles$;
    providers$ = this._user.providers$;

    username: string = '';
    roleId: number = -1;
    provider: string = '';

    user?: BuddyUserRoles;
    userLoading: boolean = false;

    @ViewChild('edituser') editPopup!: PopupComponent;
    private popIn?: PopupInstance;

    constructor(
        private _user: UserService,
        private _role: RolesService,
        private _pop: PopupService,
        _auth: AuthService
    ) { super(_auth); }

    editUser(user: BuddyUserRoles) {
        this.user = JSON.parse(JSON.stringify(user));
        this.popIn = this._pop.show(this.editPopup);
    }

    search() {
        const filter = <UserFilters>{
            page: this._user.page,
            size: 100
        };

        if (this.username) filter.username = this.username;
        if (this.provider) filter.provider = this.provider;
        if (this.roleId) filter.roleId = this.roleId;

        const obs = this._user.search(filter);
        obs.subscribe(() => {});
        return obs;
    }

    hasRole(user: BuddyUserRoles, role: BuddyRole) {
        return !!user.roles.find(t => role.id === t.id);
    }

    toggleRole(user: BuddyUserRoles, role: BuddyRole) {
        if (!this.hasRole(user, role)) {
            user.roles.push(role);
            return;
        }

        const i = user.roles.findIndex(t => t.id === role.id);
        if (i === -1) return;
        user.roles.splice(i, 1);
    }

    save() {
        if (!this.user) return;


        this.userLoading = true;
        this._user
            .setRoles(this.user.user.id, this.user.roles)
            .pipe(
                switchMap(() => this.search())
            )
            .subscribe(() => {
                this.popIn?.cancel();
                this.userLoading = false;
            });
    }
}
