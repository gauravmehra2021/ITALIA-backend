import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 2 of the school syllabus (Corso di Italiano, livello A0 → A1).
// Source: syllabus/Capitolo 2 Lezione 1–4 (.pptx) and Test Capitolo 2 (.docx).

const slides = (lez) => `/content/slides/corso-a0-cap2-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-2',
  title: 'Chapter 2: Documents, calendar, family and describing people',
  titleIt: 'Capitolo 2: Documenti, calendario, famiglia e aggettivi',
  cover: 'Identification card',
  description: 'Fill in a form with your personal details, count to 100, say dates, days and months, use the definite articles and plurals, the full verbs essere and avere, talk about your family with possessives and describe people with adjectives.',
  canDo: [
    'I can fill in a form with my personal details (nome, cognome, data di nascita, indirizzo…)',
    'I can count from 21 to 100 and say my date of birth',
    'I can say the days of the week and the months of the year',
    'I can use il / lo / la / l\' / i / gli / le and make regular plurals',
    'I can use all the forms of essere and avere',
    'I can talk about my family with mio / mia / i miei / le mie',
    'I can describe people with adjectives and ask Chi è? Com\'è? Quanti?',
  ],
  liveClassFocus: 'Fill in a real form (modulo) together, say your birthday, then interview a partner about his or her family: Chi è? Com\'è? Quanti anni ha? Quanti fratelli hai?',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — Documenti, numeri 21–100 e calendario
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · Filling in a form',
      titleIt: 'Lezione 1 · Compilare un modulo',
      summary: 'Personal details, documents and the codice fiscale.',
      minutes: 12,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 2 · Lezione 1 (PDF)' },
        {
          title: 'Dati anagrafici (personal details)',
          words: [
            wp('il nome', 'first name', 'Pahlā nām', { pron: 'eel NOH-meh', img: 'Name badge' }),
            wp('il cognome', 'surname', 'Got (surname)', { pron: 'eel kon-YOH-meh' }),
            wp('la data di nascita', 'date of birth', 'Janam tārīkh', { pron: 'lah DAH-tah dee NAH-shee-tah', img: 'Birthday cake' }),
            wp('il luogo di nascita', 'place of birth', 'Janam sathān', { pron: 'eel LWOH-goh dee NAH-shee-tah', img: 'Round pushpin' }),
            wp('l\'indirizzo', 'address (street, number, postcode, city)', 'Patā (galī, ghar nambar)', { pron: 'leen-dee-REET-tsoh', img: 'House with garden' }),
            wp('la cittadinanza', 'citizenship', 'Nāgarikatā', { pron: 'lah cheet-tah-dee-NAHN-tsah' }),
            wp('il sesso (M / F)', 'gender', 'Liṅg', { pron: 'eel SES-soh' }),
          ],
        },
        {
          title: 'Stato civile, codice fiscale e firma',
          words: [
            wp('lo stato civile', 'marital status', 'Viāhuk sathitī', { pron: 'loh STAH-toh chee-VEE-leh' }),
            wp('celibe / nubile', 'single (man / woman)', 'Kuārā / Kuārī', { pron: 'CHEH-lee-beh / NOO-bee-leh' }),
            wp('sposato / sposata', 'married (man / woman)', 'Viāhiā / Viāhī', { pron: 'spoh-ZAH-toh', img: 'Ring' }),
            wp('il codice fiscale', 'tax code (16 letters and numbers)', 'Ṭaiks kōḍ (16 akkhar)', { pron: 'eel KOH-dee-cheh fee-SKAH-leh', img: 'Credit card' }),
            wp('la carta d\'identità', 'identity card', 'Āī-ḍī kārḍ', { pron: 'lah KAHR-tah dee-den-tee-TAH', img: 'Identification card' }),
            wp('il passaporto', 'passport', 'Pāsaporṭ', { pron: 'eel pahs-sah-POR-toh', img: 'Passport control' }),
            wp('il permesso di soggiorno', 'residence permit', 'Rihāish parmiṭ', { pron: 'eel pehr-MES-soh dee sod-JOR-noh' }),
            wp('la firma', 'signature', 'Dastakhat', { pron: 'lah FEER-mah', img: 'Writing hand' }),
            wp('Firma qui!', 'Sign here!', 'Ithe dastakhat karo!', { pron: 'FEER-mah KWEE' }),
          ],
        },
        { tip: 'The **codice fiscale** is your Italian tax code: 16 letters and numbers. You need it for work, the doctor, the bank and the phone. In an address the **CAP** is the postcode and the **numero civico** is the house number: *Via Roma 12, 46100 Mantova*.' },
      ],
      exercises: [
        tr('il cognome', 'surname', 'first name', 'signature', 'address'),
        tr('nubile', 'single (woman)', 'married (woman)', 'single (man)', 'widow'),
        mc('On a form, "Luogo di nascita: Ludhiana" means…', 'You were born in Ludhiana', 'You live in Ludhiana', 'Your surname is Ludhiana', 'Your citizenship is Ludhiana'),
        mc('How many characters does a codice fiscale have?', '16', '10', '12', '8'),
        mc('The clerk says: "Firma qui!". What do you do?', 'Sign the form', 'Write your address', 'Show your passport', 'Say your name'),
      ],
      homework: 'Write your personal details in Italian: Nome, Cognome, Data di nascita, Luogo di nascita, Indirizzo, Cittadinanza, Stato civile.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 1 · Numbers 21 to 100',
      titleIt: 'Lezione 1 · I numeri da 21 a 100',
      summary: 'The tens, and the easy rule to build every number up to 100.',
      minutes: 12,
      sections: [
        {
          title: 'Le decine: da 20 a 100',
          words: [
            wp('venti', '20 — twenty', 'Vīh', { pron: 'VEN-tee' }),
            wp('trenta', '30 — thirty', 'Tīh', { pron: 'TREN-tah' }),
            wp('quaranta', '40 — forty', 'Chāḷī', { pron: 'kwah-RAHN-tah' }),
            wp('cinquanta', '50 — fifty', 'Panjāh', { pron: 'cheen-KWAHN-tah' }),
            wp('sessanta', '60 — sixty', 'Saṭṭh', { pron: 'ses-SAHN-tah' }),
            wp('settanta', '70 — seventy', 'Sattar', { pron: 'set-TAHN-tah' }),
            wp('ottanta', '80 — eighty', 'Assī', { pron: 'ot-TAHN-tah' }),
            wp('novanta', '90 — ninety', 'Nabbe', { pron: 'noh-VAHN-tah' }),
            wp('cento', '100 — one hundred', 'Sau', { pron: 'CHEN-toh', img: 'Hundred points' }),
          ],
        },
        {
          title: 'Regola generale: decina + numero',
          table: [
            ['Number', 'Decina + numero', 'Italiano'],
            ['22', 'venti + due', 'ventidue'],
            ['35', 'trenta + cinque', 'trentacinque'],
            ['47', 'quaranta + sette', 'quarantasette'],
            ['21', 'venti + uno → drop the vowel', 'ventuno (NO ventiuno)'],
            ['28', 'venti + otto → drop the vowel', 'ventotto (NO ventiotto)'],
            ['31 · 38', 'trenta + uno · trenta + otto', 'trentuno · trentotto'],
          ],
        },
        { tip: 'Watch out for **1 (uno)** and **8 (otto)**: they start with a vowel, so the tens **drop their last vowel**: ventuno, ventotto, trentuno, quarantotto, sessantuno…' },
      ],
      exercises: [
        lis('Listen and choose the number', 'ventotto', '28', '82', '38', '27'),
        lis('Listen and choose the number', 'quarantadue', '42', '24', '52', '32'),
        lis('Listen and choose the number', 'sessantacinque', '65', '75', '56', '15'),
        lis('Listen and choose the number', 'ottantuno', '81', '18', '71', '91'),
        lis('Listen and choose the number', 'novantanove', '99', '89', '19', '90'),
        mc('How do you write 21?', 'ventuno', 'ventiuno', 'venteuno', 'vent\'uno'),
        mc('How do you write 38?', 'trentotto', 'trentaotto', 'trentiotto', 'trenta e otto'),
        mc('How do you write 74?', 'settantaquattro', 'settequattro', 'settantquattro', 'sessantaquattro'),
        mc('How do you write 50?', 'cinquanta', 'cinquecento', 'cinquenta', 'quindici'),
        mc('How do you write 100?', 'cento', 'mille', 'centi', 'dieci'),
      ],
      homework: 'Say aloud the year you were born and the age of 3 people in your family.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · Days, months and your date of birth',
      titleIt: 'Lezione 1 · Giorni, mesi e data di nascita',
      summary: 'Lunedì… domenica, gennaio… dicembre, and "Sono nato il 15 maggio".',
      minutes: 12,
      sections: [
        {
          title: 'I giorni della settimana (days of the week)',
          words: [
            wp('lunedì', 'Monday', 'Somvār', { pron: 'loo-neh-DEE' }),
            wp('martedì', 'Tuesday', 'Maṅgalvār', { pron: 'mahr-teh-DEE' }),
            wp('mercoledì', 'Wednesday', 'Budhvār', { pron: 'mehr-koh-leh-DEE' }),
            wp('giovedì', 'Thursday', 'Vīrvār', { pron: 'joh-veh-DEE' }),
            wp('venerdì', 'Friday', 'Shukkarvār', { pron: 'veh-nehr-DEE' }),
            wp('sabato', 'Saturday', 'Shanīvār', { pron: 'SAH-bah-toh' }),
            wp('domenica', 'Sunday', 'Aitvār', { pron: 'doh-MEH-nee-kah' }),
          ],
        },
        {
          title: 'I 12 mesi dell\'anno (months of the year)',
          words: [
            wp('gennaio', 'January', 'Janvarī', { pron: 'jen-NAH-yoh' }),
            wp('febbraio', 'February', 'Farvarī', { pron: 'feb-BRAH-yoh' }),
            wp('marzo', 'March', 'Mārach', { pron: 'MAHR-tsoh' }),
            wp('aprile', 'April', 'Aprail', { pron: 'ah-PREE-leh' }),
            wp('maggio', 'May', 'Maī', { pron: 'MAHD-joh' }),
            wp('giugno', 'June', 'Jūn', { pron: 'JOON-yoh' }),
            wp('luglio', 'July', 'Julāī', { pron: 'LOOL-yoh' }),
            wp('agosto', 'August', 'Agast', { pron: 'ah-GOS-toh' }),
            wp('settembre', 'September', 'Satambar', { pron: 'set-TEM-breh' }),
            wp('ottobre', 'October', 'Aktūbar', { pron: 'ot-TOH-breh' }),
            wp('novembre', 'November', 'Navambar', { pron: 'noh-VEM-breh' }),
            wp('dicembre', 'December', 'Dasambar', { pron: 'dee-CHEM-breh' }),
          ],
        },
        {
          title: 'Come dire la data di nascita',
          words: [
            wp('Quando sei nato / nata?', 'When were you born? (to a man / a woman)', 'Tuhāḍī janam tārīkh kī hai?', { pron: 'KWAHN-doh SEH-ee NAH-toh', img: 'Birthday cake' }),
            wp('Sono nato il 15 maggio 1995.', 'I was born on 15 May 1995.', 'Merā janam 15 Maī 1995 nūṁ hoiā.'),
            wp('il primo gennaio', 'the 1st of January', 'Pahlī Janvarī'),
          ],
          practice: false,
        },
        { tip: 'Order: **giorno + mese + anno** (day, month, year). For the first day of the month say **il primo**: *il primo gennaio* (NOT "il uno gennaio"). Days and months are written with a small letter.' },
      ],
      exercises: [
        mc('Which day comes after "martedì"?', 'mercoledì', 'lunedì', 'giovedì', 'domenica'),
        mc('Which is the last day of the week?', 'domenica', 'sabato', 'lunedì', 'venerdì'),
        mc('Which month comes after "luglio"?', 'agosto', 'giugno', 'settembre', 'aprile'),
        mc('How do you say "1st May"?', 'il primo maggio', 'il uno maggio', 'l\'uno maggio', 'il primo maggiore'),
        mc('A woman asks you: "Quando sei nato?". You are a man and you answer…', 'Sono nato il 3 marzo 1998.', 'Sono nata il 3 marzo 1998.', 'Ho nato il 3 marzo.', 'Sono 3 marzo.'),
        wo('Sono nato il quindici maggio', 'I was born on the fifteenth of May'),
      ],
      homework: 'Write your date of birth and the birthdays of 2 people in your family in Italian (Mio padre è nato il…).',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — Articoli, plurali, essere e avere
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Plurals',
      titleIt: 'Lezione 2 · Il plurale',
      summary: 'One book, two books: -o → -i, -a → -e, -e → -i.',
      minutes: 10,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 2 · Lezione 2 (PDF)' },
        {
          title: 'Ripasso: da UN / UNA a IL / LA',
          table: [
            ['Gender', 'a / an', 'the', 'English'],
            ['masculine (-o)', 'un libro', 'il libro', 'a book → the book'],
            ['masculine (Z / S + cons.)', 'uno zaino', 'lo zaino', 'a backpack → the backpack'],
            ['feminine (-a)', 'una penna', 'la penna', 'a pen → the pen'],
            ['feminine + vowel', 'un\'aula', 'l\'aula', 'a classroom → the classroom'],
          ],
        },
        {
          title: 'Regola del plurale: -O/-I, -A/-E, -E/-I',
          table: [
            ['Genere', 'Singolare → Plurale', 'Esempio', 'English / Punjabi'],
            ['maschile', '-O → -I', 'libro → libri', 'book → books · kitāb → kitābāṁ'],
            ['femminile', '-A → -E', 'penna → penne', 'pen → pens · kalam → kalamāṁ'],
            ['maschile / femminile', '-E → -I', 'studente → studenti · chiave → chiavi', 'student / key · vidyārthī / chābī'],
          ],
        },
        {
          title: 'Dal singolare al plurale in classe',
          practice: false,
          words: [
            wp('1 quaderno → 2 quaderni', '1 notebook → 2 notebooks', '1 kāpī → 2 kāpīāṁ'),
            wp('1 sedia → 3 sedie', '1 chair → 3 chairs', '1 kursī → 3 kursīāṁ'),
            wp('1 mese → 12 mesi', '1 month → 12 months', '1 mahīnā → 12 mahīne'),
          ],
        },
      ],
      exercises: [
        fb('1 libro → 2 ___', ['libri', 'libre', 'libro', 'libra']),
        fb('1 penna → 4 ___', ['penne', 'penni', 'penna', 'penno']),
        fb('1 studente → 10 ___', ['studenti', 'studente', 'studenta', 'studentes']),
        fb('1 chiave → 2 ___', ['chiavi', 'chiave', 'chiava', 'chiavo']),
        fb('1 sedia → 3 ___', ['sedie', 'sedii', 'sedia', 'sedio']),
        fb('1 mese → 12 ___', ['mesi', 'mese', 'mesa', 'meses']),
        mc('What is the plural of "quaderno"?', 'quaderni', 'quaderne', 'quadernos', 'quaderna'),
      ],
      homework: 'Write 6 classroom objects in the singular and the plural (1 penna → 2 penne…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · The definite articles',
      titleIt: 'Lezione 2 · Gli articoli determinativi',
      summary: 'il / i · lo / gli · l\' / gli · la / le · l\' / le',
      minutes: 15,
      sections: [
        {
          title: 'Articoli maschili: IL / I · LO / GLI · L\' / GLI',
          table: [
            ['Singolare → Plurale', 'When', 'Esempi'],
            ['IL → I', 'masculine + consonant', 'il giorno → i giorni · il documento → i documenti'],
            ['LO → GLI', 'masculine + Z, S + consonant, GN, PS', 'lo zaino → gli zaini · lo studente → gli studenti'],
            ['L\' → GLI', 'masculine + vowel (A, E, I, O, U)', 'l\'anno → gli anni · l\'indirizzo → gli indirizzi'],
          ],
        },
        {
          title: 'Articoli femminili: LA / LE · L\' / LE',
          table: [
            ['Singolare → Plurale', 'When', 'Esempi'],
            ['LA → LE', 'feminine + consonant', 'la settimana → le settimane · la firma → le firme · la chiave → le chiavi'],
            ['L\' → LE', 'feminine + vowel (no apostrophe in the plural!)', 'l\'aula → le aule · l\'ora → le ore · l\'amica → le amiche'],
          ],
        },
        {
          title: 'Tabella: tutti gli articoli determinativi',
          table: [
            ['Genere e inizio parola', 'Singolare', 'Plurale', 'Esempio'],
            ['maschile + consonante', 'IL', 'I', 'il libro → i libri'],
            ['maschile + Z / S + cons.', 'LO', 'GLI', 'lo zaino → gli zaini'],
            ['maschile + vocale', 'L\'', 'GLI', 'l\'anno → gli anni'],
            ['femminile + consonante', 'LA', 'LE', 'la data → le date'],
            ['femminile + vocale', 'L\'', 'LE', 'l\'aula → le aule'],
          ],
        },
        { tip: 'In the plural, feminine words always take **LE**, without apostrophe: *l\'aula → le aule* (Punjabi: bahuvachan vich hameshā LE, bina apostrophe).' },
      ],
      exercises: [
        fb('___ documento', ['il', 'lo', 'la', 'l\'']),
        fb('___ studente', ['lo', 'il', 'la', 'l\'']),
        fb('___ figlia', ['la', 'il', 'lo', 'l\'']),
        fb('___ indirizzo', ['l\'', 'il', 'lo', 'la']),
        fb('___ documenti', ['i', 'gli', 'le', 'il']),
        fb('___ studenti', ['gli', 'i', 'le', 'lo']),
        fb('___ figlie', ['le', 'gli', 'i', 'la']),
        fb('___ anni', ['gli', 'i', 'le', 'l\'']),
        fb('___ aule', ['le', 'l\'', 'gli', 'i']),
        mc('What is the plural of "lo zaino"?', 'gli zaini', 'i zaini', 'le zaine', 'lo zaini'),
        mc('What is the plural of "la chiave"?', 'le chiavi', 'le chiave', 'gli chiavi', 'i chiavi'),
      ],
      homework: 'Take 8 words from this chapter and write them with the definite article in the singular and plural (il giorno → i giorni…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Essere and avere: all the forms',
      titleIt: 'Lezione 2 · Essere e avere (singolare e plurale)',
      summary: 'Noi siamo, voi siete, loro sono · noi abbiamo, voi avete, loro hanno.',
      minutes: 12,
      sections: [
        {
          title: 'ESSERE (to be · Hoṇā) e AVERE (to have · Koḷ hoṇā)',
          table: [
            ['Persona', 'ESSERE', 'AVERE', 'English'],
            ['io', 'sono', 'ho', 'I am · I have'],
            ['tu', 'sei', 'hai', 'you are · you have'],
            ['lui / lei', 'è', 'ha', 'he / she is · has'],
            ['noi', 'siamo', 'abbiamo', 'we are · we have'],
            ['voi', 'siete', 'avete', 'you (all) are · have'],
            ['loro', 'sono', 'hanno', 'they are · they have'],
          ],
        },
        {
          title: 'Le forme plurali',
          practice: false,
          words: [
            wp('Noi siamo', 'We are', 'Asīṁ hāṁ'),
            wp('Voi siete', 'You (all) are', 'Tusīṁ ho'),
            wp('Loro sono', 'They are', 'Uh han'),
            wp('Noi abbiamo', 'We have', 'Sāḍe kol hai'),
            wp('Voi avete', 'You (all) have', 'Tuhāḍe kol hai'),
            wp('Loro hanno', 'They have', 'Uhnāṁ kol hai'),
          ],
        },
        { tip: '**Hanno** (they have) is written with H, like ho, hai, ha — the H is silent: say "ANNO". Don\'t confuse it with **anno** (year)!' },
      ],
      exercises: [
        fb('Noi ___ studenti di italiano.', ['siamo', 'siete', 'sono', 'abbiamo']),
        fb('Noi ___ molti libri.', ['abbiamo', 'avete', 'hanno', 'siamo']),
        fb('Loro ___ due figli piccoli.', ['hanno', 'sono', 'avete', 'abbiamo']),
        fb('Loro ___ molto gentili.', ['sono', 'hanno', 'siete', 'siamo']),
        fb('Voi ___ di Mantova?', ['siete', 'siamo', 'sono', 'avete']),
        fb('Voi ___ il passaporto?', ['avete', 'abbiamo', 'hanno', 'siete']),
        wo('Noi siamo studenti', 'We are students'),
        wo('Loro hanno due figli', 'They have two children'),
      ],
      homework: 'Write 6 sentences about your class: Noi siamo…, Noi abbiamo…, Loro sono…, Loro hanno….',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — La famiglia e i possessivi
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · The family',
      titleIt: 'Lezione 3 · La famiglia',
      summary: 'Parents, children, grandparents, husband and wife, uncles, aunts and cousins.',
      minutes: 10,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 2 · Lezione 3 (PDF)' },
        {
          title: 'Genitori e figli',
          words: [
            wp('il padre (papà)', 'father (dad)', 'Pitā', { pron: 'eel PAH-dreh', img: 'Man' }),
            wp('la madre (mamma)', 'mother (mum)', 'Mātā', { pron: 'lah MAH-dreh', img: 'Woman' }),
            wp('i genitori', 'parents', 'Mātā-pitā (Māpe)', { pron: 'ee jeh-nee-TOH-ree' }),
            wp('il figlio', 'son', 'Puttar', { pron: 'eel FEEL-yoh', img: 'Boy' }),
            wp('la figlia', 'daughter', 'Dhī', { pron: 'lah FEEL-yah', img: 'Girl' }),
            wp('il fratello', 'brother', 'Bharā', { pron: 'eel frah-TEL-loh' }),
            wp('la sorella', 'sister', 'Bhaiṇ', { pron: 'lah soh-REL-lah' }),
          ],
        },
        {
          title: 'Nonni, coppia e parenti',
          words: [
            wp('il nonno', 'grandfather', 'Dādā', { pron: 'eel NON-noh', img: 'Old man' }),
            wp('la nonna', 'grandmother', 'Dādī', { pron: 'lah NON-nah', img: 'Old woman' }),
            wp('il marito', 'husband', 'Patī', { pron: 'eel mah-REE-toh', img: 'Man in tuxedo' }),
            wp('la moglie', 'wife', 'Patnī', { pron: 'lah MOL-yeh', img: 'Woman with veil' }),
            wp('lo zio', 'uncle', 'Chāchā', { pron: 'loh DZEE-oh' }),
            wp('la zia', 'aunt', 'Chāchī', { pron: 'lah DZEE-ah' }),
            wp('il cugino / la cugina', 'cousin (m / f)', 'Chāchā-tāyā dā putt / dhī', { pron: 'eel koo-JEE-noh' }),
          ],
        },
        {
          title: 'Ripasso: articoli e plurali',
          table: [
            ['Singolare', 'Plurale', 'English'],
            ['il fratello', 'i fratelli', 'the brother → the brothers'],
            ['la sorella', 'le sorelle', 'the sister → the sisters'],
            ['il figlio', 'i figli', 'the son → the children / sons'],
            ['lo zio', 'gli zii', 'the uncle → the uncles'],
          ],
        },
        { tip: '**I genitori** means the parents (mother and father), not relatives! Relatives are **i parenti**. **I figli** can mean sons, or children (boys and girls).' },
      ],
      exercises: [
        tr('la moglie', 'wife', 'mother', 'sister', 'daughter'),
        tr('i genitori', 'parents', 'relatives', 'grandparents', 'children'),
        mc('Your father\'s father is your…', 'nonno', 'zio', 'cugino', 'fratello'),
        mc('Your mother\'s sister is your…', 'zia', 'nonna', 'cugina', 'figlia'),
        mc('What is the plural of "il fratello"?', 'i fratelli', 'gli fratelli', 'le fratelle', 'i fratello'),
      ],
      homework: 'Draw your family tree and write the Italian word under each person.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 3 · Possessives: mio, tuo, suo…',
      titleIt: 'Lezione 3 · Gli aggettivi possessivi',
      summary: 'My, your, his, her, our, their — and the golden rule for family words.',
      minutes: 15,
      sections: [
        {
          title: 'Gli aggettivi possessivi',
          table: [
            ['Persona', 'masc. sing.', 'femm. sing.', 'masc. plur.', 'femm. plur.'],
            ['io (my · merā)', 'il mio', 'la mia', 'i miei', 'le mie'],
            ['tu (your · tuhāḍā)', 'il tuo', 'la tua', 'i tuoi', 'le tue'],
            ['lui / lei (his / her · uhdā)', 'il suo', 'la sua', 'i suoi', 'le sue'],
            ['noi (our · sāḍā)', 'il nostro', 'la nostra', 'i nostri', 'le nostre'],
            ['voi (your pl. · tuhāḍā)', 'il vostro', 'la vostra', 'i vostri', 'le vostre'],
            ['loro (their · uhnāṁ dā)', 'il loro', 'la loro', 'i loro', 'le loro'],
          ],
        },
        {
          title: 'Regola d\'oro: possessivi e famiglia',
          text: '**Singular family word → NO article**\n- mio padre (NO "il mio padre")\n- tua madre (NO "la tua madre")\n- suo fratello · nostra sorella\n\n**Plural, and with LORO → WITH the article**\n- i miei fratelli · le mie sorelle\n- i nostri genitori\n- il loro padre · la loro madre',
        },
        {
          title: 'Examples',
          practice: false,
          words: [
            wp('Mio padre si chiama Gurdev.', 'My father\'s name is Gurdev.', 'Mere pitā dā nām Gurdev hai.'),
            wp('Tua madre è gentile.', 'Your mother is kind.', 'Tuhāḍī mātā dayālū hai.'),
            wp('I miei fratelli sono alti.', 'My brothers are tall.', 'Mere bharā lambe han.'),
            wp('Le mie sorelle vivono in India.', 'My sisters live in India.', 'Merīāṁ bhaiṇāṁ India vich rahindīāṁ han.'),
            wp('Il loro figlio ha 5 anni.', 'Their son is 5 years old.', 'Uhnāṁ dā puttar 5 sāl dā hai.'),
          ],
        },
        { tip: '**Suo / sua** means both *his* and *her*: it agrees with the thing owned, not with the owner. *Marco e sua sorella* (Marco and his sister) · *Anna e suo fratello* (Anna and her brother).' },
      ],
      exercises: [
        mc('Choose the correct form:', 'Mio fratello è alto.', 'Il mio fratello è alto.'),
        mc('Choose the correct form:', 'Le mie sorelle sono simpatiche.', 'Mie sorelle sono simpatiche.'),
        mc('Choose the correct form:', 'I nostri genitori sono in India.', 'Nostri genitori sono in India.'),
        mc('Choose the correct form:', 'Il loro padre è giovane.', 'Loro padre è giovane.'),
        fb('___ madre si chiama Kaur. (my)', ['Mia', 'La mia', 'Mio', 'Le mie']),
        fb('Come si chiama ___ padre? (your)', ['tuo', 'il tuo', 'tua', 'i tuoi']),
        fb('___ fratelli sono a Roma. (my)', ['I miei', 'Miei', 'Mio', 'Le mie']),
        fb('Anna e ___ fratello. (her)', ['suo', 'sua', 'il suo', 'loro']),
      ],
      homework: 'Write 4 sentences about your family: Mio padre…, Mia madre…, I miei fratelli…, Le mie sorelle….',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Dialogue: my family',
      titleIt: 'Lezione 3 · Dialogo: la mia famiglia',
      summary: 'Navdeep talks about his brother and sisters.',
      minutes: 10,
      sections: [
        {
          title: 'Descrivere le persone',
          words: [
            wp('alto / basso', 'tall / short', 'Lambā / Maddharā', { pron: 'AHL-toh / BAHS-soh' }),
            wp('giovane / anziano', 'young / elderly', 'Javān / Bazurag', { pron: 'JOH-vah-neh / ahn-tsee-AH-noh' }),
            wp('simpatico / gentile', 'nice / kind', 'Milansār / Dayālū', { pron: 'seem-PAH-tee-koh / jen-TEE-leh' }),
            wp('bravo / bello', 'good / handsome', 'Vadhīā / Sohṇā', { pron: 'BRAH-voh / BEL-loh' }),
          ],
        },
        {
          title: 'Dialogo: la mia famiglia',
          situation: 'Two classmates talk about their families during the break.',
          dialogue: [
            lp('A', 'Hai fratelli o sorelle, Navdeep?', 'Do you have brothers or sisters, Navdeep?', 'Kī tuhāḍe bharā jāṁ bhaiṇāṁ han?'),
            lp('Navdeep', 'Sì! Ho un fratello e due sorelle.', 'Yes! I have one brother and two sisters.', 'Hāṁ, merā ikk bharā ate do bhaiṇāṁ han.'),
            lp('A', 'Com\'è tuo fratello? Dove vive?', 'What is your brother like? Where does he live?', 'Tuhāḍā bharā keho jihā hai? Uh kitthe rahindā hai?'),
            lp('Navdeep', 'Mio fratello è alto e molto simpatico. Vive a Mantova!', 'My brother is tall and very nice. He lives in Mantova!', 'Merā bharā lambā ate bahut milansār hai. Uh Mantova vich rahindā hai!'),
          ],
        },
        {
          title: 'Attività: parla della famiglia',
          text: '1. **Sei sposato/a o celibe/nubile?** — Are you married or single? (Kī tusīṁ viāhe ho?)\n2. **Hai figli, fratelli o sorelle?** — Do you have children or siblings?\n3. **Come si chiama tuo padre / tua madre?** — What is your father\'s / mother\'s name?',
        },
      ],
      exercises: [
        tf('Navdeep ha due fratelli.', false, 'Navdeep has two brothers.'),
        tf('Navdeep ha due sorelle.', true, 'Navdeep has two sisters.'),
        tf('Il fratello di Navdeep vive a Mantova.', true, 'Navdeep\'s brother lives in Mantova.'),
        tf('Il fratello di Navdeep è basso.', false, 'Navdeep\'s brother is short.'),
        tr('Hai fratelli o sorelle?', 'Do you have brothers or sisters?', 'Are you married?', 'Where do you live?', 'How old is your brother?'),
        wo('Mio fratello è alto', 'My brother is tall'),
      ],
      homework: 'Pair activity: ask a classmate the 3 questions about the family and write down the answers.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Aggettivi, accordo e domande
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 4 · Adjectives: appearance and character',
      titleIt: 'Lezione 4 · Aggettivi: aspetto e carattere',
      summary: 'Alto, giovane, bello, magro, simpatico, gentile, stanco, felice…',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 2 · Lezione 4 (PDF)' },
        {
          title: 'Aspetto ed età',
          words: [
            wp('alto / alta', 'tall', 'Lambā / Lambī', { pron: 'AHL-toh', img: 'Person standing' }),
            wp('basso / bassa', 'short', 'Maddharā / Maddharī', { pron: 'BAHS-soh' }),
            wp('giovane', 'young (m / f)', 'Javān', { pron: 'JOH-vah-neh', img: 'Child' }),
            wp('anziano / anziana', 'elderly', 'Bazurag', { pron: 'ahn-tsee-AH-noh', img: 'Older person' }),
            wp('bello / bella', 'handsome / beautiful', 'Sohṇā / Sohṇī', { pron: 'BEL-loh' }),
            wp('carino / carina', 'cute / pretty', 'Piārā / Piārī', { pron: 'kah-REE-noh' }),
            wp('magro / magra', 'slim / thin', 'Patlā / Patlī', { pron: 'MAH-groh' }),
            wp('forte', 'strong (m / f)', 'Mazbūt / Tākatvar', { pron: 'FOR-teh', img: 'Flexed biceps' }),
          ],
        },
        {
          title: 'Carattere e stato',
          words: [
            wp('simpatico / simpatica', 'nice / friendly', 'Milansār / Changā', { pron: 'seem-PAH-tee-koh', img: 'Smiling face with smiling eyes' }),
            wp('gentile', 'kind / polite (m / f)', 'Dayālū / Nimratā vāḷā', { pron: 'jen-TEE-leh', img: 'Folded hands' }),
            wp('stanco / stanca', 'tired', 'Thakkiā hoiā / Thakkī hoī', { pron: 'STAHN-koh', img: 'Sleeping face' }),
            wp('felice', 'happy (m / f)', 'Khush', { pron: 'feh-LEE-cheh', img: 'Grinning face' }),
            wp('bravo / brava', 'good / capable', 'Changā / Changī', { pron: 'BRAH-voh', img: 'Thumbs up' }),
            wp('tranquillo / tranquilla', 'calm / quiet', 'Shānt', { pron: 'trahn-KWEEL-loh', img: 'Relieved face' }),
            wp('timido / timida', 'shy', 'Sharmīlā / Sharmīlī', { pron: 'TEE-mee-doh' }),
            wp('intelligente', 'smart / intelligent (m / f)', 'Samajhdār / Hushiār', { pron: 'een-tel-lee-JEN-teh', img: 'Light bulb' }),
          ],
        },
        {
          title: 'Examples',
          practice: false,
          words: [
            wp('Mio fratello è alto.', 'My brother is tall.', 'Merā bharā lambā hai.'),
            wp('Mia sorella è bassa.', 'My sister is short.', 'Merī bhaiṇ maddharī hai.'),
            wp('Mio figlio è giovane.', 'My son is young.', 'Merā putt javān hai.'),
            wp('Mio nonno è anziano.', 'My grandfather is elderly.', 'Merā dādā bazurag hai.'),
          ],
        },
      ],
      exercises: [
        mc('What is the opposite of "alto"?', 'basso', 'giovane', 'magro', 'forte'),
        mc('What is the opposite of "giovane"?', 'anziano', 'alto', 'bello', 'stanco'),
        tr('stanco', 'tired', 'happy', 'shy', 'strong'),
        tr('timida', 'shy', 'tired', 'calm', 'pretty'),
      ],
      homework: 'Describe 3 people you know with 2 adjectives each (Mio padre è alto e gentile…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 4 · Noun–adjective agreement',
      titleIt: 'Lezione 4 · L\'accordo nome-aggettivo',
      summary: 'Adjectives with 4 forms (-o, -a, -i, -e) and with 2 forms (-e, -i).',
      minutes: 12,
      sections: [
        {
          title: 'Aggettivi a 4 forme (-O / -A / -I / -E)',
          table: [
            ['masc. sing. (-o)', 'femm. sing. (-a)', 'masc. plur. (-i)', 'femm. plur. (-e)'],
            ['il ragazzo alto', 'la ragazza alta', 'i ragazzi alti', 'le ragazze alte'],
            ['il nonno anziano', 'la nonna anziana', 'i nonni anziani', 'le nonne anziane'],
            ['l\'amico simpatico', 'l\'amica simpatica', 'gli amici simpatici', 'le amiche simpatiche'],
            ['l\'uomo stanco', 'la donna stanca', 'gli uomini stanchi', 'le donne stanche'],
          ],
        },
        {
          title: 'Aggettivi a 2 forme (-E → -I)',
          table: [
            ['Aggettivo', 'Singolare (m = f → -e)', 'Plurale (m = f → -i)', 'English / Punjabi'],
            ['giovane', 'mio fratello / mia sorella è giovane', 'i miei fratelli / le mie sorelle sono giovani', 'young · javān'],
            ['gentile', 'il signore / la signora è gentile', 'i signori / le signore sono gentili', 'kind · dayālū'],
            ['felice', 'lui / lei è felice', 'noi siamo / loro sono felici', 'happy · khush'],
          ],
        },
        { tip: 'The adjective takes the **gender and number** of the noun: la ragazza alt**a**, i ragazzi alt**i**. Adjectives in **-e** have only two forms: *gentile → gentili*.' },
      ],
      exercises: [
        fb('Mio fratello è molto ___.', ['alto', 'alta', 'alti', 'alte']),
        fb('Le mie sorelle sono ___.', ['simpatiche', 'simpatici', 'simpatica', 'simpatico']),
        fb('La nonna è ___.', ['anziana', 'anziano', 'anziani', 'anziane']),
        fb('I ragazzi sono ___.', ['stanchi', 'stanche', 'stanco', 'stanca']),
        fb('La signora è ___.', ['gentile', 'gentila', 'gentili', 'gentilo']),
        fb('Noi siamo ___.', ['felici', 'felice', 'felicie', 'felica']),
        fb('Mia sorella è ___.', ['giovane', 'giovana', 'giovani', 'giovano']),
        wo('Le ragazze sono alte', 'The girls are tall'),
      ],
      homework: 'Write 4 sentences, one for each form: il ragazzo…, la ragazza…, i ragazzi…, le ragazze….',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 4 · Asking questions: Chi è? Com\'è? Quanti?',
      titleIt: 'Lezione 4 · Forme interrogative',
      summary: 'Who is it? What is he / she like? How many? — with a dialogue.',
      minutes: 12,
      sections: [
        {
          title: 'Fare domande',
          words: [
            wp('Chi è? / Chi sono?', 'Who is it? / Who are they?', 'Uh kauṇ hai? / Uh kauṇ han?', { pron: 'kee EH / kee SOH-noh' }),
            wp('Com\'è? / Come sono?', 'What is he / she like? / What are they like?', 'Keho jihā hai? / Keho jihe han?', { pron: 'koh-MEH / KOH-meh SOH-noh' }),
            wp('Quanti? / Quante?', 'How many? (m / f)', 'Kinne? / Kinnīāṁ?', { pron: 'KWAHN-tee / KWAHN-teh' }),
          ],
          practice: false,
        },
        {
          title: 'Esempi',
          practice: false,
          words: [
            wp('Chi è lui? È mio padre.', 'Who is he? He\'s my father.', 'Uh kauṇ hai? Uh merā pitā hai.'),
            wp('Chi sono loro? Sono i miei amici.', 'Who are they? They\'re my friends.', 'Uh kauṇ han? Uh mere dost han.'),
            wp('Com\'è tua madre? È gentile.', 'What is your mother like? She\'s kind.', 'Tuhāḍī mātā keho jihī hai? Uh dayālū hai.'),
            wp('Quanti fratelli hai?', 'How many brothers do you have?', 'Tuhāḍe kinne bharā han?'),
            wp('Quante sorelle hai?', 'How many sisters do you have?', 'Tuhāḍīāṁ kinnīāṁ bhaiṇāṁ han?'),
          ],
        },
        {
          title: 'Dialogo: Chi è? Com\'è? Quanti sono?',
          situation: 'Gurpreet shows a photo to a friend.',
          dialogue: [
            lp('A', 'Ciao Gurpreet! Chi è questa ragazza?', 'Hi Gurpreet! Who is this girl?', 'Sat Srī Akāl Gurpreet! Eh kuṛī kauṇ hai?'),
            lp('Gurpreet', 'È mia cugina Simran! Ha 24 anni.', 'She\'s my cousin Simran! She is 24.', 'Uh merī chāchī dī dhī Simran hai! Uh 24 sāl dī hai.'),
            lp('A', 'Com\'è Simran? E quanti fratelli ha?', 'What is Simran like? And how many brothers does she have?', 'Simran keho jihī hai? Ate us de kinne bharā han?'),
            lp('Gurpreet', 'È molto gentile e felice. Ha due fratelli alti e simpatici!', 'She is very kind and happy. She has two tall and nice brothers!', 'Uh bahut dayālū ate khush hai. Uhde do lambe ate milansār bharā han!'),
          ],
        },
        {
          title: 'Intervista il tuo compagno (interview your partner)',
          text: '1. **Chi è il tuo migliore amico / la tua migliore amica?** — Si chiama…\n2. **Com\'è?** (alto/basso, giovane/anziano, simpatico, gentile…) — È alto/a, giovane e molto simpatico/a!\n3. **Quanti anni ha? Quanti fratelli o sorelle ha?** — Ha … anni e ha … fratelli / … sorelle.',
        },
      ],
      exercises: [
        tf('Simran è la sorella di Gurpreet.', false, 'Simran is Gurpreet\'s sister.'),
        tf('Simran ha 24 anni.', true, 'Simran is 24 years old.'),
        tf('Simran ha due fratelli.', true, 'Simran has two brothers.'),
        fb('— ___ lei? — È mia madre.', ['Chi è', 'Com\'è', 'Quanti', 'Quante']),
        fb('— ___ anni hanno i tuoi genitori? — Hanno 55 anni.', ['Quanti', 'Quante', 'Chi è', 'Com\'è']),
        fb('— ___ tuo fratello? — È alto e simpatico.', ['Com\'è', 'Chi è', 'Quanti', 'Quante']),
        fb('— ___ sorelle hai? — Ho due sorelle.', ['Quante', 'Quanti', 'Chi', 'Come']),
      ],
      homework: 'Interview a classmate with the 3 questions and write a short description of his or her best friend.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 2
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 2 test',
      titleIt: 'Test Capitolo 2',
      summary: 'Listening, documents, calendar, family, articles, plurals, essere / avere, possessives and adjectives. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 2',
          text: 'This test checks everything in Chapter 2 (Lessons 1–4).\n\n- **1. Listening** — days, months, documents, family, adjectives and numbers 21–100\n- **2. Vocabulary** — documents, calendar, family and adjectives\n- **3. Grammar** — articles and plurals, essere / avere, possessives, agreement and questions\n- **4. Writing** — sentences about your family\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **personal form** (modulo anagrafico) and the **oral interview** (calendar, family, describing people — 2 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A — Ascolta e scegli
        lis('1A · Listen and choose what you hear', 'Oggi è martedì.', 'MARTEDÌ', 'MERCOLEDÌ'),
        lis('1A · Listen and choose what you hear', 'Il mio compleanno è a ottobre.', 'OTTOBRE', 'NOVEMBRE'),
        lis('1A · Listen and choose what you hear', 'Scrivi il tuo codice fiscale sul modulo.', 'CODICE FISCALE', 'STATO CIVILE'),
        lis('1A · Listen and choose what you hear', 'Mia sorella è giovane e simpatica.', 'MIA SORELLA È GIOVANE', 'MIO FRATELLO È ALTO'),
        lis('1A · Listen and choose what you hear', 'Dopo il lavoro, loro sono stanchi.', 'LORO SONO STANCHI', 'NOI SIAMO FELICI'),
        // 1B — Numeri 21–100
        lis('1B · Listen and choose the number', 'ventotto', '28', '82', '38', '27'),
        lis('1B · Listen and choose the number', 'quarantadue', '42', '24', '52', '32'),
        lis('1B · Listen and choose the number', 'sessantacinque', '65', '75', '56', '15'),
        lis('1B · Listen and choose the number', 'ottantuno', '81', '18', '71', '91'),
        lis('1B · Listen and choose the number', 'novantanove', '99', '89', '19', '90'),
        // 2A — Collega
        pairs('2A · Match each Italian word with its meaning', [
          ['Indirizzo', 'Address · Patā'],
          ['Giovedì', 'Thursday · Vīrvār'],
          ['Moglie', 'Wife · Patnī'],
          ['Gentile', 'Kind / Polite · Dayālū'],
          ['Firma', 'Signature · Dastakhat'],
        ]),
        // 2B — Numeri in lettere
        mc('2B · Write the number in Italian: 21', 'ventuno', 'ventiuno', 'vent\'uno', 'ventiuna'),
        mc('2B · Write the number in Italian: 38', 'trentotto', 'trentaotto', 'trentiotto', 'trenta otto'),
        mc('2B · Write the number in Italian: 50', 'cinquanta', 'cinquenta', 'cinquecento', 'quindici'),
        mc('2B · Write the number in Italian: 74', 'settantaquattro', 'settequattro', 'sessantaquattro', 'settantquattro'),
        mc('2B · Write the number in Italian: 100', 'cento', 'centi', 'mille', 'dieci'),
        // 3A — Articoli e plurale
        fb('3A · ___ documento → i documenti', ['il', 'lo', 'la', 'l\'']),
        mc('3A · What is the plural of "il documento"?', 'i documenti', 'gli documenti', 'le documente', 'i documento'),
        fb('3A · ___ figlia → le figlie', ['la', 'il', 'lo', 'l\'']),
        mc('3A · What is the plural of "la figlia"?', 'le figlie', 'le figli', 'i figlie', 'la figlie'),
        fb('3A · ___ studente → gli studenti', ['lo', 'il', 'la', 'l\'']),
        mc('3A · What is the plural of "lo studente"?', 'gli studenti', 'i studenti', 'le studente', 'gli studente'),
        // 3B — Essere / avere al plurale
        fb('3B · Noi ___ studenti di italiano.', ['siamo', 'siete', 'sono', 'abbiamo']),
        fb('3B · Noi ___ molti libri.', ['abbiamo', 'avete', 'hanno', 'siamo']),
        fb('3B · Loro ___ due figli piccoli.', ['hanno', 'sono', 'avete', 'abbiamo']),
        fb('3B · Loro ___ molto gentili.', ['sono', 'hanno', 'siete', 'siamo']),
        // 3C — Possessivi, accordo e domande
        mc('3C · Choose the correct sentence', 'Mio fratello è molto alto.', 'Il mio fratello è molto alto.', 'Mio fratello è molto alta.'),
        mc('3C · Choose the correct sentence', 'Le mie sorelle sono simpatiche.', 'Mie sorelle sono simpatiche.', 'Le mie sorelle sono simpatici.'),
        fb('3C · — ___ lei? — È mia madre.', ['Chi è', 'Quanti', 'Com\'è']),
        fb('3C · — ___ anni hanno i tuoi genitori? — Hanno 55 anni.', ['Quanti', 'Com\'è', 'Chi è']),
        // 4B — Frasi sulla famiglia
        wo('Mio padre è alto e gentile', 'My father is tall and kind'),
        wo('I miei fratelli sono giovani', 'My brothers are young'),
        wo('Mia madre ha cinquanta anni', 'My mother is fifty years old'),
      ],
      homework: 'Written part for your teacher: fill in the form (Nome, Cognome, Data di nascita, Luogo di nascita, Indirizzo, Stato civile, Codice fiscale, Firma) and write 3 sentences about your family. Then prepare the 2-minute oral interview.',
    },
  ],
};
