import { Component, ViewChild } from '@angular/core';
import { PopupComponent, PopupInstance, PopupService } from 'src/app/components';
import { BuddyConfig, ConfigService } from 'src/app/services';

@Component({
    templateUrl: './settings-general.component.html',
    styleUrls: ['./settings-general.component.scss']
})
export class SettingsGeneralComponent {

    loading: boolean = false;
    config$ = this._config.groupedConfig$;
    config?: BuddyConfig;

    @ViewChild('editpopup') editPop!: PopupComponent;
    private _editIn?: PopupInstance;

    constructor(
        private _config: ConfigService,
        private _popup: PopupService
    ) { }

    edit(config: BuddyConfig) {
        this.config = JSON.parse(JSON.stringify(config));
        this._editIn = this._popup.show(this.editPop);
    }

    update() {
        if (!this.config) return;

        this.loading = true;
        this._config
            .put(this.config)
            .subscribe(() => {
                this._editIn?.cancel();
                this.loading = false;
            });
    }
}
