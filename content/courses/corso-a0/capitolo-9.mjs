import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 9 of the school syllabus (Corso di Italiano, livello A1 → A2).
// Source: syllabus/Capitolo 9 Lezione 1–4 (.pptx) and Test Capitolo 9 (.docx).

const slides = (lez) => `/content/slides/corso-a0-cap9-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-9',
  title: 'Chapter 9: The past with ESSERE — travel and arriving in Italy',
  titleIt: 'Capitolo 9: Il passato prossimo con ESSERE e i viaggi',
  cover: 'Airplane',
  description: 'The passato prossimo with essere and participle agreement (sono andato / andata / andati / andate), choosing avere or essere, reflexive verbs in the past (mi sono svegliato), travel words at the station, the airport and the hotel, and telling the story of a trip or of your arrival in Italy.',
  canDo: [
    'I can use the passato prossimo with essere and make the participle agree: è andato, è andata, sono andati',
    'I can choose avere or essere: che cosa? → avere · dove? → essere',
    'I can use reflexive verbs in the past: mi sono svegliato / svegliata',
    'I can buy a train ticket and ask for the platform',
    'I can check in at a hotel and ask for a single or double room',
    'I can tell the story of a trip or of my arrival in Italy',
  ],
  liveClassFocus: 'Role-play at the ticket office or the hotel reception, then interview a partner with the 5 questions about their arrival in Italy and tell the story to the class (watch the -o / -a ending!).',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — Passato prossimo con ESSERE
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 1 · The passato prossimo with ESSERE',
      titleIt: 'Lezione 1 · Il passato prossimo con ESSERE',
      summary: 'Movement and change: sono andato / andata — the participle agrees (-o, -a, -i, -e).',
      minutes: 12,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 9 · Lezione 1 (PDF)' },
        {
          title: 'AVERE o ESSERE: la regola fondamentale',
          table: [
            ['Con AVERE (Capitolo 8)', 'Con ESSERE (Capitolo 9)'],
            ['actions on things (mangiare, bere, fare)', 'movement or change of state (andare, partire)'],
            ['the participle always ends in -O', 'the participle changes: -o, -a, -i, -e!'],
            ['Maria ha mangiato la pizza.', 'Maria è andata a Roma.'],
          ],
        },
        {
          title: 'L\'accordo del participio: -O, -A, -I, -E',
          table: [
            ['Masch. singolare -O', 'Femm. singolare -A', 'Masch. plurale -I', 'Femm. plurale -E'],
            ['Marco è andato.', 'Sara è andata.', 'Marco e Paolo sono andati.', 'Sara e Giulia sono andate.'],
          ],
        },
        {
          title: 'Esempio completo: ANDARE',
          table: [
            ['Soggetto', 'Essere', 'Maschile', 'Femminile', 'Punjabi'],
            ['io', 'sono', 'sono andato', 'sono andata', 'maiṁ giā / gaī'],
            ['tu', 'sei', 'sei andato', 'sei andata', 'tūṁ giā / gaī'],
            ['lui / lei', 'è', 'lui è andato', 'lei è andata', 'uh giā / gaī'],
            ['noi', 'siamo', 'siamo andati', 'siamo andate', 'asīṁ gae / gaīāṁ'],
            ['voi', 'siete', 'siete andati', 'siete andate', 'tusīṁ gae / gaīāṁ'],
            ['loro', 'sono', 'sono andati', 'sono andate', 'uh gae / gaīāṁ'],
          ],
        },
        { tip: 'A mixed group (men and women) takes **-i**: *Marco e Sara sono andat**i***. Only a group of women takes **-e**.' },
      ],
      exercises: [
        fb('Sara è ___ a Roma.', ['andata', 'andato', 'andati', 'andate']),
        fb('Marco è ___ in stazione.', ['andato', 'andata', 'andati', 'andate']),
        fb('Marco e Paolo sono ___ al lavoro.', ['andati', 'andate', 'andato', 'andata']),
        fb('Sara e Giulia sono ___ al mercato.', ['andate', 'andati', 'andata', 'andato']),
        fb('Ieri noi ___ andati al cinema.', ['siamo', 'abbiamo', 'sono', 'siete']),
        fb('Tu ___ andata dal medico?', ['sei', 'hai', 'è', 'sono']),
        mc('Maria ___ mangiato la pizza.', 'ha', 'è', 'sono', 'hanno'),
        mc('Marco and Sara went: Marco e Sara sono…', 'andati', 'andate', 'andata', 'andato'),
      ],
      homework: 'Write where you and 3 people in your family went last week (Io sono andato / andata…, mia madre è andata…).',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · The 10 key verbs with ESSERE',
      titleIt: 'Lezione 1 · I 10 verbi di movimento e cambio di stato',
      summary: 'Andato, venuto, partito, arrivato, tornato, entrato, uscito, stato, rimasto, nato.',
      minutes: 10,
      sections: [
        {
          title: 'Verbi di movimento e cambio di stato',
          words: [
            wp('andare → andato/a', 'to go → gone', 'Giā', { img: 'Person walking', ex: 'Sono andato al lavoro.', exEn: 'I went to work.' }),
            wp('venire → venuto/a', 'to come → come', 'Āiā', { img: 'Waving hand', ex: 'Mia sorella è venuta con me.', exEn: 'My sister came with me.' }),
            wp('partire → partito/a', 'to leave → left', 'Ravānā hoiā', { img: 'Airplane departure', ex: 'Il treno è partito alle otto.', exEn: 'The train left at eight.' }),
            wp('arrivare → arrivato/a', 'to arrive → arrived', 'Pahuñchiā', { img: 'Airplane arrival', ex: 'È arrivato in orario.', exEn: 'It arrived on time.' }),
            wp('tornare → tornato/a', 'to return → returned', 'Vāpas āiā', { img: 'Counterclockwise arrows button', ex: 'Simran è tornata tardi.', exEn: 'Simran came back late.' }),
            wp('entrare → entrato/a', 'to enter → entered', 'Andar āiā', { img: 'Door', ex: 'Sono entrato in classe.', exEn: 'I came into the classroom.' }),
            wp('uscire → uscito/a', 'to go out → gone out', 'Bāhar giā', { img: 'Person running', ex: 'Simran è uscita con le amiche.', exEn: 'Simran went out with her friends.' }),
            wp('stare → stato/a', 'to stay / be → stayed / been', 'Rihā', { img: 'House', ex: 'Siamo stati tranquilli.', exEn: 'We had a quiet time.' }),
            wp('rimanere → rimasto/a', 'to remain → remained', 'Rahi giā', { img: 'Couch and lamp', ex: 'Domenica siamo rimasti a casa.', exEn: 'On Sunday we stayed at home.' }),
            wp('nascere → nato/a', 'to be born → born', 'Janam liā', { img: 'Baby', ex: 'Mio figlio è nato a Mantova.', exEn: 'My son was born in Mantova.' }),
          ],
        },
        {
          title: 'Esempi in contesto',
          practice: false,
          words: [
            wp('Il treno è partito alle otto ed è arrivato in orario.', 'The train left at eight and arrived on time.', 'Ṭren aṭṭh vaje chalī ate sameṁ sir pahuñch gaī.'),
            wp('Ieri sera Simran è uscita con le amiche ed è tornata tardi.', 'Last night Simran went out with her friends and came back late.', 'Kall rāt Simran sahelīāṁ nāl bāhar gaī ate der nāl vāpas āī.'),
            wp('Domenica noi siamo rimasti a casa.', 'On Sunday we stayed at home.', 'Aitvār nūṁ asīṁ ghar hī rahe.'),
            wp('Mio figlio è nato a Mantova due anni fa.', 'My son was born in Mantova two years ago.', 'Merā puttar do sāl pahilāṁ Mantova vich jammiā.'),
          ],
        },
        { tip: 'Three are irregular: **venire → venuto**, **rimanere → rimasto**, **nascere → nato**.' },
      ],
      exercises: [
        mc('venire → ?', 'venuto', 'venito', 'vento', 'venato'),
        mc('rimanere → ?', 'rimasto', 'rimanuto', 'rimanito', 'rimaso'),
        mc('nascere → ?', 'nato', 'nasciuto', 'nascito', 'nasto'),
        fb('Il treno è ___ alle otto.', ['partito', 'partita', 'partiti', 'partite']),
        fb('Simran è ___ con le amiche.', ['uscita', 'uscito', 'usciti', 'uscite']),
        fb('Domenica noi siamo ___ a casa.', ['rimasti', 'rimasto', 'rimasta', 'rimanuti']),
        tr('è tornata', 'she came back', 'she left', 'she stayed', 'she went out'),
      ],
      homework: 'Write 5 sentences about last weekend with 5 different verbs from the list.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Dialogue: meeting at the station',
      titleIt: 'Lezione 1 · Dialogo: incontro in stazione',
      summary: 'Gurpreet is back from India — quando sei tornato? sono arrivato ieri sera.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: incontro in stazione',
          situation: 'Elena meets Gurpreet at the station.',
          dialogue: [
            lp('Elena', 'Ciao Gurpreet! Quando sei tornato dall\'India?', 'Hi Gurpreet! When did you come back from India?', 'Sat Srī Akāl Gurpreet! Tusīṁ Bhārat toṁ kadoṁ vāpas āe?'),
            lp('Gurpreet', 'Sono arrivato ieri sera! Sono partito da Delhi alle dieci.', 'I arrived last night! I left Delhi at ten.', 'Maiṁ kall shām nūṁ pahuñchiā! Maiṁ das vaje Dillī toṁ ravānā hoiā sī.'),
            lp('Elena', 'È venuta anche tua sorella con te?', 'Did your sister come with you too?', 'Kī tuhāḍī bhaiṇ vī tuhāḍe nāl āī?'),
            lp('Gurpreet', 'No, lei è rimasta là con i nostri genitori. Io sono già uscito per lavorare!', 'No, she stayed there with our parents. I\'ve already gone out to work!', 'Nahīṁ, uh māpiāṁ nāl uthe hī rahi gaī. Maiṁ tāṁ kamm laī nikal vī piā hāṁ!'),
          ],
        },
        {
          title: '3 domande al compagno con ESSERE',
          text: '1. **Dove e quando?** Dove sei andato/a domenica? Quando sei nato/a?\n2. **Uscire o rimanere?** Ieri sera sei uscito/a o sei rimasto/a a casa?\n3. **Arrivare in Italia:** Quando sei arrivato/a in Italia? Sei venuto/a solo/a?',
        },
      ],
      exercises: [
        tf('Gurpreet è tornato dall\'India.', true, 'Gurpreet came back from India.'),
        tf('Gurpreet è arrivato stamattina.', false, 'Gurpreet arrived this morning.'),
        tf('La sorella è venuta con lui.', false, 'His sister came with him.'),
        tf('La sorella è rimasta con i genitori.', true, 'His sister stayed with their parents.'),
        wo('Sono partito da Delhi alle dieci', 'I left Delhi at ten'),
        wo('Lei è rimasta con i nostri genitori', 'She stayed with our parents'),
      ],
      homework: 'Ask a partner the 3 questions and write their answers (Lui è nato… / Lei è arrivata…).',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — AVERE o ESSERE? Verbi riflessivi al passato
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 2 · AVERE or ESSERE? The quick test',
      titleIt: 'Lezione 2 · AVERE o ESSERE? Il test rapido',
      summary: 'Che cosa? → avere · Dove? → essere — and cambiare / finire that use both.',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 9 · Lezione 2 (PDF)' },
        {
          title: 'Come scegliere: AVERE o ESSERE?',
          table: [
            ['AVERE: answers «che cosa?»', 'ESSERE: answers «dove?»'],
            ['Ho mangiato che cosa? → La pizza.', 'Sono andato/a dove? → Al lavoro.'],
            ['Ho letto che cosa? → Il giornale.', 'Siamo arrivati/e dove? → A casa.'],
            ['participle always in -O', 'agreement: -o, -a, -i, -e!'],
          ],
        },
        {
          title: 'Casi speciali: cambiare e finire',
          table: [
            ['Con un oggetto → AVERE', 'Senza oggetto → ESSERE'],
            ['Ho cambiato casa.', 'Il tempo è cambiato.'],
            ['Ho finito il turno.', 'Il film è finito.'],
          ],
        },
        { tip: 'If the verb has an object (*che cosa?*) use **avere**. If someone moves or changes on their own, use **essere**.' },
      ],
      exercises: [
        fb('Stamattina noi ___ partiti alle sette.', ['siamo', 'abbiamo', 'sono', 'siete']),
        fb('Al bar Marco ___ preso due cappuccini.', ['ha', 'è', 'hanno', 'sono']),
        fb('Dove ___ nata tua madre?', ['è', 'ha', 'sono', 'hai']),
        fb('Ieri tu ___ comprato i biglietti?', ['hai', 'sei', 'ha', 'è']),
        fb('Giulia ___ uscita con le sue amiche.', ['è', 'ha', 'sono', 'hanno']),
        fb('Il film ___ finito alle dieci.', ['è', 'ha', 'sono', 'hanno']),
        fb('Io ___ finito il turno alle sei.', ['ho', 'sono', 'è', 'ha']),
        fb('L\'anno scorso ___ cambiato casa.', ['ho', 'sono', 'è', 'siamo']),
      ],
      homework: 'Write 6 sentences about yesterday: 3 with avere and 3 with essere.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Reflexive verbs in the past',
      titleIt: 'Lezione 2 · I verbi riflessivi al passato',
      summary: 'Always with ESSERE: mi sono svegliato / svegliata, ci siamo divertiti…',
      minutes: 12,
      sections: [
        {
          title: 'SVEGLIARSI al passato prossimo',
          table: [
            ['Soggetto', 'Pronome + essere', 'Maschile', 'Femminile'],
            ['io', 'mi sono', 'mi sono svegliato', 'mi sono svegliata'],
            ['tu', 'ti sei', 'ti sei svegliato', 'ti sei svegliata'],
            ['lui / lei', 'si è', 'si è svegliato', 'si è svegliata'],
            ['noi', 'ci siamo', 'ci siamo svegliati', 'ci siamo svegliate'],
            ['voi', 'vi siete', 'vi siete svegliati', 'vi siete svegliate'],
            ['loro', 'si sono', 'si sono svegliati', 'si sono svegliate'],
          ],
        },
        {
          title: 'La mattina di Simran',
          practice: false,
          words: [
            wp('Stamattina mi sono svegliata alle sei e mezza.', 'This morning I woke up at half past six.', 'Ajj savere maiṁ sāḍhe chhe vaje jāgī.'),
            wp('Subito dopo mi sono alzata e mi sono lavata la faccia.', 'Right after, I got up and washed my face.', 'Usdoṁ bāad maiṁ uṭhī ate muṁh dhotā.'),
            wp('Poi mi sono vestita in fretta per prendere il treno.', 'Then I got dressed quickly to catch the train.', 'Phir maiṁ ṭren pheṛan laī jaldī kapṛe pāe.'),
            wp('Ieri sera invece mi sono addormentata alle undici.', 'Last night, instead, I fell asleep at eleven.', 'Kall rāt maiṁ giārāṁ vaje sauṁ gaī.'),
          ],
        },
        {
          title: 'Divertirsi, incontrarsi, sentirsi, riposarsi',
          words: [
            wp('Ci siamo divertiti molto!', 'We had a lot of fun!', 'Asīṁ bahut mazā kītā!', { img: 'Party popper', ex: 'Ieri alla festa ci siamo divertiti molto!', exEn: 'Yesterday at the party we had a lot of fun!' }),
            wp('Ci siamo incontrate al bar.', 'We met at the café. (two women)', 'Asīṁ kaife te milīāṁ.', { img: 'Handshake', ex: 'Sabato io e Sara ci siamo incontrate al bar.', exEn: 'On Saturday Sara and I met at the café.' }),
            wp('Mi sono riposato.', 'I rested.', 'Maiṁ ārām kītā.', { img: 'Relieved face' }),
            wp('Mi sono sentito bene.', 'I felt well.', 'Mainūṁ changā mahisūs hoiā.', { img: 'Smiling face with smiling eyes' }),
          ],
        },
        { tip: 'Reflexive verbs **always** take essere in the past, and the participle agrees: *Nadia **si è** svegliat**a***, *io e Paolo **ci siamo** divertit**i***.' },
      ],
      exercises: [
        fb('Stamattina Nadia ___ alle sei e mezza.', ['si è svegliata', 'ha svegliato', 'si ha svegliata', 'è svegliata si']),
        fb('Ieri sera io e Paolo ___ molto alla festa.', ['ci siamo divertiti', 'siamo divertito', 'ci abbiamo divertiti', 'ci siamo divertito']),
        fb('A che ora ___ ieri notte, Luca?', ['ti sei addormentato', 'sei ti addormentato', 'ti hai addormentato', 'ti sei addormentata']),
        fb('Domenica io (Simran) ___ tutto il giorno.', ['mi sono riposata', 'mi sono riposato', 'ho riposata', 'mi ho riposata']),
        fb('I ragazzi ___ alle sette.', ['si sono alzati', 'si è alzato', 'si sono alzate', 'hanno alzati']),
        fb('Voi ___ in fretta?', ['vi siete vestiti', 'vi avete vestiti', 'ci siamo vestiti', 'vi siete vestito']),
      ],
      homework: 'Write your morning yesterday with 5 reflexive verbs in the past.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · Dialogue: after dinner with friends',
      titleIt: 'Lezione 2 · Dialogo: dopo la cena con gli amici',
      summary: 'Aman tells Chiara about the restaurant: ci siamo incontrati, ci siamo divertiti.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: dopo la cena con gli amici',
          situation: 'Chiara asks Aman about last night.',
          dialogue: [
            lp('Chiara', 'Ciao Aman! Ieri sera siete andati al ristorante?', 'Hi Aman! Did you go to the restaurant last night?', 'Sat Srī Akāl Aman! Kī kall rāt tusīṁ resṭoraiṇṭ gae sī?'),
            lp('Aman', 'Sì! Ci siamo incontrati alle otto e abbiamo mangiato benissimo.', 'Yes! We met at eight and the food was excellent.', 'Hāṁ! Asīṁ aṭṭh vaje mile ate bahut vadhīā khāṇā khādhā.'),
            lp('Chiara', 'Vi siete divertiti? A che ora siete tornati a casa?', 'Did you have fun? What time did you get home?', 'Mazā āiā? Tusīṁ kinne vaje ghar vāpas āe?'),
            lp('Aman', 'Tantissimo! Siamo tornati a mezzanotte e stamattina mi sono svegliato tardi!', 'So much! We came back at midnight and this morning I woke up late!', 'Bahut ziādā! Asīṁ addhī rāt nūṁ vāpas āe ate ajj savere maiṁ der nāl jāgiā!'),
          ],
        },
        {
          title: 'Intervista: la tua giornata di ieri',
          text: '1. **La mattina:** A che ora ti sei svegliato/a e alzato/a ieri?\n2. **Gli spostamenti:** Dove sei andato/a e cos\'hai fatto?\n3. **La sera:** Ti sei divertito/a o ti sei riposato/a?',
        },
      ],
      exercises: [
        tf('Si sono incontrati alle otto.', true, 'They met at eight.'),
        tf('Hanno mangiato male.', false, 'The food was bad.'),
        tf('Sono tornati a casa a mezzanotte.', true, 'They got home at midnight.'),
        tf('Stamattina Aman si è svegliato presto.', false, 'This morning Aman woke up early.'),
        wo('Ci siamo incontrati alle otto', 'We met at eight'),
        wo('Stamattina mi sono svegliato tardi', 'This morning I woke up late'),
      ],
      homework: 'Interview a partner about yesterday with the 3 questions.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — In stazione, in aeroporto e in albergo
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · Travel words: station, airport, hotel',
      titleIt: 'Lezione 3 · Il vocabolario del viaggio',
      summary: 'Biglietteria, binario, andata e ritorno, volo, valigia, prenotazione, camera doppia…',
      minutes: 12,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 9 · Lezione 3 (PDF)' },
        {
          title: 'In stazione',
          words: [
            wp('la biglietteria', 'ticket office', 'Ṭikaṭ ghar', { pron: 'lah bee-lyet-teh-REE-ah', img: 'Ticket' }),
            wp('solo andata', 'one-way', 'Ikk pāse dī ṭikaṭ', { pron: 'SOH-loh ahn-DAH-tah', img: 'Right arrow' }),
            wp('andata e ritorno', 'return (round trip)', 'Āuṇ-jāṇ dī ṭikaṭ', { pron: 'ahn-DAH-tah eh ree-TOR-noh', img: 'Left-right arrow' }),
            wp('il binario', 'platform', 'Pleṭfārm', { pron: 'eel bee-NAH-ryoh', img: 'Station' }),
            wp('convalidare il biglietto', 'to validate the ticket', 'Ṭikaṭ pañch karnī', { pron: 'kon-vah-lee-DAH-reh', img: 'Check mark button' }),
            wp('in orario / in ritardo', 'on time / late', 'Sameṁ sir / Der nāl', { pron: 'een oh-RAH-ryoh / een ree-TAR-doh', img: 'Alarm clock' }),
            wp('la coincidenza', 'connection (next train)', 'Aglī gaḍḍī', { pron: 'lah ko-een-chee-DEN-tsah', img: 'High-speed train' }),
          ],
        },
        {
          title: 'In aeroporto e in albergo',
          words: [
            wp('il volo', 'flight', 'Uḍāṇ', { pron: 'eel VOH-loh', img: 'Airplane' }),
            wp('la valigia / i bagagli', 'suitcase / luggage', 'Sūṭkes / Sāmān', { pron: 'lah vah-LEE-jah', img: 'Luggage' }),
            wp('la carta d\'imbarco', 'boarding pass', 'Boarḍiṅg pās', { pron: 'lah KAR-tah deem-BAR-koh', img: 'Admission tickets' }),
            wp('il passaporto / il documento', 'passport / ID', 'Pāsporṭ / Shanākhtī kārḍ', { pron: 'eel pahs-sah-POR-toh', img: 'Passport control' }),
            wp('la prenotazione', 'booking', 'Bukiṅg', { pron: 'lah preh-noh-tah-TSYOH-neh', img: 'Calendar' }),
            wp('la camera singola', 'single room', 'Ikk bande dā kamrā', { pron: 'SEEN-goh-lah', img: 'Bed' }),
            wp('la camera doppia / matrimoniale', 'double room', 'Do bandiāṁ dā kamrā', { pron: 'DOP-pyah', img: 'Hotel' }),
            wp('colazione inclusa', 'breakfast included', 'Nāshtā shāmil', { pron: 'koh-lah-TSYOH-neh een-KLOO-zah', img: 'Croissant' }),
          ],
        },
        { tip: 'Regional train tickets bought on paper must be **convalidati** (stamped) in the green or yellow machine before you get on — otherwise you can get a fine!' },
      ],
      exercises: [
        mc('You go and come back the same day. You buy…', 'andata e ritorno', 'solo andata', 'la coincidenza', 'la carta d\'imbarco'),
        mc('Where do you wait for the train?', 'al binario', 'alla reception', 'in biglietteria', 'al check-in'),
        mc('A room for one person is…', 'una camera singola', 'una camera doppia', 'una prenotazione', 'una valigia'),
        mc('The train is not on time: it is…', 'in ritardo', 'in orario', 'inclusa', 'convalidato'),
        tr('la valigia', 'suitcase', 'ticket', 'flight', 'booking'),
      ],
      homework: 'Write the travel words you need for your next trip home.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Dialogues: at the ticket office and at the hotel',
      titleIt: 'Lezione 3 · Dialoghi: in biglietteria e alla reception',
      summary: 'A return ticket to Milano, platform 3 — and check-in at the Hotel Centrale.',
      minutes: 12,
      sections: [
        {
          title: 'Dialogo 1: comprare un biglietto in biglietteria',
          situation: 'A traveller at the station ticket office.',
          dialogue: [
            lp('Viaggiatore', 'Buongiorno! Vorrei un biglietto di andata e ritorno per Milano.', 'Good morning! I\'d like a return ticket to Milano.', 'Sat Srī Akāl! Mainūṁ Milano dī āuṇ-jāṇ dī ṭikaṭ chāhīdī hai.'),
            lp('Impiegata', 'Per oggi? C\'è un treno regionale alle 10:15. Sono 24 euro.', 'For today? There\'s a regional train at 10:15. It\'s 24 euros.', 'Ajj laī? 10:15 vaje ikk rījanal ṭren hai. 24 yūro hoṇge.'),
            lp('Viaggiatore', 'Perfetto, pago con la carta. Da quale binario parte?', 'Perfect, I\'ll pay by card. Which platform does it leave from?', 'Ṭhīk hai, maiṁ kārḍ nāl deṇdā hāṁ. Kihaṛe pleṭfārm toṁ chalegī?'),
            lp('Impiegata', 'Dal binario 3, ma attenzione: oggi ha 10 minuti di ritardo!', 'From platform 3, but be careful: today it\'s 10 minutes late!', 'Pleṭfārm 3 toṁ, par dhiān rakho: ajj das minaṭ der nāl hai!'),
          ],
        },
        {
          title: 'Dialogo 2: alla reception dell\'hotel',
          situation: 'Mr Singh checks in at the hotel.',
          dialogue: [
            lp('Receptionist', 'Buonasera, benvenuto all\'Hotel Centrale!', 'Good evening, welcome to the Hotel Centrale!', 'Sat Srī Akāl, Hoṭal Centrale vich jī āiāṁ nūṁ!'),
            lp('Cliente', 'Buonasera! Ho una prenotazione a nome Singh per due notti.', 'Good evening! I have a booking under the name Singh for two nights.', 'Sat Srī Akāl! Merī Singh de nāṁ te do rātāṁ dī bukiṅg hai.'),
            lp('Receptionist', 'Sì, una camera doppia con colazione inclusa. Mi dà il passaporto?', 'Yes, a double room with breakfast included. May I have your passport?', 'Hāṁ jī, nāshte samet do bandiāṁ dā kamrā. Pāsporṭ de sakde ho?'),
            lp('Cliente', 'Ecco il documento. A che ora è la colazione e dove posso lasciare la valigia?', 'Here is my ID. What time is breakfast and where can I leave my suitcase?', 'Eh lao pāsporṭ. Nāshtā kinne vaje hai ate maiṁ sūṭkes kitthe rakh sakdā hāṁ?'),
          ],
        },
        {
          title: 'Domande chiave del viaggiatore',
          text: '**In stazione e in aeroporto**\n- Scusi, da quale binario parte il treno?\n- Il volo è in orario o in ritardo?\n- Dove devo ritirare la mia valigia?\n\n**In albergo**\n- Avete una camera singola libera?\n- Quanto costa una doppia a notte?\n- Qual è la password del Wi-Fi?',
        },
      ],
      exercises: [
        tf('Il viaggiatore vuole solo andata.', false, 'The traveller wants a one-way ticket.'),
        tf('Il biglietto costa 24 euro.', true, 'The ticket costs 24 euros.'),
        tf('Il treno parte dal binario 3.', true, 'The train leaves from platform 3.'),
        tf('Il signor Singh ha una camera singola.', false, 'Mr Singh has a single room.'),
        tf('La colazione è inclusa.', true, 'Breakfast is included.'),
        wo('Da quale binario parte il treno', 'Which platform does the train leave from'),
        wo('Ho una prenotazione per due notti', 'I have a booking for two nights'),
      ],
      homework: 'Role-play: scenario A (buy a ticket to Bologna) or scenario B (book a room for three nights).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 3 · Yesterday\'s trip: AVERE + ESSERE together',
      titleIt: 'Lezione 3 · Il viaggio di ieri al passato prossimo',
      summary: 'Ho comprato il biglietto, sono partito, sono arrivato, ho aperto la valigia, mi sono riposato.',
      minutes: 10,
      sections: [
        {
          title: 'Il viaggio in 3 momenti',
          table: [
            ['Momento', 'Ausiliare', 'Esempio'],
            ['1. Partenza', 'AVERE + ESSERE', 'Ho comprato il biglietto, poi sono partito/a dal binario 4.'],
            ['2. Arrivo', 'ESSERE', 'Sono arrivato/a in hotel. Il treno è arrivato in orario.'],
            ['3. In camera', 'AVERE + riflessivo', 'Ho aperto la valigia. Infine mi sono riposato/a.'],
          ],
        },
        {
          title: 'Role-play a coppie',
          text: '- **Scenario A · In stazione** (bigliettaio e viaggiatore): chiedi un biglietto per Bologna, l\'orario e il binario.\n- **Scenario B · In albergo** (receptionist e ospite): chiedi una camera singola o doppia per tre notti.',
        },
      ],
      exercises: [
        fb('Ieri ___ comprato il biglietto in biglietteria.', ['ho', 'sono', 'mi sono', 'è']),
        fb('Poi ___ partita dal binario 4.', ['sono', 'ho', 'mi sono', 'hanno']),
        fb('Il treno ___ arrivato in orario.', ['è', 'ha', 'sono', 'hanno']),
        fb('In camera ___ aperto la valigia.', ['ho', 'sono', 'mi sono', 'è']),
        fb('Infine ___ riposato.', ['mi sono', 'si è', 'sono', 'mi ho']),
        fb('I miei genitori ___ arrivati a Malpensa.', ['sono', 'hanno', 'è', 'ha']),
      ],
      homework: 'Write your last trip in 6 sentences: departure, arrival and in the room.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Raccontare un viaggio o l'arrivo in Italia
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Review',
      title: 'Lesson 4 · Your arrival in Italy: 5 questions',
      titleIt: 'Lezione 4 · Il tuo arrivo in Italia: 5 domande',
      summary: 'Quando sei arrivato/a? Come sei venuto/a? — and sono partito vs sono partita.',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 9 · Lezione 4 (PDF)' },
        {
          title: '5 domande per raccontare il tuo arrivo',
          table: [
            ['Domanda (tu)', 'Risposta modello (io)'],
            ['1. Quando sei arrivato/a in Italia?', 'Sono arrivato/a due anni fa / nel 2024.'],
            ['2. Come sei venuto/a?', 'Sono venuto/a in aereo da Amritsar / Delhi.'],
            ['3. Con chi sei partito/a?', 'Sono partito/a da solo/a / con la famiglia.'],
            ['4. Dove sei stato/a all\'inizio?', 'All\'inizio sono stato/a a Brescia / Mantova.'],
            ['5. Come ti sei sentito/a il primo giorno?', 'Mi sono sentito/a emozionato/a e felice!'],
          ],
        },
        {
          title: 'Io parlo di me: -O per un uomo, -A per una donna',
          table: [
            ['Parla un uomo', 'Parla una donna'],
            ['Sono nato in Punjab.', 'Sono nata in Punjab.'],
            ['Sono arrivato da solo.', 'Sono arrivata con mio marito.'],
            ['Mi sono trovato subito bene.', 'Mi sono trovata subito bene.'],
          ],
        },
      ],
      exercises: [
        fb('(Kamal) Sono ___ in India.', ['nato', 'nata', 'nati', 'nate']),
        fb('(Simran) Sono ___ in Italia con mio marito.', ['arrivata', 'arrivato', 'arrivati', 'arrivate']),
        fb('(Harpreet, uomo) Sono ___ da solo.', ['venuto', 'venuta', 'venuti', 'venite']),
        fb('(Kiran, donna) Mi sono ___ subito bene.', ['trovata', 'trovato', 'trovati', 'trovate']),
        fb('Quando sei ___ in Italia, Gurpreet?', ['arrivato', 'arrivata', 'arrivate', 'arrivare']),
        mc('"Come ti sei sentito il primo giorno?" means…', 'How did you feel on the first day?', 'Who did you meet on the first day?', 'Where did you sleep on the first day?', 'What did you hear on the first day?'),
      ],
      homework: 'Answer the 5 questions about yourself in full sentences.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · Model stories: arriving in Italy and a day in Verona',
      titleIt: 'Lezione 4 · Testi modello: l\'arrivo in Italia e una gita a Verona',
      summary: 'Read Kamal\'s arrival story and a Sunday trip to Verona — then end your story well.',
      minutes: 12,
      sections: [
        {
          title: 'Testo modello 1: il mio arrivo in Italia',
          reading: {
            title: 'Il mio arrivo in Italia',
            it: 'Mi chiamo Kamal e sono nato in India. Sono arrivato in Italia tre anni fa, nel mese di ottobre.\n\nPrima sono partito da Nuova Delhi in aereo e sono atterrato a Milano Malpensa. Poi mio fratello è venuto a prendermi in macchina e siamo andati a Mantova.\n\nAll\'inizio sono stato un po\' timido per la lingua, ma oggi ho imparato l\'italiano e mi sono integrato bene!',
            en: 'My name is Kamal and I was born in India. I arrived in Italy three years ago, in October.\n\nFirst I left New Delhi by plane and landed at Milano Malpensa. Then my brother came to pick me up by car and we went to Mantova.\n\nAt first I was a bit shy because of the language, but today I have learnt Italian and I have settled in well!',
          },
        },
        {
          title: 'Testo modello 2: la nostra domenica a Verona',
          reading: {
            title: 'La nostra domenica a Verona',
            it: 'Domenica scorsa io e mia moglie ci siamo svegliati presto e siamo andati in stazione.\n\nAbbiamo comprato due biglietti di andata e ritorno e siamo partiti alle nove. Siamo arrivati a Verona dopo quaranta minuti.\n\nAbbiamo visitato Piazza Bra, abbiamo fatto tante foto e siamo tornati a casa alle sette di sera. Ci siamo divertiti tantissimo!',
            en: 'Last Sunday my wife and I woke up early and went to the station.\n\nWe bought two return tickets and left at nine. We arrived in Verona after forty minutes.\n\nWe visited Piazza Bra, took lots of photos and came back home at seven in the evening. We had a great time!',
          },
        },
        {
          title: '4 espressioni per concludere il racconto',
          words: [
            wp('È stato bellissimo!', 'It was wonderful!', 'Bahut sohṇā sī!', { img: 'Star-struck' }),
            wp('Ci siamo divertiti!', 'We had fun!', 'Asīṁ bahut mazā kītā!', { img: 'Party popper' }),
            wp('Mi sono trovato/a bene.', 'I felt comfortable / I got on well.', 'Mainūṁ changā lagiā.', { img: 'Relieved face' }),
            wp('È stato un viaggio lungo.', 'It was a long journey.', 'Eh lammā safar sī.', { img: 'Tired face' }),
          ],
        },
      ],
      exercises: [
        tf('Kamal è arrivato in Italia tre anni fa.', true, 'Kamal arrived in Italy three years ago.'),
        tf('Kamal è atterrato a Roma.', false, 'Kamal landed in Rome.'),
        tf('Il fratello è venuto a prenderlo in macchina.', true, 'His brother came to pick him up by car.'),
        tf('Sono partiti per Verona alle sette.', false, 'They left for Verona at seven.'),
        tf('A Verona hanno visitato Piazza Bra.', true, 'In Verona they visited Piazza Bra.'),
        wo('Siamo arrivati a Verona dopo quaranta minuti', 'We arrived in Verona after forty minutes'),
      ],
      homework: 'Write your own story (6–8 sentences) about your arrival in Italy or a trip. Use avere, essere and prima, poi, dopo, infine.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · Chapter 9 practice',
      titleIt: 'Lezione 4 · Esercizi di ripasso del Capitolo 9',
      summary: 'Mixed practice: essere and agreement, avere or essere, reflexives, travel words.',
      minutes: 10,
      sections: [
        {
          title: 'Ripasso lampo',
          text: '- **ESSERE** with movement / change: andare, venire, partire, arrivare, tornare, entrare, uscire, stare, rimanere, nascere\n- **Agreement:** -o · -a · -i · -e\n- **Test:** che cosa? → avere · dove? → essere\n- **Reflexives:** always essere (mi sono svegliato/a)\n- **Travel:** biglietteria, binario, andata e ritorno, ritardo, valigia, prenotazione, camera singola / doppia',
        },
      ],
      exercises: [
        fb('Ieri sera Sara ___ a casa alle nove.', ['è tornata', 'è tornato', 'ha tornato', 'ha tornata']),
        fb('L\'anno scorso io e mio fratello ___ in Italia.', ['siamo venuti', 'abbiamo venuto', 'siamo venute', 'sono venuti']),
        fb('Le ragazze ___ in albergo perché pioveva.', ['sono rimaste', 'sono rimasti', 'hanno rimasto', 'è rimasta']),
        fb('Ho comprato il biglietto in ___.', ['biglietteria', 'binario', 'reception', 'valigia']),
        fb('L\'aereo parte dall\'___.', ['aeroporto', 'albergo', 'binario', 'ufficio']),
        fb('Faccio il check-in alla ___.', ['reception', 'biglietteria', 'coincidenza', 'stazione']),
        fb('In hotel serve un ___ d\'identità.', ['documento', 'binario', 'ritardo', 'volo']),
      ],
      homework: 'Get ready for the test: tell your arrival in Italy aloud in 1 minute.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 9
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 9 test',
      titleIt: 'Test Capitolo 9',
      summary: 'Listening, travel words, essere and agreement, avere or essere, reflexives in the past. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 9',
          text: 'This test checks everything in Chapter 9 (Lessons 1–4).\n\n- **1. Listening** — sentences and a travel story\n- **2. Vocabulary** — station, airport and hotel\n- **3. Grammar** — essere with agreement, avere or essere, reflexive verbs in the past\n- **4. Writing** — your arrival in Italy or a trip\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (your arrival in Italy or a trip) and the **oral interview** (your arrival, at the ticket office or hotel, reflexive verbs in the past — 2 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A
        lis('1A · Listen and choose the sentence you hear', 'Stamattina Maria è partita alle otto in treno per Milano.', 'MARIA È PARTITA ALLE OTTO IN TRENO', 'MARCO È ARRIVATO IN RITARDO ALLA STAZIONE'),
        lis('1A · Listen and choose the sentence you hear', 'Ieri sera ci siamo divertiti molto alla festa di compleanno.', 'CI SIAMO DIVERTITI MOLTO ALLA FESTA', 'MI SONO SVEGLIATA PRESTO STAMATTINA'),
        lis('1A · Listen and choose the sentence you hear', 'In biglietteria abbiamo comprato due biglietti di andata e ritorno.', 'ABBIAMO COMPRATO DUE BIGLIETTI DI ANDATA E RITORNO', 'HO PRENOTATO UNA CAMERA SINGOLA'),
        lis('1A · Listen and choose the sentence you hear', 'Attenzione: il treno parte dal binario quattro tra cinque minuti.', 'IL TRENO PARTE DAL BINARIO QUATTRO', 'LA VALIGIA È RIMASTA IN AEROPORTO'),
        lis('1A · Listen and choose the sentence you hear', 'Ciao, mi chiamo Singh e sono venuto in Italia due anni fa.', 'SONO VENUTO IN ITALIA DUE ANNI FA', 'SIAMO TORNATI A CASA IERI SERA'),
        // 1B
        lis('1B · Listen to the story: "io e mia moglie siamo ___ a Roma" (1)', 'Domenica scorsa io e mia moglie siamo andati a Roma in treno.', 'ANDATI', 'RIMASTI', 'NATI', 'USCITI'),
        lis('1B · Listen again: the train had ten minutes of… (2)', 'Alla stazione il treno aveva dieci minuti di ritardo.', 'RITARDO', 'ORARIO', 'BINARIO', 'VIAGGIO'),
        lis('1B · Listen again: what room did they take? (3)', 'In albergo abbiamo preso una camera doppia con colazione inclusa.', 'DOPPIA', 'SINGOLA', 'GRANDE', 'PICCOLA'),
        lis('1B · Listen again: "Il pomeriggio ___ stati al Colosseo" (4)', 'Il pomeriggio siamo stati al Colosseo.', 'SIAMO', 'ABBIAMO', 'SONO', 'SIETE'),
        lis('1B · Listen again: "la sera ci siamo ___ molto" (5)', 'La sera ci siamo divertiti molto insieme.', 'DIVERTITI', 'SVEGLIATI', 'ALZATI', 'VESTITI'),
        // 2A
        pairs('2A · Match each word with its meaning', [
          ['Biglietto di andata e ritorno', 'Return ticket · Āuṇ-jāṇ dī ṭikaṭ'],
          ['Binario', 'Platform · Pleṭfārm'],
          ['Valigia', 'Suitcase · Sūṭkes'],
          ['Prenotazione', 'Booking · Bukiṅg'],
          ['Camera singola / doppia', 'Single / double room · Ikk / do bandiāṁ dā kamrā'],
        ]),
        // 2B
        mc('2B · Compro il biglietto in ?', 'biglietteria', 'aeroporto', 'reception', 'ritardo'),
        mc('2B · L\'aereo parte dall\'?', 'aeroporto', 'biglietteria', 'reception', 'documento'),
        mc('2B · Faccio il check-in alla ?', 'reception', 'biglietteria', 'ritardo', 'aeroporto'),
        mc('2B · Il treno non è in orario, ha ?', 'ritardo', 'documento', 'reception', 'biglietteria'),
        mc('2B · In hotel serve un ? d\'identità.', 'documento', 'ritardo', 'aeroporto', 'biglietteria'),
        // 3A
        fb('3A · Ieri sera Sara ___ (tornare) a casa alle nove.', ['è tornata', 'è tornato', 'ha tornato', 'sono tornate']),
        fb('3A · L\'anno scorso io e mio fratello ___ (venire) in Italia.', ['siamo venuti', 'abbiamo venuto', 'siamo venute', 'sono venuti']),
        fb('3A · Le ragazze ___ (rimanere) in albergo perché pioveva.', ['sono rimaste', 'sono rimasti', 'hanno rimasto', 'sono rimanute']),
        // 3B
        fb('3B · Stamattina noi ___ (partire) alle sette dalla stazione.', ['siamo partiti', 'abbiamo partito', 'siamo partito', 'abbiamo partiti']),
        fb('3B · Al bar Marco ___ (prendere) due cappuccini e un cornetto.', ['ha preso', 'è preso', 'ha prenduto', 'è presa']),
        fb('3B · Dove ___ (nascere) tua madre? In India.', ['è nata', 'ha nato', 'è nato', 'ha nata']),
        fb('3B · Ieri tu ___ (comprare) i biglietti per il treno?', ['hai comprato', 'sei comprato', 'hai comprata', 'sei comprata']),
        fb('3B · Giulia ___ (uscire) con le sue amiche sabato sera.', ['è uscita', 'ha uscito', 'è uscito', 'ha uscita']),
        // 3C
        fb('3C · Stamattina Nadia ___ alle sei e mezza.', ['si è svegliata', 'ha svegliato']),
        fb('3C · Ieri sera io e Paolo ___ molto alla festa.', ['ci siamo divertiti', 'siamo divertito']),
        fb('3C · A che ora ___ ieri notte, Luca?', ['ti sei addormentato', 'sei ti addormentato']),
        // 4
        wo('Sono arrivato in Italia due anni fa', 'I arrived in Italy two years ago'),
        wo('Ho viaggiato in aereo con mio fratello', 'I travelled by plane with my brother'),
        wo('All\'inizio mi sono sentita un po\' timida', 'At first I felt a bit shy'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences about your arrival in Italy or a recent trip. Then prepare the 2-minute oral interview.',
    },
  ],
};
