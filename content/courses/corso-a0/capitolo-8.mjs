import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 8 of the school syllabus (Corso di Italiano, livello A1 → A2).
// Source: syllabus/Capitolo 8 Lezione 1–4 (.pptx) and Test Capitolo 8 (.docx).

const slides = (lez) => `/content/slides/corso-a0-cap8-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-8',
  title: 'Chapter 8: The past with AVERE — what did you do?',
  titleIt: 'Capitolo 8: Il passato prossimo con AVERE',
  cover: 'Spiral calendar',
  description: 'The passato prossimo with avere (ho mangiato, ho venduto, ho dormito), past time expressions (ieri, la settimana scorsa, due giorni fa), the most common irregular participles (fatto, detto, letto, scritto, visto, preso, bevuto…), telling a story in order with prima, poi, dopo, infine, and writing an informal message to a friend.',
  canDo: [
    'I can form the passato prossimo with avere: ho mangiato, hai venduto, abbiamo dormito',
    'I can say when something happened: ieri, l\'altro ieri, la settimana scorsa, un mese fa',
    'I can use the 11 most common irregular participles: fatto, detto, letto, visto, preso, bevuto…',
    'I can tell what I did yesterday or at the weekend in order: prima, poi, dopo, infine',
    'I can say what I didn\'t do: non ho lavorato',
    'I can write a short WhatsApp message or email to a friend about an event',
  ],
  liveClassFocus: 'Partner interview about last week, then tell the class what your partner did (io ho → lui / lei ha). Write a 50–70 word message to a friend about your weekend.',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — Il passato prossimo con AVERE
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 1 · The passato prossimo with AVERE',
      titleIt: 'Lezione 1 · Il passato prossimo con AVERE',
      summary: 'Avere + participle: ho mangiato, ho venduto, ho dormito — -ato, -uto, -ito.',
      minutes: 12,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 8 · Lezione 1 (PDF)' },
        {
          title: 'Dal presente al passato prossimo',
          table: [
            ['Oggi (presente)', 'Ieri (passato prossimo)'],
            ['Oggi mangio la pasta.', 'Ieri ho mangiato la pasta.'],
            ['Oggi vendo la macchina.', 'Ieri ho venduto la macchina.'],
            ['Oggi dormo otto ore.', 'Ieri ho dormito otto ore.'],
          ],
        },
        { text: '**Passato prossimo = present of AVERE + past participle.** *AVERE* changes with the person (ho, hai, ha, abbiamo, avete, hanno); with *avere* the participle **always ends in -o**.' },
        {
          title: 'Le 3 desinenze: -ATO, -UTO, -ITO',
          table: [
            ['-ARE → -ATO', '-ERE → -UTO', '-IRE → -ITO'],
            ['mangiare → mangiato', 'vendere → venduto', 'dormire → dormito'],
            ['lavorare → lavorato', 'ricevere → ricevuto', 'finire → finito'],
            ['comprare → comprato', 'potere → potuto', 'capire → capito'],
          ],
        },
        {
          title: 'Coniugazione completa',
          table: [
            ['Persona', 'AVERE', 'MANGIARE', 'VENDERE', 'DORMIRE'],
            ['io', 'ho', 'ho mangiato', 'ho venduto', 'ho dormito'],
            ['tu', 'hai', 'hai mangiato', 'hai venduto', 'hai dormito'],
            ['lui / lei / Lei', 'ha', 'ha mangiato', 'ha venduto', 'ha dormito'],
            ['noi', 'abbiamo', 'abbiamo mangiato', 'abbiamo venduto', 'abbiamo dormito'],
            ['voi', 'avete', 'avete mangiato', 'avete venduto', 'avete dormito'],
            ['loro', 'hanno', 'hanno mangiato', 'hanno venduto', 'hanno dormito'],
          ],
        },
      ],
      exercises: [
        fb('Ieri sera noi ___ pasta al pomodoro.', ['abbiamo mangiato', 'hanno mangiato', 'avete mangiato', 'ho mangiato']),
        fb('L\'anno scorso Marco ___ la sua vecchia macchina.', ['ha venduto', 'ho venduto', 'hanno venduto', 'ha vendito']),
        fb('Stanotte tu ___ bene?', ['hai dormito', 'ha dormito', 'hai dormuto', 'ho dormito']),
        fb('Ieri io ___ fino alle sei.', ['ho lavorato', 'ha lavorato', 'ho lavoruto', 'hai lavorato']),
        fb('Loro ___ il pane al mercato.', ['hanno comprato', 'ha comprato', 'abbiamo comprato', 'hanno compruto']),
        mc('vendere → ?', 'venduto', 'vendato', 'vendito', 'venduta'),
        mc('finire → ?', 'finito', 'finuto', 'finato', 'finita'),
        mc('capire → ?', 'capito', 'caputo', 'capato', 'capisce'),
      ],
      homework: 'Write 5 things you did yesterday with avere (Ieri ho mangiato…, ho lavorato…).',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · When did it happen? Past time expressions',
      titleIt: 'Lezione 1 · Espressioni di tempo passato',
      summary: 'Ieri, ieri sera, l\'altro ieri, la settimana scorsa, il mese scorso, due giorni fa.',
      minutes: 10,
      sections: [
        {
          title: 'Quando è successo?',
          words: [
            wp('ieri · ieri sera · ieri mattina', 'yesterday · last night · yesterday morning', 'Kall (bītiā hoiā)', { pron: 'YEH-ree', img: 'Spiral calendar' }),
            wp('l\'altro ieri', 'the day before yesterday', 'Parsoṁ (bītiā hoiā)', { pron: 'LAHL-troh YEH-ree', img: 'Calendar' }),
            wp('la settimana scorsa', 'last week', 'Pichhle hafte', { pron: 'lah set-tee-MAH-nah SKOR-sah' }),
            wp('il mese scorso · l\'anno scorso', 'last month · last year', 'Pichhle mahīne · pichhle sāl', { pron: 'eel MEH-zeh SKOR-soh' }),
            wp('due giorni fa', 'two days ago', 'Do din pahilāṁ', { pron: 'DOO-eh JOR-nee fah', img: 'Hourglass done' }),
            wp('una settimana fa · un mese fa', 'a week ago · a month ago', 'Ikk hafta pahilāṁ · ikk mahīnā pahilāṁ', { pron: 'OO-nah set-tee-MAH-nah fah' }),
          ],
        },
        {
          title: 'Scorso / scorsa e fa',
          table: [
            ['Forma', 'Regola', 'Esempio'],
            ['SCORSO', 'with masculine nouns', 'Il mese scorso ho lavorato tanto. Il weekend scorso, l\'anno scorso.'],
            ['SCORSA', 'with feminine nouns', 'La settimana scorsa ho comprato un cappotto. L\'estate scorsa.'],
            ['FA', 'always at the end: number + time + fa', 'Ho finito il corso un mese fa.'],
          ],
        },
        { tip: '**Fa** always comes **after** the time: *due giorni **fa***, never ~~fa due giorni~~. **Scorso / scorsa** agrees with the noun: *il mese scors**o**, la settimana scors**a***.' },
      ],
      exercises: [
        fb('___ settimana scorsa ho comprato un cappotto.', ['La', 'Il', 'Lo', 'L\'']),
        fb('Il mese ___ ho lavorato tanto.', ['scorso', 'scorsa', 'fa', 'ieri']),
        fb('L\'estate ___ ho lavorato in un ristorante.',['scorsa', 'scorso', 'fa', 'prossima']),
        fb('Ho finito il corso un mese ___.', ['fa', 'scorso', 'scorsa', 'ieri']),
        fb('Sono arrivato in Italia sei mesi ___.', ['fa', 'scorso', 'scorsi', 'ieri']),
        mc('"L\'altro ieri" means…', 'the day before yesterday', 'yesterday evening', 'last week', 'the other day next week'),
        tr('due giorni fa', 'two days ago', 'in two days', 'every two days', 'for two days'),
      ],
      homework: 'Write 4 sentences with ieri, l\'altro ieri, la settimana scorsa and un mese fa.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Dialogue: What did you do yesterday?',
      titleIt: 'Lezione 1 · Dialogo: cos\'hai fatto ieri?',
      summary: 'Elena and Gurpreet talk about shopping, cooking and a new bike.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: cos\'hai fatto ieri?',
          situation: 'Elena meets Gurpreet the day after work.',
          dialogue: [
            lp('Elena', 'Ciao Gurpreet! Cos\'hai fatto ieri dopo il lavoro?', 'Hi Gurpreet! What did you do yesterday after work?', 'Sat Srī Akāl Gurpreet! Kall kamm toṁ bāad tusīṁ kī kītā?'),
            lp('Gurpreet', 'Ieri ho comprato le verdure al mercato e ho cucinato il riso.', 'Yesterday I bought vegetables at the market and cooked rice.', 'Kall maiṁ bāzār toṁ sabzīāṁ kharīdīāṁ ate chaul pakāe.'),
            lp('Elena', 'Bravo! Io invece la settimana scorsa ho venduto la mia vecchia bici.', 'Well done! Last week, instead, I sold my old bike.', 'Shābāsh! Maiṁ pichhle hafte āpṇā purāṇā saikil vech dittā.'),
            lp('Gurpreet', 'Davvero? Anch\'io ho comprato una bici nuova un mese fa!', 'Really? I bought a new bike a month ago too!', 'Sachchī? Maiṁ vī ikk mahīnā pahilāṁ navāṁ saikil kharīdiā sī!'),
          ],
        },
        {
          title: 'Intervista al compagno: racconta il passato',
          text: '1. **Ieri e l\'altro ieri:** Cos\'hai mangiato? Quanto hai dormito?\n2. **La settimana scorsa:** Dove hai lavorato? Cosa hai comprato?\n3. **Un mese fa:** Chi hai incontrato o chiamato?',
        },
      ],
      exercises: [
        tf('Gurpreet ha comprato le verdure al mercato.', true, 'Gurpreet bought vegetables at the market.'),
        tf('Gurpreet ha cucinato la pasta.', false, 'Gurpreet cooked pasta.'),
        tf('Elena ha venduto la sua vecchia bici.', true, 'Elena sold her old bike.'),
        tf('Gurpreet ha comprato una bici nuova ieri.', false, 'Gurpreet bought a new bike yesterday.'),
        wo('Ieri ho cucinato il riso', 'Yesterday I cooked rice'),
        wo('Ho comprato una bici nuova un mese fa', 'I bought a new bike a month ago'),
      ],
      homework: 'Interview a partner with the 3 questions and write their answers.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — I participi passati irregolari
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 2 · The 11 key irregular participles',
      titleIt: 'Lezione 2 · Gli 11 participi irregolari più frequenti',
      summary: 'Fatto, detto, letto, scritto, visto, preso, bevuto, aperto, chiuso, chiesto, risposto.',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 8 · Lezione 2 (PDF)' },
        {
          title: 'Regolari e irregolari: cosa cambia?',
          table: [
            ['Regolari (-ato / -uto / -ito)', 'Irregolari (forme speciali)'],
            ['mangiare → ho mangiato', 'fare → ho fatto (not ~~facciuto~~)'],
            ['vendere → ho venduto', 'prendere → ho preso (not ~~prenduto~~)'],
            ['dormire → ho dormito', 'aprire → ho aperto (not ~~aprito~~)'],
          ],
        },
        {
          title: 'Infinito → participio passato',
          words: [
            wp('fare → fatto', 'to do / make → done', 'Kītā', { img: 'Flexed biceps', ex: 'Ho fatto colazione.', exEn: 'I had breakfast.' }),
            wp('dire → detto', 'to say → said', 'Kihā', { img: 'Speaking head', ex: 'Il direttore ha detto di sì.', exEn: 'The manager said yes.' }),
            wp('leggere → letto', 'to read → read', 'Paṛhiā', { img: 'Open book', ex: 'Ho letto un libro in italiano.', exEn: 'I read a book in Italian.' }),
            wp('scrivere → scritto', 'to write → written', 'Likhiā', { img: 'Writing hand', ex: 'Ho scritto un messaggio al capo.', exEn: 'I wrote a message to the boss.' }),
            wp('vedere → visto', 'to see → seen', 'Vekhiā', { img: 'Eyes', ex: 'Abbiamo visto un bel film.', exEn: 'We watched a good film.' }),
            wp('prendere → preso', 'to take → taken', 'Laiā', { img: 'Bus', ex: 'Ho preso l\'autobus numero 4.', exEn: 'I took bus number 4.' }),
            wp('bere → bevuto', 'to drink → drunk', 'Pītā', { img: 'Hot beverage', ex: 'Ho bevuto un caffè.', exEn: 'I drank a coffee.' }),
            wp('aprire → aperto', 'to open → opened', 'Kholhiā', { img: 'Door', ex: 'Il negozio ha aperto alle nove.', exEn: 'The shop opened at nine.' }),
            wp('chiudere → chiuso', 'to close → closed', 'Band kītā', { img: 'Locked', ex: 'La banca ha chiuso alle cinque.', exEn: 'The bank closed at five.' }),
            wp('chiedere → chiesto', 'to ask → asked', 'Puchhiā', { img: 'Person raising hand', ex: 'Ho chiesto un giorno di ferie.', exEn: 'I asked for a day off.' }),
            wp('rispondere → risposto', 'to answer → answered', 'Javāb dittā', { img: 'Mobile phone', ex: 'Lui ha risposto subito.', exEn: 'He answered at once.' }),
          ],
        },
        {
          title: '4 gruppi facili da ricordare',
          table: [
            ['Gruppo', 'Participi'],
            ['-TTO', 'fatto · detto · letto · scritto'],
            ['-SO', 'preso · chiuso'],
            ['-TO / -STO', 'visto · aperto · chiesto · risposto'],
            ['speciale -UTO', 'bere → bevuto'],
          ],
        },
        { tip: 'Golden rule: these verbs always take **AVERE + participle in -o**: *ho fatto, hai letto, ha bevuto, abbiamo visto*.' },
      ],
      exercises: [
        mc('fare → ?', 'fatto', 'faciuto', 'farato', 'fattato'),
        mc('prendere → ?', 'preso', 'prenduto', 'prendito', 'presto'),
        mc('aprire → ?', 'aperto', 'aprito', 'aputo', 'aprato'),
        mc('vedere → ?', 'visto', 'vedato', 'vedito', 'vidato'),
        mc('dire → ?', 'detto', 'dito', 'diciuto', 'diretto'),
        mc('leggere → ?', 'letto', 'leggiuto', 'leggito', 'letuto'),
        tr('ho chiesto', 'I asked', 'I answered', 'I closed', 'I said'),
      ],
      homework: 'Learn the 11 participles: cover the right column and say them aloud.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Irregular participles in context',
      titleIt: 'Lezione 2 · I participi irregolari nella vita quotidiana',
      summary: 'At home, at work and in the shops: ho bevuto, ho scritto, ha risposto, ha chiuso…',
      minutes: 12,
      sections: [
        {
          title: 'Contesto 1 · Casa, colazione e tempo libero',
          practice: false,
          words: [
            wp('Stamattina ho bevuto un caffè e ho fatto colazione.', 'This morning I drank a coffee and had breakfast.', 'Ajj savere maiṁ kauffī pītī ate nāshtā kītā.'),
            wp('Ieri pomeriggio ho letto un libro in italiano.', 'Yesterday afternoon I read a book in Italian.', 'Kall dupahir maiṁ Iṭālian vich kitāb paṛhī.'),
            wp('Ieri sera noi abbiamo visto un bel film in TV.', 'Last night we watched a good film on TV.', 'Kall rāt asīṁ ṭīvī te vadhīā film vekhī.'),
            wp('Per venire qui ho preso l\'autobus numero 4.', 'To come here I took bus number 4.', 'Ithe āuṇ laī maiṁ nambar 4 bass laī.'),
          ],
        },
        {
          title: 'Contesto 2 · Lavoro, messaggi e negozi',
          table: [
            ['Coppia', 'Esempio'],
            ['scritto e risposto', 'Ho scritto un messaggio al capo. Lui ha risposto subito!'],
            ['chiesto e detto', 'Ho chiesto un giorno di ferie. Il direttore ha detto di sì!'],
            ['aperto e chiuso', 'Il negozio ha aperto alle nove. La banca ha chiuso alle cinque.'],
          ],
        },
      ],
      exercises: [
        fb('Stamattina io ___ un cappuccino al bar.', ['ho bevuto', 'ho beuto', 'ho bevato', 'ha bevuto']),
        fb('Sabato scorso i miei amici ___ il treno per Verona.', ['hanno preso', 'hanno prenduto', 'ha preso', 'abbiamo preso']),
        fb('Tu ___ l\'email all\'ufficio ieri?', ['hai scritto', 'hai scrivuto', 'ha scritto', 'hai scrito']),
        fb('Il supermercato ___ alle otto di sera.', ['ha chiuso', 'ha chiudato', 'hanno chiuso', 'ha chiuduto']),
        fb('Noi ___ un\'informazione all\'insegnante.', ['abbiamo chiesto', 'abbiamo chieduto', 'avete chiesto', 'hanno chiesto']),
        fb('… e lei ___ subito.', ['ha risposto', 'ha risponduto', 'ho risposto', 'ha rispostato']),
        fb('Ieri sera ___ un bel film in TV.', ['abbiamo visto', 'abbiamo vedato', 'abbiamo vedito', 'avete vedito']),
        fb('Il direttore ___ di sì!', ['ha detto', 'ha dito', 'ha diciuto', 'ha dicato']),
      ],
      homework: 'Write 6 sentences about your week with 6 different irregular participles.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · Dialogue: a morning at the office',
      titleIt: 'Lezione 2 · Dialogo: una mattina in ufficio',
      summary: 'Simran opened the office, read the emails and answered the client.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: una mattina in ufficio',
          situation: 'Marco arrives at the office where Simran works.',
          dialogue: [
            lp('Marco', 'Ciao Simran! Hai già aperto l\'ufficio stamattina?', 'Hi Simran! Have you already opened the office this morning?', 'Sat Srī Akāl Simran! Kī tusīṁ ajj savere daftar khol dittā?'),
            lp('Simran', 'Sì! Ho preso il treno delle sette, ho letto le email e ho già risposto.', 'Yes! I took the seven o\'clock train, read the emails and already answered.', 'Hāṁ! Maiṁ satt vaje dī ṭren laī, īmel paṛhīāṁ ate javāb vī de dittā.'),
            lp('Marco', 'Brava! Cos\'ha detto il cliente? Hai scritto il preventivo?', 'Well done! What did the client say? Did you write the quote?', 'Shābāsh! Grāhak ne kī kihā? Kī tusīṁ kīmat dā aṁdāzā likhiā?'),
            lp('Simran', 'Ha chiesto uno sconto, ma ho fatto tutto. Adesso prendiamo un caffè!', 'He asked for a discount, but I did everything. Now let\'s have a coffee!', 'Us ne chhoṭ maṅgī, par maiṁ sārā kamm kar liā. Huṇ kauffī pīe!'),
          ],
        },
        {
          title: 'Gioco della memoria: 3 domande lampo',
          text: '1. **Bere e prendere:** Cos\'hai bevuto stamattina? Quale autobus hai preso?\n2. **Leggere e scrivere:** A chi hai scritto ieri? Cos\'hai letto?\n3. **Vedere e fare:** Quale film hai visto? Cos\'hai fatto domenica?',
        },
      ],
      exercises: [
        tf('Simran ha preso il treno delle sette.', true, 'Simran took the seven o\'clock train.'),
        tf('Simran non ha letto le email.', false, 'Simran didn\'t read the emails.'),
        tf('Il cliente ha chiesto uno sconto.', true, 'The client asked for a discount.'),
        tf('Marco ha scritto il preventivo.', false, 'Marco wrote the quote.'),
        wo('Ho letto le email e ho risposto', 'I read the emails and answered'),
        wo('Il cliente ha chiesto uno sconto', 'The client asked for a discount'),
      ],
      homework: 'Ask a partner the 3 quick questions and write their answers with lui / lei ha…',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — Che cosa hai fatto ieri / nel fine settimana?
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 3 · Telling a story in order: prima, poi, dopo, infine',
      titleIt: 'Lezione 3 · I connettivi del racconto',
      summary: 'First, then, after, finally — Harpreet\'s Sunday in 4 steps.',
      minutes: 10,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 8 · Lezione 3 (PDF)' },
        {
          title: 'I 4 connettivi del racconto',
          words: [
            wp('prima', 'first', 'Pahilāṁ', { pron: 'PREE-mah', img: 'Keycap 1' }),
            wp('poi', 'then', 'Phir', { pron: 'POY', img: 'Keycap 2' }),
            wp('dopo', 'after / afterwards', 'Bāad vich', { pron: 'DOH-poh', img: 'Keycap 3' }),
            wp('infine', 'finally / in the end', 'Ākhir vich', { pron: 'een-FEE-neh', img: 'Keycap 4' }),
          ],
        },
        {
          title: 'La domenica di Harpreet in 4 passi',
          practice: false,
          words: [
            wp('Prima ho bevuto un tè caldo e ho fatto colazione.', 'First I drank a hot tea and had breakfast.', 'Pahilāṁ maiṁ garam chāh pītī ate nāshtā kītā.'),
            wp('Poi ho preso la bici e ho fatto la spesa al mercato.', 'Then I took the bike and did the shopping at the market.', 'Phir maiṁ saikil laiā ate bāzār toṁ saudā liā.'),
            wp('Dopo ho cucinato il pranzo e ho chiamato la mia famiglia.', 'After that I cooked lunch and called my family.', 'Bāad vich maiṁ dupahir dā khāṇā banāiā ate āpṇe parivār nūṁ fon kītā.'),
            wp('Infine ho visto un film e ho dormito presto.', 'Finally I watched a film and went to sleep early.', 'Ākhir vich maiṁ film vekhī ate jaldī sauṁ giā.'),
          ],
        },
      ],
      exercises: [
        fb('___ ho fatto colazione, poi ho lavato i piatti.', ['Prima', 'Infine', 'Scorso', 'Fa']),
        fb('Prima ho fatto la spesa, ___ ho cucinato.', ['poi', 'prima', 'fa', 'scorso']),
        fb('___ la sera ho visto un film e ho dormito.', ['Infine', 'Ieri fa', 'Scorsa', 'Fa']),
        mc('"Infine" means…', 'finally', 'first', 'then', 'before'),
        wo('Prima ho bevuto un tè caldo', 'First I drank a hot tea'),
        wo('Infine ho visto un film', 'Finally I watched a film'),
      ],
      homework: 'Write your last Sunday in 4 steps with prima, poi, dopo, infine.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · The weekend: questions and answers',
      titleIt: 'Lezione 3 · Il fine settimana: domande e risposte',
      summary: 'Ho pulito casa, ho riposato… · Com\'è andato il fine settimana? · non ho lavorato.',
      minutes: 12,
      sections: [
        {
          title: 'Attività frequenti nel fine settimana',
          words: [
            wp('Ho pulito casa.', 'I cleaned the house.', 'Maiṁ ghar sāf kītā.', { img: 'Broom' }),
            wp('Ho lavato i vestiti.', 'I washed the clothes.', 'Maiṁ kapṛe dhote.', { img: 'Soap' }),
            wp('Ho cucinato e ho riposato.', 'I cooked and rested.', 'Maiṁ khāṇā banāiā ate ārām kītā.', { img: 'Cooking' }),
            wp('Ho fatto la spesa al supermercato.', 'I did the shopping at the supermarket.', 'Maiṁ suparmārkiṭ toṁ saudā liā.', { img: 'Shopping cart' }),
            wp('Ho bevuto un caffè con un amico.', 'I had a coffee with a friend.', 'Maiṁ dost nāl kauffī pītī.', { img: 'Hot beverage' }),
            wp('Ho visto una partita.', 'I watched a match.', 'Maiṁ maich vekhiā.', { img: 'Soccer ball' }),
            wp('Ho scritto messaggi ai parenti.', 'I wrote messages to my relatives.', 'Maiṁ rishtedārāṁ nūṁ saneh likhe.', { img: 'Mobile phone' }),
          ],
        },
        {
          title: 'Domande e risposte: ieri e il fine settimana',
          table: [
            ['Domanda (tu)', 'Risposta modello (io)'],
            ['Che cosa hai fatto ieri?', 'Ieri ho lavorato fino alle sei.'],
            ['Com\'è andato il fine settimana?', 'Benissimo! Ho riposato tanto.'],
            ['Cos\'hai mangiato ieri sera?', 'Ho mangiato riso e verdure.'],
            ['Hai visto qualcuno sabato?', 'Sì, ho incontrato mio cugino.'],
            ['Hai fatto la spesa ieri?', 'No, non ho avuto tempo.'],
            ['A che ora hai finito di lavorare?', 'Ho finito alle cinque e mezza.'],
          ],
        },
        {
          title: 'Dire quello che NON hai fatto',
          table: [
            ['Affermativo (+)', 'Negativo (NON + avere)'],
            ['Sabato ho lavorato.', 'Domenica non ho lavorato.'],
            ['Ieri ho visto la TV.', 'Ieri non ho visto la TV.'],
            ['Stamattina ho bevuto il caffè.', 'Stamattina non ho bevuto il caffè.'],
          ],
        },
        { tip: '**Non** goes **before avere**: *non ho visto* — never ~~ho non visto~~.' },
      ],
      exercises: [
        fb('Ieri sera ___ la televisione perché ero stanco.', ['non ho visto', 'ho non visto', 'non visto ho', 'ho visto non']),
        fb('Domenica ___ lavorato.', ['non ho', 'ho non', 'no ho', 'non hai']),
        mc('"Com\'è andato il fine settimana?" — a good answer is…', 'Benissimo! Ho riposato tanto.', 'Vado al mare.', 'Mi chiamo Ravi.', 'Domani lavoro.'),
        mc('"Hai fatto la spesa ieri?" — "No, …"', 'non ho avuto tempo.', 'ho non avuto tempo.', 'non ho tempo domani.', 'non faccio.'),
        tr('Ho pulito casa.', 'I cleaned the house.', 'I left the house.', 'I bought a house.', 'I stayed at home.'),
      ],
      homework: 'Write 3 things you did and 2 things you didn\'t do last weekend.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Dialogue: Monday morning at the café',
      titleIt: 'Lezione 3 · Dialogo: lunedì mattina al bar',
      summary: 'Luca asks Manpreet about his weekend.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: lunedì mattina al bar',
          situation: 'Luca and Manpreet meet at the café on Monday morning.',
          dialogue: [
            lp('Luca', 'Ciao Manpreet! Com\'è andato il fine settimana?', 'Hi Manpreet! How did your weekend go?', 'Sat Srī Akāl Manpreet! Hafte dā ant kiven rihā?'),
            lp('Manpreet', 'Molto bene! Sabato prima ho pulito casa, poi ho fatto la spesa.', 'Very well! On Saturday first I cleaned the house, then I did the shopping.', 'Bahut vadhīā! Shanivār nūṁ pahilāṁ maiṁ ghar sāf kītā, phir saudā liā.'),
            lp('Luca', 'E domenica cos\'hai fatto? Hai lavorato?', 'And on Sunday what did you do? Did you work?', 'Ate aitvār nūṁ kī kītā? Kamm kītā?'),
            lp('Manpreet', 'No, non ho lavorato! Dopo pranzo ho visto gli amici e infine ho dormito.', 'No, I didn\'t work! After lunch I saw my friends and finally I slept.', 'Nahīṁ, kamm nahīṁ kītā! Dupahir de khāṇe toṁ bāad dostāṁ nūṁ miliā ate ākhir vich sauṁ giā.'),
          ],
        },
        {
          title: 'Racconta il tuo sabato o la tua domenica',
          text: '1. **Fai 3 domande:** chiedi cos\'ha fatto sabato e domenica.\n2. **Usa i 4 connettivi:** rispondi con *prima, poi, dopo, infine*.\n3. **Riferisci in classe:** racconta cosa ha fatto il tuo compagno!',
        },
      ],
      exercises: [
        tf('Sabato Manpreet ha pulito casa.', true, 'On Saturday Manpreet cleaned the house.'),
        tf('Domenica Manpreet ha lavorato.', false, 'On Sunday Manpreet worked.'),
        tf('Dopo pranzo ha visto gli amici.', true, 'After lunch he saw his friends.'),
        wo('Com\'è andato il fine settimana', 'How did the weekend go'),
        wo('Poi ho fatto la spesa', 'Then I did the shopping'),
      ],
      homework: 'Tell a partner about your weekend in order, then tell the class what your partner did.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Raccontare una giornata o un evento
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Review',
      title: 'Lesson 4 · Interview: your partner\'s last week',
      titleIt: 'Lezione 4 · Intervista: la settimana scorsa del compagno',
      summary: 'The Chapter 8 toolbox and how to report: io ho → lui / lei ha.',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 8 · Lezione 4 (PDF)' },
        {
          title: 'La cassetta degli attrezzi del Capitolo 8',
          table: [
            ['Domanda', 'Strumenti'],
            ['Quando?', 'ieri, l\'altro ieri, la settimana scorsa, due giorni fa'],
            ['In che ordine?', 'prima, poi, dopo, infine'],
            ['Quali verbi?', 'ho fatto, visto, preso, bevuto, letto, scritto'],
          ],
        },
        {
          title: 'Intervista: la settimana scorsa del tuo compagno',
          table: [
            ['Domanda', 'Aiuto per rispondere'],
            ['1. Quanti giorni hai lavorato la settimana scorsa?', 'Ho lavorato cinque giorni…'],
            ['2. Hai fatto qualcosa di speciale nel weekend?', 'Sì, ho visto… / No, ho riposato.'],
            ['3. Hai cucinato o hai mangiato fuori?', 'Ho cucinato… / Ho mangiato…'],
            ['4. Hai comprato qualcosa in negozio o al mercato?', 'Ho comprato… / Non ho comprato…'],
            ['5. Hai scritto o chiamato la tua famiglia?', 'Sì, ho chiamato… / Ho scritto…'],
            ['6. Hai letto un libro o visto un film?', 'Ho visto un film… / Ho letto…'],
          ],
        },
        {
          title: 'Racconta alla classe: da IO HO a LUI / LEI HA',
          table: [
            ['Il compagno dice', 'Tu racconti'],
            ['«La settimana scorsa ho lavorato cinque giorni.»', '«La settimana scorsa Sukhdeep ha lavorato cinque giorni.»'],
            ['«Sabato ho fatto la spesa e ho visto un film.»', '«Sabato Kiran ha fatto la spesa e ha visto un film.»'],
          ],
        },
        { tip: 'When you report, change **io ho → lui / lei ha** (Punjabi: *maiṁ → us ne*). The participle stays the same: *ho lavorato → ha lavorato*.' },
      ],
      exercises: [
        fb('Kiran dice: «Ho visto un film». Tu racconti: «Kiran ___ un film».', ['ha visto', 'ho visto', 'hai visto', 'hanno visto']),
        fb('Sukhdeep dice: «Ho lavorato cinque giorni». Tu: «Sukhdeep ___ cinque giorni».', ['ha lavorato', 'ho lavorato', 'hai lavorato', 'abbiamo lavorato']),
        fb('Tu chiedi: «___ cucinato o hai mangiato fuori?»', ['Hai', 'Ha', 'Ho', 'Hanno']),
        fb('I miei amici dicono: «Abbiamo bevuto un tè». Tu: «I miei amici ___ un tè».', ['hanno bevuto', 'abbiamo bevuto', 'ha bevuto', 'avete bevuto']),
        mc('"Hai fatto qualcosa di speciale nel weekend?" means…', 'Did you do anything special at the weekend?', 'Are you doing anything special this weekend?', 'Do you work at the weekend?', 'What is special about the weekend?'),
      ],
      homework: 'Interview a partner with the 6 questions and write a report of 5 sentences with lui / lei ha.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · Writing an informal message',
      titleIt: 'Lezione 4 · Scrivere un messaggio informale',
      summary: 'WhatsApp or email to a friend in 3 parts — with a model text about a birthday party.',
      minutes: 12,
      sections: [
        {
          title: 'Un messaggio informale in 3 parti',
          table: [
            ['Parte', 'Esempi'],
            ['1. Saluto iniziale', 'Ciao Sara! Come stai? · Caro Marco, tutto bene?'],
            ['2. Racconto dell\'evento', 'Ieri ho passato una bella giornata! Prima…, poi…, dopo…, infine…'],
            ['3. Domanda e saluto', 'E tu cos\'hai fatto nel weekend? A presto! Un abbraccio, Ravi.'],
          ],
        },
        {
          title: 'Testo modello: una festa di compleanno',
          reading: {
            title: 'Oggetto: La festa di ieri sera!',
            it: 'Ciao Giulia! Come stai?\n\nTi scrivo per raccontarti la festa di mio fratello. Prima io e mia madre abbiamo cucinato tanti piatti buoni. Poi gli ospiti hanno portato una torta al cioccolato e abbiamo bevuto il succo di frutta. Dopo abbiamo ascoltato musica e abbiamo fatto tante foto insieme. Infine mio fratello ha aperto tutti i regali!\n\nE tu cos\'hai fatto domenica? Scrivimi presto! Un abbraccio, Jaspreet.',
            en: 'Hi Giulia! How are you?\n\nI\'m writing to tell you about my brother\'s party. First my mother and I cooked lots of good dishes. Then the guests brought a chocolate cake and we drank fruit juice. After that we listened to music and took lots of photos together. Finally my brother opened all his presents!\n\nAnd what did you do on Sunday? Write to me soon! A hug, Jaspreet.',
          },
        },
        {
          title: 'Compito scritto: il tuo messaggio (50–70 parole)',
          text: '1. **Scegli l\'argomento:** la giornata di ieri, il weekend o una festa.\n2. **Usa le parole chiave:** almeno 5 verbi al passato + *prima, poi, dopo, infine*.\n3. **Scambia e rispondi:** il compagno legge e risponde con 2 frasi.',
        },
      ],
      exercises: [
        tf('Jaspreet scrive della festa di sua sorella.', false, 'Jaspreet writes about her sister\'s party.'),
        tf('Gli ospiti hanno portato una torta al cioccolato.', true, 'The guests brought a chocolate cake.'),
        tf('Hanno bevuto il vino.', false, 'They drank wine.'),
        tf('Il fratello ha aperto tutti i regali.', true, 'The brother opened all the presents.'),
        mc('How do you end an informal message?', 'A presto! Un abbraccio, Ravi.', 'Distinti saluti, Ravi.', 'Buongiorno, sono Ravi.', 'Vorrei fissare un appuntamento.'),
        wo('Ieri ho passato una bella giornata', 'Yesterday I had a lovely day'),
      ],
      homework: 'Write your own message (50–70 words) to a friend about yesterday, your weekend or a party. Use at least 5 past verbs.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · Chapter 8 practice',
      titleIt: 'Lezione 4 · Esercizi di ripasso del Capitolo 8',
      summary: 'Mixed practice: regular and irregular participles, time expressions and connectors.',
      minutes: 10,
      sections: [
        {
          title: 'Ripasso lampo',
          text: '- **Regolari:** -are → **-ato** · -ere → **-uto** · -ire → **-ito**\n- **Irregolari:** fatto, detto, letto, scritto, visto, preso, bevuto, aperto, chiuso, chiesto, risposto\n- **Quando:** ieri, l\'altro ieri, la settimana scorsa, il mese scorso, due giorni fa\n- **Ordine:** prima → poi → dopo → infine\n- **Negativo:** *non* + avere + participio',
        },
      ],
      exercises: [
        fb('Ieri ___ la pizza con gli amici.', ['ho mangiato', 'ho mangiata', 'mangiato ho', 'ho mangiuto']),
        fb('Abbiamo ___ un bel film al cinema.', ['visto', 'vedato', 'vedito', 'vista']),
        fb('Il negozio ha ___ alle sette.', ['chiuso', 'chiudito', 'chiuduto', 'chiusa']),
        fb('Tu hai ___ il libro?', ['letto', 'leggito', 'leggiuto', 'letta']),
        fb('Loro hanno ___ un messaggio.', ['scritto', 'scrivuto', 'scrito', 'scrivato']),
        fb('Il supermercato ha ___ alle nove.', ['aperto', 'aprito', 'apruto', 'aperta']),
        fb('Ho fatto la spesa ___ settimana scorsa.', ['la', 'il', 'lo', 'gli']),
        fb('Ho bevuto un caffè con Marco un mese ___.', ['fa', 'scorso', 'scorsa', 'ieri']),
        mc('"L\'altro ieri ho dormito fino a mezzogiorno" means…', 'The day before yesterday I slept until midday', 'Yesterday I slept until midnight', 'Last week I slept at midday', 'The other day I didn\'t sleep'),
      ],
      homework: 'Get ready for the test: say aloud what you did yesterday, in order, in 6 sentences.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 8
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 8 test',
      titleIt: 'Test Capitolo 8',
      summary: 'Listening, past time expressions and participles, the passato prossimo with avere. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 8',
          text: 'This test checks everything in Chapter 8 (Lessons 1–4).\n\n- **1. Listening** — sentences and a story about yesterday\n- **2. Vocabulary** — past time expressions, connectors and irregular participles\n- **3. Grammar** — the passato prossimo with avere (regular and irregular)\n- **4. Writing** — a message to a friend about your weekend\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (a message to a friend about last weekend) and the **oral interview** (what you did yesterday, irregular participles, your last weekend in order — 2 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A — Ascolta e scegli
        lis('1A · Listen and choose the sentence you hear', 'Ieri ho mangiato la pizza con gli amici in centro.', 'IERI HO MANGIATO LA PIZZA CON GLI AMICI', 'L\'ALTRO IERI HO DORMITO FINO A MEZZOGIORNO'),
        lis('1A · Listen and choose the sentence you hear', 'Stamattina ho scritto un messaggio al direttore per l\'orario.', 'HO SCRITTO UN MESSAGGIO AL DIRETTORE', 'HO LETTO UN LIBRO INTERESSANTE'),
        lis('1A · Listen and choose the sentence you hear', 'Ieri sera abbiamo visto un bel film al cinema.', 'ABBIAMO VISTO UN BEL FILM AL CINEMA', 'ABBIAMO PRESO L\'AUTOBUS DELLE OTTO'),
        lis('1A · Listen and choose the sentence you hear', 'L\'altro ieri ho chiesto un giorno di ferie al mio capo.', 'HO CHIESTO UN GIORNO DI FERIE', 'IL NEGOZIO HA CHIUSO ALLE SETTE'),
        lis('1A · Listen and choose the sentence you hear', 'La settimana scorsa ho fatto la spesa al mercato.', 'LA SETTIMANA SCORSA HO FATTO LA SPESA', 'UN MESE FA HO BEVUTO UN CAFFÈ CON MARCO'),
        // 1B — Il racconto di ieri
        lis('1B · Listen to the story: "___ mattina ho lavorato fino all\'una." (1)', 'Ieri mattina ho lavorato fino all\'una.', 'IERI', 'DOMANI', 'OGGI', 'SEMPRE'),
        lis('1B · Listen again: "___ ho preparato il pranzo" (2)', 'Poi ho preparato il pranzo.', 'POI', 'PRIMA', 'FA', 'MAI'),
        lis('1B · Listen again: what did the person drink? (3)', 'Ho bevuto un tè caldo.', 'BEVUTO', 'PRESO', 'FATTO', 'VISTO'),
        lis('1B · Listen again: what did the person do with the emails? (4)', 'Nel pomeriggio ho letto le email del lavoro.', 'LETTO', 'SCRITTO', 'CHIUSO', 'APERTO'),
        lis('1B · Listen again: "___ ho chiamato la mia famiglia" (5)', 'Infine ho chiamato la mia famiglia.', 'INFINE', 'PRIMA', 'IERI', 'SCORSO'),
        // 2A — Collega
        pairs('2A · Match each expression with its meaning', [
          ['L\'altro ieri', 'The day before yesterday · Parsoṁ'],
          ['La settimana scorsa', 'Last week · Pichhle hafte'],
          ['Un mese fa', 'A month ago · Ikk mahīnā pahilāṁ'],
          ['Infine', 'Finally · Ākhir vich'],
          ['Chiesto / Risposto', 'Asked / Answered · Puchhiā / Javāb dittā'],
        ]),
        // 2B — Participi irregolari
        mc('2B · fare → ?', 'fatto', 'faciuto', 'farato', 'fato'),
        mc('2B · dire → ?', 'detto', 'dito', 'diciuto', 'direto'),
        mc('2B · leggere → ?', 'letto', 'leggiuto', 'leggito', 'leto'),
        mc('2B · vedere → ?', 'visto', 'vedito', 'vidato', 'vesto'),
        mc('2B · aprire → ?', 'aperto', 'aprito', 'apruto', 'aprato'),
        // 3A — Regolari
        fb('3A · Ieri sera noi ___ (mangiare) pasta al pomodoro e insalata.', ['abbiamo mangiato', 'avete mangiato', 'hanno mangiato', 'abbiamo mangiare']),
        fb('3A · L\'anno scorso Marco ___ (vendere) la sua vecchia macchina.', ['ha venduto', 'ha vendito', 'ho venduto', 'ha vendato']),
        fb('3A · Stanotte tu ___ (dormire) bene o sei stanco?', ['hai dormito', 'hai dormuto', 'ha dormito', 'hai dormato']),
        // 3B — Irregolari
        fb('3B · Stamattina io ___ (bere) un cappuccino al bar.', ['ho bevuto', 'ho beuto', 'ho bevato', 'ho bevito']),
        fb('3B · Sabato scorso i miei amici ___ (prendere) il treno per Verona.', ['hanno preso', 'hanno prenduto', 'ha preso', 'hanno prendito']),
        fb('3B · Tu ___ (scrivere) l\'email all\'ufficio ieri?', ['hai scritto', 'hai scrivuto', 'hai scrivito', 'ha scritto']),
        fb('3B · Il supermercato ___ (chiudere) alle otto di sera.', ['ha chiuso', 'ha chiudito', 'ha chiuduto', 'hanno chiuso']),
        fb('3B · Noi ___ (chiedere) un\'informazione all\'insegnante…', ['abbiamo chiesto', 'abbiamo chieduto', 'abbiamo chiedito', 'avete chiesto']),
        fb('3B · … e lei ___ (rispondere) subito.', ['ha risposto', 'ha risponduto', 'ha rispondito', 'ho risposto']),
        // 3C — Racconto al passato
        fb('3C · ___ ho fatto colazione, poi ho lavato i piatti.', ['Prima', 'Infine']),
        fb('3C · Ieri sera ___ la televisione perché ero stanco.', ['non ho visto', 'ho non visto']),
        fb('3C · Sono arrivato in Italia sei mesi ___.', ['fa', 'scorso']),
        // 4 — Messaggio a un amico
        wo('Sabato mattina prima ho pulito casa', 'On Saturday morning first I cleaned the house'),
        wo('Domenica a pranzo ho mangiato la pizza', 'On Sunday I had pizza for lunch'),
        wo('E tu che cosa hai fatto nel fine settimana', 'And what did you do at the weekend'),
      ],
      homework: 'Written part for your teacher: complete the 5 lines of the message to a friend about last weekend. Then prepare the 2-minute oral interview.',
    },
  ],
};
