const supportedMarkup = /<\/?(?:p|br|strong|b|em|i|u|sub|sup|ol|ul|li|span)(?:\s[^>]*)?>/i;

export function isRichContent(value?: string | null): boolean {
  return !!value && supportedMarkup.test(value);
}

export function hasMeaningfulRichContent(value?: string | null): boolean {
  if (!value) return false;
  const document = new DOMParser().parseFromString(value, 'text/html');
  const formula = document.querySelector('.ql-formula[data-value]')?.getAttribute('data-value');
  return !!formula?.trim() || !!document.body.textContent?.replace(/[\s\u00a0]/g, '');
}
