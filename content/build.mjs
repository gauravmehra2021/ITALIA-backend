// Builds course content into what the API serves.
//
//   npm install && npm run build
//
// Outputs:
//   ../backend/SSeuropa.Infrastructure/Seed/<slug>.json   course seed (loaded on API startup)
//   ../backend/SSeuropa.Api/wwwroot/content/img/*.png     illustrations (Microsoft Fluent Emoji 3D, MIT)
//   ../backend/SSeuropa.Api/wwwroot/content/pdf/*.pdf     printable unit study sheets
//
// Illustrations are downloaded once and cached in wwwroot; delete a PNG to refetch it.

import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import PDFDocument from 'pdfkit';
import { rng, shuffle, hashString } from './lib.mjs';
import italianA1 from './courses/italian-a1/index.mjs';
import italianA2 from './courses/italian-a2/index.mjs';
import italianWork from './courses/italian-work/index.mjs';
import italianStudy from './courses/italian-study/index.mjs';
import italianVisit from './courses/italian-visit/index.mjs';

const here = path.dirname(fileURLToPath(import.meta.url));
const backend = path.resolve(here, '../backend');
const seedDir = path.join(backend, 'SSeuropa.Infrastructure/Seed');
const imgDir = path.join(backend, 'SSeuropa.Api/wwwroot/content/img');
const pdfDir = path.join(backend, 'SSeuropa.Api/wwwroot/content/pdf');
const cacheDir = path.join(here, '.cache');
const logoPath = path.join(here, 'assets/logo.png');

const COURSES = [italianA1, italianA2, italianWork, italianStudy, italianVisit];
const XP = { Vocabulary: 20, Dialogue: 25, Grammar: 25, Review: 40, Conversation: 30 };
const MINUTES = { Vocabulary: 10, Dialogue: 12, Grammar: 15, Review: 15, Conversation: 8 };

for (const d of [seedDir, imgDir, pdfDir, cacheDir]) fs.mkdirSync(d, { recursive: true });

// ── Illustrations ───────────────────────────────────────────────────────────

const slugify = (s) => s.toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '').replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
const imageUrl = (name) => (name ? `/content/img/${slugify(name)}.png` : null);

async function fluentIndex() {
  const file = path.join(cacheDir, 'fluent-3d-index.json');
  if (fs.existsSync(file)) return JSON.parse(fs.readFileSync(file, 'utf8'));
  console.log('Fetching Fluent Emoji file index from GitHub…');
  const res = await fetch('https://api.github.com/repos/microsoft/fluentui-emoji/git/trees/main?recursive=1');
  if (!res.ok) throw new Error(`GitHub tree request failed: ${res.status}`);
  const tree = await res.json();
  const map = {};
  for (const { path: p } of tree.tree) {
    if (/^assets\/[^/]+\/(Default\/)?3D\/[^/]+_3d(_default)?\.png$/.test(p)) map[p.split('/')[1]] = p;
  }
  fs.writeFileSync(file, JSON.stringify(map));
  return map;
}

async function ensureImages(names) {
  const index = await fluentIndex();
  const missing = [...names].filter((n) => !index[n]);
  if (missing.length) {
    throw new Error(`Unknown Fluent Emoji names (fix them in the course content):\n  ${missing.join('\n  ')}`);
  }
  let downloaded = 0;
  for (const name of names) {
    const target = path.join(imgDir, `${slugify(name)}.png`);
    if (fs.existsSync(target)) continue;
    const file = index[name].split('/').map(encodeURIComponent).join('/');
    const url = `https://cdn.jsdelivr.net/gh/microsoft/fluentui-emoji@main/${file}`;
    let res = await fetch(url);
    // The CDN sometimes refuses files (403); fall back to GitHub directly.
    if (!res.ok) res = await fetch(`https://raw.githubusercontent.com/microsoft/fluentui-emoji/main/${file}`);
    if (!res.ok) throw new Error(`Download failed (${res.status}): ${url}`);
    fs.writeFileSync(target, Buffer.from(await res.arrayBuffer()));
    downloaded++;
  }
  console.log(`Illustrations: ${names.size} used, ${downloaded} downloaded.`);
}

// ── Exercises ───────────────────────────────────────────────────────────────

const opt = (text, img = null) => ({ text, imageUrl: img });

/** Authored exercise → API shape. The first authored option is the correct one. */
function authored(ex, rand) {
  const base = {
    type: ex.type,
    prompt: ex.prompt,
    italian: ex.italian ?? null,
    imageUrl: null,
    options: [],
    correctIndex: 0,
    answer: ex.answer ?? null,
    pairs: [],
    explanation: ex.explanation ?? null,
  };
  if (ex.type === 'WordOrder') return base;
  if (ex.fixedOrder) {
    return { ...base, options: ex.options.map((t) => opt(t)), correctIndex: ex.correctIndex ?? 0 };
  }
  const correct = ex.options[0];
  const options = shuffle(ex.options, rand);
  return { ...base, options: options.map((t) => opt(t)), correctIndex: options.indexOf(correct) };
}

function pickDistractors(target, pool, rand, needImage) {
  const candidates = pool.filter(
    (w) => w !== target && w.en !== target.en && w.it !== target.it && (!needImage || (w.img && w.img !== target.img)),
  );
  return shuffle(candidates, rand).slice(0, 3);
}

function listenPick(target, pool, rand) {
  const withImages = target.img && pool.filter((w) => w.img).length >= 4;
  const choices = shuffle([target, ...pickDistractors(target, pool, rand, withImages)], rand);
  return {
    type: 'ListenPick',
    prompt: withImages ? 'Listen and choose the right picture' : 'Listen and choose the word you hear',
    italian: target.it,
    imageUrl: null,
    options: choices.map((w) => opt(w.it, withImages ? imageUrl(w.img) : null)),
    correctIndex: choices.indexOf(target),
    answer: null,
    pairs: [],
    explanation: `${target.it} = ${target.en}`,
  };
}

function pickWord(target, pool, rand) {
  const choices = shuffle([target, ...pickDistractors(target, pool, rand, false)], rand);
  return {
    type: 'PickWord',
    prompt: 'What is this in Italian?',
    italian: null,
    imageUrl: imageUrl(target.img),
    options: choices.map((w) => opt(w.it)),
    correctIndex: choices.indexOf(target),
    answer: null,
    pairs: [],
    explanation: `${target.it} = ${target.en}`,
  };
}

function pickTranslation(target, pool, rand) {
  const choices = shuffle([target, ...pickDistractors(target, pool, rand, false)], rand);
  return {
    type: 'PickTranslation',
    prompt: 'What does it mean?',
    italian: target.it,
    imageUrl: null,
    options: choices.map((w) => opt(w.en)),
    correctIndex: choices.indexOf(target),
    answer: null,
    pairs: [],
    explanation: null,
  };
}

function matchPairs(words, rand) {
  return {
    type: 'MatchPairs',
    prompt: 'Match the Italian words with their meaning',
    italian: null,
    imageUrl: null,
    options: [],
    correctIndex: 0,
    answer: null,
    pairs: shuffle(words, rand).slice(0, 5).map((w) => ({ left: w.it, right: w.en })),
    explanation: null,
  };
}

/** "What do you say?" questions from a role-play: the other person's line → the right answer. */
function replyQuestions(turns, rand, max = 4) {
  const learnerTurns = turns.filter((t) => t.learner);
  const out = [];
  turns.forEach((t, i) => {
    if (!t.learner || i === 0 || turns[i - 1].learner) return;
    const others = learnerTurns.filter((o) => o !== t).map((o) => o.it);
    if (others.length < 2) return;
    const options = shuffle([t.it, ...shuffle(others, rand).slice(0, 2)], rand);
    out.push({
      type: 'PickTranslation',
      prompt: `They say: “${turns[i - 1].it}” — what do you answer?`,
      italian: null,
      imageUrl: null,
      options: options.map((o) => opt(o)),
      correctIndex: options.indexOf(t.it),
      answer: null,
      pairs: [],
      explanation: `${turns[i - 1].it} (${turns[i - 1].en}) → ${t.it} (${t.en})`,
    });
  });
  return shuffle(out, rand).slice(0, max);
}

/** Practice set generated from a list of words. */
function practiceFromWords(words, rand, { listen = 3, pictures = 2, translate = 2, match = 1 } = {}) {
  const out = [];
  const order = shuffle(words, rand);
  const withImg = order.filter((w) => w.img);
  withImg.slice(0, listen).forEach((t) => out.push(listenPick(t, words, rand)));
  withImg.slice(listen, listen + pictures).forEach((t) => out.push(pickWord(t, words, rand)));
  order.filter((w) => !out.some((e) => e.italian === w.it)).slice(0, translate)
    .forEach((t) => out.push(pickTranslation(t, words, rand)));
  if (match && words.length >= 4) out.push(matchPairs(words, rand));
  return out;
}

// ── Lessons ─────────────────────────────────────────────────────────────────

const item = (w) => ({
  italian: w.it,
  english: w.en,
  pronunciation: w.pron ?? null,
  imageUrl: imageUrl(w.img),
  audioUrl: null,
  example: w.ex ?? null,
  exampleEnglish: w.exEn ?? null,
});

const block = (type, extra) => ({
  type,
  title: null,
  body: null,
  mediaUrl: null,
  caption: null,
  items: [],
  lines: [],
  table: [],
  ...extra,
});

function buildLesson(course, unit, unitIndex, lesson, lessonIndex) {
  const rand = rng(hashString(`${course.slug}:${unitIndex}:${lessonIndex}`));
  const blocks = [];
  let exercises = [];

  switch (lesson.kind) {
    case 'Vocabulary':
      if (lesson.intro) blocks.push(block('Text', { body: lesson.intro }));
      blocks.push(block('Vocabulary', { title: 'New words', items: lesson.words.map(item) }));
      exercises = practiceFromWords(lesson.words, rand);
      break;

    case 'Dialogue':
      blocks.push(block('Text', { title: 'The situation', body: lesson.situation }));
      blocks.push(block('Dialogue', {
        title: lesson.titleIt,
        lines: lesson.lines.map((l) => ({ speaker: l.speaker, italian: l.it, english: l.en, audioUrl: null })),
      }));
      blocks.push(block('Vocabulary', { title: 'Key phrases', items: lesson.phrases.map(item) }));
      if (lesson.table) blocks.push(block('Table', { title: lesson.tableTitle ?? null, table: lesson.table }));
      break;

    case 'Grammar':
      blocks.push(block('Text', { title: lesson.titleIt, body: lesson.explanation }));
      if (lesson.table) blocks.push(block('Table', { table: lesson.table }));
      if (lesson.examples) blocks.push(block('Vocabulary', { title: 'Examples', items: lesson.examples.map(item) }));
      if (lesson.words) blocks.push(block('Vocabulary', { title: 'New words', items: lesson.words.map(item) }));
      if (lesson.words) exercises.push(...practiceFromWords(lesson.words, rand, { listen: 2, pictures: 0, translate: 0, match: 1 }));
      break;

    case 'Conversation':
      blocks.push(block('Text', { title: 'The situation', body: lesson.scene }));
      blocks.push(block('Roleplay', {
        title: lesson.titleIt,
        caption: lesson.partner,
        lines: lesson.turns.map((t) => ({
          speaker: t.learner ? 'You' : lesson.partner,
          italian: t.it,
          english: t.en,
          audioUrl: null,
          isLearner: t.learner,
          replies: (t.replies ?? []).map((r) => ({ italian: r.it, english: r.en })),
        })),
      }));
      blocks.push(block('Vocabulary', { title: 'Useful phrases', items: lesson.phrases.map(item) }));
      exercises = [
        ...replyQuestions(lesson.turns, rand),
        ...practiceFromWords(lesson.phrases, rand, { listen: 0, pictures: 0, translate: 2, match: 1 }),
      ];
      break;

    case 'Review': {
      blocks.push(block('Text', {
        title: 'What you can do now',
        body: unit.canDo.map((c) => `- ${c}`).join('\n'),
      }));
      if (lesson.reading) {
        blocks.push(block('Text', { title: lesson.reading.title, body: lesson.reading.it, caption: lesson.reading.en }));
      }
      blocks.push(block('Pdf', {
        title: 'Unit study sheet (PDF)',
        body: 'All the words, dialogues and grammar of this unit on one printable sheet.',
        mediaUrl: unitPdfUrl(course, unitIndex),
      }));
      if (unitIndex === course.units.length - 1) {
        blocks.push(block('Pdf', {
          title: 'Course phrasebook (PDF)',
          body: 'Every word and phrase from the whole course, unit by unit — print it or keep it on your phone.',
          mediaUrl: phrasebookUrl(course),
        }));
      }
      const unitWords = unit.lessons.filter((l) => l.kind === 'Vocabulary').flatMap((l) => l.words);
      exercises = practiceFromWords(unitWords, rand, { listen: 2, pictures: 1, translate: 2, match: 1 });
      break;
    }
    default:
      throw new Error(`Unknown lesson kind ${lesson.kind}`);
  }

  if (lesson.tip) blocks.push(block('Tip', { title: 'Good to know', body: lesson.tip }));
  exercises.push(...(lesson.exercises ?? []).map((e) => authored(e, rand)));

  return {
    order: lessonIndex + 1,
    title: lesson.title,
    titleIt: lesson.titleIt,
    kind: lesson.kind,
    summary: lesson.summary,
    estimatedMinutes: lesson.minutes ?? MINUTES[lesson.kind],
    xpReward: XP[lesson.kind],
    status: 'Published',
    isFreePreview: Boolean(lesson.freePreview),
    blocks,
    exercises,
    homework: lesson.homework ?? null,
  };
}

const unitPdfUrl = (course, unitIndex) => `/content/pdf/${course.slug}-unit-${String(unitIndex + 1).padStart(2, '0')}.pdf`;
const phrasebookUrl = (course) => `/content/pdf/${course.slug}-phrasebook.pdf`;

function buildCourse(course) {
  return {
    slug: course.slug,
    title: course.title,
    shortDescription: course.shortDescription,
    description: course.description,
    objectives: course.objectives,
    level: course.level,
    cefrLevel: course.cefrLevel,
    category: course.category,
    durationWeeks: course.durationWeeks,
    displayOrder: course.displayOrder,
    thumbnailUrl: imageUrl(course.cover),
    units: course.units.map((u, ui) => ({
      order: ui + 1,
      title: u.title,
      titleIt: u.titleIt,
      description: u.description,
      canDo: u.canDo,
      imageUrl: imageUrl(u.cover),
      pdfUrl: unitPdfUrl(course, ui),
      liveClassFocus: u.liveClassFocus,
      status: 'Published',
      lessons: u.lessons.map((l, li) => buildLesson(course, u, ui, l, li)),
    })),
  };
}

function collectImages(course) {
  const names = new Set([course.cover]);
  for (const u of course.units) {
    names.add(u.cover);
    for (const l of u.lessons) for (const w of [...(l.words ?? []), ...(l.phrases ?? []), ...(l.examples ?? [])]) if (w.img) names.add(w.img);
  }
  names.delete(null);
  names.delete(undefined);
  return names;
}

// ── PDF study sheets ────────────────────────────────────────────────────────

const BLUE = '#0b3d91';
const GREEN = '#008c45';
const RED = '#cd212a';
const INK = '#1f2937';
const MUTED = '#6b7280';

// Standard PDF fonts only cover Latin-1; replace characters they can't draw.
const pdfText = (s) => String(s ?? '')
  .replace(/\*\*(.+?)\*\*/g, '$1').replace(/\*(.+?)\*/g, '$1')
  .replace(/→/g, '->').replace(/[“”]/g, '"').replace(/[‘’]/g, "'");

function unitPdf(course, unit, unitIndex) {
  return new Promise((resolve, reject) => {
    const file = path.join(pdfDir, path.basename(unitPdfUrl(course, unitIndex)));
    const doc = new PDFDocument({ size: 'A4', margin: 48, bufferPages: true, info: { Title: `${course.title} — Unit ${unitIndex + 1}: ${unit.title}`, Author: 'SS Europa' } });
    const out = fs.createWriteStream(file);
    doc.pipe(out);
    out.on('finish', resolve);
    out.on('error', reject);

    const width = doc.page.width - 96;

    // Header band
    doc.rect(0, 0, doc.page.width, 92).fill(BLUE);
    doc.rect(0, 92, doc.page.width / 3, 4).fill(GREEN);
    doc.rect(doc.page.width / 3, 92, doc.page.width / 3, 4).fill('#ffffff');
    doc.rect((doc.page.width / 3) * 2, 92, doc.page.width / 3, 4).fill(RED);
    if (fs.existsSync(logoPath)) doc.image(logoPath, 48, 14, { height: 64 });
    doc.fillColor('#ffffff').font('Helvetica-Bold').fontSize(18).text(`Unit ${unitIndex + 1}: ${pdfText(unit.title)}`, 124, 24, { width: width - 76 });
    doc.font('Helvetica').fontSize(11).fillColor('#dbe7ff').text(`${pdfText(unit.titleIt)}  ·  ${pdfText(course.title)}`, 124, 50, { width: width - 76 });
    doc.y = 116;
    doc.x = 48;

    const heading = (t) => {
      if (doc.y > doc.page.height - 120) doc.addPage();
      doc.moveDown(0.6).font('Helvetica-Bold').fontSize(13).fillColor(BLUE).text(pdfText(t), 48, doc.y, { width });
      doc.moveTo(48, doc.y + 2).lineTo(48 + width, doc.y + 2).strokeColor('#c7d2fe').lineWidth(1).stroke();
      doc.moveDown(0.5);
    };
    const para = (t, opts = {}) => doc.font(opts.bold ? 'Helvetica-Bold' : 'Helvetica').fontSize(opts.size ?? 10).fillColor(opts.color ?? INK).text(pdfText(t), 48, doc.y, { width, ...opts });

    const table = (rows, widths) => {
      const total = widths.reduce((a, b) => a + b, 0);
      const cols = widths.map((w) => (w / total) * width);
      rows.forEach((row, i) => {
        const h = Math.max(...row.map((c, j) => doc.font(i === 0 ? 'Helvetica-Bold' : 'Helvetica').fontSize(9.5).heightOfString(pdfText(c), { width: cols[j] - 10 }))) + 8;
        if (doc.y + h > doc.page.height - 56) doc.addPage();
        const y = doc.y;
        if (i === 0) doc.rect(48, y, width, h).fill('#eef2ff');
        else if (i % 2 === 0) doc.rect(48, y, width, h).fill('#f9fafb');
        let x = 48;
        row.forEach((c, j) => {
          doc.font(i === 0 ? 'Helvetica-Bold' : j === 0 ? 'Helvetica-Bold' : 'Helvetica').fontSize(9.5).fillColor(i === 0 ? BLUE : INK)
            .text(pdfText(c), x + 5, y + 4, { width: cols[j] - 10 });
          x += cols[j];
        });
        doc.y = y + h;
      });
      doc.moveDown(0.4);
    };

    heading('By the end of this unit you can…');
    unit.canDo.forEach((c) => para(`•  ${c}`));

    for (const lesson of unit.lessons) {
      heading(`${lesson.title}  —  ${lesson.titleIt}`);
      if (lesson.words) table([['Italian', 'Say it', 'English'], ...lesson.words.map((w) => [w.it, w.pron ?? '', w.en])], [3, 3, 3]);
      if (lesson.lines) {
        para(lesson.situation, { color: MUTED, size: 9.5 });
        doc.moveDown(0.3);
        table([['Speaker', 'Italian', 'English'], ...lesson.lines.map((l) => [l.speaker, l.it, l.en])], [1.4, 4, 4]);
      }
      if (lesson.turns) {
        para(lesson.scene, { color: MUTED, size: 9.5 });
        doc.moveDown(0.3);
        table([['Who', 'Italian', 'English'], ...lesson.turns.map((t) => [
          t.learner ? 'You' : lesson.partner,
          t.learner && t.replies?.length ? `${t.it}  (or: ${t.replies.map((r) => r.it).join(' / ')})` : t.it,
          t.en,
        ])], [1.4, 4, 4]);
      }
      if (lesson.phrases) table([['Key phrase', 'English'], ...lesson.phrases.map((w) => [w.it, w.en])], [4, 5]);
      if (lesson.explanation) { para(lesson.explanation.replace(/\n- /g, '\n•  ')); doc.moveDown(0.4); }
      if (lesson.table) table(lesson.table, lesson.table[0].map(() => 1));
      if (lesson.examples) table([['Example', 'English'], ...lesson.examples.map((w) => [w.it, w.en])], [4, 4]);
      if (lesson.words && lesson.kind === 'Grammar') table([['Italian', 'English'], ...lesson.words.map((w) => [w.it, w.en])], [4, 4]);
      if (lesson.reading) {
        para(lesson.reading.title, { bold: true });
        para(lesson.reading.it);
        doc.moveDown(0.2);
        para(lesson.reading.en, { color: MUTED, size: 9 });
      }
      if (lesson.tip) { doc.moveDown(0.3); para(`Good to know: ${lesson.tip}`, { color: GREEN, size: 9.5 }); }
    }

    if (unit.lessons.some((l) => l.kind === 'Vocabulary' && l.words.some((w) => w.it === 'il modulo'))) {
      heading('Practice: fill in the form with your details');
      for (const f of ['Cognome', 'Nome', 'Nazionalità', 'Data di nascita', 'Indirizzo', 'Telefono', 'Codice fiscale', 'Firma']) {
        if (doc.y > doc.page.height - 70) doc.addPage();
        para(`${f}:`, { bold: true });
        doc.moveTo(160, doc.y - 4).lineTo(48 + width, doc.y - 4).strokeColor('#9ca3af').stroke();
        doc.moveDown(0.6);
      }
    }

    heading('Homework & live class');
    unit.lessons.filter((l) => l.homework).forEach((l) => para(`•  ${l.homework}`));
    doc.moveDown(0.4);
    para(`Live class: ${unit.liveClassFocus}`, { bold: true, color: BLUE });

    // Footer on every page
    const range = doc.bufferedPageRange();
    for (let i = range.start; i < range.start + range.count; i++) {
      doc.switchToPage(i);
      doc.page.margins.bottom = 0; // let the footer sit below the margin without adding a page
      doc.font('Helvetica').fontSize(8).fillColor(MUTED)
        .text(`SS Europa — Sostegno e Soluzioni per l'Europa  ·  ${pdfText(course.title)}  ·  Page ${i + 1} of ${range.count}`,
          48, doc.page.height - 36, { width, align: 'center', lineBreak: false });
    }
    doc.end();
  });
}

/** One PDF with every word and phrase of the course, unit by unit. */
function phrasebookPdf(course) {
  return new Promise((resolve, reject) => {
    const file = path.join(pdfDir, path.basename(phrasebookUrl(course)));
    const doc = new PDFDocument({ size: 'A4', margin: 48, bufferPages: true, info: { Title: `${course.title} — Phrasebook`, Author: 'SS Europa' } });
    const out = fs.createWriteStream(file);
    doc.pipe(out);
    out.on('finish', resolve);
    out.on('error', reject);
    const width = doc.page.width - 96;

    doc.rect(0, 0, doc.page.width, 92).fill(BLUE);
    doc.rect(0, 92, doc.page.width / 3, 4).fill(GREEN);
    doc.rect((doc.page.width / 3) * 2, 92, doc.page.width / 3, 4).fill(RED);
    if (fs.existsSync(logoPath)) doc.image(logoPath, 48, 14, { height: 64 });
    doc.fillColor('#ffffff').font('Helvetica-Bold').fontSize(20).text('Phrasebook', 124, 22, { width: width - 76 });
    doc.font('Helvetica').fontSize(11).fillColor('#dbe7ff').text(pdfText(course.title), 124, 50, { width: width - 76 });
    doc.y = 116;

    const rows = (list) => list.forEach((wd, i) => {
      const it = pdfText(wd.it);
      const en = pdfText(wd.en);
      const h = Math.max(doc.font('Helvetica-Bold').fontSize(9.5).heightOfString(it, { width: width * 0.45 }), doc.font('Helvetica').heightOfString(en, { width: width * 0.5 })) + 6;
      if (doc.y + h > doc.page.height - 56) doc.addPage();
      const y = doc.y;
      if (i % 2 === 1) doc.rect(48, y, width, h).fill('#f5f7fb');
      doc.font('Helvetica-Bold').fontSize(9.5).fillColor(INK).text(it, 53, y + 3, { width: width * 0.45 });
      doc.font('Helvetica').fontSize(9.5).fillColor(MUTED).text(en, 53 + width * 0.48, y + 3, { width: width * 0.5 });
      doc.y = y + h;
    });

    course.units.forEach((u, ui) => {
      const words = u.lessons.flatMap((l) => [...(l.words ?? []), ...(l.phrases ?? [])]);
      const seen = new Set();
      const unique = words.filter((wd) => (seen.has(wd.it) ? false : seen.add(wd.it)));
      if (doc.y > doc.page.height - 140) doc.addPage();
      doc.moveDown(0.6).font('Helvetica-Bold').fontSize(13).fillColor(BLUE).text(`${ui + 1}. ${pdfText(u.title)}`, 48, doc.y, { width });
      doc.font('Helvetica').fontSize(9.5).fillColor(MUTED).text(pdfText(u.titleIt), 48, doc.y, { width });
      doc.moveDown(0.3);
      rows(unique);
    });

    const range = doc.bufferedPageRange();
    for (let i = range.start; i < range.start + range.count; i++) {
      doc.switchToPage(i);
      doc.page.margins.bottom = 0; // let the footer sit below the margin without adding a page
      doc.font('Helvetica').fontSize(8).fillColor(MUTED)
        .text(`SS Europa  ·  ${pdfText(course.title)} — Phrasebook  ·  Page ${i + 1} of ${range.count}`, 48, doc.page.height - 36, { width, align: 'center', lineBreak: false });
    }
    doc.end();
  });
}

// ── Run ─────────────────────────────────────────────────────────────────────

for (const course of COURSES) {
  await ensureImages(collectImages(course));
  const built = buildCourse(course);
  const seedFile = path.join(seedDir, `${course.slug}.json`);
  fs.writeFileSync(seedFile, JSON.stringify(built, null, 2) + '\n');
  const lessons = built.units.reduce((n, u) => n + u.lessons.length, 0);
  const exercises = built.units.reduce((n, u) => n + u.lessons.reduce((m, l) => m + l.exercises.length, 0), 0);
  console.log(`${course.title}: ${built.units.length} units, ${lessons} lessons, ${exercises} exercises -> ${path.relative(here, seedFile)}`);
  for (let i = 0; i < course.units.length; i++) await unitPdf(course, course.units[i], i);
  await phrasebookPdf(course);
  console.log(`  ${course.units.length} PDF study sheets + phrasebook -> ${path.relative(here, pdfDir)}`);
}
