import capitolo1 from './capitolo-1.mjs';
import capitolo2 from './capitolo-2.mjs';
import capitolo3 from './capitolo-3.mjs';
import capitolo4 from './capitolo-4.mjs';
import capitolo5 from './capitolo-5.mjs';
import capitolo6 from './capitolo-6.mjs';
import capitolo7 from './capitolo-7.mjs';
import capitolo8 from './capitolo-8.mjs';
import capitolo9 from './capitolo-9.mjs';
import capitolo10 from './capitolo-10.mjs';
import capitolo11 from './capitolo-11.mjs';
import capitolo12 from './capitolo-12.mjs';

// The school's own syllabus (client material in /syllabus). Each chapter (Capitolo) is its own
// course, so a batch can be given one chapter at a time and a course stays small (13 lessons):
// three short self-study lessons per class (Lezione) and a chapter test.
// Chapters 1–7 take the learner from zero to A1 (final A1 test in Chapter 7),
// chapters 8–12 from A1 to A2 (final A2 test in Chapter 12).
//
// Until 25 Nov 2026 all chapters were one course, `corso-italiano-a0` (156 lessons). `splitFrom`
// tells the API to move each chapter of that course — lessons, student progress, enrolments
// and batches — into its new chapter course once, then remove the old course.
//
// To add the next chapter: write capitolo-N.mjs (with key: 'capitolo-N') and add it to CHAPTERS.
// The study sheets keep their old file names (corso-italiano-a0-unit-NN.pdf), and all chapters
// share one phrasebook (corso-italiano-a0-phrasebook.pdf), so old links keep working.

const CHAPTERS = [capitolo1, capitolo2, capitolo3, capitolo4, capitolo5, capitolo6, capitolo7, capitolo8, capitolo9, capitolo10, capitolo11, capitolo12];

export const OLD_SLUG = 'corso-italiano-a0';

const INTRO = 'Part of the official SS Europa Italian course used in our classes, for learners from India, Pakistan and Bangladesh who are going to Italy to work, study or live. Chapters 1–7 take you from zero to level A1 and Chapters 8–12 to level A2 — study them in order.';
const HOW = 'Every 60-minute class of the chapter is split into three short lessons you can study on your phone, with pictures, audio, grammar tables and exercises. Every word comes with its English and Punjabi meaning, the slides of each class are attached as PDF, and the chapter ends with a test (pass mark 60%).';
const FINAL = {
  7: 'This chapter also has the final A1 test (Chapters 1–7); the written part and the oral interview are done with your teacher.',
  12: 'This chapter also has the final A2 test (Chapters 8–12); the written part and the oral interview are done with your teacher.',
};

/** "Chapter 1: Alphabet, …" → "Capitolo 1 · Alphabet, …" */
export const courseTitle = (n, unit) => `Capitolo ${n} · ${unit.title.replace(/^Chapter \d+:\s*/, '')}`;

export const phrasebookCourse = {
  slug: OLD_SLUG,
  title: 'Corso di Italiano — Livello A0 · A1 · A2',
  units: CHAPTERS,
};

export default CHAPTERS.map((unit, i) => {
  const n = i + 1;
  return {
    slug: `corso-italiano-capitolo-${n}`,
    title: courseTitle(n, unit),
    shortDescription: unit.description,
    description: [INTRO, HOW, FINAL[n]].filter(Boolean).join('\n\n'),
    objectives: unit.canDo.join('\n'),
    level: 'Beginner',
    cefrLevel: n <= 7 ? 'A1' : 'A2',
    category: 'Core',
    durationWeeks: 2,
    displayOrder: i,
    cover: unit.cover,
    unitLabel: 'Capitolo',
    units: [unit],
    // Old one-course layout: keeps generated exercises, PDF names and the phrasebook identical.
    splitFrom: OLD_SLUG,
    contentKey: OLD_SLUG,
    unitOffset: i,
    pdfSlug: OLD_SLUG,
    phrasebookSlug: OLD_SLUG,
  };
});
