import { definePreset, palette } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

const brand = palette('#8a1538');

export const TawtheefPreset = definePreset(Aura, {
  semantic: {
    primary: brand,
  }
});
