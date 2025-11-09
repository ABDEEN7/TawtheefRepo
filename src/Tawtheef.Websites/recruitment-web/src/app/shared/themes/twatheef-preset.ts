import { definePreset, palette } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

const brand = palette('#7b1e3a');

export const TawtheefPreset = definePreset(Aura, {
  semantic: {
    primary: brand,
  }
});
