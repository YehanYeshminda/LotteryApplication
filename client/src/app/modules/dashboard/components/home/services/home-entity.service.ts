import { Injectable } from "@angular/core";
import { EntityCollectionServiceBase, EntityCollectionServiceElementsFactory } from "@ngrx/data";
import { Home } from "../models/home";

@Injectable()
export class HomeEntityService extends EntityCollectionServiceBase<Home> {

    constructor(serviceElementsFactory: EntityCollectionServiceElementsFactory) {
        super('Home', serviceElementsFactory);
    }
}
