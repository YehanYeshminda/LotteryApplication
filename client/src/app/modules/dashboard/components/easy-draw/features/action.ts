import { createAction, props } from "@ngrx/store";

// actions.ts
export const AddToCartError = createAction('[Cart] Add To Cart Error', props<{ error: any }>());
