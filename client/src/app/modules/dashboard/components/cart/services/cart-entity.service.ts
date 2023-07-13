import { Injectable } from "@angular/core";
import { EntityCollectionServiceBase, EntityCollectionServiceElementsFactory } from "@ngrx/data";
import { CartReponse } from "../models/cart";

@Injectable()
export class CartEntityService extends EntityCollectionServiceBase<CartReponse> {

    constructor(serviceElementsFactory: EntityCollectionServiceElementsFactory) {
        super('Cart', serviceElementsFactory);
    }
}