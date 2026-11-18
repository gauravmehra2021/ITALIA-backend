import units1 from './units-01-04.mjs';
import units2 from './units-05-08.mjs';

export default {
  slug: 'italian-a2-everyday-life',
  title: 'Italian A2 — Everyday Life',
  shortDescription: 'The next step after A1: talk about your day, your past and your plans, handle the doctor and offices — and prepare for the CILS A2 exam.',
  description:
    'For learners who finished A1 (or already know the basics). In 8 weekly units you learn to describe your routine, tell stories in the past (passato prossimo and imperfetto), talk about your home, your life before Italy, your health and your plans for the future, and make polite requests in offices with the conditional.\n\n' +
    'Every unit has picture words with audio, one clear grammar point with exercises, a speaking role-play where you answer out loud, and a review with a printable study sheet. A2 is the level required for the long-term EU residence permit.',
  objectives: [
    'Describe your daily routine with reflexive verbs',
    'Talk about the past with the passato prossimo and the imperfetto',
    'Describe your home and solve problems with the landlord',
    'Explain symptoms and understand the doctor\'s instructions',
    'Talk about plans with the future tense',
    'Make polite requests with the conditional and prepare for CILS A2',
  ].join('\n'),
  level: 'Intermediate',
  cefrLevel: 'A2',
  category: 'Core',
  durationWeeks: 8,
  displayOrder: 2,
  cover: 'Books',
  units: [...units1, ...units2],
};
