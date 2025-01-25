export function isNullOrWhitespace(input?: string) {
  return !input || !input.trim();
}