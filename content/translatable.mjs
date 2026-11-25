// Which texts of a course are shown to students in English and can carry a Punjabi /
// Hindi translation. The same rules run in the API (TranslationService.ExtractSources)
// so the admin "Translations" page lists exactly these texts.
//
// Italian text is never translated (words, dialogue lines, Italian options, Italian
// table cells). Standard exercise instructions are translated by the student app itself.

/** Instructions the student app already translates (src/i18n translateContent). */
const STANDARD = [
  /^(\d+[A-Z]? · )?(Listen and choose the right picture|Listen and choose the word you hear|Listen: which word do you hear\?|Listen and choose the number|Listen and choose the sentence you hear|Listen and choose what you hear|Listen and choose the time|Listen: what time is it\?|Listen and choose the price or weight|Listen: how much is it\?|What is this in Italian\?|What does it mean\?|Match the Italian words with their meaning|Match each Italian word with its meaning|Match each word with its meaning|Match each expression with its meaning|Choose the missing word|True or false\?|Choose the correct sentence:?|Choose the correct form:)$/,
  /^(New words|Key phrases|Useful phrases|Examples|The situation|Good to know|What you can do now|Unit study sheet \(PDF\)|Course phrasebook \(PDF\))$/,
  /^All the words, dialogues and grammar of this unit on one printable sheet\.$/,
  /^The slides your teacher uses in class\. Open them on your phone or print them\.$/,
  /^Class slides: .+ \(PDF\)$/,
];

const isStandard = (s) => STANDARD.some((re) => re.test(s));

/** Inner English text of standard instructions that carry their own sentence. */
function innerOf(prompt) {
  const m = /^(?:\d+[A-Z]? · )?True or false\? \((.+)\)$/s.exec(prompt)
    ?? /^(?:\d+[A-Z]? · )?Put the words in order: "(.+)"$/s.exec(prompt);
  return m ? m[1] : null;
}

/** Header of a column that holds "English · Punjabi" in each cell. */
export const MIXED_HEADER = /^English\s*[·/]\s*Punjabi$/;

/** "the biggest · sab ton vadda" → ["the biggest", "sab ton vadda"] (Punjabi may be missing). */
export const splitMeaning = (cell) => {
  const i = (cell ?? '').indexOf(' · ');
  return i < 0 ? [cell ?? '', null] : [cell.slice(0, i), cell.slice(i + 3)];
};

/**
 * Grammar tables with meanings: [English, Punjabi] of every row — from an "English" and a "Punjabi"
 * column, or from one "English · Punjabi" column. Students see the Punjabi only when they chose
 * Punjabi; in Hindi / Urdu they see the translation of the English, in English nothing more.
 * (Same rule in the API — TranslationService — and the student app — LessonBlocks.)
 */
export function tableMeanings(table) {
  const [head, ...rows] = table ?? [];
  if (!head) return [];
  const en = head.indexOf('English');
  const pa = head.indexOf('Punjabi');
  if (en >= 0 && pa >= 0) return rows.map((r) => [r[en], r[pa]]).filter(([e]) => e && e.trim());
  const mixed = head.findIndex((h) => MIXED_HEADER.test(h.trim()));
  if (mixed >= 0) return rows.map((r) => splitMeaning(r[mixed])).filter(([e]) => e && e.trim());
  return [];
}

/** Collects the translatable texts of a built course (seed JSON shape). */
export function collectTexts(course) {
  const out = new Set();
  const add = (s) => {
    if (typeof s !== 'string') return;
    const t = s.trim();
    if (t && !isStandard(t) && /[a-z]/i.test(t)) out.add(t);
  };

  add(course.title);
  add(course.shortDescription);
  for (const p of (course.description ?? '').split(/\n{2,}/)) add(p);
  for (const o of (course.objectives ?? '').split('\n')) add(o);

  for (const u of course.units) {
    add(u.title);
    add(u.description);
    add(u.liveClassFocus);
    u.canDo.forEach(add);
    for (const l of u.lessons) {
      add(l.title);
      add(l.summary);
      add(l.homework);
      for (const b of l.blocks) {
        add(b.title);
        if (['Text', 'Tip', 'Pdf'].includes(b.type)) add(b.body);
        if (b.type === 'Text' && b.caption) add(b.caption); // English translation of a reading
        if (b.type === 'Table' && b.table.length) b.table[0].forEach(add); // header row…
        if (b.type === 'Table') for (const [en] of tableMeanings(b.table)) add(en); // …and English cells next to a Punjabi column
        for (const it of b.items ?? []) { add(it.english); add(it.exampleEnglish); }
        for (const ln of b.lines ?? []) { add(ln.english); for (const r of ln.replies ?? []) add(r.english); }
      }
      for (const e of l.exercises) {
        const inner = innerOf(e.prompt);
        if (inner) add(inner); else add(e.prompt);
        // Options are English meanings only in "what does it mean?" questions.
        if (e.type === 'PickTranslation' && /What does it mean\?$/.test(e.prompt)) e.options.forEach((o) => add(o.text));
        if (e.type === 'MatchPairs') e.pairs.forEach((p) => add(p.right.split(' · ')[0]));
      }
    }
  }
  return out;
}
