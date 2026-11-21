// Helpers for writing course content.
//
// Authoring convention: in multiple-choice helpers the FIRST option is the
// correct one. The build step shuffles the options (deterministically) and
// records the correct index.

/** Vocabulary word: [italian, english, pronunciation, illustration, example, exampleEnglish] */
export const w = (it, en, pron, img = null, ex = null, exEn = null) => ({ it, en, pron, img, ex, exEn });

/** Dialogue line. */
export const line = (speaker, it, en) => ({ speaker, it, en });

/** Word with a Punjabi meaning: (italian, english, punjabi, { pron, img, ex, exEn }). */
export const wp = (it, en, pa, { pron = null, img = null, ex = null, exEn = null } = {}) => ({ it, en, pa, pron, img, ex, exEn });

/** Dialogue line with a Punjabi translation. */
export const lp = (speaker, it, en, pa) => ({ speaker, it, en, pa });

/** Listening question: the app reads `spoken` aloud; the first option is correct. */
export const lis = (prompt, spoken, ...options) => ({ type: 'ListenPick', prompt, italian: spoken, options });

/** Match pairs written by hand: [[italian, meaning], …]. */
export const pairs = (prompt, list) => ({ type: 'MatchPairs', prompt, pairs: list.map(([left, right]) => ({ left, right })) });

/** Fill the gap (multiple choice). Use ___ for the gap. */
export const fb = (sentence, options, explanation = null) => ({
  type: 'FillBlank', prompt: 'Choose the missing word', italian: sentence, options, explanation,
});

/** Put the words in order to make the Italian sentence. */
export const wo = (italian, english) => ({
  type: 'WordOrder', prompt: `Put the words in order: "${english}"`, answer: italian,
});

/** True / false about a dialogue or reading. */
export const tf = (italianStatement, isTrue, english = null) => ({
  type: 'TrueFalse',
  prompt: english ? `True or false? (${english})` : 'True or false?',
  italian: italianStatement,
  options: ['Vero (true)', 'Falso (false)'],
  correctIndex: isTrue ? 0 : 1,
  fixedOrder: true,
});

/** Italian → English meaning. */
export const tr = (italian, ...options) => ({
  type: 'PickTranslation', prompt: 'What does it mean?', italian, options,
});

/** Free multiple-choice question (situations, reading comprehension, grammar). */
export const mc = (prompt, ...options) => ({ type: 'PickTranslation', prompt, options });

// ── Deterministic shuffling ─────────────────────────────────────────────────

export function rng(seed) {
  let a = seed >>> 0;
  return () => {
    a |= 0; a = (a + 0x6d2b79f5) | 0;
    let t = Math.imul(a ^ (a >>> 15), 1 | a);
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

export function shuffle(list, rand) {
  const a = [...list];
  for (let i = a.length - 1; i > 0; i--) {
    const j = Math.floor(rand() * (i + 1));
    [a[i], a[j]] = [a[j], a[i]];
  }
  return a;
}

export function hashString(s) {
  let h = 2166136261;
  for (let i = 0; i < s.length; i++) h = Math.imul(h ^ s.charCodeAt(i), 16777619);
  return h >>> 0;
}

// ── Speaking practice (role-play) ───────────────────────────────────────────

/** A line said by the other person in a role-play. */
export const bot = (it, en) => ({ it, en, learner: false });

/**
 * The learner's turn. Each argument is [italian, english]; the first is the model
 * answer, the others are also accepted and shown as suggestions.
 */
export const you = (...replies) => ({
  it: replies[0][0],
  en: replies[0][1],
  learner: true,
  replies: replies.slice(1).map(([it, en]) => ({ it, en })),
});
