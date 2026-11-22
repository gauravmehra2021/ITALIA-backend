import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 3 of the school syllabus (Corso di Italiano, livello A0).
// Source: syllabus/Capitolo 3 Lezione 1–4 (.pptx) and Test Capitolo 3 (.docx).

const slides = (lez) => `/content/slides/corso-a0-cap3-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-3',
  title: 'Chapter 3: Verbs, the time and free time',
  titleIt: 'Capitolo 3: I verbi, l\'ora e il tempo libero',
  cover: 'Alarm clock',
  description: 'The present tense of regular verbs in -ARE, -ERE and -IRE, negative sentences with NON, the most important irregular verbs, telling the time, hobbies and sports, how often you do things and what you like.',
  canDo: [
    'I can use regular verbs in -ARE, -ERE and -IRE in the present tense',
    'I can build a sentence and make it negative with NON',
    'I can use irregular verbs: andare, fare, stare, bere, volere, potere, dovere, venire, uscire…',
    'I can ask and tell the time and say when I do things',
    'I can talk about my hobbies and how often I do them',
    'I can say what I like and don\'t like with mi piace / mi piacciono',
  ],
  liveClassFocus: 'Interview a partner about his or her day: what do you do and not do, what time do you go to work or school, what do you like doing in your free time and how often?',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — I verbi in -ARE e la frase
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 1 · Verbs in -ARE',
      titleIt: 'Lezione 1 · I verbi in -ARE',
      summary: 'The three verb groups and the present tense of parlare, lavorare, abitare…',
      minutes: 15,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 3 · Lezione 1 (PDF)' },
        {
          title: 'Cos\'è un verbo? Le 3 coniugazioni',
          table: [
            ['Gruppo', 'Ending', 'Examples', 'English'],
            ['1ª coniugazione', '-ARE', 'parlare · lavorare', 'to speak · to work (the biggest group!)'],
            ['2ª coniugazione', '-ERE', 'leggere · scrivere', 'to read · to write'],
            ['3ª coniugazione', '-IRE', 'dormire · aprire', 'to sleep · to open'],
          ],
        },
        {
          title: '6 verbi importanti in -ARE',
          words: [
            wp('parlare', 'to speak', 'Bolṇā', { pron: 'pahr-LAH-reh', img: 'Speaking head' }),
            wp('lavorare', 'to work', 'Kamm karnā', { pron: 'lah-voh-RAH-reh', img: 'Briefcase' }),
            wp('abitare', 'to live (in a place)', 'Rahiṇā', { pron: 'ah-bee-TAH-reh', img: 'House' }),
            wp('studiare', 'to study', 'Paṛhnā', { pron: 'stoo-dee-AH-reh', img: 'Books' }),
            wp('cucinare', 'to cook', 'Khāṇā pakāuṇā', { pron: 'koo-chee-NAH-reh', img: 'Cooking' }),
            wp('guardare', 'to watch / to look at', 'Vekhṇā', { pron: 'gwahr-DAH-reh', img: 'Television' }),
          ],
        },
        {
          title: 'Radice + desinenza: PARL-ARE, LAVOR-ARE',
          table: [
            ['Soggetto', 'PARLARE', 'LAVORARE', 'English', 'Punjabi'],
            ['io', 'parlo', 'lavoro', 'I speak / work', 'Maiṁ boldā hāṁ'],
            ['tu', 'parli', 'lavori', 'you speak / work', 'Tūṁ boldā haiṁ'],
            ['lui / lei', 'parla', 'lavora', 'he / she speaks', 'Uh boldā / boldī hai'],
            ['noi', 'parliamo', 'lavoriamo', 'we speak / work', 'Asīṁ bolde hāṁ'],
            ['voi', 'parlate', 'lavorate', 'you (pl.) speak', 'Tusīṁ bolde ho'],
            ['loro', 'parlano', 'lavorano', 'they speak / work', 'Uh bolde han'],
          ],
        },
        {
          title: 'Verbi in pratica',
          practice: false,
          words: [
            wp('Mia madre cucina bene.', 'My mother cooks well.', 'Merī mātā vadhīā khāṇā pakāuṇdī hai.'),
            wp('Noi abitiamo a Mantova.', 'We live in Mantova.', 'Asīṁ Mantova vich rahinde hāṁ.'),
            wp('Loro guardano la TV la sera.', 'They watch TV in the evening.', 'Uh shām nūṁ TV vekhde han.'),
            wp('Io studio, tu studi, noi studiamo.', 'I study, you study, we study.', 'Maiṁ paṛhdā hāṁ…'),
          ],
        },
        { tip: 'Take the ending **-are** off and add **-o, -i, -a, -iamo, -ate, -ano**. Verbs in **-iare** (studiare, mangiare) keep only **one i** with tu and noi: *tu studi, noi studiamo* (NOT studii).' },
      ],
      exercises: [
        fb('Io ___ italiano.', ['parlo', 'parli', 'parla', 'parlano']),
        fb('Tu ___ in un ristorante?', ['lavori', 'lavoro', 'lavora', 'lavorate']),
        fb('Lei ___ a Suzzara.', ['abita', 'abito', 'abiti', 'abitano']),
        fb('Noi ___ l\'italiano la sera.', ['studiamo', 'studiate', 'studiano', 'studio']),
        fb('Voi ___ la cena?', ['cucinate', 'cucinamo', 'cucinano', 'cucini']),
        fb('Loro ___ la TV.', ['guardano', 'guarda', 'guardate', 'guardiamo']),
        mc('Tu… (studiare)', 'studi', 'studii', 'studia', 'studio'),
        mc('Which group does "dormire" belong to?', '3ª coniugazione (-IRE)', '1ª coniugazione (-ARE)', '2ª coniugazione (-ERE)'),
        wo('Noi abitiamo a Mantova', 'We live in Mantova'),
      ],
      homework: 'Write 6 sentences about your family with 6 different -ARE verbs (Mio padre lavora…, Mia sorella studia…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 1 · Building a sentence and saying NO',
      titleIt: 'Lezione 1 · La frase affermativa e negativa',
      summary: 'Subject + verb + complement, and the negative with NON.',
      minutes: 10,
      sections: [
        {
          title: 'Struttura della frase: soggetto + verbo + complemento',
          table: [
            ['1. Soggetto (chi?)', '2. Verbo (azione)', '3. Complemento (cosa / dove?)'],
            ['Io', 'parlo', 'italiano.'],
            ['Maria', 'cucina', 'il riso.'],
            ['Noi', 'studiamo', 'a scuola.'],
          ],
        },
        { text: 'In Italian the subject can be left out, because the verb already says who: **(Io) parlo italiano** = **Parlo italiano**.\n\nPunjabi: Kartā (soggetto) + Kriyā (verbo) + Karam / Thāṁ (complemento).' },
        {
          title: 'Frase affermativa (+) e negativa (−)',
          table: [
            ['(+) Affermativa', '(−) Negativa: NON + verbo', 'English'],
            ['Io lavoro domenica.', 'Io non lavoro domenica.', 'I (don\'t) work on Sunday.'],
            ['Noi parliamo francese.', 'Noi non parliamo francese.', 'We (don\'t) speak French.'],
            ['Loro guardano la TV.', 'Loro non guardano la TV.', 'They (don\'t) watch TV.'],
          ],
        },
        {
          title: 'Examples',
          practice: false,
          words: [
            wp('Parlo punjabi e italiano.', 'I speak Punjabi and Italian.', 'Maiṁ Punjabi ate Italian boldā hāṁ.'),
            wp('Non parlo tedesco.', 'I don\'t speak German.', 'Maiṁ German nahīṁ boldā.'),
            wp('Oggi non lavoro.', 'I\'m not working today.', 'Ajj maiṁ kamm nahīṁ kardā.'),
          ],
        },
        { tip: '**NON always goes right before the verb** (Punjabi: NON hameshā kriyā toṁ pahilāṁ lagdā hai). *Io non lavoro* — never "Io lavoro non".' },
      ],
      exercises: [
        mc('Make it negative: "Io lavoro oggi."', 'Io non lavoro oggi.', 'Io lavoro non oggi.', 'Non io lavoro oggi.', 'Io lavoro oggi non.'),
        mc('Make it negative: "Loro guardano la TV."', 'Loro non guardano la TV.', 'Loro guardano non la TV.', 'Non loro guardano la TV.'),
        mc('Which sentence is correct?', 'Parlo italiano.', 'Parlo io italiano non.', 'Italiano parlo non.'),
        tr('Non parlo tedesco.', 'I don\'t speak German.', 'I speak German.', 'I study German.', 'I don\'t understand German.'),
        wo('Io non lavoro domenica', 'I don\'t work on Sunday'),
        wo('Noi non parliamo francese', 'We don\'t speak French'),
      ],
      homework: 'Write 3 things you do and 3 things you don\'t do (Cucino ogni giorno. Non guardo la TV…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Dialogue: Do you speak Italian? Are you working today?',
      titleIt: 'Lezione 1 · Dialogo: Parli italiano? Lavori oggi?',
      summary: 'Manpreet talks about where she lives and what she does today.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: Parli italiano? Lavori oggi?',
          situation: 'Two classmates meet after the Italian lesson.',
          dialogue: [
            lp('A', 'Ciao Manpreet! Tu abiti a Mantova?', 'Hi Manpreet! Do you live in Mantova?', 'Sat Srī Akāl Manpreet! Kī tūṁ Mantova vich rahindī haiṁ?'),
            lp('Manpreet', 'No, non abito a Mantova. Abito a Suzzara.', 'No, I don\'t live in Mantova. I live in Suzzara.', 'Nahīṁ, maiṁ Mantova vich nahīṁ rahindī. Maiṁ Suzzara vich rahindī hāṁ.'),
            lp('A', 'Lavori oggi pomeriggio o studi italiano?', 'Are you working this afternoon or studying Italian?', 'Kī ajj dupahir nūṁ kamm karnā hai jāṁ Italian paṛhnī hai?'),
            lp('Manpreet', 'Oggi non lavoro! Studio italiano e poi cucino a casa.', 'I\'m not working today! I study Italian and then I cook at home.', 'Ajj maiṁ kamm nahīṁ kardī! Maiṁ Italian paṛhdī hāṁ ate phir ghar khāṇā pakāuṇdī hāṁ.'),
          ],
        },
        {
          title: 'Intervista il tuo compagno (sì / no)',
          text: '1. **Che lingue parli?** — Sì: *Parlo punjabi e italiano.* · No: *Non parlo tedesco.*\n2. **Cucini a casa? Guardi la TV la sera?** — Sì: *Cucino ogni giorno.* · No: *Non guardo la TV.*\n3. **Dove abiti e dove lavori?** — *Abito a… e lavoro / non lavoro…*',
        },
      ],
      exercises: [
        tf('Manpreet abita a Mantova.', false, 'Manpreet lives in Mantova.'),
        tf('Manpreet abita a Suzzara.', true, 'Manpreet lives in Suzzara.'),
        tf('Oggi Manpreet lavora.', false, 'Manpreet is working today.'),
        tf('Dopo la lezione Manpreet cucina a casa.', true, 'After the lesson Manpreet cooks at home.'),
        tr('Che lingue parli?', 'What languages do you speak?', 'Where do you live?', 'Do you cook at home?', 'Are you working today?'),
        wo('Studio italiano e poi cucino', 'I study Italian and then I cook'),
      ],
      homework: 'Pair activity: ask a classmate the 3 interview questions and answer with sì or no (with NON).',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — I verbi in -ERE e -IRE
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Verbs in -ERE',
      titleIt: 'Lezione 2 · I verbi in -ERE',
      summary: 'Leggere, scrivere, prendere, vivere in the present tense.',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 3 · Lezione 2 (PDF)' },
        {
          title: '4 verbi importanti in -ERE',
          words: [
            wp('leggere', 'to read', 'Paṛhnā (kitāb)', { pron: 'LED-jeh-reh', img: 'Open book' }),
            wp('scrivere', 'to write', 'Likhṇā', { pron: 'SKREE-veh-reh', img: 'Writing hand' }),
            wp('prendere', 'to take / to have (bus, coffee)', 'Laiṇā (bass / kāfī)', { pron: 'PREN-deh-reh', img: 'Bus' }),
            wp('vivere', 'to live', 'Rahiṇā / Jīuṇā', { pron: 'VEE-veh-reh', img: 'House with garden' }),
          ],
        },
        {
          title: 'Presente: LEGG-ERE e SCRIV-ERE',
          table: [
            ['Soggetto', 'LEGGERE', 'SCRIVERE', 'English', 'Punjabi'],
            ['io', 'leggo', 'scrivo', 'I read / write', 'Maiṁ paṛhdā / likhdā hāṁ'],
            ['tu', 'leggi', 'scrivi', 'you read / write', 'Tūṁ paṛhdā / likhdā haiṁ'],
            ['lui / lei', 'legge', 'scrive', 'he / she reads', 'Uh paṛhdā / paṛhdī hai'],
            ['noi', 'leggiamo', 'scriviamo', 'we read / write', 'Asīṁ paṛhde hāṁ'],
            ['voi', 'leggete', 'scrivete', 'you (pl.) read', 'Tusīṁ paṛhde ho'],
            ['loro', 'leggono', 'scrivono', 'they read / write', 'Uh paṛhde han'],
          ],
        },
        {
          title: 'Esempi quotidiani',
          practice: false,
          words: [
            wp('Io leggo il libro di italiano.', 'I read the Italian book.', 'Maiṁ Italian kitāb paṛhdā hāṁ.'),
            wp('Tu scrivi sul quaderno.', 'You write in the notebook.', 'Tūṁ kāpī te likhdā haiṁ.'),
            wp('Noi prendiamo l\'autobus la mattina.', 'We take the bus in the morning.', 'Asīṁ savere bass lainde hāṁ.'),
            wp('Loro vivono in Italia da due anni.', 'They have lived in Italy for two years.', 'Uh do sāl toṁ Italy vich rahinde han.'),
          ],
        },
        { tip: 'Endings of -ERE verbs: **-o, -i, -e, -iamo, -ete, -ono**. Compare with -ARE: only lui/lei (**-e**), voi (**-ete**) and loro (**-ono**) are different.' },
      ],
      exercises: [
        fb('Io ___ il libro di italiano.', ['leggo', 'leggi', 'legge', 'leggono']),
        fb('Tu ___ sul quaderno.', ['scrivi', 'scrivo', 'scrive', 'scrivete']),
        fb('Lui ___ un caffè.', ['prende', 'prendi', 'prendo', 'prendono']),
        fb('Noi ___ l\'autobus la mattina.', ['prendiamo', 'prendete', 'prendono', 'prende']),
        fb('Voi ___ in Italia?', ['vivete', 'vivono', 'viviamo', 'vive']),
        fb('Loro ___ in Italia da due anni.', ['vivono', 'vivano', 'vive', 'vivete']),
        tr('Prendo l\'autobus.', 'I take the bus.', 'I drive the bus.', 'I wait for the bus.', 'I miss the bus.'),
        wo('Noi prendiamo l\'autobus', 'We take the bus'),
      ],
      homework: 'Write what you read, what you write and what transport you take every day (Leggo…, Scrivo…, Prendo…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Verbs in -IRE and -ISC-',
      titleIt: 'Lezione 2 · I verbi in -IRE e in -ISC-',
      summary: 'Dormire, aprire, partire — and capire, finire with -isc-.',
      minutes: 12,
      sections: [
        {
          title: 'Verbi in -IRE (gruppo normale)',
          words: [
            wp('dormire', 'to sleep', 'Sauṇā', { pron: 'dor-MEE-reh', img: 'Bed' }),
            wp('aprire', 'to open', 'Kholhṇā', { pron: 'ah-PREE-reh', img: 'Door' }),
            wp('partire', 'to leave', 'Ravānā hoṇā / Jāṇā', { pron: 'pahr-TEE-reh', img: 'Airplane departure' }),
          ],
        },
        {
          title: '-IRE normale: DORMIRE',
          table: [
            ['io', 'tu', 'lui / lei', 'noi', 'voi', 'loro'],
            ['dormo', 'dormi', 'dorme', 'dormiamo', 'dormite', 'dormono'],
            ['apro', 'apri', 'apre', 'apriamo', 'aprite', 'aprono'],
            ['parto', 'parti', 'parte', 'partiamo', 'partite', 'partono'],
          ],
        },
        {
          title: 'Verbi con -ISC-: CAPIRE e FINIRE',
          table: [
            ['Soggetto', 'CAPIRE (understand)', 'FINIRE (finish)', 'Punjabi'],
            ['io', 'capisco', 'finisco', 'Maiṁ samajhdā hāṁ'],
            ['tu', 'capisci', 'finisci', 'Tūṁ samajhdā haiṁ'],
            ['lui / lei', 'capisce', 'finisce', 'Uh samajhdā hai'],
            ['noi', 'capiamo (no -isc-)', 'finiamo (no -isc-)', 'Asīṁ samajhde hāṁ'],
            ['voi', 'capite (no -isc-)', 'finite (no -isc-)', 'Tusīṁ samajhde ho'],
            ['loro', 'capiscono', 'finiscono', 'Uh samajhde han'],
          ],
        },
        {
          title: 'Examples',
          practice: false,
          words: [
            wp('capire', 'to understand', 'Samajhṇā', { pron: 'kah-PEE-reh' }),
            wp('finire', 'to finish', 'Khatam karnā', { pron: 'fee-NEE-reh' }),
            wp('Capisco abbastanza.', 'I understand quite well.', 'Maiṁ kāfī samajhdā hāṁ.'),
            wp('Finisco il lavoro alle cinque.', 'I finish work at five.', 'Maiṁ 5 vaje kamm khatam kardā hāṁ.'),
          ],
        },
        { tip: '**-ISC-** goes in io, tu, lui/lei and loro, but **not in noi and voi**: *capisco, capisci, capisce, capiamo, capite, capiscono*. In **-sco** and **-scono** the sound is hard [sk]; in **-sci** and **-sce** it is soft [sh].' },
      ],
      exercises: [
        fb('Io ___ otto ore.', ['dormo', 'dormi', 'dorme', 'dormono']),
        fb('Tu ___ la finestra?', ['apri', 'apro', 'apre', 'aprite']),
        fb('Il treno ___ alle nove.', ['parte', 'parti', 'parto', 'partono']),
        fb('Io ___ bene l\'italiano.', ['capisco', 'capo', 'capisci', 'capiscono']),
        fb('Noi ___ alle sei.', ['finiamo', 'finiscamo', 'finiscono', 'finite']),
        fb('Voi ___ la lezione?', ['capite', 'capiscete', 'capiscite', 'capiamo']),
        fb('Loro ___ il lavoro tardi.', ['finiscono', 'finono', 'finiamo', 'finisce']),
        mc('Which form is correct for "noi"?', 'capiamo', 'capisciamo', 'capiscamo', 'capiscono'),
        wo('A che ora finisci il lavoro', 'What time do you finish work'),
      ],
      homework: 'Write at what time you finish work or school and how many hours you sleep (Finisco alle…, Dormo … ore).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · Dialogue: Do you understand Italian?',
      titleIt: 'Lezione 2 · Dialogo: Capisci l\'italiano?',
      summary: 'Harman reads and writes every day and takes the bus to school.',
      minutes: 10,
      sections: [
        {
          title: 'Ripasso: verbi in -ARE e NON',
          table: [
            ['Singolare', 'Plurale', 'Negativa'],
            ['io parlo · tu parli · lui parla', 'noi parliamo · voi parlate · loro parlano', 'Io non lavoro domenica.'],
          ],
        },
        {
          title: 'Dialogo: Capisci l\'italiano?',
          situation: 'Harman meets a friend at the bus stop.',
          dialogue: [
            lp('A', 'Ciao Harman! Capisci bene l\'italiano adesso?', 'Hi Harman! Do you understand Italian well now?', 'Sat Srī Akāl Harman! Kī tūṁ huṇ Italian chaṅgī tarhāṁ samajhdā haiṁ?'),
            lp('Harman', 'Sì, capisco abbastanza! Leggo e scrivo ogni giorno.', 'Yes, I understand quite well! I read and write every day.', 'Hāṁ, maiṁ kāfī samajhdā hāṁ! Maiṁ har roz paṛhdā ate likhdā hāṁ.'),
            lp('A', 'A che ora finisci il lavoro oggi?', 'What time do you finish work today?', 'Ajj kamm kadoṁ khatam kardā haiṁ?'),
            lp('Harman', 'Finisco alle cinque e poi prendo l\'autobus per andare a scuola!', 'I finish at five and then I take the bus to go to school!', 'Maiṁ 5 vaje kamm khatam kardā hāṁ ate phir skūl jāṇ laī bass laindā hāṁ!'),
          ],
        },
        {
          title: 'Intervista il tuo compagno',
          text: '1. **Leggi o scrivi in italiano a casa?** — *Leggo e scrivo ogni sera.* / *Non leggo molto.*\n2. **Prendi l\'autobus o il treno per andare al lavoro?** — *Prendo l\'autobus.* / *Non prendo il treno.*\n3. **Capisci l\'italiano? A che ora finisci il lavoro?** — *Capisco abbastanza! Finisco alle…*',
        },
      ],
      exercises: [
        tf('Harman capisce abbastanza l\'italiano.', true, 'Harman understands Italian quite well.'),
        tf('Harman legge e scrive ogni giorno.', true, 'Harman reads and writes every day.'),
        tf('Harman finisce il lavoro alle sette.', false, 'Harman finishes work at seven.'),
        tf('Dopo il lavoro Harman prende il treno.', false, 'After work Harman takes the train.'),
        tr('A che ora finisci il lavoro?', 'What time do you finish work?', 'What time do you start work?', 'Where do you work?', 'Do you like your work?'),
        wo('Capisco abbastanza l\'italiano', 'I understand Italian quite well'),
      ],
      homework: 'Pair activity: ask a classmate the 3 questions and write down the answers.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — Verbi irregolari e l'ora
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 3 · Irregular verbs',
      titleIt: 'Lezione 3 · I verbi irregolari',
      summary: 'Andare, fare, dare, stare · bere, volere, potere, dovere · venire, uscire, dire, salire.',
      minutes: 15,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 3 · Lezione 3 (PDF)' },
        {
          title: 'Irregolari in -ARE: andare, fare, dare, stare',
          table: [
            ['Soggetto', 'ANDARE (go · jāṇā)', 'FARE (do / make · karnā)', 'DARE (give · deṇā)', 'STARE (stay / be · rahiṇā)'],
            ['io', 'vado', 'faccio', 'do', 'sto'],
            ['tu', 'vai', 'fai', 'dai', 'stai'],
            ['lui / lei', 'va', 'fa', 'dà', 'sta'],
            ['noi', 'andiamo', 'facciamo', 'diamo', 'stiamo'],
            ['voi', 'andate', 'fate', 'date', 'state'],
            ['loro', 'vanno', 'fanno', 'danno', 'stanno'],
          ],
        },
        {
          title: 'Irregolari in -ERE: bere, volere, potere, dovere',
          table: [
            ['Soggetto', 'BERE (drink · pīṇā)', 'VOLERE (want · chāhuṇā)', 'POTERE (can · sakṇā)', 'DOVERE (must · chāhīdā)'],
            ['io', 'bevo', 'voglio', 'posso', 'devo'],
            ['tu', 'bevi', 'vuoi', 'puoi', 'devi'],
            ['lui / lei', 'beve', 'vuole', 'può', 'deve'],
            ['noi', 'beviamo', 'vogliamo', 'possiamo', 'dobbiamo'],
            ['voi', 'bevete', 'volete', 'potete', 'dovete'],
            ['loro', 'bevono', 'vogliono', 'possono', 'devono'],
          ],
        },
        {
          title: 'Irregolari in -IRE: venire, uscire, dire, salire',
          table: [
            ['Soggetto', 'VENIRE (come · āuṇā)', 'USCIRE (go out · bāhar jāṇā)', 'DIRE (say · kahiṇā)', 'SALIRE (go up / get on · chaṛhnā)'],
            ['io', 'vengo', 'esco', 'dico', 'salgo'],
            ['tu', 'vieni', 'esci', 'dici', 'sali'],
            ['lui / lei', 'viene', 'esce', 'dice', 'sale'],
            ['noi', 'veniamo', 'usciamo', 'diciamo', 'saliamo'],
            ['voi', 'venite', 'uscite', 'dite', 'salite'],
            ['loro', 'vengono', 'escono', 'dicono', 'salgono'],
          ],
        },
        {
          title: 'Examples',
          practice: false,
          words: [
            wp('Come stai? — Sto bene!', 'How are you? — I\'m fine!', 'Kī hāl hai? — Maiṁ ṭhīk hāṁ!'),
            wp('Vado al lavoro alle otto.', 'I go to work at eight.', 'Maiṁ aṭṭh vaje kamm te jāndā hāṁ.'),
            wp('Faccio colazione al bar.', 'I have breakfast at the café.', 'Maiṁ bār vich nāshtā kardā hāṁ.'),
            wp('Bevo un tè. Voglio riposare.', 'I drink tea. I want to rest.', 'Maiṁ chāh pīndā hāṁ. Maiṁ ārām karnā chāhundā hāṁ.'),
            wp('Posso venire alle cinque.', 'I can come at five.', 'Maiṁ panj vaje ā sakdā hāṁ.'),
            wp('Esco di casa alle sette.', 'I leave home at seven.', 'Maiṁ satt vaje ghar toṁ nikaldā hāṁ.'),
          ],
        },
        { tip: '**Volere, potere, dovere** are followed by a verb in the infinitive: *Voglio **uscire**. Posso **venire**? Devo **lavorare**.*' },
      ],
      exercises: [
        fb('Io ___ al lavoro in bicicletta.', ['vado', 'vai', 'va', 'andiamo']),
        fb('Noi ___ sport il sabato.', ['facciamo', 'fate', 'fanno', 'faccio']),
        fb('Come ___? (tu)', ['stai', 'sto', 'sta', 'state']),
        fb('Loro ___ il tè caldo.', ['bevono', 'beve', 'beviamo', 'bevi']),
        fb('Io ___ uscire stasera.', ['voglio', 'vuoi', 'vuole', 'vogliono']),
        fb('Tu ___ venire alle cinque?', ['puoi', 'posso', 'può', 'potete']),
        fb('Noi ___ lavorare domani.', ['dobbiamo', 'dovete', 'devono', 'devo']),
        fb('Lui ___ con gli amici.', ['esce', 'esco', 'esci', 'escono']),
        fb('Loro ___ al parco all\'una.', ['vengono', 'vengo', 'viene', 'venite']),
        mc('"Faccio" is a form of…', 'fare', 'finire', 'dare', 'stare'),
      ],
      homework: 'Write 4 sentences about your day with andare, fare, bere and uscire.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · Telling the time',
      titleIt: 'Lezione 3 · Che ore sono?',
      summary: 'È l\'una, sono le due… e un quarto, e mezza, meno un quarto — and the parts of the day.',
      minutes: 12,
      sections: [
        {
          title: 'Chiedere e dire l\'ora',
          words: [
            wp('Che ore sono? / Che ora è?', 'What time is it?', 'Kiṁne vaje han?', { pron: 'keh OH-reh SOH-noh', img: 'Mantelpiece clock' }),
            wp('È l\'una.', 'It\'s one o\'clock.', 'Ikk vajiā hai.', { pron: 'eh LOO-nah' }),
            wp('Sono le due.', 'It\'s two o\'clock.', 'Do vaje han.', { pron: 'SOH-noh leh DOO-eh' }),
            wp('È mezzogiorno.', 'It\'s midday.', 'Dupahir de bārāṁ vaje han.', { pron: 'eh med-dzoh-JOR-noh', img: 'Sun' }),
            wp('È mezzanotte.', 'It\'s midnight.', 'Adhī rāt hai.', { pron: 'eh med-dzah-NOT-teh', img: 'Crescent moon' }),
            wp('le tre e un quarto', '3:15 (quarter past three)', 'Savā tinn', { pron: 'leh TREH eh oon KWAHR-toh' }),
            wp('le otto e mezza', '8:30 (half past eight)', 'Sāḍhe aṭṭh', { pron: 'leh OT-toh eh MED-dzah' }),
            wp('le nove meno un quarto', '8:45 (quarter to nine)', 'Pauṇe nauṁ', { pron: 'leh NOH-veh MEH-noh oon KWAHR-toh' }),
          ],
        },
        {
          title: 'Le ore e i minuti',
          table: [
            ['Time', 'Italiano', 'Punjabi'],
            ['1:00', 'È l\'una. (singular!)', 'Ikk vajiā hai'],
            ['2:00 – 11:00', 'Sono le due / le undici.', 'Do / giārāṁ vaje han'],
            ['12:00', 'È mezzogiorno / mezzanotte.', 'Bārāṁ vaje'],
            [':15', 'e un quarto', 'Savā'],
            [':30', 'e mezza / e mezzo', 'Sāḍhe'],
            [':45', 'meno un quarto (of the next hour)', 'Pauṇe'],
          ],
        },
        {
          title: 'A che ora…? Le fasce del giorno',
          words: [
            wp('di mattina (6–12)', 'in the morning', 'Saver', { img: 'Sunrise', ex: 'Vado al lavoro alle otto di mattina.', exEn: 'I go to work at eight in the morning.' }),
            wp('di pomeriggio (13–18)', 'in the afternoon', 'Dupahar', { img: 'Sun', ex: 'Finisco il lavoro alle cinque di pomeriggio.', exEn: 'I finish work at five in the afternoon.' }),
            wp('di sera (18–23)', 'in the evening', 'Shām', { img: 'Sunset', ex: 'Vengo a scuola alle sette di sera.', exEn: 'I come to school at seven in the evening.' }),
            wp('di notte (23–5)', 'at night', 'Rāt', { img: 'Night with stars', ex: 'Dormo da mezzanotte alle sei.', exEn: 'I sleep from midnight to six.' }),
          ],
        },
        { tip: 'Asking **"A che ora…?"** (at what time?), answer with **alle** + hour: *alle otto, alle cinque*. With one o\'clock say **all\'una**: *Vado a casa all\'una.*' },
      ],
      exercises: [
        lis('Listen: what time is it?', 'Sono le sette e un quarto.', '07:15', '07:45', '06:15', '07:30'),
        lis('Listen: what time is it?', 'Sono le quattro e mezza.', '04:30', '04:15', '05:30', '03:30'),
        lis('Listen: what time is it?', 'Sono le otto meno un quarto.', '07:45', '08:15', '08:45', '07:15'),
        mc('01:00 = ?', 'È l\'una.', 'Sono le una.', 'È le uno.', 'Sono l\'una.'),
        mc('09:15 = ?', 'Sono le nove e un quarto.', 'Sono le nove meno un quarto.', 'Sono le nove e mezza.', 'Sono le dieci e un quarto.'),
        mc('12:00 (midday) = ?', 'È mezzogiorno.', 'È mezzanotte.', 'Sono le due.', 'È l\'una.'),
        fb('Vado a casa ___ una.', ['all\'', 'alle', 'al', 'a']),
        fb('Finisco il lavoro ___ cinque.', ['alle', 'all\'', 'a', 'le']),
      ],
      homework: 'Write your timetable: at what time you get up, go to work or school, eat and go to bed.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Dialogue: What time is it?',
      titleIt: 'Lezione 3 · Dialogo: Che ore sono?',
      summary: 'Jaspreet and a classmate take a coffee break.',
      minutes: 8,
      sections: [
        {
          title: 'Dialogo: Che ore sono?',
          situation: 'Two classmates during the Italian lesson.',
          dialogue: [
            lp('A', 'Scusa Jaspreet, che ore sono adesso?', 'Excuse me Jaspreet, what time is it now?', 'Māf karnā Jaspreet, huṇ kiṁne vaje han?'),
            lp('Jaspreet', 'Sono le dieci e un quarto! Facciamo una pausa?', 'It\'s quarter past ten! Shall we take a break?', 'Savā das vaje han! Thoṛhā ārām karīe?'),
            lp('A', 'Volentieri! Io bevo un caffè, e tu cosa bevi?', 'Gladly! I\'ll drink a coffee, and what will you drink?', 'Zarūr! Maiṁ kāfī pīndā hāṁ, tūṁ kī pīndā haiṁ?'),
            lp('Jaspreet', 'Io bevo un tè caldo. Poi all\'una vado a casa!', 'I\'ll drink a hot tea. Then at one I go home!', 'Maiṁ garam chāh pīndā hāṁ. Phir ikk vaje ghar jāndā hāṁ!'),
          ],
        },
        {
          title: 'Intervista il tuo compagno',
          text: '1. **Come stai oggi? A che ora vai al lavoro?** — *Sto bene! Vado al lavoro alle…*\n2. **Cosa bevi la mattina? Cosa vuoi fare?** — *Bevo tè o caffè. Voglio riposare!*\n3. **A che ora esci di casa? Quando vieni a scuola?** — *Esco alle… e vengo a scuola alle…*',
        },
      ],
      exercises: [
        tf('Sono le dieci e mezza.', false, 'It is half past ten.'),
        tf('Jaspreet beve un tè caldo.', true, 'Jaspreet drinks a hot tea.'),
        tf('Jaspreet va a casa all\'una.', true, 'Jaspreet goes home at one.'),
        tr('Facciamo una pausa?', 'Shall we take a break?', 'Shall we go home?', 'Shall we have lunch?', 'Shall we start?'),
        wo('Che ore sono adesso', 'What time is it now'),
        wo('Io bevo un tè caldo', 'I drink a hot tea'),
      ],
      homework: 'Pair activity: ask a classmate the 3 questions and answer with irregular verbs.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Tempo libero e frequenza
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 4 · Hobbies and sports',
      titleIt: 'Lezione 4 · Hobby, sport e tempo libero',
      summary: 'Giocare a cricket, andare in palestra, ascoltare musica, uscire con gli amici…',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 3 · Lezione 4 (PDF)' },
        {
          title: 'Hobby e sport',
          words: [
            wp('giocare a calcio', 'to play football (soccer)', 'Fuṭbāl kheḍṇā', { pron: 'joh-KAH-reh ah KAHL-choh', img: 'Soccer ball' }),
            wp('giocare a cricket', 'to play cricket', 'Krikeṭ kheḍṇā', { img: 'Cricket game' }),
            wp('giocare a carte', 'to play cards', 'Tāsh kheḍṇā', { img: 'Joker' }),
            wp('andare in palestra', 'to go to the gym', 'Jim jāṇā', { pron: 'pah-LES-trah', img: 'Person lifting weights' }),
            wp('andare in piscina', 'to go to the swimming pool', 'Pūl te jāṇā', { pron: 'pee-SHEE-nah', img: 'Person swimming' }),
            wp('andare in bici', 'to go cycling', 'Saikil chalāuṇā', { img: 'Bicycle' }),
            wp('correre', 'to run', 'Dauṛnā', { pron: 'KOR-reh-reh', img: 'Person running' }),
            wp('passeggiare al parco', 'to walk in the park', 'Pārk vich sair karnā', { pron: 'pahs-sed-JAH-reh', img: 'Person walking' }),
            wp('ascoltare musica', 'to listen to music', 'Sangīt suṇnā', { pron: 'ahs-kol-TAH-reh MOO-zee-kah', img: 'Headphone' }),
            wp('guardare film', 'to watch films', 'Filmāṁ vekhṇīāṁ', { img: 'Clapper board' }),
          ],
        },
        {
          title: 'Cosa fai nel tempo libero?',
          words: [
            wp('cucinare', 'to cook', 'Khāṇā baṇāuṇā', { img: 'Cooking', ex: 'Nel tempo libero cucino piatti indiani.', exEn: 'In my free time I cook Indian dishes.' }),
            wp('viaggiare', 'to travel', 'Safar karnā', { pron: 'vee-ahd-JAH-reh', img: 'Airplane', ex: 'In estate viaggiamo in treno in Italia.', exEn: 'In summer we travel around Italy by train.' }),
            wp('uscire con gli amici', 'to go out with friends', 'Dostāṁ nāḷ bāhar jāṇā', { img: 'Party popper', ex: 'Il venerdì sera esco con gli amici.', exEn: 'On Friday evening I go out with friends.' }),
            wp('restare a casa', 'to stay at home', 'Ghar rahiṇā', { img: 'Couch and lamp', ex: 'La domenica resto a casa e guardo la TV.', exEn: 'On Sunday I stay at home and watch TV.' }),
          ],
        },
        { tip: 'Sports with a ball or cards use **giocare a**: *gioco a calcio, a cricket, a carte*. Places use **andare in**: *vado in palestra, in piscina, in bici*.' },
      ],
      exercises: [
        fb('Il sabato gioco ___ cricket.', ['a', 'in', 'al', 'con']),
        fb('Vado ___ palestra tre volte a settimana.', ['in', 'a', 'al', 'per']),
        tr('uscire con gli amici', 'to go out with friends', 'to stay at home', 'to travel', 'to play cards'),
      ],
      homework: 'Write 4 things you do in your free time (Nel tempo libero gioco a…, vado in…, ascolto…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 4 · How often? Sempre, spesso, mai',
      titleIt: 'Lezione 4 · Gli avverbi di frequenza',
      summary: 'Sempre, spesso, qualche volta, raramente, non… mai — and where to put them.',
      minutes: 10,
      sections: [
        {
          title: 'Sempre, spesso, qualche volta, raramente, non… mai',
          words: [
            wp('sempre (100%)', 'always', 'Hameshā', { pron: 'SEM-preh', ex: 'Gioco sempre a cricket il sabato pomeriggio.', exEn: 'I always play cricket on Saturday afternoon.' }),
            wp('spesso (75%)', 'often', 'Aksar', { pron: 'SPES-soh', ex: 'Andiamo spesso a passeggiare al parco.', exEn: 'We often go for a walk in the park.' }),
            wp('qualche volta (40%)', 'sometimes', 'Kade-kade', { pron: 'KWAHL-keh VOL-tah', ex: 'Qualche volta guardo un film al cinema.', exEn: 'Sometimes I watch a film at the cinema.' }),
            wp('raramente (15%)', 'rarely', 'Bahut ghaṭṭ', { pron: 'rah-rah-MEN-teh', ex: 'Vado raramente in piscina in inverno.', exEn: 'I rarely go to the pool in winter.' }),
            wp('non… mai (0%)', 'never', 'Kade nahīṁ', { pron: 'non… MAH-ee', ex: 'Io non gioco mai a carte!', exEn: 'I never play cards!' }),
          ],
        },
        {
          title: 'Dove mettiamo gli avverbi?',
          table: [
            ['Rule', 'Examples'],
            ['sempre, spesso, raramente → after the verb', 'Io ascolto sempre la musica. · Lui va spesso in palestra.'],
            ['double negative: NON + verbo + MAI', 'Io non lavoro mai la domenica. · Noi non beviamo mai vino.'],
            ['qualche volta → at the start or at the end', 'Qualche volta vado al cinema. · Vado al cinema qualche volta.'],
          ],
        },
        { tip: 'In Italian **NON and MAI go together** (Punjabi: NON ate MAI ikaṭṭhe āuṇde han): *Non corro mai.* Saying "Corro mai" alone is wrong.' },
      ],
      exercises: [
        mc('Choose the correct sentence:', 'Io non vado mai in piscina.', 'Io vado non mai in piscina.', 'Io mai vado in piscina non.', 'Non io vado mai in piscina.'),
        mc('Choose the correct sentence:', 'Lui va spesso in palestra.', 'Lui spesso va non in palestra.', 'Spesso lui in palestra va.'),
        mc('"Raramente" means…', 'rarely', 'always', 'often', 'never'),
        mc('Which adverb means 100%?', 'sempre', 'spesso', 'qualche volta', 'raramente'),
        tr('Non lavoro mai la domenica.', 'I never work on Sunday.', 'I always work on Sunday.', 'I sometimes work on Sunday.', 'I often work on Sunday.'),
        wo('Io ascolto sempre la musica', 'I always listen to music'),
        wo('Noi non beviamo mai vino', 'We never drink wine'),
      ],
      homework: 'Write one sentence with each adverb: sempre, spesso, qualche volta, raramente, non… mai.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 4 · I like… Mi piace / Mi piacciono',
      titleIt: 'Lezione 4 · Esprimere gusti: mi piace',
      summary: 'Say what you like and don\'t like, with a dialogue about free time.',
      minutes: 12,
      sections: [
        {
          title: 'Mi piace / non mi piace + nome o infinito',
          table: [
            ['Struttura', 'Quando si usa', 'Esempio', 'English / Punjabi'],
            ['mi piace + infinito', 'with an action', 'Mi piace giocare a calcio.', 'I like playing · kheḍṇā pasand hai'],
            ['mi piace + nome singolare', 'with a singular noun', 'Mi piace lo sport / la musica.', 'I like sport · sangīt pasand hai'],
            ['mi piacciono + plurale', 'with a plural noun', 'Mi piacciono i film d\'azione.', 'I like action films · filmāṁ pasand han'],
            ['non mi piace + inf. / nome', 'negative', 'Non mi piace correre al freddo.', 'I don\'t like · pasand nahīṁ hai'],
            ['ti piace…?', 'question (+ molto / per niente)', 'Ti piace cucinare? — Sì, molto!', 'Do you like…? · kī tainūṁ pasand hai?'],
          ],
        },
        {
          title: 'Dialogo: Cosa ti piace fare nel tempo libero?',
          situation: 'Two friends talk about their hobbies.',
          dialogue: [
            lp('A', 'Ciao Gurpreet! Cosa ti piace fare nel tempo libero?', 'Hi Gurpreet! What do you like doing in your free time?', 'Sat Srī Akāl Gurpreet! Vehle sameṁ vich tainūṁ kī karnā pasand hai?'),
            lp('Gurpreet', 'Mi piace molto giocare a cricket e ascoltare musica. E a te?', 'I really like playing cricket and listening to music. And you?', 'Mainūṁ krikeṭ kheḍṇā ate sangīt suṇnā bahut pasand hai. Ate tainūṁ?'),
            lp('A', 'A me piace andare in palestra. Ci vai spesso anche tu?', 'I like going to the gym. Do you go there often too?', 'Mainūṁ jim jāṇā pasand hai. Kī tūṁ vī aksar jāndā haiṁ?'),
            lp('Gurpreet', 'Qualche volta! Invece non mi piace per niente correre, non corro mai!', 'Sometimes! But I don\'t like running at all, I never run!', 'Kade-kade! Par mainūṁ dauṛnā bilkul pasand nahīṁ, maiṁ kade nahīṁ dauṛdā!'),
          ],
        },
        {
          title: 'Intervista il tuo compagno',
          text: '1. **Cosa ti piace fare nel tempo libero?** — *Mi piace molto… / Non mi piace…*\n2. **Quale sport fai spesso? Quale raramente?** — *Gioco spesso a… Vado raramente…*\n3. **Cosa non fai mai nel tempo libero?** — *Non guardo mai… Non vado mai…*',
        },
        { tip: '**Mi piace** with one thing or an action, **mi piacciono** with more things: *Mi piace il caffè · Mi piacciono i cornetti.*' },
      ],
      exercises: [
        fb('___ cucinare piatti italiani e indiani.', ['Mi piace', 'Mi piacciono']),
        fb('Non ___ gli sport pericolosi.', ['mi piacciono', 'mi piace']),
        fb('___ la musica indiana.', ['Mi piace', 'Mi piacciono']),
        fb('___ i film d\'azione.', ['Mi piacciono', 'Mi piace']),
        tf('A Gurpreet piace giocare a cricket.', true, 'Gurpreet likes playing cricket.'),
        tf('Gurpreet corre spesso.', false, 'Gurpreet often runs.'),
        tr('Ti piace cucinare?', 'Do you like cooking?', 'Can you cook?', 'Are you cooking?', 'Do you cook often?'),
        wo('Non mi piace correre', 'I don\'t like running'),
      ],
      homework: 'Write 3 things you like and 2 things you don\'t like, using mi piace / mi piacciono / non mi piace.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 3
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 3 test',
      titleIt: 'Test Capitolo 3',
      summary: 'Listening, free time, the time, regular and irregular verbs, NON and mi piace. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 3',
          text: 'This test checks everything in Chapter 3 (Lessons 1–4).\n\n- **1. Listening** — sentences and times\n- **2. Vocabulary** — free time, sports, frequency and the time\n- **3. Grammar** — regular and irregular verbs, NON, mi piace / mi piacciono\n- **4. Writing** — sentences about your day\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (my day and free time) and the **oral interview** (daily actions, the time, hobbies — 2 minutes) are done **with your teacher** in the live class.' },
      ],
      exercises: [
        // 1A — Ascolta e scegli
        lis('1A · Listen and choose the sentence you hear', 'Io non parlo italiano molto bene, ma studio ogni giorno.', 'IO NON PARLO ITALIANO', 'IO PARLO ITALIANO'),
        lis('1A · Listen and choose the sentence you hear', 'Il venerdì finisco alle cinque del pomeriggio.', 'FINISCO ALLE CINQUE', 'FINISCO ALLE SETTE'),
        lis('1A · Listen and choose the sentence you hear', 'Nel tempo libero vado raramente in palestra.', 'VADO RARAMENTE IN PALESTRA', 'VADO SEMPRE IN PALESTRA'),
        lis('1A · Listen and choose the sentence you hear', 'Mi piace cucinare per la mia famiglia.', 'MI PIACE CUCINARE', 'NON MI PIACE CUCINARE'),
        lis('1A · Listen and choose the sentence you hear', 'La mattina noi beviamo un caffè insieme.', 'BEVIAMO UN CAFFÈ', 'LEGGIAMO UN LIBRO'),
        // 1B — Gli orari
        lis('1B · Listen and choose the time', 'Sono le sette e un quarto.', '07:15', '07:45', '06:15', '07:30'),
        lis('1B · Listen and choose the time', 'È l\'una.', '01:00', '11:00', '12:00', '01:30'),
        lis('1B · Listen and choose the time', 'Sono le quattro e mezza.', '04:30', '04:15', '05:30', '03:30'),
        lis('1B · Listen and choose the time', 'Sono le otto meno un quarto.', '07:45', '08:15', '08:45', '07:15'),
        lis('1B · Listen and choose the time', 'Sono le nove di sera.', '21:00', '19:00', '20:00', '21:30'),
        // 2A — Collega
        pairs('2A · Match each expression with its meaning', [
          ['Ascoltare musica', 'To listen to music · Sangīt suṇnā'],
          ['Giocare a calcio', 'To play football · Fuṭbāl kheḍṇā'],
          ['Uscire con gli amici', 'To go out with friends · Dostāṁ nāḷ bāhar jāṇā'],
          ['Sempre', 'Always · Hameshā'],
          ['Non … mai', 'Never · Kade nahīṁ'],
        ]),
        // 2B — L'ora in lettere
        mc('2B · 01:00 = ?', 'È l\'una.', 'Sono le una.', 'È le uno.', 'Sono l\'uno.'),
        mc('2B · 04:30 = ?', 'Sono le quattro e mezza.', 'Sono le quattro e un quarto.', 'Sono le cinque meno mezza.', 'È le quattro e mezza.'),
        mc('2B · 09:15 = ?', 'Sono le nove e un quarto.', 'Sono le nove meno un quarto.', 'Sono le dieci e un quarto.', 'Sono le nove e mezza.'),
        mc('2B · 12:00 = ?', 'È mezzogiorno.', 'Sono le dodici e mezza.', 'È l\'una.', 'Sono le due.'),
        mc('2B · 07:45 = ?', 'Sono le otto meno un quarto.', 'Sono le sette meno un quarto.', 'Sono le otto e un quarto.', 'Sono le sette e mezza.'),
        // 3A — Verbi regolari
        fb('3A · Noi ___ (lavorare) in fabbrica dal lunedì al venerdì.', ['lavoriamo', 'lavorate', 'lavorano', 'lavoro']),
        fb('3A · Tu ___ (leggere) un libro in italiano ogni sera?', ['leggi', 'leggo', 'legge', 'leggete']),
        fb('3A · Maria e Luca ___ (abitare) a Mantova.', ['abitano', 'abita', 'abitate', 'abitiamo']),
        fb('3A · Io ___ (capire) bene la lezione di italiano!', ['capisco', 'capo', 'capisci', 'capiamo']),
        fb('3A · Voi ___ (dormire) otto ore di notte.', ['dormite', 'dormiamo', 'dormono', 'dorme']),
        // 3B — Verbi irregolari e negazione
        fb('3B · Io ___ (andare) al lavoro in bicicletta.', ['vado', 'vai', 'va', 'andiamo']),
        fb('3B · Io ___ (fare) colazione al bar.', ['faccio', 'fai', 'fa', 'fanno']),
        fb('3B · Loro ___ (bere) il tè caldo.', ['bevono', 'beve', 'bevete', 'beviamo']),
        fb('3B · Loro ___ (uscire) di casa alle otto.', ['escono', 'escie', 'uscono', 'esce']),
        mc('3B · Make it negative: "Tu guardi la televisione di sera."', 'Tu non guardi la televisione di sera.', 'Tu guardi non la televisione di sera.', 'Non tu guardi la televisione di sera.', 'Tu guardi la televisione non di sera.'),
        // 3C — Mi piace e non… mai
        fb('3C · ___ cucinare piatti italiani e indiani.', ['Mi piace', 'Mi piacciono']),
        fb('3C · Non ___ gli sport pericolosi.', ['mi piacciono', 'mi piace']),
        mc('3C · Choose the correct sentence', 'Io non vado mai in piscina d\'inverno.', 'Io vado non mai in piscina d\'inverno.'),
        // 4 — La mia giornata
        wo('La mattina faccio colazione alle sette', 'In the morning I have breakfast at seven'),
        wo('Nel tempo libero gioco spesso a cricket', 'In my free time I often play cricket'),
        wo('Mi piace molto cucinare', 'I really like cooking'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences about your day, your timetable, your free time, what you never do and what you like. Then prepare the 2-minute oral interview.',
    },
  ],
};
