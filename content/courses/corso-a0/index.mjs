import capitolo1 from './capitolo-1.mjs';
import capitolo2 from './capitolo-2.mjs';

// The school's own syllabus (client material in /syllabus): one unit per chapter
// (Capitolo), three short self-study lessons per class (Lezione) and a chapter test.
// To add Capitolo 3, write capitolo-3.mjs and add it to `units` below. On the next
// deploy the API adds the new chapter to the existing course without touching
// the chapters, edits or student progress that are already there.

export default {
  slug: 'corso-italiano-a0',
  title: 'Corso di Italiano — Livello A0',
  shortDescription: 'The SS Europa classroom course, chapter by chapter: start from zero with the same lessons, slides and tests as your teacher. With English and Punjabi.',
  description:
    'This is the official SS Europa Italian course used in our classes, for complete beginners from India, Pakistan and Bangladesh who are going to Italy to work, study or live.\n\n' +
    'Each chapter (Capitolo) follows the lessons of the classroom: every 60-minute class is split into three short lessons you can study on your phone, with pictures, audio, grammar tables and exercises. Every word comes with its English and Punjabi meaning, and the slides of each class are attached as PDF. Each chapter ends with a test (pass mark 60%); the written part and the oral interview are done with your teacher.',
  objectives: [
    'Read and pronounce Italian correctly: the alphabet, C and G, the final accents',
    'Greet people and introduce yourself: name, city, nationality, age and phone number',
    'Understand the teacher and name the objects of the classroom',
    'Fill in a form with your personal details, dates and numbers up to 100',
    'Talk about your family and describe people',
    'Use essere and avere, the articles, plurals, possessives and adjectives',
  ].join('\n'),
  level: 'Beginner',
  cefrLevel: 'A0',
  category: 'Core',
  durationWeeks: 4,
  displayOrder: 0,
  cover: 'School',
  unitLabel: 'Capitolo',
  units: [capitolo1, capitolo2],
};
