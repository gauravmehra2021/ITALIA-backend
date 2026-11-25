import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 5 of the school syllabus (Corso di Italiano, livello A0 → A1).
// Source: syllabus/Capitolo 5 Lezione 1–4 (.pptx) and Test Capitolo 5 (.docx).
// Note: the teacher's key of Test Capitolo 5, exercise 2B, swaps armadio and lavatrice;
// we follow the lesson slide (armadio → camera da letto, lavatrice → bagno / cucina).

const slides = (lez) => `/content/slides/corso-a0-cap5-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-5',
  title: 'Chapter 5: The city, directions and the home',
  titleIt: 'Capitolo 5: La città, le indicazioni e la casa',
  cover: 'Round pushpin',
  description: 'Places in town and public transport, andare and venire with in / a, asking for and giving directions, saying where things are (vicino a, di fronte a…), the rooms and furniture of the home, and the simple and articulated prepositions (al, dal, nel, sul…).',
  canDo: [
    'I can name places in town and say how I get there (in autobus, a piedi…)',
    'I can use andare and venire with in and a',
    'I can ask for directions and understand the answer',
    'I can say where a place is: vicino a, lontano da, di fronte a, accanto a, tra',
    'I can describe my home: rooms and furniture',
    'I can use the simple and articulated prepositions: a casa, dal medico, sul tavolo, nel frigo',
  ],
  liveClassFocus: 'Map role-play: guide a partner from the station to the pharmacy, then describe your home (rooms and furniture) and where your things are.',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — I luoghi della città e i trasporti
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · Places in town',
      titleIt: 'Lezione 1 · I luoghi della città',
      summary: 'La stazione, la farmacia, la banca, la posta, la piazza, il parco…',
      minutes: 10,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 5 · Lezione 1 (PDF)' },
        {
          title: 'I luoghi della città: servizi',
          words: [
            wp('la stazione', 'train station', 'Relve steshan', { pron: 'lah stah-tsee-OH-neh', img: 'Station' }),
            wp('la fermata dell\'autobus', 'bus stop', 'Bass adda', { pron: 'lah fehr-MAH-tah del-LOW-toh-boos', img: 'Bus stop' }),
            wp('la farmacia', 'pharmacy', 'Davaiaan di dukan', { pron: 'lah fahr-mah-CHEE-ah', img: 'Pill' }),
            wp('l\'ospedale', 'hospital', 'Haspatal', { pron: 'los-peh-DAH-leh', img: 'Hospital' }),
            wp('la banca', 'bank', 'Baink', { pron: 'lah BAHN-kah', img: 'Bank' }),
            wp('la posta', 'post office', 'Daakkhana', { pron: 'lah POS-tah', img: 'Post office' }),
            wp('il supermercato', 'supermarket', 'Suparmarkit', { pron: 'eel soo-pehr-mehr-KAH-toh', img: 'Shopping cart' }),
          ],
        },
        {
          title: 'Spazi aperti',
          words: [
            wp('la piazza', 'square', 'Chaunk', { pron: 'lah pee-AHT-tsah', img: 'Fountain', ex: 'La fermata dell\'autobus è in piazza.', exEn: 'The bus stop is in the square.' }),
            wp('il parco', 'park', 'Paarak', { pron: 'eel PAHR-koh', img: 'Deciduous tree', ex: 'La domenica porto i bambini al parco.', exEn: 'On Sunday I take the children to the park.' }),
            wp('il centro', 'town centre', 'Shahir da kendar', { pron: 'eel CHEN-troh', img: 'Cityscape' }),
          ],
        },
        { tip: '**La piazza** is the heart of every Italian town: you find the bar, shops, the market and often the bus stop there.' },
      ],
      exercises: [
        mc('You need medicine. Where do you go?', 'in farmacia', 'in banca', 'in posta', 'al parco'),
        mc('You want to send a letter. Where do you go?', 'in posta', 'in farmacia', 'in stazione', 'al supermercato'),
        mc('You want to take the train. Where do you go?', 'in stazione', 'in ospedale', 'in piazza', 'in banca'),
      ],
      homework: 'Write the 5 places in your town that you go to most often.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · Getting around: transport',
      titleIt: 'Lezione 1 · I mezzi di trasporto',
      summary: 'L\'autobus, il treno, il tram, la metro, la macchina, la bici — and a piedi.',
      minutes: 8,
      sections: [
        {
          title: 'I trasporti pubblici',
          words: [
            wp('l\'autobus', 'bus', 'Bass', { pron: 'LOW-toh-boos', img: 'Bus' }),
            wp('il treno', 'train', 'Rel gaddi', { pron: 'eel TREH-noh', img: 'High-speed train' }),
            wp('il tram', 'tram', 'Traam', { pron: 'eel TRAHM', img: 'Tram' }),
            wp('la metropolitana (la metro)', 'underground / metro', 'Metro', { pron: 'lah meh-troh-poh-lee-TAH-nah', img: 'Metro' }),
          ],
        },
        {
          title: 'Mezzi privati e a piedi',
          words: [
            wp('la macchina (l\'auto)', 'car', 'Kaar / Gaddi', { pron: 'lah MAHK-kee-nah', img: 'Automobile' }),
            wp('la bicicletta (la bici)', 'bicycle', 'Saikil', { pron: 'lah bee-chee-KLET-tah', img: 'Bicycle' }),
            wp('a piedi', 'on foot', 'Paidal', { pron: 'ah pee-EH-dee', img: 'Person walking' }),
          ],
        },
        {
          title: 'Examples',
          practice: false,
          words: [
            wp('Prendo il treno in stazione alle otto.', 'I take the train at the station at eight.', 'Main atth vaje steshan ton tren lainda haan.'),
            wp('Vado al lavoro in bici o a piedi.', 'I go to work by bike or on foot.', 'Main saikil te jaan paidal kamm te jaanda haan.'),
          ],
        },
        { tip: 'With a vehicle use **in**: *in autobus, in treno, in macchina, in bici*. Without a vehicle: **a piedi**.' },
      ],
      exercises: [
        fb('Vado al lavoro ___ autobus.', ['in', 'a', 'con il', 'per']),
        fb('Vado a scuola ___ piedi.', ['a', 'in', 'con', 'per']),
        tr('la macchina', 'car', 'bus', 'train', 'bicycle'),
      ],
      homework: 'Write how you go to work, to school and to the supermarket (in autobus, a piedi…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Andare and venire with in / a',
      titleIt: 'Lezione 1 · Andare, venire e le preposizioni in / a',
      summary: 'Vado in banca, vado al parco, vieni con me? — with a dialogue.',
      minutes: 12,
      sections: [
        {
          title: 'ANDARE e VENIRE (irregolari)',
          table: [
            ['Soggetto', 'ANDARE (go · jaana)', 'VENIRE (come · auna)', 'Esempio'],
            ['io', 'vado', 'vengo', 'Io vado in banca; vengo a casa alle sei.'],
            ['tu', 'vai', 'vieni', 'Dove vai adesso? Vieni al parco con me?'],
            ['lui / lei', 'va', 'viene', 'Lei va in farmacia; lui viene in stazione.'],
            ['noi', 'andiamo', 'veniamo', 'Noi andiamo al supermercato insieme.'],
            ['voi', 'andate', 'venite', 'A che ora andate in posta oggi?'],
            ['loro', 'vanno', 'vengono', 'Loro vanno in ospedale in autobus.'],
          ],
        },
        {
          title: '3 regole d\'oro: IN, AL / ALLA e A PIEDI',
          table: [
            ['IN + place (no article)', 'A + article (AL / ALLA)', 'IN + vehicle'],
            ['in banca, in posta, in farmacia', 'al supermercato, al parco, al bar', 'in autobus, in treno, in macchina'],
            ['in ospedale, in stazione, in piazza, in città', 'alla fermata dell\'autobus, a casa', 'in bicicletta, in metro — but A PIEDI!'],
          ],
        },
        { text: '**ANDARE** = movement towards another place. **VENIRE** = movement towards the person who speaks or listens: *Vieni con me?* (Are you coming with me?)' },
        {
          title: 'Dialogo: Dove vai stamattina?',
          situation: 'Marco meets Sara in the street.',
          dialogue: [
            lp('Marco', 'Ciao Sara! Dove vai così di fretta?', 'Hi Sara! Where are you going in such a hurry?', 'Sat Sri Akal Sara! Ini kaahli kitthe ja rahi hain?'),
            lp('Sara', 'Ciao Marco! Vado in posta e poi in farmacia. Tu dove vai?', 'Hi Marco! I\'m going to the post office and then to the pharmacy. Where are you going?', 'Main daakkhane ate phir davaiaan di dukan ja rahi haan. Tu kitthe ja riha hain?'),
            lp('Marco', 'Io vado al supermercato. Ci vai a piedi o in autobus?', 'I\'m going to the supermarket. Are you going on foot or by bus?', 'Main suparmarkit ja riha haan. Paidal ja rahi hain jaan bass raahi?'),
            lp('Sara', 'Vado in autobus: la fermata è lì in piazza. Vieni con me?', 'By bus: the stop is there in the square. Are you coming with me?', 'Bass raahi: adda othe chaunk vich hai. Mere naal chalega?'),
          ],
        },
      ],
      exercises: [
        fb('Stamattina io ___ in farmacia.', ['vado', 'vai', 'va', 'vanno']),
        fb('Tu ___ al parco con noi?', ['vieni', 'vengo', 'viene', 'venite']),
        fb('Marco e Sara ___ a Milano in treno.', ['vanno', 'va', 'andiamo', 'andate']),
        fb('Noi ___ a scuola in bicicletta.', ['veniamo', 'venite', 'vengono', 'vengo']),
        fb('Vado ___ banca.', ['in', 'al', 'alla', 'a']),
        fb('Vado ___ supermercato.', ['al', 'in', 'alla', 'a']),
        tf('Sara va in posta e in farmacia.', true, 'Sara is going to the post office and the pharmacy.'),
        tf('Sara va a piedi.', false, 'Sara is going on foot.'),
      ],
      homework: 'Role-play: ask a partner "Dove vai oggi? Come ci vai?" and answer with two places and a means of transport.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — Chiedere e dare indicazioni
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 2 · Asking for directions',
      titleIt: 'Lezione 2 · Chiedere la strada',
      summary: 'Scusi, dov\'è…? Sempre dritto, a destra, a sinistra, attraversa, all\'incrocio…',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 5 · Lezione 2 (PDF)' },
        {
          title: 'Chiedere la strada e le direzioni',
          words: [
            wp('Scusi, dov\'è…?', 'Excuse me, where is…?', 'Maaf karna, … kitthe hai?', { pron: 'SKOO-zee doh-VEH', img: 'Person tipping hand' }),
            wp('Per andare a…?', 'How do I get to…?', '… kiven jaana hai?', { img: 'World map', pron: 'pehr ahn-DAH-reh ah' }),
            wp('Vai (Vada) sempre dritto.', 'Go straight ahead.', 'Sidha jao.', { pron: 'VAH-ee SEM-preh DREET-toh', img: 'Up arrow' }),
            wp('Gira (Giri) a destra.', 'Turn right.', 'Sajje paase mudo.', { pron: 'JEE-rah ah DES-trah', img: 'Right arrow' }),
            wp('Gira (Giri) a sinistra.', 'Turn left.', 'Khabbe paase mudo.', { pron: 'JEE-rah ah see-NEES-trah', img: 'Left arrow' }),
          ],
        },
        {
          title: 'Attraversare e orientarsi',
          words: [
            wp('Attraversa la strada.', 'Cross the road.', 'Sadak paar karo.', { pron: 'aht-trah-VEHR-sah', img: 'Person walking', ex: 'Attraversa la piazza sulle strisce.', exEn: 'Cross the square on the zebra crossing.' }),
            wp('all\'incrocio', 'at the crossroads', 'Chaurahe te', { pron: 'leen-KROH-choh', img: 'Motorway', ex: 'All\'incrocio gira a destra in Via Roma.', exEn: 'At the crossroads turn right into Via Roma.' }),
            wp('Prendi la prima / seconda strada.', 'Take the first / second street.', 'Pahili / duji gali lao.', { img: 'Motorway', ex: 'Prendi la prima strada a sinistra.', exEn: 'Take the first street on the left.' }),
            wp('il semaforo', 'traffic lights', 'Traifik lait', { pron: 'eel seh-MAH-foh-roh', img: 'Vertical traffic light' }),
          ],
        },
        { tip: '**Vai, gira, attraversa** are for a friend (tu). With a stranger use the polite form: **Vada** sempre dritto, **giri** a destra, **attraversi** la strada.' },
      ],
      exercises: [
        mc('"Gira a destra" means…', 'Turn right', 'Turn left', 'Go straight on', 'Cross the road'),
        mc('"Sempre dritto" means…', 'Straight ahead', 'Always right', 'Turn back', 'At the corner'),
        mc('You ask a stranger the way. You start with…', 'Scusi, dov\'è…?', 'Ciao, dove vai?', 'Che ore sono?', 'Quanto costa?'),
      ],
      homework: 'Write how to go from your home to the nearest bus stop (vai sempre dritto, gira a…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Where is it? Vicino a, di fronte a…',
      titleIt: 'Lezione 2 · Le preposizioni di posizione',
      summary: 'Vicino a, lontano da, di fronte a, accanto a, tra… e…',
      minutes: 12,
      sections: [
        {
          title: 'Dove si trova?',
          table: [
            ['Espressione', '+ articolo', 'English / Punjabi', 'Esempio'],
            ['VICINO A', 'vicino al / alla / all\'', 'near · de nede', 'La farmacia è vicino alla banca.'],
            ['LONTANO DA', 'lontano dal / dalla / dall\'', 'far from · ton door', 'La stazione è lontano dal parco.'],
            ['DI FRONTE A', 'di fronte al / alla / all\'', 'opposite · de saahmane', 'La posta è di fronte al supermercato.'],
            ['ACCANTO A', 'accanto al / alla / all\'', 'next to · de bilkul naal', 'Il bar è accanto alla fermata dell\'autobus.'],
            ['TRA / IN MEZZO A', 'tra … e …', 'between · de vichkar', 'La banca è tra la stazione e la posta.'],
          ],
        },
        {
          title: 'Frasi utili in città',
          practice: false,
          words: [
            wp('vicino a', 'near (2 minutes on foot)', 'De nede', { ex: 'Il supermercato è vicino al parco.', exEn: 'The supermarket is near the park.' }),
            wp('lontano da', 'far from (you need the bus)', 'Ton door', { ex: 'Casa mia è lontano dal centro.', exEn: 'My home is far from the centre.' }),
            wp('di fronte a', 'opposite (other side of the road)', 'De saahmane', { ex: 'La banca è di fronte alla piazza centrale.', exEn: 'The bank is opposite the main square.' }),
            wp('accanto a', 'next to (right beside)', 'De bilkul naal', { ex: 'La posta è accanto alla banca.', exEn: 'The post office is next to the bank.' }),
          ],
        },
        { tip: '**A** and **DA** join the article: vicino **al** parco (a + il), vicino **alla** banca (a + la), lontano **dal** centro (da + il), lontano **dalla** stazione (da + la).' },
      ],
      exercises: [
        fb('La farmacia è vicino ___ banca.', ['alla', 'al', 'dalla', 'della']),
        fb('La stazione è lontano ___ parco.', ['dal', 'al', 'del', 'nel']),
        fb('La posta è di fronte ___ supermercato.', ['al', 'alla', 'dal', 'del']),
        fb('Il bar è accanto ___ fermata dell\'autobus.', ['alla', 'al', 'dalla', 'nella']),
        fb('La banca è ___ la stazione e la posta.', ['tra', 'di fronte', 'vicino', 'accanto']),
        mc('"Di fronte a" means…', 'opposite', 'next to', 'far from', 'between'),
        mc('"Accanto a" means…', 'next to', 'opposite', 'far from', 'behind'),
        wo('La posta è accanto alla banca', 'The post office is next to the bank'),
      ],
      homework: 'Describe where 3 places are near your home (Il supermercato è vicino a…, La farmacia è di fronte a…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · Dialogue: How do I get to the pharmacy?',
      titleIt: 'Lezione 2 · Dialogo: Scusi, per andare in farmacia?',
      summary: 'Directions in the street and reading a simple town map.',
      minutes: 12,
      sections: [
        {
          title: 'Dialogo: Scusi, per andare in farmacia?',
          situation: 'A person asks a passer-by the way to the pharmacy.',
          dialogue: [
            lp('Passante 1', 'Scusi, per andare alla farmacia centrale? È lontano da qui?', 'Excuse me, how do I get to the main pharmacy? Is it far from here?', 'Maaf karna, vaddi davaiaan di dukan kiven jaana hai? Ki ithon door hai?'),
            lp('Passante 2', 'No, è molto vicino! Vai sempre dritto fino all\'incrocio con Via Roma.', 'No, it\'s very close! Go straight on until the crossroads with Via Roma.', 'Nahi, bahut nede hai! Via Roma de chaurahe tak sidha jao.'),
            lp('Passante 2', 'All\'incrocio gira a sinistra e attraversa Piazza Centrale.', 'At the crossroads turn left and cross the main square.', 'Chaurahe te khabbe mudo ate chaunk paar karo.'),
            lp('Passante 2', 'La farmacia è lì, di fronte alla posta e accanto al bar!', 'The pharmacy is right there, opposite the post office and next to the café!', 'Davaiaan di dukan othe hai, daakkhane de saahmane ate baar de naal!'),
          ],
        },
        {
          title: 'Orientarsi sulla piantina (the town map)',
          text: 'On the map: **Corso Italia** goes north–south, **Via Roma** and **Via Verdi** cross it.\n\n- **Dalla stazione al bar:** vai sempre dritto in Corso Italia — il bar è di fronte alla Piazza Centrale.\n- **Dove si trova la posta?** La posta è accanto alla banca e di fronte alla farmacia.\n- **Dal parco all\'ospedale:** attraversa Piazza Centrale, vai in Via Verdi e gira a destra — è lontano dalla stazione.',
        },
        {
          title: 'Role-play sulla mappa: guidami in città!',
          text: '1. **Studente A chiede:** «Scusi, per andare dalla stazione all\'ospedale / al supermercato?»\n2. **Studente B guida:** vai sempre dritto, all\'incrocio, gira a destra / sinistra, attraversa.\n3. **Indica la posizione:** vicino a, lontano da, di fronte a, accanto a — poi cambiate ruolo!',
        },
      ],
      exercises: [
        tf('La farmacia è lontano.', false, 'The pharmacy is far.'),
        tf('All\'incrocio deve girare a sinistra.', true, 'At the crossroads he must turn left.'),
        tf('La farmacia è accanto al bar.', true, 'The pharmacy is next to the café.'),
        tf('La farmacia è di fronte alla stazione.', false, 'The pharmacy is opposite the station.'),
        wo('Vai sempre dritto fino all\'incrocio', 'Go straight on until the crossroads'),
        wo('Gira a sinistra e attraversa la piazza', 'Turn left and cross the square'),
      ],
      homework: 'Role-play on the map with a partner: give directions from the station to the hospital, then change roles.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — La casa: stanze e arredamento
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · The rooms of the house',
      titleIt: 'Lezione 3 · Le stanze della casa',
      summary: 'L\'ingresso, il soggiorno, la cucina, la camera da letto, il bagno, il balcone…',
      minutes: 10,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 5 · Lezione 3 (PDF)' },
        {
          title: 'Le stanze della casa',
          words: [
            wp('l\'ingresso', 'entrance / hallway', 'Pravesh duar', { pron: 'leen-GRES-soh', img: 'Door' }),
            wp('il soggiorno', 'living room', 'Baithak / Living room', { pron: 'eel sod-JOR-noh', img: 'Couch and lamp' }),
            wp('la cucina', 'kitchen', 'Rasoi', { pron: 'lah koo-CHEE-nah', img: 'Cooking' }),
            wp('la camera da letto', 'bedroom', 'Saun vaala kamra', { pron: 'lah KAH-meh-rah dah LET-toh', img: 'Bed' }),
            wp('il bagno', 'bathroom', 'Baathrum', { pron: 'eel BAHN-yoh', img: 'Bathtub' }),
            wp('il balcone', 'balcony', 'Baalkani', { pron: 'eel bahl-KOH-neh', img: 'Potted plant' }),
            wp('il giardino', 'garden', 'Bagicha', { pron: 'eel jahr-DEE-noh', img: 'House with garden' }),
            wp('l\'appartamento', 'apartment / flat', 'Flait', { pron: 'lahp-pahr-tah-MEN-toh', img: 'Office building' }),
          ],
        },
        {
          title: 'Examples',
          practice: false,
          words: [
            wp('In casa mia ci sono due camere da letto.', 'In my home there are two bedrooms.', 'Mere ghar vich do kamre han.'),
            wp('Il bagno è accanto alla camera da letto.', 'The bathroom is next to the bedroom.', 'Baathrum kamre de naal hai.'),
            wp('Abito in un appartamento al secondo piano.', 'I live in a flat on the second floor.', 'Main duji manzil te flait vich rahinda haan.'),
          ],
        },
      ],
      exercises: [
        mc('Where do you sleep?', 'nella camera da letto', 'in cucina', 'nell\'ingresso', 'sul balcone'),
        mc('Where do you cook?', 'in cucina', 'in bagno', 'in soggiorno', 'in camera da letto'),
        mc('Where do you have a shower?', 'in bagno', 'in cucina', 'nell\'ingresso', 'in soggiorno'),
      ],
      homework: 'Write the rooms of your home in Italian.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · Furniture and appliances',
      titleIt: 'Lezione 3 · I mobili e gli elettrodomestici',
      summary: 'Il letto, l\'armadio, il tavolo, la sedia, il divano, il frigo, la lavatrice.',
      minutes: 10,
      sections: [
        {
          title: 'I mobili principali della casa',
          words: [
            wp('il letto', 'bed', 'Baid / Manjha', { pron: 'eel LET-toh', img: 'Bed', ex: 'Nella camera da letto.', exEn: 'In the bedroom.' }),
            wp('l\'armadio', 'wardrobe', 'Almari', { img: 'Door', pron: 'lahr-MAH-dee-oh', ex: 'Nella camera da letto.', exEn: 'In the bedroom.' }),
            wp('il tavolo', 'table', 'Mez', { img: 'Fork and knife with plate', pron: 'eel TAH-voh-loh', ex: 'In cucina / in soggiorno.', exEn: 'In the kitchen / living room.' }),
            wp('la sedia', 'chair', 'Kursi', { pron: 'lah SEH-dee-ah', img: 'Chair', ex: 'In cucina / in soggiorno.', exEn: 'In the kitchen / living room.' }),
            wp('il divano', 'sofa', 'Sofa', { pron: 'eel dee-VAH-noh', img: 'Couch and lamp', ex: 'In soggiorno.', exEn: 'In the living room.' }),
            wp('il frigo (frigorifero)', 'fridge', 'Frij', { img: 'Ice', pron: 'eel FREE-goh', ex: 'In cucina.', exEn: 'In the kitchen.' }),
            wp('la lavatrice', 'washing machine', 'Kapde dhon vaali mashin', { img: 'Bubbles', pron: 'lah lah-vah-TREE-cheh', ex: 'In bagno / in cucina.', exEn: 'In the bathroom / kitchen.' }),
          ],
        },
        {
          title: 'Singolare e plurale',
          table: [
            ['Singolare', 'Plurale', 'Dove si trova?'],
            ['il letto', 'i letti', 'nella camera da letto'],
            ['l\'armadio', 'gli armadi', 'nella camera da letto'],
            ['il tavolo', 'i tavoli', 'in cucina / in soggiorno'],
            ['la sedia', 'le sedie', 'in cucina / in soggiorno'],
            ['il divano', 'i divani', 'in soggiorno'],
            ['il frigo', 'i frigo', 'in cucina'],
            ['la lavatrice', 'le lavatrici', 'in bagno / in cucina'],
          ],
        },
        { tip: '**Il frigo** (short for frigorifero) doesn\'t change in the plural: *un frigo, due frigo*.' },
      ],
      exercises: [
        mc('Where is the bed (il letto)?', 'nella camera da letto', 'in cucina', 'in bagno', 'nell\'ingresso'),
        mc('Where is the fridge (il frigo)?', 'in cucina', 'in camera da letto', 'in soggiorno', 'in bagno'),
        mc('Where is the sofa (il divano)?', 'in soggiorno', 'in bagno', 'in cucina', 'nell\'ingresso'),
        mc('Where is the wardrobe (l\'armadio)?', 'nella camera da letto', 'in bagno', 'in cucina', 'sul balcone'),
        mc('Where is the washing machine (la lavatrice)?', 'in bagno / in cucina', 'in camera da letto', 'in soggiorno', 'nell\'ingresso'),
        mc('What is the plural of "la sedia"?', 'le sedie', 'le sedia', 'i sedi', 'gli sedie'),
        tr('l\'armadio', 'wardrobe', 'sofa', 'fridge', 'table'),
      ],
      homework: 'Write the furniture in each room of your home.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Dialogue: your new flat',
      titleIt: 'Lezione 3 · Dialogo: com\'è il tuo nuovo appartamento?',
      summary: 'Priya describes her new flat — and in / nella / nell\' with c\'è and ci sono.',
      minutes: 12,
      sections: [
        {
          title: 'Dire cosa c\'è nelle stanze: IN, NELLA, NELL\'',
          table: [
            ['Forma', 'Stanze', 'Esempio'],
            ['IN', 'cucina, soggiorno, bagno', 'In cucina c\'è il frigo.'],
            ['NELLA', 'camera da letto', 'Nella camera c\'è l\'armadio.'],
            ['NELL\'', 'ingresso', 'Nell\'ingresso c\'è una sedia.'],
          ],
        },
        { text: 'Remember: **c\'è** + singular (un divano, un letto) · **ci sono** + plural (quattro sedie, due letti)!' },
        {
          title: 'Dialogo: com\'è il tuo nuovo appartamento?',
          situation: 'Amir asks Priya about her new flat.',
          dialogue: [
            lp('Amir', 'Ciao Priya! Com\'è il tuo nuovo appartamento?', 'Hi Priya! What\'s your new flat like?', 'Sat Sri Akal Priya! Tuhada navaan flait kiven da hai?'),
            lp('Priya', 'È grande e luminoso! Ci sono un ingresso, una cucina e un soggiorno.', 'It\'s big and bright! There\'s an entrance, a kitchen and a living room.', 'Ih vadda ate rushni vaala hai! Ikk pravesh duar, rasoi ate baithak hai.'),
            lp('Amir', 'E quante camere ci sono? Cosa c\'è in soggiorno?', 'And how many bedrooms are there? What\'s in the living room?', 'Kinne kamre han? Baithak vich ki hai?'),
            lp('Priya', 'Ci sono due camere da letto e un bagno. In soggiorno ci sono un divano, un tavolo e sei sedie!', 'There are two bedrooms and a bathroom. In the living room there\'s a sofa, a table and six chairs!', 'Do kamre ate ikk baathrum hai. Baithak vich sofa, mez ate chhe kursiaan han!'),
          ],
        },
        {
          title: 'Role-play: descrivi casa tua',
          text: '1. **Chiedi:** «Com\'è casa tua? Quante stanze ci sono?»\n2. **Elenca le stanze:** ingresso, cucina, soggiorno, camera da letto, bagno.\n3. **Descrivi i mobili:** c\'è / ci sono + letto, armadio, tavolo, sedie, divano, frigo, lavatrice.',
        },
      ],
      exercises: [
        fb('___ cucina c\'è il frigo.', ['In', 'Nella', 'Nell\'', 'Al']),
        fb('___ camera da letto c\'è l\'armadio.', ['Nella', 'In la', 'Nel', 'Nell\'']),
        fb('___ ingresso c\'è una sedia.', ['Nell\'', 'Nella', 'In l\'', 'Nel']),
        fb('In soggiorno ___ un divano.', ['c\'è', 'ci sono']),
        fb('In cucina ___ quattro sedie.', ['ci sono', 'c\'è']),
        tf('L\'appartamento di Priya è piccolo.', false, 'Priya\'s flat is small.'),
        tf('Ci sono due camere da letto.', true, 'There are two bedrooms.'),
        tf('In soggiorno ci sono sei sedie.', true, 'In the living room there are six chairs.'),
      ],
      homework: 'Role-play with a partner: describe your home — the rooms and what there is in each room.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Preposizioni semplici e articolate
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 4 · The simple prepositions',
      titleIt: 'Lezione 4 · Le preposizioni semplici',
      summary: 'Di, a, da, in, con, su, per, tra / fra — with everyday examples.',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 5 · Lezione 4 (PDF)' },
        {
          title: 'Le 8 preposizioni semplici',
          table: [
            ['Preposizione', 'English · Punjabi', 'Esempio'],
            ['DI', 'of / from · da, di, de', 'Un bicchiere di acqua.'],
            ['A', 'to / at · nu, vich', 'Vado a casa.'],
            ['DA', 'from / at (a person\'s place) · ton, kol', 'Vengo da Milano.'],
            ['IN', 'in / into · vich, andar', 'Lavoro in ufficio.'],
            ['CON', 'with · de naal', 'Esco con Sara.'],
            ['SU', 'on / about · de utte', 'Leggo un libro su Roma.'],
            ['PER', 'for / to · lai', 'Parto per Torino.'],
            ['TRA / FRA', 'between / in (time) · vichkar, baad', 'Arrivo tra dieci minuti.'],
          ],
        },
        {
          title: 'Dove sei? Luoghi quotidiani',
          practice: false,
          words: [
            wp('a casa', 'at home', 'Ghar', { img: 'House', ex: 'Sono a casa adesso.', exEn: 'I\'m at home now.' }),
            wp('in ufficio', 'at the office', 'Daftar vich', { img: 'Office building', ex: 'Marco lavora in ufficio.', exEn: 'Marco works in an office.' }),
            wp('dal medico', 'at / to the doctor\'s', 'Daaktar kol', { img: 'Stethoscope', ex: 'Oggi vado dal medico.', exEn: 'Today I\'m going to the doctor\'s.' }),
            wp('al supermercato', 'at / to the supermarket', 'Suparmarkit', { img: 'Shopping cart', ex: 'Compro il pane al supermercato.', exEn: 'I buy bread at the supermarket.' }),
          ],
        },
        { tip: 'Use **DA** for going to a **person**: *vado **dal** medico, **dal** dentista, **da** Marco*. Use **A / IN** for places: *vado **al** supermercato, **in** farmacia*.' },
      ],
      exercises: [
        fb('Vado ___ casa.', ['a', 'in', 'da', 'su']),
        fb('Lavoro ___ ufficio.', ['in', 'a', 'da', 'con']),
        fb('Vengo ___ Milano.', ['da', 'di', 'in', 'su']),
        fb('Esco ___ Sara.', ['con', 'per', 'tra', 'di']),
        fb('Parto ___ Torino.', ['per', 'con', 'su', 'di']),
        fb('Arrivo ___ dieci minuti.', ['tra', 'per', 'con', 'di']),
        fb('Un bicchiere ___ acqua.', ['di', 'da', 'a', 'in']),
        mc('You are going to the doctor\'s. You say…', 'Vado dal medico.', 'Vado al medico.', 'Vado in medico.', 'Vado del medico.'),
      ],
      homework: 'Write 8 sentences, one with each simple preposition.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 4 · Prepositions + article: al, dal, nel, sul…',
      titleIt: 'Lezione 4 · Le preposizioni articolate',
      summary: 'A + il = al, da + il = dal, su + il = sul — the full table.',
      minutes: 12,
      sections: [
        {
          title: 'Come nascono le articolate',
          table: [
            ['Preposizione + articolo', '=', 'Esempio'],
            ['A + IL', 'AL', 'Vado al supermercato.'],
            ['DA + IL', 'DAL', 'Vado dal medico.'],
            ['SU + IL', 'SUL', 'Il libro è sul tavolo.'],
          ],
        },
        {
          title: 'Schema completo',
          table: [
            ['Semplice', '+ il', '+ lo / l\'', '+ la', '+ i / gli / le'],
            ['A', 'al', 'allo / all\'', 'alla', 'ai / agli / alle'],
            ['DA', 'dal', 'dallo / dall\'', 'dalla', 'dai / dagli / dalle'],
            ['IN', 'nel', 'nello / nell\'', 'nella', 'nei / negli / nelle'],
            ['DI', 'del', 'dello / dell\'', 'della', 'dei / degli / delle'],
            ['SU', 'sul', 'sullo / sull\'', 'sulla', 'sui / sugli / sulle'],
          ],
        },
        {
          title: 'Oggetti in casa',
          words: [
            wp('sul tavolo (su + il)', 'on the table', 'Mez utte', { img: 'Key', ex: 'Le chiavi sono sul tavolo.', exEn: 'The keys are on the table.' }),
            wp('sulla sedia (su + la)', 'on the chair', 'Kursi utte', { img: 'Handbag', ex: 'La borsa è sulla sedia.', exEn: 'The bag is on the chair.' }),
            wp('nel frigo (in + il)', 'in the fridge', 'Frij vich', { img: 'Glass of milk', ex: 'Il latte fresco è nel frigo.', exEn: 'The fresh milk is in the fridge.' }),
            wp('nella stanza (in + la)', 'in the room', 'Kamre vich', { img: 'Door', ex: 'Il letto è nella stanza.', exEn: 'The bed is in the room.' }),
          ],
        },
        { tip: '**CON, PER, TRA / FRA** almost always stay simple: *con il treno, per la scuola, tra la banca e la posta*.' },
      ],
      exercises: [
        fb('Le chiavi sono ___ tavolo della cucina.', ['sul', 'su', 'sulla', 'nel']),
        fb('Il latte è ___ frigo.', ['nel', 'in', 'nella', 'sul']),
        fb('La mattina vado ___ ufficio.', ['in', 'al', 'nel', 'all\'']),
        fb('Il sabato vado ___ supermercato.', ['al', 'in', 'a', 'nel']),
        fb('Oggi mia madre va ___ medico.', ['dal', 'al', 'da', 'del']),
        fb('Poi torna ___ casa.', ['a', 'in', 'alla', 'nella']),
        fb('La borsa è ___ sedia.', ['sulla', 'sul', 'su', 'nella']),
        mc('in + la = ?', 'nella', 'nel', 'inla', 'alla'),
        mc('da + il = ?', 'dal', 'dil', 'del', 'dallo'),
      ],
      homework: 'Look around your room and write where 5 things are (Il telefono è sul tavolo…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 4 · Dialogue: Where are you going today?',
      titleIt: 'Lezione 4 · Dialogo: Dove vai oggi?',
      summary: 'Noor goes to the doctor, then to the supermarket and home.',
      minutes: 8,
      sections: [
        {
          title: 'Dialogo: Dove vai oggi?',
          situation: 'Luca meets Noor in the morning.',
          dialogue: [
            lp('Luca', 'Ciao Noor! Dove vai adesso?', 'Hi Noor! Where are you going now?', 'Sat Sri Akal Noor! Hun kitthe ja rahi hain?'),
            lp('Noor', 'Vado dal medico per un controllo.', 'I\'m going to the doctor\'s for a check-up.', 'Main chekap lai daaktar kol ja rahi haan.'),
            lp('Luca', 'Dopo torni a casa o vai in ufficio?', 'Afterwards are you going home or to the office?', 'Phir ghar vaapas jaavegi jaan daftar?'),
            lp('Noor', 'Passo al supermercato, poi torno a casa.', 'I\'ll stop at the supermarket, then go back home.', 'Suparmarkit ja ke ghar vaapas aavangi.'),
          ],
        },
        {
          title: 'Role-play: la tua giornata',
          text: '1. **Chiedi dove va:** «Dove vai oggi?»\n2. **Usa i luoghi:** scegli due luoghi della città (dal medico, al supermercato, in ufficio, a casa…).\n3. **Descrivi gli oggetti:** di\' cosa c\'è sul tavolo (sul tavolo ci sono le chiavi…).',
        },
      ],
      exercises: [
        tf('Noor va dal medico.', true, 'Noor is going to the doctor\'s.'),
        tf('Dopo il medico Noor va in ufficio.', false, 'After the doctor Noor goes to the office.'),
        tf('Noor passa al supermercato.', true, 'Noor stops at the supermarket.'),
        tr('Torno a casa.', 'I go back home.', 'I stay at home.', 'I leave home.', 'I clean the house.'),
        wo('Vado dal medico per un controllo', 'I\'m going to the doctor\'s for a check-up'),
      ],
      homework: 'Tell a partner where you go today, in order, with the right prepositions.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 5
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 5 test',
      titleIt: 'Test Capitolo 5',
      summary: 'Listening, places and directions, rooms and furniture, andare / venire and prepositions. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 5',
          text: 'This test checks everything in Chapter 5 (Lessons 1–4).\n\n- **1. Listening** — sentences and a route through town\n- **2. Vocabulary** — city places, directions, rooms and furniture\n- **3. Grammar** — andare / venire, prepositions and directions\n- **4. Writing** — sentences about your town and your home\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (my town and my home) and the **oral interview** (town and transport, giving directions, describing your home — 2 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A — Ascolta e scegli
        lis('1A · Listen and choose the sentence you hear', 'Stamattina vado in banca in autobus perché piove.', 'VADO IN BANCA IN AUTOBUS', 'VADO IN POSTA A PIEDI'),
        lis('1A · Listen and choose the sentence you hear', 'Scusi, dov\'è l\'ospedale? L\'ospedale è di fronte alla stazione.', 'L\'OSPEDALE È DI FRONTE ALLA STAZIONE', 'LA FARMACIA È ACCANTO AL PARCO'),
        lis('1A · Listen and choose the sentence you hear', 'Per andare in farmacia, all\'incrocio gira a sinistra.', 'ALL\'INCROCIO GIRA A SINISTRA', 'ALL\'INCROCIO GIRA A DESTRA'),
        lis('1A · Listen and choose the sentence you hear', 'Nel mio appartamento in soggiorno c\'è un divano grande.', 'IN SOGGIORNO C\'È UN DIVANO GRANDE', 'IN CAMERA DA LETTO C\'È UN ARMADIO'),
        lis('1A · Listen and choose the sentence you hear', 'Non mi sento bene: oggi vado dal medico.', 'OGGI VADO DAL MEDICO', 'IL TELEFONO È SUL TAVOLO'),
        // 1B — Il percorso
        lis('1B · Listen to the route. Where are you going? (1)', 'Per andare in farmacia, esci dalla stazione, vai sempre dritto, attraversa la piazza e poi gira a destra: l\'edificio è accanto al supermercato.', 'FARMACIA', 'BANCA', 'POSTA', 'OSPEDALE'),
        lis('1B · Listen again: "vai sempre…" (2)', 'Esci dalla stazione, vai sempre dritto.', 'DRITTO', 'DESTRA', 'SINISTRA', 'LONTANO'),
        lis('1B · Listen again: what do you do with the square? (3)', 'Attraversa la piazza.', 'ATTRAVERSA', 'GIRA', 'PRENDI', 'ESCI'),
        lis('1B · Listen again: "poi gira a…" (4)', 'Poi gira a destra.', 'DESTRA', 'SINISTRA', 'DRITTO', 'FRONTE'),
        lis('1B · Listen again: where is the building? (5)', 'L\'edificio è accanto al supermercato.', 'ACCANTO', 'LONTANO', 'DI FRONTE', 'TRA'),
        // 2A — Collega
        pairs('2A · Match each word with its meaning', [
          ['Fermata dell\'autobus', 'Bus stop · Bass adda'],
          ['All\'incrocio', 'At the crossroads · Chaurahe te'],
          ['Di fronte a', 'Opposite · Saahmane'],
          ['Soggiorno', 'Living room · Baithak'],
          ['Lavatrice', 'Washing machine · Kapde dhon vaali mashin'],
        ]),
        // 2B — Stanze e mobili
        mc('2B · Il letto → ?', 'camera da letto', 'cucina', 'soggiorno', 'bagno'),
        mc('2B · Il frigo → ?', 'cucina', 'camera da letto', 'soggiorno', 'ingresso'),
        mc('2B · Il divano → ?', 'soggiorno', 'bagno', 'cucina', 'ingresso'),
        mc('2B · L\'armadio → ?', 'camera da letto', 'bagno', 'cucina', 'ingresso'),
        mc('2B · La lavatrice → ?', 'bagno', 'camera da letto', 'soggiorno', 'ingresso'),
        // 3A — Andare e venire
        fb('3A · Stamattina io ___ (andare) in farmacia e poi in posta.', ['vado', 'vai', 'va', 'andiamo']),
        fb('3A · Tu ___ (venire) al parco con noi oggi pomeriggio?', ['vieni', 'vengo', 'viene', 'venite']),
        fb('3A · Marco e Sara ___ (andare) a Milano in treno.', ['vanno', 'va', 'andate', 'vado']),
        fb('3A · Noi ___ (venire) a scuola di italiano in bicicletta.', ['veniamo', 'venite', 'vengono', 'vengo']),
        // 3B — Preposizioni
        fb('3B · La mattina vado ___ ufficio.', ['in', 'al']),
        fb('3B · Il sabato vado ___ supermercato.', ['al', 'in']),
        fb('3B · Oggi pomeriggio mia madre va ___ medico.', ['dal', 'al']),
        fb('3B · … e poi torna ___ casa.', ['a', 'in']),
        fb('3B · Le chiavi sono ___ tavolo della cucina.', ['sul', 'su']),
        fb('3B · … e il latte è ___ frigo.', ['nel', 'in']),
        // 3C — Indicazioni
        fb('3C · Per arrivare in piazza ___ per cento metri su questa strada.', ['vai sempre dritto', 'gira a sinistra', 'vicino a', 'lontano da']),
        fb('3C · All\'incrocio ___: la banca è subito lì all\'angolo.', ['gira a sinistra', 'vai sempre dritto', 'vicino a', 'lontano da']),
        fb('3C · La fermata dell\'autobus è molto ___ casa mia: bastano due minuti a piedi!', ['vicino a', 'lontano da', 'gira a sinistra', 'vai sempre dritto']),
        fb('3C · L\'ospedale è ___ centro città: devi prendere l\'autobus numero 4.', ['lontano dal', 'vicino al', 'gira a sinistra', 'vai sempre dritto']),
        // 4 — La mia città e la mia casa
        wo('La mattina vado a scuola in autobus', 'In the morning I go to school by bus'),
        wo('Vicino a casa mia c\'è una farmacia', 'Near my home there is a pharmacy'),
        wo('Nella mia camera c\'è un armadio', 'In my bedroom there is a wardrobe'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences about how you move around town, what is near your home, your rooms and your furniture. Then prepare the 2-minute oral interview.',
    },
  ],
};
