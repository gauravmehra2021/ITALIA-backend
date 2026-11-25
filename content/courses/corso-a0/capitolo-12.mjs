import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 12 of the school syllabus (Corso di Italiano, livello A2) — the last chapter.
// Source: syllabus/Capitolo 12 Lezione 1–4 (.pptx) and Test Finale Livello A2 (.docx).
// Note: Test finale 3A.3 ("io e mia sorella … siamo andate") only works for a woman speaker,
// so the app says who is speaking (Sofia).

const slides = (lez) => `/content/slides/corso-a0-cap12-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-12',
  title: 'Chapter 12: The future, indirect pronouns, real life — A2 review',
  titleIt: 'Capitolo 12: Il futuro, i pronomi indiretti, la vita in Italia — ripasso A2',
  cover: 'Graduation cap',
  description: 'The futuro semplice for plans, forecasts and promises (parlerò, sarò, avrò, andrò, farò), the indirect pronouns (mi, ti, gli, le, ci, vi) and the connectors perché, quindi, però, anche se, three real-life situations (post office and Comune, a job interview, phoning the plumber), then the complete A1–A2 review and the final A2 test.',
  canDo: [
    'I can talk about my plans and make forecasts with the futuro semplice',
    'I can use the irregular futures: sarò, avrò, andrò, farò, verrò, potrò, dovrò',
    'I can use the indirect pronouns: gli telefono, le regalo un fiore, ti consiglio…',
    'I can link sentences with perché, quindi, però, anche se',
    'I can manage the post office, the Comune, a simple job interview and a call about a repair at home',
    'I can use the 4 main tenses together — level A2',
  ],
  liveClassFocus: 'Role-play two real-life scenarios (post office / Comune, job interview, plumber), then the 3 parts of the final A2 oral exam: talk about yourself with all 4 tenses, a role-play, and a then-and-now comparison.',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — Il futuro semplice
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 1 · The futuro semplice',
      titleIt: 'Lezione 1 · Il futuro semplice: forme regolari e irregolari',
      summary: 'Parlerò, prenderò, partirò — and sarò, avrò, andrò, farò, verrò, potrò, dovrò.',
      minutes: 12,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 12 · Lezione 1 (PDF)' },
        {
          title: 'Verbi regolari: -ARE / -ERE → -ERÒ · -IRE → -IRÒ',
          table: [
            ['Persona', 'PARLARE', 'PRENDERE', 'PARTIRE', 'English', 'Punjabi'],
            ['io', 'parlerò', 'prenderò', 'partirò', 'I will speak', 'main bolanga'],
            ['tu', 'parlerai', 'prenderai', 'partirai', 'you will speak', 'tu bolega'],
            ['lui / lei', 'parlerà', 'prenderà', 'partirà', 'he / she will speak', 'uh bolega / bolegi'],
            ['noi', 'parleremo', 'prenderemo', 'partiremo', 'we will speak', 'asi bolange'],
            ['voi', 'parlerete', 'prenderete', 'partirete', 'you (pl.) will speak', 'tusi bologe'],
            ['loro', 'parleranno', 'prenderanno', 'partiranno', 'they will speak', 'uh bolange'],
          ],
        },
        {
          title: 'I 7 verbi irregolari più usati',
          table: [
            ['Infinito', 'io', 'tu', 'lui / lei', 'noi / voi / loro'],
            ['ESSERE', 'sarò', 'sarai', 'sarà', 'saremo / sarete / saranno'],
            ['AVERE', 'avrò', 'avrai', 'avrà', 'avremo / avrete / avranno'],
            ['ANDARE', 'andrò', 'andrai', 'andrà', 'andremo / andrete / andranno'],
            ['FARE', 'farò', 'farai', 'farà', 'faremo / farete / faranno'],
            ['VENIRE', 'verrò', 'verrai', 'verrà', 'verremo / verrete / verranno'],
            ['POTERE', 'potrò', 'potrai', 'potrà', 'potremo / potrete / potranno'],
            ['DOVERE', 'dovrò', 'dovrai', 'dovrà', 'dovremo / dovrete / dovranno'],
          ],
        },
        { tip: 'Verbs in **-care / -gare** keep the hard sound with an **h**: cer**c**are → cer**ch**erò, gio**c**are → gio**ch**erò, pa**g**are → pa**gh**erò.' },
      ],
      exercises: [
        fb('L\'anno prossimo io ___ una casa più grande.', ['cercherò', 'cercerò', 'cercarò', 'cerco ieri']),
        fb('L\'anno prossimo io ___ un corso.', ['farò', 'farerò', 'facerò', 'ho fatto']),
        fb('Domani noi ___ dal medico.', ['andremo', 'anderemo', 'andaremo', 'siamo andati']),
        fb('Domani noi ___ chiedere la ricetta.', ['potremo', 'poteremo', 'possiamo ieri', 'potevamo']),
        fb('Stasera ___ io la cena al ristorante!', ['pagherò', 'pagerò', 'pagarò', 'pagavo']),
        fb('Tra due anni loro ___ un lavoro nuovo.', ['avranno', 'averanno', 'avrano', 'hanno avuto']),
        fb('Tu ___ a Roma in primavera?', ['verrai', 'venirai', 'venerai', 'vieni ieri']),
        fb('Domani ___ bel tempo.', ['sarà', 'essera', 'era', 'è stato']),
      ],
      homework: 'Write the future of 5 regular verbs and the 7 irregular ones for io and noi.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · Plans, forecasts and promises',
      titleIt: 'Lezione 1 · Progetti, previsioni e promesse',
      summary: 'Domani, tra poco, l\'anno prossimo — work, study and holidays.',
      minutes: 10,
      sections: [
        {
          title: '3 usi principali del futuro',
          table: [
            ['Uso', 'Esempio'],
            ['1. Progetti di lavoro e studio', 'A settembre farò un corso nuovo.'],
            ['2. Previsioni (meteo e futuro)', 'Domani pioverà e farà freddo.'],
            ['3. Promesse e decisioni', 'Ti chiamerò stasera, te lo prometto!'],
          ],
        },
        {
          title: 'Parole segnale',
          words: [
            wp('domani', 'tomorrow', 'Kall (aun vaala)', { img: 'Calendar' }),
            wp('tra poco', 'soon / in a moment', 'Thodi der vich', { img: 'Hourglass not done' }),
            wp('tra due anni', 'in two years', 'Do saal baad', { img: 'Spiral calendar' }),
            wp('la settimana prossima', 'next week', 'Agale hafte', { img: 'Tear-off calendar' }),
            wp('l\'anno prossimo', 'next year', 'Agale saal', { img: 'Calendar' }),
          ],
        },
        {
          title: 'Cosa farai quest\'anno?',
          practice: false,
          words: [
            wp('Il mese prossimo avrò un colloquio e troverò un lavoro migliore.', 'Next month I\'ll have an interview and find a better job.', 'Agale mahine meri intarviu hovegi ate main vadhia kamm labhanga.'),
            wp('L\'anno prossimo prenderò la patente e studierò italiano B1.', 'Next year I\'ll get my driving licence and study Italian B1.', 'Agale saal main draiving laisains lavanga ate B1 Italian padhanga.'),
            wp('Ad agosto andremo in India e vedremo tutta la famiglia!', 'In August we\'ll go to India and see the whole family!', 'Agasat vich asi Bhaarat jaavange ate saare parivar nu milange!'),
          ],
        },
      ],
      exercises: [
        mc('"Domani pioverà" is…', 'a forecast', 'a promise', 'a past habit', 'an order'),
        mc('"Ti chiamerò stasera, te lo prometto!" is…', 'a promise', 'a forecast', 'a habit', 'a description'),
        fb('A settembre ___ un corso nuovo.', ['farò', 'facevo', 'ho fatto', 'faccio ieri']),
        fb('Domani ___ freddo.', ['farà', 'faceva', 'ha fatto', 'fa ieri']),
        fb('Ad agosto ___ in India.', ['andremo', 'andavamo', 'siamo andati', 'andiamo ieri']),
        tr('l\'anno prossimo', 'next year', 'last year', 'this year', 'every year'),
      ],
      homework: 'Write 3 plans for this year: one for work, one for study, one for holidays.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Gurpreet\'s and Navdeep\'s plans',
      titleIt: 'Lezione 1 · I progetti di Gurpreet e Navdeep',
      summary: 'Cambierò casa e lavoro · Farò un corso e viaggerò — then ask your partner.',
      minutes: 10,
      sections: [
        {
          title: 'Due piani a confronto',
          table: [
            ['Gurpreet · lavoro e casa', 'Navdeep · studio e viaggi'],
            ['«Cambierò casa e lavoro»', '«Farò un corso e viaggerò»'],
            ['Avrò un contratto fisso, comprerò un\'auto usata e affitterò un appartamento più grande.', 'Finirò il corso di cucina, verrò a Roma in primavera e visiterò il Colosseo.'],
          ],
        },
        {
          title: 'Dialogo: cosa farai questo fine settimana?',
          situation: 'Two colleagues talk on Friday afternoon.',
          dialogue: [
            lp('Luca', 'Cosa farai questo fine settimana?', 'What will you do this weekend?', 'Is hafte de ant te tusi ki karoge?'),
            lp('Navdeep', 'Sabato andrò al mercato e pulirò casa.', 'On Saturday I\'ll go to the market and clean the house.', 'Shanivar nu main baazar jaavanga ate ghar saaf karanga.'),
            lp('Luca', 'E dove andrai in vacanza quest\'estate?', 'And where will you go on holiday this summer?', 'Ate is garmiaan chhuttiaan vich kitthe jaoge?'),
            lp('Navdeep', 'Ad agosto partirò per Roma e visiterò il Colosseo!', 'In August I\'ll leave for Rome and visit the Colosseum!', 'Agasat vich main Roma jaavanga ate Kolosiam vekhanga!'),
          ],
        },
      ],
      exercises: [
        tf('Gurpreet comprerà un\'auto nuova.', false, 'Gurpreet will buy a new car.'),
        tf('Gurpreet affitterà un appartamento più grande.', true, 'Gurpreet will rent a bigger flat.'),
        tf('Navdeep finirà il corso di cucina.', true, 'Navdeep will finish the cooking course.'),
        tf('Navdeep andrà a Roma in inverno.', false, 'Navdeep will go to Rome in winter.'),
        wo('Sabato andrò al mercato e pulirò casa', 'On Saturday I\'ll go to the market and clean the house'),
        wo('Tra due anni avrò un lavoro nuovo', 'In two years I\'ll have a new job'),
      ],
      homework: 'Ask a partner: Cosa farai questo fine settimana? Dove andrai in vacanza? Che lavoro farai tra due anni?',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — Pronomi indiretti e connettivi
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Indirect pronouns: mi, ti, gli, le, ci, vi',
      titleIt: 'Lezione 2 · I pronomi indiretti (a chi?)',
      summary: 'Gli telefono, le regalo un fiore, ti consiglio di riposare…',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 12 · Lezione 2 (PDF)' },
        {
          title: 'Rispondono alla domanda «A chi?»',
          table: [
            ['A chi?', 'Pronome', 'Esempio', 'English · Punjabi'],
            ['a me', 'mi', 'Marco mi telefona ogni sera.', 'to me · mainu'],
            ['a te', 'ti', 'Domani ti scrivo un messaggio.', 'to you · tainu'],
            ['a lui', 'gli', 'Gli regalo un libro per il compleanno.', 'to him · usnu'],
            ['a lei / a Lei', 'le / Le', 'Le chiedo un consiglio sul lavoro.', 'to her / to you (polite) · usnu / tuhanu (aap nu)'],
            ['a noi', 'ci', 'Il maestro ci spiega la regola.', 'to us · saanu'],
            ['a voi', 'vi', 'Ragazzi, vi consiglio questo film!', 'to you all · tuhanu'],
            ['a loro', 'gli', 'I nonni aspettano: gli porto la spesa.', 'to them · uhnaan nu'],
          ],
        },
        {
          title: 'Verbi con «a»',
          practice: false,
          words: [
            wp('telefonare a — Ora gli telefono.', 'to phone — I\'ll phone him now.', 'Hun main usnu fon karda haan.'),
            wp('scrivere a — Sara ci scrive ogni giorno.', 'to write to — Sara writes to us every day.', 'Sara saanu har roz likhdi hai.'),
            wp('chiedere a — Ti chiedo aiuto.', 'to ask — I\'m asking you for help.', 'Main tainu madad lai puchhda haan.'),
            wp('regalare a — Le regaliamo dei fiori.', 'to give (a present) — We give her flowers.', 'Asi usnu phul dinde haan.'),
            wp('consigliare a — Ti consiglio di riposare.', 'to advise — I advise you to rest.', 'Main tainu aaram karan di salah dinda haan.'),
          ],
        },
        {
          title: 'Due regole pratiche',
          table: [
            ['Regola', 'Esempi'],
            ['GLI = a lui / a loro · LE = a lei', 'Parlo con Paolo → Gli parlo. Parlo con Anna → Le parlo. Parlo con i ragazzi → Gli parlo.'],
            ['before the verb or joined to the infinitive', 'Ti voglio telefonare = Voglio telefonarti. Devo scrivergli un\'email.'],
          ],
        },
      ],
      exercises: [
        fb('Mio fratello è a casa, ora ___ telefono.', ['gli', 'le', 'lo', 'mi']),
        fb('È la festa della mamma: ___ regaliamo dei fiori.', ['le', 'gli', 'la', 'li']),
        fb('Hai mal di testa? ___ consiglio di riposare.', ['Ti', 'Gli', 'Le', 'Vi']),
        fb('Domani è il compleanno di Maria: ___ regalerò un libro.', ['le', 'la', 'gli', 'lo']),
        fb('Il maestro ___ spiega la regola (a noi).', ['ci', 'vi', 'gli', 'mi']),
        fb('Ragazzi, ___ consiglio questo film!', ['vi', 'ci', 'gli', 'ti']),
        fb('I nonni aspettano: ___ porto la spesa.', ['gli', 'le', 'li', 'lo']),
        fb('Devo ___ un\'email (a lui).', ['scrivergli', 'gli scrivere', 'scriverlo', 'scriverle']),
      ],
      homework: 'Write 5 sentences: who you phone, write to, ask, give presents to and advise (Gli telefono…, le scrivo…).',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 2 · Connectors: perché, quindi, però, anche se',
      titleIt: 'Lezione 2 · I connettivi logici',
      summary: 'Cause, consequence, contrast and concession — and perché vs quindi.',
      minutes: 10,
      sections: [
        {
          title: '4 connettivi fondamentali',
          words: [
            wp('perché', 'because', 'Kiunki', { img: 'Light bulb', ex: 'Studio perché voglio un buon lavoro.', exEn: 'I study because I want a good job.' }),
            wp('quindi', 'so / therefore', 'Is lai', { img: 'Right arrow', ex: 'Piove, quindi prendo l\'ombrello.', exEn: 'It\'s raining, so I take my umbrella.' }),
            wp('però', 'but / however', 'Par', { img: 'Left-right arrow', ex: 'Sono stanco, però vengo alla festa.', exEn: 'I\'m tired, but I\'m coming to the party.' }),
            wp('anche se', 'even if / although', 'Bhaaven ki', { img: 'Umbrella', ex: 'Esco anche se fa molto freddo.', exEn: 'I go out even though it\'s very cold.' }),
          ],
        },
        {
          title: 'Perché (motivo) vs quindi (risultato)',
          table: [
            ['Risultato → PERCHÉ → motivo', 'Motivo → QUINDI → risultato'],
            ['Prendo il treno perché è veloce.', 'Il treno è veloce, quindi lo prendo.'],
          ],
        },
      ],
      exercises: [
        fb('Pioveva forte, ___ ho preso il bus.', ['quindi', 'perché', 'anche se', 'però']),
        fb('Studio ___ voglio un buon lavoro.', ['perché', 'quindi', 'anche se', 'però']),
        fb('Esco ___ fa molto freddo.', ['anche se', 'quindi', 'perché', 'come']),
        fb('Sono stanco, ___ vengo alla festa.', ['però', 'perché', 'quindi', 'come']),
        fb('Prendo il treno ___ è veloce.', ['perché', 'quindi', 'anche se', 'però']),
        fb('Il treno è veloce, ___ lo prendo.', ['quindi', 'perché', 'anche se', 'però']),
        tr('anche se', 'even though', 'because', 'so', 'when'),
      ],
      homework: 'Write 4 sentences about your life in Italy, one with each connector.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · A phone call between friends',
      titleIt: 'Lezione 2 · Un consiglio al telefono tra amici',
      summary: 'Kiran and Priya choose a present for Luca — pronouns and connectors together.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: che cosa gli regaliamo?',
          situation: 'Kiran phones Priya about Luca\'s birthday.',
          dialogue: [
            lp('Kiran', 'Ciao Priya! Ti telefono perché domani è il compleanno di Luca. Che cosa gli regaliamo?', 'Hi Priya! I\'m calling you because tomorrow is Luca\'s birthday. What shall we give him?', 'Sat Sri Akal Priya! Main tainu fon kita kiunki kall Luca da janamdin hai. Asi usnu ki tohfa deie?'),
            lp('Priya', 'Lui ama leggere, quindi ti consiglio un bel romanzo italiano!', 'He loves reading, so I suggest a good Italian novel!', 'Usnu padhna pasand hai, is lai main ikk vadhia Italvi naaval di salah dindi haan!'),
            lp('Kiran', 'Ottima idea! Oggi pomeriggio passo in libreria anche se piove, però poi mi aiuti a scrivere il biglietto!', 'Great idea! This afternoon I\'ll go to the bookshop even if it\'s raining, but then you help me write the card!', 'Bahut vadhia! Ajj dupahir main kitabaan di dukan jaavangi bhaaven minh pai riha hai, par phir tu kaard likhan vich meri madad kari!'),
          ],
        },
        {
          title: 'Gioco di ruolo: chiedi e dai consigli',
          text: '- **Studente A chiede un consiglio:** «Ti chiedo un consiglio perché devo comprare un regalo…»\n- **Studente B consiglia e motiva:** «Ti consiglio un orologio perché è utile, però costa un po\'.»',
        },
      ],
      exercises: [
        tf('Domani è il compleanno di Luca.', true, 'Tomorrow is Luca\'s birthday.'),
        tf('Luca ama cucinare.', false, 'Luca loves cooking.'),
        tf('Priya consiglia un romanzo.', true, 'Priya suggests a novel.'),
        tf('Kiran va in libreria solo se non piove.', false, 'Kiran goes to the bookshop only if it doesn\'t rain.'),
        wo('Che cosa gli regaliamo', 'What shall we give him'),
        wo('Ti consiglio un bel romanzo', 'I suggest a good novel'),
      ],
      homework: 'Role-play with a partner: ask for advice about a present and answer with ti consiglio… perché / però…',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — Situazioni pratiche della vita in Italia
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · At the post office and the Comune',
      titleIt: 'Lezione 3 · All\'ufficio postale e in Comune',
      summary: 'Una raccomandata, una bolletta, il cambio di residenza — with Lei.',
      minutes: 12,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 12 · Lezione 3 (PDF)' },
        {
          title: 'Parole chiave',
          words: [
            wp('la raccomandata', 'registered letter', 'Rajistard chitthi', { pron: 'lah rahk-koh-mahn-DAH-tah', img: 'Envelope' }),
            wp('la bolletta / il bollettino', 'bill / payment slip', 'Bill / Bill di parchi', { pron: 'lah bol-LET-tah', img: 'Light bulb' }),
            wp('il codice fiscale', 'tax code', 'Taiks kod', { pron: 'eel KOH-dee-cheh fees-KAH-leh', img: 'Identification card' }),
            wp('il cambio di residenza', 'change of address (registration)', 'Rihaish di tabdili', { pron: 'reh-zee-DEN-tsah', img: 'House' }),
            wp('il modulo', 'form', 'Faaram', { pron: 'eel MOH-doo-loh', img: 'Clipboard' }),
            wp('la marca da bollo', 'revenue stamp', 'Sarkari tikat', { pron: 'lah MAR-kah dah BOL-loh', img: 'Postbox' }),
            wp('la ricevuta', 'receipt', 'Rasid', { pron: 'lah ree-cheh-VOO-tah', img: 'Receipt' }),
            wp('lo sportello', 'counter / window', 'Khidki', { pron: 'loh spor-TEL-loh', img: 'Office building' }),
          ],
        },
        {
          title: 'Scenario 1: allo sportello',
          situation: 'At the post office and then at the Comune.',
          dialogue: [
            lp('Utente', 'Buongiorno, vorrei spedire questa raccomandata e pagare una bolletta.', 'Good morning, I\'d like to send this registered letter and pay a bill.', 'Sat Sri Akal, main eh rajistard chitthi bhejni ate ikk bill bharna chaahunda haan.'),
            lp('Impiegato', 'Certamente. Mi dà il Suo documento d\'identità e il codice fiscale?', 'Certainly. May I have your ID and your tax code?', 'Zarur. Aapna shanakhti kaard ate taiks kod de sakde ho?'),
            lp('Utente', 'Ecco. In Comune devo fare il cambio di residenza: quali moduli devo compilare?', 'Here you are. At the Comune I have to change my address: which forms do I have to fill in?', 'Eh lao. Kamiun vich mainu rihaish badlni hai: kihade faaram bharne painge?'),
          ],
        },
      ],
      exercises: [
        mc('A letter with proof of delivery is…', 'una raccomandata', 'una bolletta', 'un modulo', 'una ricevuta'),
        mc('You moved house. At the Comune you do…', 'il cambio di residenza', 'la raccomandata', 'la marca da bollo', 'il codice fiscale'),
        mc('"Compilare il modulo" means…', 'fill in the form', 'sign the receipt', 'pay the bill', 'send the letter'),
        tf('L\'utente vuole pagare una bolletta.', true, 'The customer wants to pay a bill.'),
        tf('L\'impiegato chiede il passaporto e la patente.', false, 'The clerk asks for the passport and driving licence.'),
        wo('Vorrei spedire questa raccomandata', 'I would like to send this registered letter'),
      ],
      homework: 'Write what you need to say at the post office to send a parcel and pay a bill.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · A simple job interview',
      titleIt: 'Lezione 3 · Un colloquio di lavoro semplice',
      summary: 'Past experience (lavoravo, ho lavorato), driving licence, shifts and «sarò disponibile».',
      minutes: 12,
      sections: [
        {
          title: 'Scenario 2: il colloquio',
          situation: 'A candidate at a job interview.',
          dialogue: [
            lp('Datore di lavoro', 'Buongiorno! Mi parli delle Sue esperienze precedenti.', 'Good morning! Tell me about your previous experience.', 'Sat Sri Akal! Mainu aapne pichhle tajurbe baare dasso.'),
            lp('Candidato', 'Nel mio Paese lavoravo in un magazzino. Qui in Italia ho lavorato sei mesi come operaio.', 'In my country I worked in a warehouse. Here in Italy I worked for six months as a factory worker.', 'Aapne desh vich main godam vich kamm karda si. Ithe Itali vich main chhe mahine mazdur vajon kamm kita.'),
            lp('Datore di lavoro', 'Ha la patente B? È disponibile a fare i turni?', 'Do you have a B licence? Are you available for shifts?', 'Ki tuhade kol B laisains hai? Ki tusi shiftaan vich kamm kar sakde ho?'),
            lp('Candidato', 'Sì, ho la patente, quindi posso venire in auto, e sarò disponibile subito!', 'Yes, I have a licence, so I can come by car, and I\'ll be available straight away!', 'Haan, mere kol laisains hai, is lai main kaar te aa sakda haan, ate main turant tiar hovanga!'),
          ],
        },
        {
          title: 'Frasi utili per il colloquio',
          practice: false,
          words: [
            wp('Ho esperienza come magazziniere / saldatore / cuoco.', 'I have experience as a warehouse worker / welder / cook.', 'Mainu godam vaale / veldar / rasoie da tajurba hai.'),
            wp('Che contratto e che orario di lavoro offrite?', 'What contract and working hours do you offer?', 'Tusi kihada ikrarnama ate kamm da samaan dinde ho?'),
            wp('Sono disponibile a fare i turni.', 'I\'m available for shifts.', 'Main shiftaan vich kamm karan lai tiar haan.'),
          ],
        },
        { tip: 'In an interview use all your tenses: **imperfetto** for your old job (*lavoravo*), **passato prossimo** for a finished experience (*ho lavorato sei mesi*), **futuro** for what you will do (*sarò disponibile*).' },
      ],
      exercises: [
        tf('Nel suo Paese il candidato lavorava in un magazzino.', true, 'In his country the candidate worked in a warehouse.'),
        tf('In Italia ha lavorato due anni come cuoco.', false, 'In Italy he worked two years as a cook.'),
        tf('Il candidato ha la patente.', true, 'The candidate has a driving licence.'),
        fb('Nel mio Paese ___ in un magazzino.', ['lavoravo', 'lavorerò', 'lavoro domani', 'ho lavorato domani']),
        fb('Qui in Italia ___ sei mesi come operaio.', ['ho lavorato', 'lavoravo sempre', 'lavorerò ieri', 'lavoro ieri']),
        fb('___ disponibile subito!', ['Sarò', 'Ero', 'Sono stato', 'Essere']),
        wo('Ho esperienza come magazziniere', 'I have experience as a warehouse worker'),
      ],
      homework: 'Prepare your answers for an interview: your past job, your experience in Italy, your availability.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · A problem at home: calling the plumber',
      titleIt: 'Lezione 3 · Un problema in casa: telefonare all\'idraulico',
      summary: 'Il rubinetto perde, la caldaia non funziona — quando potrà venire?',
      minutes: 10,
      sections: [
        {
          title: 'La casa e i guasti',
          words: [
            wp('l\'idraulico', 'plumber', 'Palambar', { pron: 'lee-DROW-lee-koh', img: 'Wrench' }),
            wp('il proprietario / l\'inquilino', 'landlord / tenant', 'Maalak makan / Kiraedar', { pron: 'proh-pryeh-TAH-ryoh / leen-kwee-LEE-noh', img: 'Key' }),
            wp('Il rubinetto perde acqua.', 'The tap is leaking.', 'Tuti chon paani chonda hai.', { img: 'Droplet' }),
            wp('La caldaia non funziona.', 'The boiler isn\'t working.', 'Boilar kamm nahi kar riha.', { img: 'Fire' }),
            wp('Il lavandino è otturato.', 'The sink is blocked.', 'Sink band hai.', { img: 'Potable water' }),
            wp('Il riscaldamento è rotto.', 'The heating is broken.', 'Hitar kharab hai.', { img: 'Cold face' }),
          ],
        },
        {
          title: 'Scenario 3: la telefonata',
          text: '**Spiegare il guasto**\n- «Pronto, buongiorno! Sono l\'inquilino di via Roma 12.»\n- «Il rubinetto della cucina perde acqua.»\n- «La caldaia non funziona e non c\'è acqua calda.»\n\n**Chiedere intervento e prezzo**\n- «Quando potrà venire a controllare?»\n- «Domani mattina sarò a casa fino alle dodici.»\n- «Quanto costerà la riparazione, più o meno?»',
        },
        {
          title: 'Role-play: scegliete 2 scenari',
          text: '- **Scenario A · Poste / Comune:** spedire un pacco o il cambio di residenza — «Buongiorno, vorrei… Mi può dare…? Quanto costa?»\n- **Scenario B · Colloquio:** candidato e datore di lavoro — imperfetto (*lavoravo…*), passato prossimo e futuro (*sarò…*).\n- **Scenario C · Guasto in casa:** inquilino e idraulico — spiega il problema con *perché / quindi* e fissa l\'orario.',
        },
      ],
      exercises: [
        mc('No hot water: probably…', 'la caldaia non funziona', 'il lavandino è otturato', 'la raccomandata è persa', 'il modulo è sbagliato'),
        mc('Who repairs a leaking tap?', 'l\'idraulico', 'il farmacista', 'l\'impiegato', 'il commesso'),
        mc('You rent the flat: you are…', 'l\'inquilino', 'il proprietario', 'l\'idraulico', 'il datore di lavoro'),
        fb('Quando ___ venire a controllare?', ['potrà', 'poteva', 'ha potuto', 'puoi ieri']),
        fb('Domani mattina ___ a casa fino alle dodici.', ['sarò', 'ero', 'sono stato', 'essere']),
        fb('Quanto ___ la riparazione?', ['costerà', 'costava', 'è costato ieri', 'costare']),
      ],
      homework: 'Write the phone call to your landlord about a problem in your home (6 sentences).',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Ripasso generale intensivo A1–A2
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Review',
      title: 'Lesson 4 · A2 review: the 4 tenses',
      titleIt: 'Lezione 4 · Ripasso A2: la mappa dei tempi verbali',
      summary: 'Imperfetto, passato prossimo, presente, futuro — when to use each one.',
      minutes: 15,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 12 · Lezione 4 (PDF)' },
        {
          title: 'I 4 tempi fondamentali',
          table: [
            ['Tempo', 'Quando si usa', 'Esempio', 'English', 'Punjabi'],
            ['IMPERFETTO', 'past habit / background', 'Da piccolo andavo a scuola a piedi.', 'I used to go', 'jaanda si'],
            ['PASSATO PROSSIMO', 'finished action / event', 'Ieri sono andato al mercato.', 'I went', 'gia si'],
            ['PRESENTE', 'today / current habits', 'Oggi lavoro in un\'azienda.', 'I work', 'kamm karda haan'],
            ['FUTURO SEMPLICE', 'plans / forecasts', 'L\'anno prossimo farò un corso B1.', 'I will do', 'karanga'],
          ],
        },
        {
          title: '4 aree lessicali chiave del livello A2',
          table: [
            ['Area', 'Parole'],
            ['Casa e città', 'affitto, bolletta, residenza, quartiere'],
            ['Lavoro e studio', 'contratto, turni, colloquio, patente'],
            ['Salute e spesa', 'ricetta, farmacia, scontrino, reparto'],
            ['Viaggi e tempo', 'binario, ritardo, prenotazione, clima'],
          ],
        },
      ],
      exercises: [
        fb('Da bambina Sofia ___ timida.', ['era', 'è stata', 'sarà', 'è']),
        fb('Da bambina Sofia ___ molti libri.', ['leggeva', 'ha letto ieri', 'leggerà', 'legge']),
        fb('Ieri sera mentre noi ___, ha suonato il campanello.', ['cenavamo', 'abbiamo cenato', 'ceneremo', 'ceniamo']),
        fb('Ieri sera mentre noi cenavamo, ___ il campanello.', ['ha suonato', 'suonava', 'suonerà', 'suona']),
        fb('Oggi ___ in un\'azienda.', ['lavoro', 'lavoravo', 'lavorerò ieri', 'ho lavorato domani']),
        fb('L\'anno prossimo ___ un corso B1.', ['farò', 'facevo', 'ho fatto', 'faccio ieri']),
        mc('"Ieri sono andato al mercato" is in the…', 'passato prossimo', 'imperfetto', 'futuro', 'presente'),
      ],
      homework: 'Write one sentence about yourself in each of the 4 tenses.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · A2 review: direct or indirect pronoun?',
      titleIt: 'Lezione 4 · Ripasso A2: pronomi diretti e indiretti',
      summary: 'Chi? Che cosa? → lo, la, li, le · A chi? → mi, ti, gli, le, ci, vi.',
      minutes: 12,
      sections: [
        {
          title: 'Non confondere CHI e A CHI',
          table: [
            ['DIRETTI (chi? che cosa?)', 'INDIRETTI (a chi?)'],
            ['Compro il pane → Lo compro.', 'Telefono a Paolo → Gli telefono.'],
            ['Chiamo Maria → La chiamo.', 'Regalo un fiore a Maria → Le regalo un fiore.'],
            ['Saluto gli amici → Li saluto.', 'Scrivo ai miei genitori → Gli scrivo.'],
          ],
        },
        { tip: '**Telefonare a** takes an indirect pronoun (*le telefono*), but **chiamare** takes a direct one (*la chiamo*) — same meaning, different pronoun!' },
      ],
      exercises: [
        fb('Compri tu i biglietti? — Sì, ___ compro io oggi pomeriggio.', ['li', 'gli', 'le', 'lo']),
        fb('Domani è il compleanno di Maria: ___ regalerò un libro.', ['le', 'la', 'gli', 'li']),
        fb('… e ___ telefonerò.', ['le', 'la', 'gli', 'lo']),
        fb('Chiamo Maria → ___ chiamo.', ['La', 'Le', 'Gli', 'Lo']),
        fb('Scrivo ai miei genitori → ___ scrivo.', ['Gli', 'Li', 'Le', 'Lo']),
        fb('Saluto gli amici → ___ saluto.', ['Li', 'Gli', 'Le', 'Lo']),
        fb('Il pane? ___ compro al mercato.', ['Lo', 'Gli', 'Le', 'La']),
        fb('Paolo è malato: ___ porto la medicina.', ['gli', 'lo', 'le', 'li']),
      ],
      homework: 'Write 3 sentences with a direct pronoun and 3 with an indirect pronoun about your family.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · Preparing the A2 oral exam',
      titleIt: 'Lezione 4 · Preparare l\'esame orale A2',
      summary: 'The 3 parts of the oral exam and the examiner\'s questions.',
      minutes: 15,
      sections: [
        {
          title: 'Le 3 prove del colloquio orale A2 (10–12 minuti)',
          table: [
            ['Prova', 'Durata', 'Contenuto'],
            ['1 · Presentazione personale', '3 minuti', 'chi sei, il passato, il lavoro attuale e i progetti futuri'],
            ['2 · Dialogo di ruolo', '4 minuti', 'al medico, in posta, al colloquio o con l\'idraulico'],
            ['3 · Descrivere e confrontare', '3 minuti', 'città vs campagna o il tuo Paese ieri e oggi'],
          ],
        },
        {
          title: 'Prova 1 · Le domande dell\'esaminatore',
          text: '1. Come ti chiami, da dove vieni e dove abiti adesso? (**presente**)\n2. Che cosa facevi nel tuo Paese prima di venire in Italia? (**imperfetto**)\n3. Quando sei arrivato/a in Italia e cosa hai fatto all\'inizio? (**passato prossimo**)\n4. Quali progetti hai per il lavoro, lo studio o le vacanze? (**futuro**)',
        },
        {
          title: 'Simulazione a coppie: scegli una carta',
          text: '- **Carta A · situazione pratica (Lei formale):** colloquio o ufficio pubblico — usa *vorrei, posso, quindi, perché* e i pronomi indiretti.\n- **Carta B · confronto ieri e oggi:** città vs campagna / Paese vs Italia — usa imperfetto, presente e comparativi (*più … di / che, meno … di, -issimo*).',
        },
        { tip: 'Congratulations: you have finished the whole A1–A2 programme! Before the exam your teacher will say **«In bocca al lupo!»** — answer **«Crepi!»**' },
      ],
      exercises: [
        fb('Prima di venire in Italia ___ l\'autista.', ['facevo', 'farò', 'faccio domani', 'ho fatto domani']),
        fb('Tre anni fa sono ___ in Italia in aereo.', ['arrivato', 'arrivare', 'arrivavo', 'arriverò']),
        fb('All\'inizio la vita era più difficile ___ non parlavo italiano.', ['perché', 'quindi', 'però', 'come']),
        fb('L\'estate prossima ___ in vacanza con la mia famiglia.', ['andrò', 'andavo', 'sono andato', 'vado ieri']),
        fb('Viaggiare in treno è più comodo ___ viaggiare in autobus.', ['che', 'di', 'del', 'come']),
        fb('Questo telefono è ___ ed è il migliore del negozio.', ['bellissimo', 'più bellissimo', 'il più bellissimo', 'bellissimo di']),
        mc('In Prova 1, "Che cosa facevi nel tuo Paese?" needs…', 'the imperfetto', 'the futuro', 'the imperative', 'the presente'),
      ],
      homework: 'Prepare Prova 1: answer the 4 examiner\'s questions aloud in 3 minutes.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST FINALE LIVELLO A2 (Capitoli 8–12)
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Final A2 test (Chapters 8–12)',
      titleIt: 'Test finale livello A2 (Capitoli 8–12)',
      summary: 'The final A2 test: listening, vocabulary and grammar from Chapters 8–12. Pass mark: 60%.',
      minutes: 30,
      practice: false,
      sections: [
        {
          title: 'Test finale livello A2',
          text: 'This is the final test of level A2. It checks everything in Chapters 8–12.\n\n- **1. Listening** — sentences and a story in past, present and future\n- **2. Vocabulary** — travel, health, daily life and offices\n- **3. Grammar** — the tenses, the pronouns, the imperative and comparisons\n- **4. Writing** — my story: yesterday, today and tomorrow\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (your story with the 4 tenses) and the **final oral exam** (your past compared with today · a real situation in Italy · your plans for the future — 3 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A
        lis('1A · Listen and choose the sentence you hear', 'In stazione: ieri ho preso il treno dal binario tre per andare a Milano.', 'IERI HO PRESO IL TRENO DAL BINARIO TRE', 'MIA MOGLIE È PARTITA IN AEREO STAMATTINA'),
        lis('1A · Listen and choose the sentence you hear', 'Ho camminato tutto il giorno e mi fanno male le gambe da ieri sera.', 'MI FANNO MALE LE GAMBE DA IERI SERA', 'HO MAL DI GOLA E HO PRESO UNO SCIROPPO'),
        lis('1A · Listen and choose the sentence you hear', 'Il dottore dice: prenda queste compresse dopo i pasti per tre giorni.', 'PRENDA QUESTE COMPRESSE DOPO I PASTI', 'NON USCIRE OGGI, RIPOSA A CASA!'),
        lis('1A · Listen and choose the sentence you hear', 'Stamattina mentre dormivo, è suonata la sveglia alle sei.', 'MENTRE DORMIVO, È SUONATA LA SVEGLIA', 'DA PICCOLO ABITAVO IN UN PICCOLO VILLAGGIO'),
        lis('1A · Listen and choose the sentence you hear', 'Ho trovato un nuovo contratto: l\'anno prossimo andrò a lavorare a Milano.', 'L\'ANNO PROSSIMO ANDRÒ A LAVORARE A MILANO', 'GLI HO TELEFONATO PERCHÉ AVEVO UN PROBLEMA'),
        // 1B
        lis('1B · Listen: "Quando ___ nel mio Paese, facevo l\'autista." (1)', 'Quando abitavo nel mio Paese, facevo l\'autista.', 'ABITAVO', 'ABITERÒ', 'ABITO', 'HO ABITATO'),
        lis('1B · Listen again: "Tre anni fa sono ___ in Italia" (2)', 'Tre anni fa sono arrivato in Italia in aereo.', 'ARRIVATO', 'PARTITO', 'RIMASTO', 'NATO'),
        lis('1B · Listen again: "la vita era più difficile ___ non parlavo italiano" (3)', 'All\'inizio la vita era più difficile perché non parlavo italiano.', 'PERCHÉ', 'QUINDI', 'PERÒ', 'ANCHE SE'),
        lis('1B · Listen again: "l\'estate prossima ___ in vacanza" (4)', 'Oggi lavoro in un\'azienda e l\'estate prossima andrò in vacanza con la mia famiglia.', 'ANDRÒ', 'ANDAVO', 'SONO ANDATO', 'VADO'),
        lis('1B · Listen again: "e ___ molti regali ai miei genitori" (5)', 'E porterò molti regali ai miei genitori.', 'PORTERÒ', 'PORTAVO', 'HO PORTATO', 'PORTO'),
        // 2A
        pairs('2A · Match each word with its meaning', [
          ['Prenotazione / Binario', 'Booking / Platform · Buking / Pletfarm'],
          ['Ricetta medica / Sciroppo', 'Prescription / Syrup · Parchi / Sharbat'],
          ['Caotico / Frenetico', 'Chaotic / Hectic · Raula / Tez'],
          ['Raccomandata / Bollettino', 'Registered letter / Payment slip · Rajistard chitthi / Bill'],
          ['Idraulico / Caldaia', 'Plumber / Boiler · Palambar / Boilar'],
        ]),
        // 2B
        mc('2B · Prima di partire preparo la ?', 'valigia', 'febbre', 'campagna', 'patente'),
        mc('2B · Ho 39 di temperatura: ho la ?', 'febbre', 'valigia', 'patente', 'campagna'),
        mc('2B · L\'aria è pulita e c\'è silenzio in ?', 'campagna', 'valigia', 'febbre', 'patente'),
        mc('2B · Per guidare il furgone serve la ?', 'patente', 'valigia', 'campagna', 'febbre'),
        mc('2B · Pioveva forte, ___ ho preso il bus.', 'quindi', 'perché', 'anche se', 'come'),
        // 3A
        fb('3A · Da bambina Sofia ___ (essere) timida…', ['era', 'è stata', 'sarà', 'è']),
        fb('3A · … e ___ (leggere) molti libri.', ['leggeva', 'ha letto', 'leggerà', 'legge']),
        fb('3A · Ieri sera mentre noi ___ (cenare)…', ['cenavamo', 'abbiamo cenato', 'ceneremo', 'ceniamo']),
        fb('3A · … ___ (suonare) il campanello.', ['ha suonato', 'suonava', 'suonerà', 'suona']),
        fb('3A · (Sofia) Domenica scorsa io e mia sorella ___ (andare) a Verona…', ['siamo andate', 'siamo andati', 'abbiamo andato', 'andavamo']),
        fb('3A · … e ___ (divertirsi) molto.', ['ci siamo divertite', 'ci siamo divertiti', 'abbiamo divertito', 'ci divertivamo']),
        // 3B
        fb('3B · L\'anno prossimo io ___ (cercare) una casa più grande…', ['cercherò', 'cercerò', 'cercavo', 'ho cercato']),
        fb('3B · … e ___ (fare) un corso.', ['farò', 'facerò', 'facevo', 'ho fatto']),
        fb('3B · Domani noi ___ (andare) dal medico…', ['andremo', 'anderemo', 'andavamo', 'siamo andati']),
        fb('3B · … e ___ (potere) chiedere la ricetta.', ['potremo', 'poteremo', 'potevamo', 'abbiamo potuto']),
        fb('3B · (tu) «Hai mal di testa: ___ (prendere) una compressa…»', ['prendi', 'prenda', 'prenderai ieri', 'prendere']),
        fb('3B · (tu) «… e non ___ (uscire)!»', ['uscire', 'esci', 'esca', 'uscirai']),
        fb('3B · (Lei) «Buongiorno Signora, ___ (compilare) questo modulo…»', ['compili', 'compila', 'compilare', 'compilerà']),
        fb('3B · (Lei) «… e ___ (firmare) qui.»', ['firmi', 'firma', 'firmare', 'firmerai']),
        // 3C
        fb('3C · Compri tu i biglietti? — Sì, ___ compro io oggi pomeriggio.', ['li', 'gli']),
        fb('3C · Domani è il compleanno di Maria: ___ regalerò un libro…', ['le', 'la']),
        fb('3C · … e ___ telefonerò.', ['le', 'la']),
        fb('3C · Viaggiare in treno è più comodo ___ viaggiare in autobus.', ['che', 'di']),
        fb('3C · Questo telefono è ___', ['bellissimo', 'più bellissimo']),
        fb('3C · … ed è ___ del negozio.', ['il migliore', 'migliore']),
        // 4
        wo('Quando abitavo nel mio Paese facevo l\'autista', 'When I lived in my country I was a driver'),
        wo('Sono arrivato in Italia tre anni fa', 'I arrived in Italy three years ago'),
        wo('Studio con impegno perché voglio un buon lavoro', 'I study hard because I want a good job'),
        wo('L\'anno prossimo farò un corso di italiano', 'Next year I will do an Italian course'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences of your story (imperfetto, passato prossimo, presente + comparative, a connector, futuro). Then prepare the 3-minute final oral exam.',
    },
  ],
};
