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

// The school's own syllabus (client material in /syllabus): one unit per chapter
// (Capitolo), three short self-study lessons per class (Lezione) and a chapter test.
// Chapters 1–7 take the learner from zero to A1 (final A1 test in Chapter 7),
// chapters 8–12 from A1 to A2 (final A2 test in Chapter 12).
// To add the next chapter, write capitolo-N.mjs (with key: 'capitolo-N') and add it to `units` below. On the next
// deploy the API adds the new chapter to the existing course without touching
// the chapters, edits or student progress that are already there.
// `previousTitles`: when new chapters are added to a live course whose title is still one of
// these, the API also refreshes the course title, descriptions and level (never after an admin rename).

export default {
  slug: 'corso-italiano-a0',
  title: 'Corso di Italiano — Livello A0 · A1 · A2',
  previousTitles: ['Corso di Italiano — Livello A0'],
  shortDescription: 'The SS Europa classroom course, chapter by chapter: from zero to level A2 with the same lessons, slides and tests as your teacher. With English and Punjabi.',
  description:
    'This is the official SS Europa Italian course used in our classes, for learners from India, Pakistan and Bangladesh who are going to Italy to work, study or live. You start from zero: Chapters 1–7 take you to level A1 and Chapters 8–12 to level A2.\n\n' +
    'Each chapter (Capitolo) follows the lessons of the classroom: every 60-minute class is split into three short lessons you can study on your phone, with pictures, audio, grammar tables and exercises. Every word comes with its English and Punjabi meaning, and the slides of each class are attached as PDF. Each chapter ends with a test (pass mark 60%), Chapter 7 with the final A1 test and Chapter 12 with the final A2 test; the written part and the oral interview are done with your teacher.',
  objectives: [
    'Read and pronounce Italian correctly: the alphabet, C and G, the final accents',
    'Greet people and introduce yourself: name, city, nationality, age and phone number',
    'Fill in a form with your personal details, dates and numbers',
    'Talk about your family, your day, your hobbies and your job',
    'Order at the bar and the restaurant, do the shopping and buy clothes',
    'Get around town, ask for directions and describe your home and the weather',
    'Look for a job and book an appointment on the phone',
    'Tell what you did with the passato prossimo and how things used to be with the imperfetto',
    'Travel by train and plane, check in at a hotel and tell the story of your arrival in Italy',
    'Explain your symptoms to the doctor and buy medicine at the pharmacy',
    'Make comparisons, talk about your plans with the future tense',
    'Manage the post office, the Comune, a job interview and repairs at home (level A2)',
  ].join('\n'),
  level: 'Beginner',
  cefrLevel: 'A0–A2',
  category: 'Core',
  durationWeeks: 24,
  displayOrder: 0,
  cover: 'School',
  unitLabel: 'Capitolo',
  units: [capitolo1, capitolo2, capitolo3, capitolo4, capitolo5, capitolo6, capitolo7, capitolo8, capitolo9, capitolo10, capitolo11, capitolo12],
};
