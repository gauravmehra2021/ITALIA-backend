// Roman Punjabi as Punjabi speakers type it (WhatsApp style): "Main Simran haan", "Tusi ki karde ho?".
// Turns the scholarly spelling (Maiṁ Simran hāṁ, Tusīṁ…) into plain letters. Only words with one of
// these marks are changed, so Italian (à, è, ì, ò, ù) and English are never touched.
// The API runs the same rules on content already in the database (PlainPunjabi.cs) — keep them equal.

const MARKS = /[āīūṇṭḍṛṁṅñḷēōĀĪŪṆṬḌṚṀṄÑḶĒŌ]/;
const VOWEL = 'aeiouāīūēō';

function plainWord(word) {
  if (!MARKS.test(word)) return word;
  const upper = word[0] !== word[0].toLowerCase();
  let w = word.toLowerCase();
  if (w === 'ā') return upper ? 'Aa' : 'aa';
  // Nasal vowels.
  w = w.replace(/aiṁ/g, 'ain')
    .replace(/āṁ/g, 'aan')
    .replace(/īṁ$/, 'i').replace(/īṁ/g, 'in')
    .replace(/ūṁ$/, 'u').replace(/ūṁ/g, 'un')
    .replace(/ṁ/g, 'n').replace(/ṅ/g, 'n').replace(/ñ/g, 'n');
  // Long vowels as people type them: ā is "aa" in the first syllable (naam, raat, paani) and
  // "a" after it (tuhada, kitab); ī / ū are "ee" / "oo" only in one-syllable words (theek, door).
  // At the end of a word or next to another vowel they are single (mera, tusi, saiz).
  const isVowel = (ch) => ch !== undefined && VOWEL.includes(ch);
  const syllables = (w.match(/[aeiouāīūēō]+/g) ?? []).length;
  let out = '';
  let syllable = 0;
  for (let i = 0; i < w.length; i++) {
    const c = w[i];
    if (isVowel(c) && !isVowel(w[i - 1])) syllable++;
    const next = w[i + 1];
    const single = next === undefined || isVowel(next) || isVowel(w[i - 1]);
    if (c === 'ā') out += !single && syllable === 1 ? 'aa' : 'a';
    else if (c === 'ī') out += !single && syllables === 1 ? 'ee' : 'i';
    else if (c === 'ū') out += !single && syllables === 1 ? 'oo' : 'u';
    else out += c;
  }
  out = out.replace(/ṛ/g, 'd').replace(/ṇ/g, 'n').replace(/ṭ/g, 't').replace(/ḍ/g, 'd').replace(/ḷ/g, 'l').replace(/ē/g, 'e').replace(/ō/g, 'o');
  return upper ? out[0].toUpperCase() + out.slice(1) : out;
}

/** Plain Roman Punjabi for every marked word in a text. */
export const plainPunjabi = (text) => (typeof text === 'string' ? text.replace(/[\p{L}\p{M}]+/gu, plainWord) : text);
