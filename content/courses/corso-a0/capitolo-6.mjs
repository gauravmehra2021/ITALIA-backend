import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 6 of the school syllabus (Corso di Italiano, livello A0 → A1).
// Source: syllabus/Capitolo 6 Lezione 1–4 (.pptx) and Test Capitolo 6 (.docx).

const slides = (lez) => `/content/slides/corso-a0-cap6-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-6',
  title: 'Chapter 6: Must, can, want — daily routine and work',
  titleIt: 'Capitolo 6: Dovere, potere, volere — la routine e il lavoro',
  cover: 'Briefcase',
  description: 'The modal verbs dovere, potere and volere + infinitive and polite requests, the reflexive verbs of the daily routine (mi sveglio, mi alzo…) with the time, jobs and workplaces (che lavoro fai?), reading a job advert and booking an appointment on the phone with Lei.',
  canDo: [
    'I can say what I must, can and want to do: devo lavorare, posso entrare?, voglio imparare',
    'I can ask politely: Posso uscire un attimo? Puoi ripetere? Vorrei un\'informazione',
    'I can describe my daily routine with reflexive verbs and the time',
    'I can say what job I do and where I work',
    'I can read a simple job advert: full-time, part-time, contratto, stipendio, esperienza',
    'I can phone an employer and book an appointment using Lei',
  ],
  liveClassFocus: 'Job interview role-play: describe your routine, say what work you do or want to do, then phone the "employer" to book an appointment with Vorrei fissare un appuntamento.',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — Dovere, potere, volere
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 1 · Must, can, want: modal verb + infinitive',
      titleIt: 'Lezione 1 · Dovere, potere, volere + infinito',
      summary: 'Devo lavorare, posso entrare?, voglio imparare — the full present tense.',
      minutes: 12,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 6 · Lezione 1 (PDF)' },
        {
          title: 'Verbo modale + infinito',
          practice: false,
          words: [
            wp('Devo lavorare.', 'I must / have to work.', 'Mainūṁ kamm karnā pavegā.', { img: 'Briefcase' }),
            wp('Posso entrare?', 'May I / Can I come in?', 'Kī maiṁ andar ā sakdā hāṁ?', { img: 'Door' }),
            wp('Voglio imparare l\'italiano.', 'I want to learn Italian.', 'Maiṁ Iṭālian sikhṇā chāhundā hāṁ.', { img: 'Open book' }),
          ],
        },
        {
          title: 'Presente indicativo (irregolare)',
          table: [
            ['Soggetto', 'DOVERE (must · paiṇā)', 'POTERE (can · sakṇā)', 'VOLERE (want · chāhuṇā)'],
            ['io', 'devo', 'posso', 'voglio'],
            ['tu', 'devi', 'puoi', 'vuoi'],
            ['lui / lei', 'deve', 'può', 'vuole'],
            ['noi', 'dobbiamo', 'possiamo', 'vogliamo'],
            ['voi', 'dovete', 'potete', 'volete'],
            ['loro', 'devono', 'possono', 'vogliono'],
          ],
        },
        {
          title: 'Quando usiamo dovere, potere e volere?',
          table: [
            ['Verbo', 'Uso', 'Esempio'],
            ['DOVERE', 'obligation · something you have to do', 'Domani devo lavorare presto.'],
            ['POTERE', 'permission, possibility, polite request', 'Scusi, posso entrare in aula?'],
            ['VOLERE', 'wish, personal goal', 'Voglio imparare bene l\'italiano.'],
          ],
        },
        { tip: 'The second verb **always stays in the infinitive**: *Voglio imparare* — never ~~Voglio imparo~~. *Devo lavorare, posso entrare, voglio prendere un caffè, possiamo aprire la finestra?*' },
      ],
      exercises: [
        fb('Domani io ___ lavorare presto.', ['devo', 'devi', 'deve', 'dobbiamo']),
        fb('Tu ___ venire con noi stasera?', ['puoi', 'posso', 'può', 'potete']),
        fb('Lei ___ imparare l\'italiano.', ['vuole', 'voglio', 'vuoi', 'vogliono']),
        fb('Noi ___ arrivare alle otto.', ['dobbiamo', 'devono', 'dovete', 'devo']),
        fb('Voi ___ aprire la finestra, per favore?', ['potete', 'possiamo', 'possono', 'puoi']),
        fb('Loro ___ prendere un caffè.', ['vogliono', 'vuole', 'volete', 'vogliamo']),
        fb('Voglio ___ l\'italiano.', ['imparare', 'imparo', 'impari', 'impara']),
        mc('"Devo lavorare" means…', 'I have to work', 'I can work', 'I want to work', 'I work'),
      ],
      homework: 'Write 3 sentences about today: one with devo, one with posso, one with voglio.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · Polite requests in class',
      titleIt: 'Lezione 1 · Richieste cortesi e permessi',
      summary: 'Posso entrare? Puoi ripetere, per favore? Posso uscire un attimo? Vorrei…',
      minutes: 8,
      sections: [
        {
          title: 'Richieste cortesi e permessi',
          words: [
            wp('Posso entrare?', 'May I come in?', 'Kī maiṁ andar ā sakdā hāṁ?', { pron: 'POS-soh en-TRAH-reh', img: 'Door', ex: 'Scusi il ritardo, posso entrare?', exEn: 'Sorry I\'m late, may I come in?' }),
            wp('Puoi ripetere, per favore?', 'Can you repeat, please?', 'Kirpā karke dubārā dasso?', { pron: 'PWOY ree-PEH-teh-reh', img: 'Ear' }),
            wp('Può ripetere, per favore?', 'Could you repeat, please? (polite)', 'Kirpā karke dubārā dasso jī? (ādar nāl)', { pron: 'PWOH ree-PEH-teh-reh', img: 'Person raising hand' }),
            wp('Posso uscire un attimo?', 'May I go out for a moment?', 'Kī maiṁ thoṛī der laī bāhar jā sakdā hāṁ?', { pron: 'POS-soh oo-SHEE-reh oon AHT-tee-moh', img: 'Person walking' }),
            wp('Vorrei un\'informazione.', 'I would like some information.', 'Mainūṁ ikk jāṇkārī chāhīdī hai.', { pron: 'vor-RAY oo-neen-for-mah-TSYOH-neh', img: 'Person tipping hand' }),
          ],
        },
        { tip: '**Vorrei** (I would like) is the polite form of **voglio**. In a shop, an office or on the phone always say *vorrei*: *Vorrei un caffè. Vorrei un\'informazione.*' },
      ],
      exercises: [
        mc('You arrive late to class. You say…', 'Scusi il ritardo, posso entrare?', 'Voglio entrare.', 'Devo entrare.', 'Entro adesso.'),
        mc('You didn\'t understand the teacher. You say…', 'Può ripetere, per favore?', 'Posso uscire un attimo?', 'Vorrei un caffè.', 'Devo lavorare.'),
        mc('At the information desk you say…', 'Vorrei un\'informazione.', 'Voglio informazione!', 'Devo un\'informazione.', 'Posso informazione.'),
        tr('Posso uscire un attimo?', 'May I go out for a moment?', 'Can you repeat?', 'May I come in?', 'I want to leave.'),
      ],
      homework: 'Learn the 5 polite requests and use two of them in your next class.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Dialogue: permissions and plans in class',
      titleIt: 'Lezione 1 · Dialogo: permessi e progetti in classe',
      summary: 'Harpreet is late: posso entrare? devo uscire alle undici…',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: permessi e progetti in classe',
          situation: 'Harpreet arrives late to the Italian class.',
          dialogue: [
            lp('Harpreet', 'Buongiorno! Scusi il ritardo, posso entrare?', 'Good morning! Sorry I\'m late, may I come in?', 'Sat Srī Akāl! Der laī māfī, kī maiṁ andar ā sakdā hāṁ?'),
            lp('Docente', 'Sì, certo! Puoi sederti vicino a Sara.', 'Yes, of course! You can sit next to Sara.', 'Hāṁ, zarūr! Tusīṁ Sārā de koḷ baiṭh sakde ho.'),
            lp('Harpreet', 'Grazie! Oggi devo uscire alle undici perché devo lavorare.', 'Thanks! Today I have to leave at eleven because I have to work.', 'Dhanvād! Ajj mainūṁ giārāṁ vaje jāṇā pavegā kiuṅki mainūṁ kamm karnā hai.'),
            lp('Docente', 'Nessun problema! Tu vuoi imparare l\'italiano e sei molto bravo.', 'No problem! You want to learn Italian and you\'re very good.', 'Koī gall nahīṁ! Tusīṁ Iṭālian sikhṇī chāhunde ho ate bahut vadhīā ho.'),
          ],
        },
        {
          title: 'Role-play: permessi, doveri e desideri',
          text: '1. **Chiedi un permesso:** usa *posso* o *puoi* con cortesia.\n2. **Parla di un dovere:** spiega cosa *devi* fare oggi.\n3. **Esprimi un desiderio:** di\' cosa *vuoi* imparare o fare.',
        },
      ],
      exercises: [
        tf('Harpreet arriva in ritardo.', true, 'Harpreet arrives late.'),
        tf('Harpreet si siede vicino a Marco.', false, 'Harpreet sits next to Marco.'),
        tf('Harpreet deve uscire alle undici.', true, 'Harpreet has to leave at eleven.'),
        tf('Harpreet esce perché vuole andare al bar.', false, 'Harpreet leaves because he wants to go to the café.'),
        wo('Oggi devo uscire alle undici', 'Today I have to leave at eleven'),
        wo('Voglio imparare bene l\'italiano', 'I want to learn Italian well'),
      ],
      homework: 'Role-play with a partner: ask a permission, say one thing you must do and one thing you want to do.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — I verbi riflessivi e la routine
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 2 · The daily routine: reflexive verbs',
      titleIt: 'Lezione 2 · I verbi riflessivi della routine',
      summary: 'Mi sveglio, mi alzo, mi lavo, mi vesto, mi pettino, mi preparo, mi addormento.',
      minutes: 10,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 6 · Lezione 2 (PDF)' },
        {
          title: 'I 7 verbi riflessivi della routine',
          words: [
            wp('svegliarsi — mi sveglio', 'to wake up — I wake up', 'Jāgṇā', { pron: 'zveh-LYAR-see', img: 'Sunrise', ex: 'Mi sveglio alle sette.', exEn: 'I wake up at seven.' }),
            wp('alzarsi — mi alzo', 'to get up — I get up', 'Uṭhṇā', { pron: 'ahl-TSAR-see', img: 'Person standing', ex: 'Mi alzo alle sette e un quarto.', exEn: 'I get up at a quarter past seven.' }),
            wp('lavarsi — mi lavo', 'to wash — I wash', 'Nahāuṇā / Muṁh dhoṇā', { pron: 'lah-VAR-see', img: 'Droplet', ex: 'Mi lavo alle sette e venti.', exEn: 'I wash at twenty past seven.' }),
            wp('vestirsi — mi vesto', 'to get dressed — I get dressed', 'Kapṛe pāuṇā', { pron: 'ves-TEER-see', img: 'Coat', ex: 'Mi vesto alle sette e trentacinque.', exEn: 'I get dressed at 7:35.' }),
            wp('pettinarsi — mi pettino', 'to comb your hair — I comb my hair', 'Vāl vāhuṇā', { pron: 'pet-tee-NAR-see', img: 'Woman', ex: 'Mi pettino alle sette e quarantacinque.', exEn: 'I comb my hair at 7:45.' }),
            wp('prepararsi — mi preparo', 'to get ready — I get ready', 'Tiār hoṇā', { pron: 'preh-pah-RAR-see', img: 'Backpack', ex: 'Mi preparo alle otto.', exEn: 'I get ready at eight.' }),
            wp('addormentarsi — mi addormento', 'to fall asleep — I fall asleep', 'Sauṁ jāṇā', { pron: 'ahd-dor-men-TAR-see', img: 'Sleeping face', ex: 'Mi addormento alle undici.', exEn: 'I fall asleep at eleven.' }),
          ],
        },
        { tip: 'Reflexive verbs end in **-si** (svegliar**si**) and the action goes back to the person: *Lavo la macchina* (I wash the car) but ***Mi** lavo le mani* (I wash my hands — myself).' },
      ],
      exercises: [
        mc('You open your eyes in the morning:', 'Mi sveglio.', 'Mi addormento.', 'Mi vesto.', 'Mi pettino.'),
        mc('You put on your clothes:', 'Mi vesto.', 'Mi lavo.', 'Mi alzo.', 'Mi sveglio.'),
        mc('You go to sleep at night:', 'Mi addormento.', 'Mi preparo.', 'Mi alzo.', 'Mi pettino.'),
        tr('mi alzo', 'I get up', 'I wake up', 'I get dressed', 'I get ready'),
      ],
      homework: 'Write your morning in order with 5 reflexive verbs (Mi sveglio, poi mi alzo…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Reflexive pronouns and the time',
      titleIt: 'Lezione 2 · I pronomi riflessivi e a che ora?',
      summary: 'Mi, ti, si, ci, vi, si — and alle sette, all\'una, a mezzogiorno.',
      minutes: 12,
      sections: [
        {
          title: 'Pronomi: mi, ti, si, ci, vi, si',
          table: [
            ['Soggetto', 'Pronome', 'ALZARSI (-ARE)', 'VESTIRSI (-IRE)'],
            ['io', 'mi', 'mi alzo', 'mi vesto'],
            ['tu', 'ti', 'ti alzi', 'ti vesti'],
            ['lui / lei', 'si', 'si alza', 'si veste'],
            ['noi', 'ci', 'ci alziamo', 'ci vestiamo'],
            ['voi', 'vi', 'vi alzate', 'vi vestite'],
            ['loro', 'si', 'si alzano', 'si vestono'],
          ],
        },
        { text: 'The pronoun goes **before** the verb: *Io **mi** sveglio alle sette.* The verb ending is the normal present tense (alz**o**, alz**i**, alz**a**…).' },
        {
          title: 'A che ora? ALLE, ALL\' e A',
          table: [
            ['Forma', 'Quando', 'Esempio'],
            ['ALLE', 'with the hours from 2 on', 'Mi sveglio alle sette. Alle otto e mezza.'],
            ['ALL\'', 'with una (1 o\'clock)', 'Pranziamo all\'una. All\'una e un quarto.'],
            ['A', 'mezzogiorno, mezzanotte', 'Mi addormento a mezzanotte.'],
          ],
        },
        { tip: 'Key question: **«A che ora ti alzi la mattina?»** — **«Mi alzo alle sette e un quarto.»**' },
      ],
      exercises: [
        fb('La mattina io ___ alle sei e mezza.', ['mi sveglio', 'ti svegli', 'si sveglia', 'sveglio']),
        fb('A che ora tu ___ nei giorni feriali?', ['ti alzi', 'mi alzo', 'si alza', 'alzi']),
        fb('Prima di uscire lui ___.', ['si lava', 'mi lavo', 'ti lavi', 'lava']),
        fb('Noi ___ velocemente per andare a lezione.', ['ci prepariamo', 'vi preparate', 'si preparano', 'prepariamo']),
        fb('Voi ___ in dieci minuti?', ['vi vestite', 'ci vestiamo', 'si vestono', 'vestite']),
        fb('La sera loro ___ verso le undici.', ['si addormentano', 'si addormenta', 'ci addormentiamo', 'addormentano']),
        fb('Pranziamo ___ una.', ['all\'', 'alle', 'a', 'al']),
        fb('Mi addormento ___ mezzanotte.', ['a', 'alle', 'all\'', 'al']),
        fb('Mi sveglio ___ sette.', ['alle', 'a', 'all\'', 'al']),
      ],
      homework: 'Write at what time you, a friend and your family wake up and go to sleep (Io mi sveglio alle…, mia madre si sveglia alle…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · Dialogue: What time do you get up?',
      titleIt: 'Lezione 2 · Dialogo: a che ora ti alzi?',
      summary: 'Elena and Jaspreet compare their routine — la mia giornata tipo.',
      minutes: 10,
      sections: [
        {
          title: 'La mia giornata tipo',
          table: [
            ['Ora', 'Momento', 'Cosa faccio'],
            ['07:00', 'la mattina', 'Mi sveglio e mi alzo subito.'],
            ['07:30', 'in bagno', 'Mi lavo e mi pettino.'],
            ['08:00', 'prima di uscire', 'Mi vesto e mi preparo.'],
            ['23:00', 'la sera', 'Mi addormento alle undici.'],
          ],
        },
        {
          title: 'Dialogo: a che ora ti alzi?',
          situation: 'Elena and Jaspreet talk about their mornings.',
          dialogue: [
            lp('Elena', 'Ciao Jaspreet! A che ora ti svegli di solito?', 'Hi Jaspreet! What time do you usually wake up?', 'Sat Srī Akāl Jaspreet! Tusīṁ āmtaur te kinne vaje jāgde ho?'),
            lp('Jaspreet', 'Mi sveglio alle sei e mezza e mi alzo alle sette.', 'I wake up at half past six and I get up at seven.', 'Maiṁ sāḍhe chhe vaje jāgdā hāṁ ate satt vaje uṭhdā hāṁ.'),
            lp('Elena', 'Anch\'io! Mi lavo, mi vesto e mi preparo in trenta minuti.', 'Me too! I wash, get dressed and get ready in thirty minutes.', 'Maiṁ vī! Maiṁ tīh minṭāṁ vich nahā ke, kapṛe pā ke tiār ho jāndī hāṁ.'),
            lp('Jaspreet', 'La sera però siamo stanchi e ci addormentiamo alle dieci!', 'But in the evening we\'re tired and we fall asleep at ten!', 'Par shām nūṁ asīṁ thakk jānde hāṁ ate das vaje sauṁ jānde hāṁ!'),
          ],
        },
        {
          title: 'Role-play: la tua routine quotidiana',
          text: '1. **Chiedi gli orari:** «A che ora ti svegli?»\n2. **Racconta la mattina:** usa quattro verbi riflessivi.\n3. **Chiudi con la sera:** di\' a che ora ti addormenti.',
        },
      ],
      exercises: [
        tf('Jaspreet si sveglia alle sei e mezza.', true, 'Jaspreet wakes up at half past six.'),
        tf('Jaspreet si alza alle otto.', false, 'Jaspreet gets up at eight.'),
        tf('Elena si prepara in trenta minuti.', true, 'Elena gets ready in thirty minutes.'),
        tf('La sera si addormentano a mezzanotte.', false, 'In the evening they fall asleep at midnight.'),
        wo('A che ora ti svegli di solito', 'What time do you usually wake up'),
        wo('Mi lavo e mi vesto in trenta minuti', 'I wash and get dressed in thirty minutes'),
      ],
      homework: 'Ask a partner about their routine and write 4 sentences about it (Lui / lei si sveglia alle…).',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — Mestieri, professioni e ambienti di lavoro
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · Jobs and professions',
      titleIt: 'Lezione 3 · I mestieri e le professioni',
      summary: 'L\'operaio, il commesso, l\'autista, la badante, il cuoco, il cameriere, l\'impiegato…',
      minutes: 12,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 6 · Lezione 3 (PDF)' },
        {
          title: 'Servizi, ristorazione e negozio',
          words: [
            wp('il commesso / la commessa', 'shop assistant', 'Dukāndār / Selzmain', { pron: 'kom-MES-soh', img: 'Shopping cart', ex: 'La commessa lavora in negozio.', exEn: 'The shop assistant works in a shop.' }),
            wp('il cuoco / la cuoca', 'cook / chef', 'Rasoīā / Kukk', { pron: 'KWOH-koh', img: 'Cook', ex: 'Il cuoco lavora al ristorante.', exEn: 'The cook works at the restaurant.' }),
            wp('il cameriere / la cameriera', 'waiter / waitress', 'Veṭar', { pron: 'kah-meh-RYEH-reh', img: 'Fork and knife with plate' }),
            wp('il / la badante', 'caregiver (for elderly people)', 'Buzurgāṁ dī dekhbhāl karan vāḷā', { pron: 'bah-DAHN-teh', img: 'Older person', ex: 'La badante lavora in casa di anziani.', exEn: 'The caregiver works in elderly people\'s homes.' }),
          ],
        },
        {
          title: 'Fabbrica, ufficio, scuola e cantiere',
          words: [
            wp('l\'operaio / l\'operaia', 'factory worker', 'Faikṭarī mazdūr', { pron: 'loh-peh-RAH-yoh', img: 'Factory worker', ex: 'L\'operaio lavora in fabbrica.', exEn: 'The factory worker works in a factory.' }),
            wp('l\'autista', 'driver', 'Ḍrāīvar', { pron: 'low-TEES-tah', img: 'Delivery truck', ex: 'L\'autista lavora in autobus o in camion.', exEn: 'The driver works on a bus or a lorry.' }),
            wp('l\'impiegato / l\'impiegata', 'office clerk', 'Daftarī karmachārī', { pron: 'leem-pyeh-GAH-toh', img: 'Office worker', ex: 'L\'impiegata lavora in ufficio.', exEn: 'The clerk works in an office.' }),
            wp('l\'insegnante', 'teacher', 'Adhiāpak', { pron: 'leen-sen-YAHN-teh', img: 'Teacher', ex: 'L\'insegnante lavora a scuola.', exEn: 'The teacher works at a school.' }),
            wp('il muratore', 'bricklayer / builder', 'Rājmistrī', { pron: 'eel moo-rah-TOH-reh', img: 'Construction worker', ex: 'Il muratore lavora in cantiere.', exEn: 'The builder works on a building site.' }),
          ],
        },
        {
          title: 'Maschile e femminile',
          table: [
            ['Maschile', 'Femminile', 'Dove lavora?'],
            ['l\'operaio', 'l\'operaia', 'in fabbrica'],
            ['il commesso', 'la commessa', 'in negozio / al supermercato'],
            ['l\'autista', 'l\'autista', 'in autobus / in camion'],
            ['il badante', 'la badante', 'in casa (di anziani)'],
            ['il cuoco / il cameriere', 'la cuoca / la cameriera', 'al ristorante / al bar'],
            ['l\'impiegato', 'l\'impiegata', 'in ufficio'],
            ['l\'insegnante', 'l\'insegnante', 'a scuola'],
            ['il muratore', 'la muratrice', 'in cantiere'],
          ],
        },
        { tip: 'Words in **-ista** and **-ante / -ente** are the same for men and women: *l\'autista, il / la badante, l\'insegnante*. Only the article changes.' },
      ],
      exercises: [
        mc('Who works in a factory?', 'l\'operaio', 'l\'insegnante', 'il cameriere', 'la commessa'),
        mc('Who looks after elderly people at home?', 'la badante', 'l\'autista', 'il muratore', 'l\'impiegata'),
        mc('Who works on a building site (in cantiere)?', 'il muratore', 'il cuoco', 'la commessa', 'l\'insegnante'),
        mc('Who works at the computer in an office?', 'l\'impiegato', 'l\'operaio', 'il muratore', 'la badante'),
        tr('il cuoco', 'cook', 'waiter', 'driver', 'shop assistant'),
      ],
      homework: 'Write the jobs of 4 people in your family in Italian (Mio padre fa…, mia sorella è…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 3 · What job do you do? Where do you work?',
      titleIt: 'Lezione 3 · Che lavoro fai? Dove lavori?',
      summary: 'Faccio il / la… · Sono… · in fabbrica, al ristorante, a scuola — and work shifts.',
      minutes: 12,
      sections: [
        {
          title: 'Chiedere e dire che lavoro si fa',
          table: [
            ['Domanda / risposta', 'Regola', 'Esempio'],
            ['Che lavoro fai? / Che lavoro fa?', 'informale (tu) / formale (Lei)', 'Che lavoro fa, signor Singh?'],
            ['FACCIO IL / LA…', 'with the article', 'Faccio l\'operaio. Faccio la commessa.'],
            ['SONO…', 'without the article', 'Sono autista. Sono insegnante.'],
          ],
        },
        { text: 'Remember **FARE**: io *faccio* · tu *fai* · lui / lei *fa* · noi *facciamo* · voi *fate* · loro *fanno*.' },
        {
          title: 'Dove lavori? Ambienti di lavoro',
          table: [
            ['IN', 'AL', 'A'],
            ['in fabbrica, in cantiere', 'al supermercato', 'a scuola'],
            ['in negozio, in ufficio', 'al ristorante, al bar', 'a casa'],
          ],
        },
        {
          title: 'Frasi utili sul lavoro e sui turni',
          practice: false,
          words: [
            wp('Lavoro di mattina / di pomeriggio.', 'I work in the morning / in the afternoon.', 'Maiṁ savere / dupahir nūṁ kamm kardā hāṁ.'),
            wp('Faccio il turno di notte.', 'I do the night shift.', 'Maiṁ rāt dī shifṭ kardā hāṁ.'),
            wp('Cerco lavoro come autista.', 'I\'m looking for work as a driver.', 'Maiṁ ḍrāīvar vajoṁ kamm labh rihā hāṁ.'),
            wp('Ho la patente B e C.', 'I have a B and C driving licence.', 'Mere kol B ate C lāīsaiṅs hai.'),
            wp('Domani devo lavorare otto ore.', 'Tomorrow I have to work eight hours.', 'Kall mainūṁ aṭṭh ghaṇṭe kamm karnā pavegā.'),
            wp('Voglio trovare un buon lavoro.', 'I want to find a good job.', 'Maiṁ ikk changā kamm labhṇā chāhundā hāṁ.'),
          ],
        },
      ],
      exercises: [
        fb('Io ___ l\'operaio in fabbrica.', ['faccio', 'fai', 'fa', 'fanno']),
        fb('Che lavoro ___, signor Singh?', ['fa', 'fai', 'faccio', 'fate']),
        fb('Mia sorella fa ___ insegnante.', ['l\'', 'la', 'il', 'lo']),
        fb('Che lavoro fai? — Sono ___.', ['autista', 'il autista', 'un autista il', 'la autisto']),
        fb('Il cuoco lavora ___ ristorante.', ['al', 'in', 'a', 'alla']),
        fb('L\'operaio lavora ___ fabbrica.', ['in', 'al', 'a', 'nel']),
        fb('L\'insegnante lavora ___ scuola.', ['a', 'in la', 'al', 'di']),
        mc('"Faccio il turno di notte" means…', 'I do the night shift', 'I work at night only on Sunday', 'I sleep at night', 'I start at nine'),
      ],
      homework: 'Write 3 sentences about your job: what you do, where you work and your shift.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Dialogue: What job do you do?',
      titleIt: 'Lezione 3 · Dialogo: che lavoro fai?',
      summary: 'Manpreet and Giulia talk about their jobs and their families\' jobs.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: che lavoro fai?',
          situation: 'Manpreet meets his friend Giulia.',
          dialogue: [
            lp('Manpreet', 'Ciao Giulia! Che lavoro fai adesso?', 'Hi Giulia! What job do you do now?', 'Sat Srī Akāl Giulia! Huṇ tusīṁ kī kamm karde ho?'),
            lp('Giulia', 'Faccio la commessa in un negozio in centro. E tu?', 'I\'m a shop assistant in a shop in the centre. And you?', 'Maiṁ shahir de vichkār ikk dukān vich selzgarl dā kamm kardī hāṁ. Ate tusīṁ?'),
            lp('Manpreet', 'Io faccio l\'autista, e mio fratello è cuoco al ristorante.', 'I\'m a driver, and my brother is a cook at the restaurant.', 'Maiṁ ḍrāīvar hāṁ, ate merā bharā resṭoraiṇṭ vich rasoīā hai.'),
            lp('Giulia', 'Bello! Mia sorella invece fa l\'insegnante a scuola.', 'Nice! My sister, instead, is a teacher at a school.', 'Bahut vadhīā! Merī bhaiṇ sakūl vich adhiāpak hai.'),
          ],
        },
        {
          title: 'Role-play: intervista sul lavoro',
          text: '1. **Chiedi la professione:** «Che lavoro fai?»\n2. **Di\' dove lavori:** in fabbrica, in ufficio, al bar…\n3. **Aggiungi gli orari:** spiega quando inizi e finisci («Inizio alle otto e finisco alle quattro»).',
        },
      ],
      exercises: [
        tf('Giulia fa la commessa.', true, 'Giulia is a shop assistant.'),
        tf('Manpreet fa il cuoco.', false, 'Manpreet is a cook.'),
        tf('Il fratello di Manpreet lavora al ristorante.', true, 'Manpreet\'s brother works at the restaurant.'),
        tf('La sorella di Giulia lavora in ufficio.', false, 'Giulia\'s sister works in an office.'),
        wo('Faccio la commessa in un negozio', 'I\'m a shop assistant in a shop'),
        wo('Mio fratello è cuoco al ristorante', 'My brother is a cook at the restaurant'),
      ],
      homework: 'Interview a partner: their job, where they work and their working hours. Write 3 sentences.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Cercare lavoro e appuntamenti
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 4 · Reading a job advert',
      titleIt: 'Lezione 4 · Leggere un annuncio di lavoro',
      summary: 'L\'annuncio, full-time, part-time, il contratto, lo stipendio, l\'esperienza.',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 6 · Lezione 4 (PDF)' },
        {
          title: 'Parole chiave in un annuncio di lavoro',
          words: [
            wp('l\'annuncio di lavoro', 'job advert', 'Naukarī dā ishtihār', { pron: 'lahn-NOON-choh', img: 'Newspaper', ex: 'Ho letto il vostro annuncio.', exEn: 'I read your advert.' }),
            wp('full-time (tempo pieno)', 'full-time (40 hours)', 'Pūrā samāṁ (ful-ṭāīm)', { img: 'Mantelpiece clock', ex: 'Cerco un lavoro full-time.', exEn: 'I\'m looking for a full-time job.' }),
            wp('part-time (tempo parziale)', 'part-time (20 hours)', 'Addhā samāṁ (pārṭ-ṭāīm)', { img: 'Alarm clock', ex: 'Preferisco un turno part-time.', exEn: 'I prefer a part-time shift.' }),
            wp('il contratto', 'employment contract', 'Kamm dā ikrārnāmā', { pron: 'eel kon-TRAHT-toh', img: 'Page facing up', ex: 'Offriamo un contratto regolare.', exEn: 'We offer a regular contract.' }),
            wp('lo stipendio', 'salary / wage', 'Tankhāh', { pron: 'loh stee-PEN-dyoh', img: 'Euro banknote', ex: 'Lo stipendio è 1.400 euro al mese.', exEn: 'The salary is 1,400 euros a month.' }),
            wp('l\'esperienza', 'work experience', 'Kamm dā tajurbā', { pron: 'les-peh-RYEN-tsah', img: 'Hundred points', ex: 'Ho tre anni di esperienza.', exEn: 'I have three years\' experience.' }),
          ],
        },
        {
          title: 'Contratto e requisiti',
          table: [
            ['Parola', 'Significato'],
            ['contratto fisso / a termine', 'permanent / fixed-term contract'],
            ['stipendio mensile', 'monthly pay'],
            ['con esperienza', 'you already know the work'],
            ['senza esperienza', 'first time — no experience needed'],
          ],
        },
        {
          title: 'Analizziamo un annuncio vero',
          text: '**OFFERTA DI LAVORO • MANTOVA — Cercasi cameriere / a**\n\n- Orario: part-time (solo sera)\n- Si richiede minima esperienza\n- Contratto a norma di legge\n- Chiamare il 333-4567890\n\n**1. Che lavoro è?** Cameriere o cameriera al ristorante.\n**2. Qual è l\'orario?** Part-time, lavoro solo la sera.\n**3. Cosa devo fare?** Telefonare per fissare un appuntamento.',
        },
        { tip: '**Cercasi** at the top of an advert means "wanted": *Cercasi cameriere* = Waiter wanted.' },
      ],
      exercises: [
        mc('A job of only 20 hours a week is…', 'part-time', 'full-time', 'a termine', 'senza contratto'),
        mc('"Lo stipendio" is…', 'the salary', 'the contract', 'the advert', 'the shift'),
        mc('In the Mantova advert, what is the job?', 'cameriere / cameriera', 'cuoco', 'autista', 'commessa'),
        mc('In the Mantova advert, when do you work?', 'solo la sera', 'solo la mattina', 'tutto il giorno', 'la notte'),
        tr('l\'esperienza', 'work experience', 'salary', 'contract', 'advert'),
      ],
      homework: 'Find a job advert (online or in a shop window) and write: what job, what hours, what you must do.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 4 · Booking an appointment on the phone (Lei)',
      titleIt: 'Lezione 4 · L\'appuntamento al telefono con il Lei',
      summary: 'Buongiorno, sono… · Chiamo per l\'annuncio · Vorrei fissare un appuntamento · Quando può?',
      minutes: 10,
      sections: [
        {
          title: 'I 4 passi della telefonata',
          table: [
            ['Passo', 'Cosa dici'],
            ['1. Salutare e presentarsi', 'Buongiorno, sono Harpreet Singh.'],
            ['2. Spiegare il motivo', 'Chiamo per l\'annuncio di lavoro.'],
            ['3. Fissare l\'appuntamento', 'Vorrei fissare un appuntamento.'],
            ['4. Confermare giorno e ora', 'Giovedì alle dieci va benissimo.'],
          ],
        },
        {
          title: '3 formule d\'oro per chiamare',
          words: [
            wp('Vorrei fissare un appuntamento.', 'I would like to book an appointment.', 'Maiṁ mulākāt taiā karnā chāhundā hāṁ.', { img: 'Calendar' }),
            wp('Quando posso venire in ufficio?', 'When can I come to the office?', 'Maiṁ daftar kadoṁ ā sakdā hāṁ?', { img: 'Office building' }),
            wp('Grazie mille e arrivederci!', 'Thank you very much and goodbye!', 'Bahut dhanvād ate phir milāṅge!', { img: 'Waving hand' }),
          ],
        },
        { tip: 'On the phone with an employer always use **Lei** (the polite form): *Quando **può**?* (not *puoi*), *Buongiorno*, *Arrivederci* — never *Ciao*!' },
      ],
      exercises: [
        mc('On the phone with the office you say…', 'Vorrei fissare un appuntamento.', 'Voglio un appuntamento.', 'Dammi un appuntamento.', 'Ciao, appuntamento?'),
        mc('To ask the employer when they are free you say…', 'Quando può ricevermi per un colloquio?', 'Quando puoi?', 'Quando vuoi tu?', 'Devi ricevermi.'),
        mc('You end the formal call with…', 'Grazie mille e arrivederci.', 'Ciao ciao!', 'A dopo, bello!', 'Buonanotte.'),
        fb('Buongiorno, ___ per l\'annuncio di lavoro.', ['chiamo', 'chiami', 'chiama', 'chiamano']),
        fb('Giovedì alle dieci ___ benissimo.', ['va', 'vado', 'vai', 'vanno']),
        wo('Vorrei fissare un appuntamento', 'I would like to book an appointment'),
      ],
      homework: 'Write the 4 steps of a phone call to the Mantova restaurant with your name and a day and time.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 4 · Phone call: I\'d like to book an appointment',
      titleIt: 'Lezione 4 · Telefonata: vorrei fissare un appuntamento',
      summary: 'Simran calls the restaurant "Da Luigi" about the waitress job.',
      minutes: 10,
      sections: [
        {
          title: 'Telefonata: vorrei fissare un appuntamento',
          situation: 'Simran phones the restaurant that published the job advert.',
          dialogue: [
            lp('Segretaria', 'Pronto, Ristorante Da Luigi, buongiorno.', 'Hello, Restaurant Da Luigi, good morning.', 'Hailo, Resṭoraiṇṭ Da Luigi, Sat Srī Akāl.'),
            lp('Simran', 'Buongiorno, sono Simran Kaur. Vorrei fissare un appuntamento per l\'annuncio.', 'Good morning, I\'m Simran Kaur. I\'d like to book an appointment about the advert.', 'Sat Srī Akāl, maiṁ Simran Kaur hāṁ. Maiṁ ishtihār bāre mulākāt taiā karnā chāhundī hāṁ.'),
            lp('Segretaria', 'Ha già esperienza come cameriera? Può venire venerdì alle 15:00?', 'Do you already have experience as a waitress? Can you come on Friday at 3 pm?', 'Kī tuhānūṁ veṭar dā tajurbā hai? Kī tusīṁ shukkarvār tinn vaje ā sakde ho?'),
            lp('Simran', 'Sì, ho due anni di esperienza. Venerdì alle tre va benissimo, grazie!', 'Yes, I have two years\' experience. Friday at three is perfect, thank you!', 'Hāṁ jī, do sāl dā tajurbā hai. Shukkarvār tinn vaje ṭhīk hai, dhanvād!'),
          ],
        },
        {
          title: 'Role-play: chiamata per un colloquio',
          text: '1. **Presentati al telefono:** saluta con *Buongiorno* e di\' il tuo nome.\n2. **Chiedi l\'appuntamento:** usa *Vorrei fissare un appuntamento*.\n3. **Fissa giorno e ora:** conferma l\'orario e saluta con *Arrivederci*.',
        },
      ],
      exercises: [
        tf('Simran chiama un ristorante.', true, 'Simran calls a restaurant.'),
        tf('Simran non ha esperienza.', false, 'Simran has no experience.'),
        tf('L\'appuntamento è venerdì alle tre.', true, 'The appointment is on Friday at three.'),
        tf('La segretaria usa il "tu".', false, 'The secretary uses "tu".'),
        wo('Ho due anni di esperienza', 'I have two years\' experience'),
        wo('Venerdì alle tre va benissimo', 'Friday at three is perfect'),
      ],
      homework: 'Role-play with a partner: phone for a job interview, introduce yourself and fix a day and time.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 6
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 6 test',
      titleIt: 'Test Capitolo 6',
      summary: 'Listening, jobs and job adverts, dovere / potere / volere and reflexive verbs. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 6',
          text: 'This test checks everything in Chapter 6 (Lessons 1–4).\n\n- **1. Listening** — sentences and a phone call for a job appointment\n- **2. Vocabulary** — jobs, routine and job adverts\n- **3. Grammar** — dovere, potere, volere and reflexive verbs\n- **4. Writing** — sentences about your routine, your job and an appointment\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (my routine, my job, an appointment) and the **oral interview** (routine, jobs and modal verbs, a formal phone call — 2 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A — Ascolta e scegli
        lis('1A · Listen and choose the sentence you hear', 'Scusi, posso entrare, per favore? Ho una domanda.', 'POSSO ENTRARE, PER FAVORE?', 'DEVO LAVORARE OGGI'),
        lis('1A · Listen and choose the sentence you hear', 'La domenica non lavoro e mi addormento a mezzanotte.', 'MI ADDORMENTO A MEZZANOTTE', 'MI SVEGLIO ALLE SEI E MEZZA'),
        lis('1A · Listen and choose the sentence you hear', 'Faccio il cuoco al ristorante dal martedì alla domenica.', 'FACCIO IL CUOCO AL RISTORANTE', 'FACCIO L\'AUTISTA IN FABBRICA'),
        lis('1A · Listen and choose the sentence you hear', 'Nell\'annuncio c\'è scritto: il lavoro è part-time con contratto.', 'IL LAVORO È PART-TIME CON CONTRATTO', 'LO STIPENDIO È MENSILE SENZA ESPERIENZA'),
        lis('1A · Listen and choose the sentence you hear', 'Studio ogni giorno perché voglio imparare l\'italiano bene.', 'VOGLIO IMPARARE L\'ITALIANO', 'VORREI FISSARE UN APPUNTAMENTO'),
        // 1B — La telefonata
        lis('1B · Listen to the call. What is the advert for? (1)', 'Buongiorno, chiamo per l\'annuncio da magazziniere.', 'MAGAZZINIERE', 'INSEGNANTE', 'BADANTE', 'COMMESSA'),
        lis('1B · Listen again: "Vorrei fissare un…" (2)', 'Vorrei fissare un appuntamento per il lavoro full-time.', 'APPUNTAMENTO', 'CONTRATTO', 'STIPENDIO', 'ANNUNCIO'),
        lis('1B · Listen again: what kind of job? (3)', 'Vorrei fissare un appuntamento per il lavoro full-time.', 'FULL-TIME', 'PART-TIME', 'DI NOTTE', 'A TERMINE'),
        lis('1B · Listen again: "Ho già…" (4)', 'Ho già esperienza in questo settore.', 'ESPERIENZA', 'LA PATENTE', 'UN CONTRATTO', 'UN LAVORO'),
        lis('1B · Listen again: when can the person come? (5)', 'Posso venire domani alle dieci.', 'DOMANI', 'OGGI', 'VENERDÌ', 'STASERA'),
        // 2A — Collega
        pairs('2A · Match each word with its meaning', [
          ['Badante', 'Caregiver · Buzurgāṁ dī dekhbhāl'],
          ['Commesso / Commessa', 'Shop assistant · Dukāndār'],
          ['Stipendio', 'Salary · Tankhāh'],
          ['Contratto', 'Work contract · Ikrārnāmā'],
          ['Vestirsi', 'To get dressed · Kapṛe pāuṇā'],
        ]),
        // 2B — Mestieri e annunci
        mc('2B · Lavora al computer in ufficio: ?', 'impiegato', 'operaio', 'muratore', 'part-time'),
        mc('2B · Lavora in fabbrica: ?', 'operaio', 'impiegato', 'esperienza', 'muratore'),
        mc('2B · Lavora in cantiere: ?', 'muratore', 'impiegato', 'operaio', 'esperienza'),
        mc('2B · Solo 20 ore a settimana: ?', 'part-time', 'full-time', 'esperienza', 'muratore'),
        mc('2B · Anni di lavoro già fatti: ?', 'esperienza', 'part-time', 'impiegato', 'operaio'),
        // 3A — Modali
        fb('3A · Scusi, ___ (potere - io) fissare un appuntamento per il colloquio?', ['posso', 'puoi', 'può', 'possono']),
        fb('3A · Noi ___ (dovere) arrivare al lavoro alle otto in punto.', ['dobbiamo', 'dovete', 'devono', 'devo']),
        fb('3A · Tu ___ (volere) lavorare full-time o part-time?', ['vuoi', 'voglio', 'vuole', 'volete']),
        // 3B — Riflessivi
        fb('3B · La mattina io ___ (svegliarsi) alle sei e mezza.', ['mi sveglio', 'si sveglia', 'ti svegli', 'sveglio']),
        fb('3B · A che ora tu ___ (alzarsi) nei giorni feriali?', ['ti alzi', 'mi alzo', 'si alza', 'alzi']),
        fb('3B · Prima di uscire lui ___ (lavarsi), si pettina e si veste.', ['si lava', 'mi lavo', 'ti lavi', 'lava']),
        fb('3B · Noi ___ (prepararsi) velocemente per andare a lezione.', ['ci prepariamo', 'si preparano', 'vi preparate', 'prepariamo']),
        fb('3B · La sera loro ___ (addormentarsi) verso le undici.', ['si addormentano', 'si addormenta', 'ci addormentiamo', 'addormentano']),
        // 3C — Scegli
        fb('3C · Ogni mattina io ___ alle sette e faccio colazione.', ['mi alzo', 'alzo']),
        fb('3C · Al telefono con l\'ufficio diciamo: «___»', ['Vorrei fissare un appuntamento', 'Voglio un appuntamento']),
        fb('3C · Per chiedere in modo formale: «Quando ___ ricevermi per un colloquio?»', ['può', 'puoi']),
        // 4 — Produzione guidata
        wo('La mattina mi sveglio alle sette', 'In the morning I wake up at seven'),
        wo('Cerco un lavoro con orario part-time', 'I\'m looking for a part-time job'),
        wo('Vorrei fissare un appuntamento per il colloquio', 'I would like to book an appointment for the interview'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences about your routine, your job and booking an appointment. Then prepare the 2-minute oral interview.',
    },
  ],
};
