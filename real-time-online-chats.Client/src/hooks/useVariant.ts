/**
 * Func
 * @constructor
 * @param {string} variants - The variants array to go through.
 * @param {string} variant - The that should be taken variant.
 * @param {string} defaultStyle - The default style to which taken variant will be concatenated
 * @returns {string} The string style representation that can be used in className of component
 */
function getVariantStyle<T extends { variant?: string }>(
  variants: { key: T["variant"]; style: string }[],
  variant: string,
  defaultStyle: string
): string {
  for (const v of variants) {
    if (v.key == variant) {
      return [defaultStyle, v.style].join(" ").trim();
    }
  }

  throw new Error(getVariantStyle.name);
}

const useVariant = (
  variants: {
    key: string | undefined;
    style: string;
  }[],
  variant: string,
  defaultStyle: string
) => {
  return getVariantStyle(variants, variant, defaultStyle);
};

export default useVariant;
