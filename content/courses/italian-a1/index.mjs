import units1 from './units-01-05.mjs';
import units2 from './units-06-10.mjs';

export default {
  slug: 'italian-a1-foundations',
  title: 'Italian A1 — Foundations',
  shortDescription: 'Start from zero and speak everyday Italian for life and work in Italy. Aligned with the CILS A1 Integrazione exam.',
  description:
    'A complete beginner course for people who are moving to, or already living in, Italy. In 10 weekly units you learn the Italian you need every day: greeting people, giving your personal details at the Questura or the Comune, talking about your family and your job, renting a flat, shopping, getting around town and visiting the doctor.\n\n' +
    'Every unit follows the same rhythm: learn new words with pictures and audio, listen to a real-life dialogue, understand one grammar point, then check yourself with a review and a printable study sheet. Each week ends with a live speaking class with a teacher.',
  objectives: [
    'Introduce yourself and talk about your family, job and home',
    'Fill in forms and handle everyday offices: Questura, Comune, bank, post office',
    'Order at a bar or restaurant, shop at the market and pay',
    'Ask for directions and use public transport',
    'Explain a health problem to a doctor or pharmacist',
    'Prepare for the CILS A1 Integrazione exam',
  ].join('\n'),
  level: 'Beginner',
  cefrLevel: 'A1',
  category: 'Core',
  durationWeeks: 10,
  displayOrder: 20,
  cover: 'Graduation cap',
  units: [...units1, ...units2],
};
