import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Http } from '@angular/http';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/modal-options.class';
import { v4 as uuid } from 'uuid';

import { BuntladeStalle } from "../../../models/buntladeStalle.model"

@Component({
    selector: 'bunt',
    templateUrl: './bunt-list.component.html'
})
export class BuntListComponent implements OnInit {
    private modalService: BsModalService;
    private http: Http;

    constructor(http: Http, modalService: BsModalService) {
        this.modalService = modalService;
        this.http = http;
    }

    ngOnInit(): void {
        this.reload();
    }

    @ViewChild('editModal') editModalTemplate: TemplateRef<any>;

    modalRef: BsModalRef;
    buntladeStallen: BuntladeStalle[];
    selectedBuntladeStalle: BuntladeStalle | undefined;

    private reload() {
        this.http.get("/api/bunt").subscribe(data => {
            this.buntladeStallen = data.json();
        });
    }

    taBort(buntladeStalle: BuntladeStalle) {
        this.http.delete(`/api/bunt/${buntladeStalle.id}`).subscribe(data => {
            this.reload();
        });
    }

    redigera(buntladeStalle: BuntladeStalle) {
        this.selectedBuntladeStalle = buntladeStalle;
        this.modalRef = this.modalService.show(this.editModalTemplate);
    }

    laggTill() {
        this.redigera({ id: uuid(), index: 0, adress: "", typ: "1", buntladeNummer: 0 });
    }

    avbrytRedigera() {
        this.selectedBuntladeStalle = undefined;
        this.modalRef.hide();
    }

    spara() {
        if (this.selectedBuntladeStalle) {
            this.http.put(`/api/bunt/${this.selectedBuntladeStalle.id}`,
                {
                    adress: this.selectedBuntladeStalle.adress,
                    typ: this.selectedBuntladeStalle.typ
                }).subscribe(() => {
                    this.avbrytRedigera();
                    this.reload();
            });
        }
    }

    flyttaUpp(buntladeStalle: BuntladeStalle) {
        this.http.post(`/api/bunt/${buntladeStalle.id}/flyttaUpp`, {}).subscribe(() => {
            this.reload();
        });
    }

    flyttaNer(buntladeStalle: BuntladeStalle) {
        this.http.post(`/api/bunt/${buntladeStalle.id}/flyttaNer`, {}).subscribe(() => {
            this.reload();
        });
    }
}
