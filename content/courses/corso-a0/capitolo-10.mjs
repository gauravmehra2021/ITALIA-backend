import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 10 of the school syllabus (Corso di Italiano, livello A1 → A2).
// Source: syllabus/Capitolo 10 Lezione 1–4 (.pptx) and Test Capitolo 10 (.docx).
// Correction to the client material: Lezione 3 and Test 3B.2 say "Hai la ricetta? → Sì, la ho".
// With avere the pronoun is elided ("l'ho", in speech "ce l'ho"), so the app uses
// "Porti la ricetta? → Sì, la porto" and explains l'ho in a tip.

const slides = (lez) => `/content/slides/corso-a0-cap10-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-10',
  title: 'Chapter 10: Health — the body, the doctor and the pharmacy',
  titleIt: 'Capitolo 10: La salute, i pronomi diretti e l\'imperativo',
  cover: 'Stethoscope',
  description: 'The parts of the body and saying what hurts (ho mal di…, mi fa / mi fanno male, ho la febbre), at the doctor\'s and at the pharmacy (la ricetta, lo sciroppo, le compresse, la pomata), the direct object pronouns lo, la, li, le, and the imperative for advice and instructions — informal (prendi!) and formal (prenda!).',
  canDo: [
    'I can name the main parts of the body',
    'I can say what hurts: ho mal di testa, mi fa male la schiena, mi fanno male le gambe',
    'I can explain my symptoms to the doctor and ask for a prescription',
    'I can buy medicine at the pharmacy and understand how to take it',
    'I can avoid repeating nouns with lo, la, li, le: Prendi lo sciroppo? Sì, lo prendo',
    'I can give advice to a friend (riposa, non uscire!) and understand formal instructions (prenda, firmi qui)',
  ],
  liveClassFocus: 'Role-play at the doctor\'s (two symptoms, allergies, prescription) and at the pharmacy (syrup or ointment, how to take it), then give a sick friend 3 pieces of advice with the imperative.',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — Il corpo umano e i sintomi
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · The parts of the body',
      titleIt: 'Lezione 1 · Le parti del corpo',
      summary: 'La testa, la gola, la pancia, la schiena, il braccio, la gamba, il dente, la mano, il piede.',
      minutes: 10,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 10 · Lezione 1 (PDF)' },
        {
          title: 'Le parti del corpo più frequenti',
          words: [
            wp('la testa', 'head', 'Sir', { pron: 'lah TES-tah', img: 'Face with head-bandage' }),
            wp('la gola', 'throat', 'Gala', { pron: 'lah GOH-lah', img: 'Face with thermometer' }),
            wp('la pancia', 'stomach / belly', 'Pet', { pron: 'lah PAHN-chah', img: 'Nauseated face' }),
            wp('la schiena', 'back', 'Pitth / Lakk', { pron: 'lah SKYEH-nah', img: 'Person standing' }),
            wp('il braccio — le braccia', 'arm — arms', 'Baanh', { pron: 'eel BRAHT-choh', img: 'Flexed biceps' }),
            wp('la gamba — le gambe', 'leg — legs', 'Latt', { pron: 'lah GAHM-bah', img: 'Leg' }),
            wp('il dente — i denti', 'tooth — teeth', 'Dand', { pron: 'eel DEN-teh', img: 'Tooth' }),
            wp('la mano — le mani', 'hand — hands', 'Hatth', { pron: 'lah MAH-noh', img: 'Raised hand' }),
            wp('il piede — i piedi', 'foot — feet', 'Pair', { pron: 'eel PYEH-deh', img: 'Foot' }),
          ],
        },
        { tip: 'Two irregular plurals: **il braccio → le braccia** and **la mano → le mani** (la mano is feminine even though it ends in -o).' },
      ],
      exercises: [
        mc('Plural of "il braccio"?', 'le braccia', 'i bracci', 'le braccie', 'i braccia'),
        mc('Plural of "la mano"?', 'le mani', 'le mane', 'i mani', 'le mano'),
        mc('You eat with your…', 'denti', 'piedi', 'gambe', 'braccia'),
        tr('la schiena', 'back', 'throat', 'stomach', 'leg'),
        tr('la gola', 'throat', 'head', 'tooth', 'hand'),
      ],
      homework: 'Touch and name 8 parts of your body in Italian.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 1 · Saying what hurts',
      titleIt: 'Lezione 1 · Ho mal di…, mi fa male…, ho la febbre',
      summary: '3 structures for symptoms — and mi fa male (one) vs mi fanno male (two or more).',
      minutes: 12,
      sections: [
        {
          title: '3 strutture fondamentali',
          table: [
            ['Struttura', 'Esempio', 'Altri esempi'],
            ['AVERE MAL DI + nome', 'Ho mal di testa.', 'mal di gola, di pancia, di schiena, di denti'],
            ['MI FA / MI FANNO MALE', 'Mi fa male il braccio.', 'singolare: fa male · plurale: fanno male'],
            ['AVERE + sintomo', 'Ho la febbre.', 'il raffreddore, la tosse, la nausea'],
          ],
        },
        {
          title: 'Mi fa male vs mi fanno male',
          table: [
            ['Una parte (singolare)', 'Due o più parti (plurale)'],
            ['Mi fa male la testa.', 'Mi fanno male le gambe.'],
            ['Mi fa male la schiena.', 'Mi fanno male le braccia.'],
            ['Mi fa male un dente.', 'Mi fanno male i denti.'],
          ],
        },
        {
          title: 'I sintomi',
          words: [
            wp('Ho la febbre.', 'I have a temperature.', 'Mainu bukhar hai.', { img: 'Face with thermometer', ex: 'Ho la febbre a 38.', exEn: 'I have a temperature of 38.' }),
            wp('Ho il raffreddore.', 'I have a cold.', 'Mainu zukam hai.', { img: 'Sneezing face' }),
            wp('Ho la tosse.', 'I have a cough.', 'Mainu khangh hai.', { img: 'Face with medical mask' }),
            wp('Ho la nausea.', 'I feel sick (nausea).', 'Mera ji kacha ho riha hai.', { img: 'Nauseated face' }),
            wp('Mi gira la testa.', 'I feel dizzy.', 'Mainu chakkar aa rahe han.', { img: 'Face with spiral eyes' }),
          ],
        },
      ],
      exercises: [
        fb('Ho camminato molto oggi e ___ le gambe.', ['mi fanno male', 'mi fa male', 'ho mal di', 'ho male']),
        fb('Ho lavorato tanto e adesso ___ il braccio destro.', ['mi fa male', 'mi fanno male', 'ho mal di', 'mi fai male']),
        fb('Non posso mangiare cose fredde perché ___ i denti.', ['mi fanno male', 'mi fa male', 'ho la', 'mi fa']),
        fb('Ho ___ testa.', ['mal di', 'male la', 'mal la', 'malo di']),
        fb('Ho ___ febbre a 38.', ['la', 'il', 'mal di', 'una mal']),
        fb('Ho preso freddo e stamattina ho ___ raffreddore.', ['il', 'la', 'lo', 'mal di']),
        mc('"Ho mangiato troppo veloce e ora ho…"', 'mal di pancia', 'mal di denti', 'la febbre', 'mal di schiena'),
      ],
      homework: 'Write 4 sentences for the doctor about a time you were ill (Ho la febbre…, mi fa male…, mi fanno male…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Dialogue: at the family doctor\'s',
      titleIt: 'Lezione 1 · Dialogo: dal medico di base',
      summary: 'Amandeep explains his symptoms — and how to answer "Come stai?".',
      minutes: 10,
      sections: [
        {
          title: 'Come stai? Risposte utili',
          words: [
            wp('Sto male.', 'I feel ill.', 'Tabiat theek nahi.', { img: 'Face with thermometer', ex: 'Mi gira la testa e ho la tosse.', exEn: 'I feel dizzy and I have a cough.' }),
            wp('Sono stanco / stanca.', 'I\'m tired.', 'Main thakkia / thakki hoi haan.', { img: 'Tired face', ex: 'Non ho dormito bene stanotte.', exEn: 'I didn\'t sleep well last night.' }),
            wp('Sto meglio.', 'I feel better.', 'Hun changa mahisus karda haan.', { img: 'Smiling face with smiling eyes', ex: 'Oggi mi sento molto meglio!', exEn: 'Today I feel much better!' }),
          ],
        },
        {
          title: 'Dialogo: dal medico di base',
          situation: 'Amandeep visits his family doctor.',
          dialogue: [
            lp('Dottoressa', 'Buongiorno Amandeep! Che cosa si sente oggi?', 'Good morning Amandeep! How are you feeling today?', 'Sat Sri Akal Amandeep! Ajj tuhanu ki taklif hai?'),
            lp('Amandeep', 'Buongiorno dottoressa! Da ieri ho mal di gola e ho la febbre.', 'Good morning, doctor! Since yesterday I\'ve had a sore throat and a temperature.', 'Sat Sri Akal daaktar ji! Kall ton mera gala kharab hai ate bukhar hai.'),
            lp('Dottoressa', 'Le fa male anche la testa o ha il raffreddore?', 'Does your head hurt too, or do you have a cold?', 'Ki sir vich vi dard hai jaan zukam hai?'),
            lp('Amandeep', 'Sì, ho mal di testa e mi fanno male anche le spalle e la schiena.', 'Yes, I have a headache and my shoulders and back hurt too.', 'Haan, sir dard hai ate modhiaan ate pitth vich vi dard hai.'),
          ],
        },
        { tip: 'The doctor uses **Lei**: *Che cosa **si sente**? **Le** fa male la testa?* With a friend: *Come **ti** senti? **Ti** fa male la testa?*' },
      ],
      exercises: [
        tf('Amandeep ha mal di gola.', true, 'Amandeep has a sore throat.'),
        tf('Amandeep non ha la febbre.', false, 'Amandeep doesn\'t have a temperature.'),
        tf('Gli fanno male le spalle e la schiena.', true, 'His shoulders and back hurt.'),
        mc('A friend asks "Come stai?" and you are tired:', 'Sono stanco.', 'Sto meglio.', 'Ho fame.', 'Mi fa male.'),
        wo('Da ieri ho mal di gola', 'Since yesterday I\'ve had a sore throat'),
        wo('Mi fanno male le spalle e la schiena', 'My shoulders and back hurt'),
      ],
      homework: 'Mime game with a partner: touch a part of your body, your partner guesses "Hai mal di…!".',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — Dal medico e in farmacia
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 2 · At the doctor\'s and the pharmacy',
      titleIt: 'Lezione 2 · Le parole della salute',
      summary: 'La visita, la ricetta, la medicina, lo sciroppo, le compresse, la pomata, l\'allergia.',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 10 · Lezione 2 (PDF)' },
        {
          title: 'Parole fondamentali dal medico e in farmacia',
          words: [
            wp('la visita (medica)', 'medical check-up / appointment', 'Daaktari jaanch', { pron: 'lah VEE-zee-tah', img: 'Stethoscope', ex: 'Prenotare una visita dal medico.', exEn: 'To book an appointment with the doctor.' }),
            wp('la ricetta (medica)', 'prescription', 'Daaktar di parchi', { pron: 'lah ree-CHET-tah', img: 'Page facing up', ex: 'Farmaci con o senza ricetta.', exEn: 'Medicines with or without prescription.' }),
            wp('la medicina / il farmaco', 'medicine', 'Davai', { pron: 'lah meh-dee-CHEE-nah', img: 'Medical symbol', ex: 'Prendere una medicina dopo i pasti.', exEn: 'To take a medicine after meals.' }),
            wp('lo sciroppo', 'syrup', 'Sharbat (khangh di davai)', { pron: 'loh shee-ROP-poh', img: 'Spoon', ex: 'Un cucchiaio di sciroppo per la tosse.', exEn: 'A spoonful of cough syrup.' }),
            wp('le compresse / le pastiglie', 'tablets / pills', 'Goliaan', { pron: 'leh kom-PRES-seh', img: 'Pill', ex: 'Due compresse al giorno con acqua.', exEn: 'Two tablets a day with water.' }),
            wp('la pomata / la crema', 'ointment / cream', 'Maraham', { pron: 'lah poh-MAH-tah', img: 'Lotion bottle', ex: 'Mettere la pomata sulla schiena.', exEn: 'Put the ointment on your back.' }),
            wp('l\'allergia', 'allergy', 'Alarji', { pron: 'lahl-lehr-JEE-ah', img: 'Sneezing face', ex: 'Ha qualche allergia ai farmaci?', exEn: 'Are you allergic to any medicines?' }),
            wp('la tessera sanitaria', 'health card', 'Sihat kaard', { pron: 'lah TES-seh-rah sah-nee-TAH-ryah', img: 'Identification card' }),
          ],
        },
        {
          title: 'Come e quando prendere i farmaci',
          table: [
            ['Farmaco', 'Istruzioni'],
            ['Compresse', 'Una compressa ogni 8 ore. A stomaco pieno (dopo mangiato).'],
            ['Sciroppo', 'Un cucchiaio tre volte al giorno: mattina, pomeriggio e sera.'],
            ['Pomata', 'Applicare due volte al giorno. Solo uso esterno (on the skin).'],
          ],
        },
        { tip: 'In Italy many medicines need **la ricetta** of your *medico di base* (family doctor). Always bring your **tessera sanitaria** to the doctor and the pharmacy.' },
      ],
      exercises: [
        mc('The paper the doctor writes for your medicine is…', 'la ricetta', 'la pomata', 'la visita', 'la tessera'),
        mc('For a cough you take…', 'lo sciroppo', 'la pomata', 'la tessera', 'la visita'),
        mc('You put it on your skin:', 'la pomata', 'lo sciroppo', 'le compresse', 'la ricetta'),
        mc('"A stomaco pieno" means…', 'after eating', 'before eating', 'with an empty stomach', 'at night'),
        tr('le compresse', 'tablets', 'syrup', 'ointment', 'prescription'),
      ],
      homework: 'Look at a medicine box at home and find: name, how many tablets, how many times a day.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · Dialogues: the doctor\'s visit and the pharmacy counter',
      titleIt: 'Lezione 2 · Dialoghi: la visita e il bancone della farmacia',
      summary: 'Explain your symptoms, answer about allergies, ask for the prescription and buy the medicine.',
      minutes: 12,
      sections: [
        {
          title: 'Dialogo 1: la visita dal medico di base',
          situation: 'A patient at the family doctor\'s.',
          dialogue: [
            lp('Medico', 'Buongiorno! Che disturbi ha? Dove le fa male?', 'Good morning! What symptoms do you have? Where does it hurt?', 'Sat Sri Akal! Ki taklif hai? Kitthe dard hai?'),
            lp('Paziente', 'Ho mal di gola, la febbre e una forte tosse da tre giorni.', 'I\'ve had a sore throat, a temperature and a bad cough for three days.', 'Tinn dinaan ton gala kharab hai, bukhar hai ate bahut khangh hai.'),
            lp('Medico', 'Ha qualche allergia alle medicine?', 'Are you allergic to any medicines?', 'Ki tuhanu kise davai ton alarji hai?'),
            lp('Paziente', 'No, nessuna allergia. Mi fa la ricetta medica?', 'No, no allergies. Can you write me a prescription?', 'Nahi, koi alarji nahi. Ki tusi parchi likh dioge?'),
          ],
        },
        {
          title: 'Dialogo 2: al bancone della farmacia',
          situation: 'The patient takes the prescription to the pharmacy.',
          dialogue: [
            lp('Cliente', 'Buongiorno! Ho questa ricetta del medico e vorrei anche uno sciroppo.', 'Good morning! I have this prescription from the doctor and I\'d also like a syrup.', 'Sat Sri Akal! Mere kol daaktar di eh parchi hai ate mainu sharbat vi chaahida hai.'),
            lp('Farmacista', 'Ecco le compresse della ricetta e lo sciroppo per la tosse.', 'Here are the tablets on the prescription and the cough syrup.', 'Eh lao parchi vaaliaan goliaan ate khangh da sharbat.'),
            lp('Cliente', 'Grazie! Ha anche una pomata per il mal di schiena senza ricetta?', 'Thank you! Do you also have an ointment for back pain without a prescription?', 'Dhanvad! Ki pitth dard lai bina parchi maraham vi hai?'),
            lp('Farmacista', 'Sì, certo! Sono 18 euro in tutto. Mi dà la tessera sanitaria?', 'Yes, of course! That\'s 18 euros altogether. May I have your health card?', 'Haan, zarur! Kull 18 yuro. Aapna sihat kaard de sakde ho?'),
          ],
        },
        {
          title: 'Cosa dire dal medico e al farmacista',
          text: '**Dal medico**\n- Vorrei prenotare una visita.\n- Sono allergico/a alla penicillina.\n- Può farmi la ricetta?\n\n**In farmacia**\n- Avete qualcosa per il mal di testa?\n- Serve la ricetta medica?\n- Quante compresse devo prendere?',
        },
      ],
      exercises: [
        tf('Il paziente ha la tosse da tre giorni.', true, 'The patient has had a cough for three days.'),
        tf('Il paziente è allergico alle medicine.', false, 'The patient is allergic to medicines.'),
        tf('La pomata è per il mal di testa.', false, 'The ointment is for a headache.'),
        tf('In tutto spende 18 euro.', true, 'Altogether he spends 18 euros.'),
        mc('At the pharmacy you ask if you need a prescription:', 'Serve la ricetta medica?', 'Mi fa male la ricetta?', 'Ho la ricetta di gola.', 'Prenoto la ricetta.'),
        wo('Vorrei prenotare una visita', 'I would like to book an appointment'),
        wo('Quante compresse devo prendere', 'How many tablets do I have to take'),
      ],
      homework: 'Role-play: scenario 1 (doctor and patient) and scenario 2 (pharmacist and customer).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Understanding the pharmacist',
      titleIt: 'Lezione 2 · Capire le istruzioni del farmacista',
      summary: 'Ogni 8 ore, tre volte al giorno, dopo i pasti, a stomaco vuoto…',
      minutes: 8,
      sections: [
        {
          title: 'Quando e quanto?',
          words: [
            wp('ogni 8 ore', 'every 8 hours', 'Har atth ghante', { img: 'Alarm clock' }),
            wp('tre volte al giorno', 'three times a day', 'Din vich tinn vaar', { img: 'Keycap 3' }),
            wp('dopo i pasti', 'after meals', 'Khaane ton baad', { img: 'Fork and knife with plate' }),
            wp('a stomaco vuoto', 'on an empty stomach', 'Khaali pet', { img: 'Nauseated face' }),
            wp('un cucchiaio', 'a spoonful', 'Ikk chammach', { img: 'Spoon' }),
            wp('solo uso esterno', 'for external use only', 'Sirf baahari varton', { img: 'Lotion bottle' }),
          ],
        },
      ],
      exercises: [
        mc('"Una compressa ogni 8 ore" = how many a day?', '3', '8', '1', '2'),
        mc('"Tre volte al giorno" means…', 'three times a day', 'for three days', 'every three hours', 'three tablets'),
        mc('"A stomaco vuoto" means…', 'before eating / empty stomach', 'after eating', 'with milk', 'at night'),
        fb('Prenda lo sciroppo ___ i pasti.', ['dopo', 'ogni', 'volte', 'uso']),
        fb('Un cucchiaio tre ___ al giorno.', ['volte', 'ore', 'pasti', 'ogni']),
      ],
      homework: 'Write the instructions of a medicine you know (Prendo … compresse … volte al giorno…).',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — I pronomi diretti
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 3 · Direct pronouns: lo, la, li, le',
      titleIt: 'Lezione 3 · I pronomi diretti: lo, la, li, le',
      summary: 'Prendi la medicina? Sì, la prendo — no repetition.',
      minutes: 12,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 10 · Lezione 3 (PDF)' },
        {
          title: 'Evitare le ripetizioni',
          table: [
            ['Senza pronome (ripetitivo)', 'Con il pronome (naturale)'],
            ['— Prendi la medicina? — Sì, prendo la medicina.', '— Prendi la medicina? — Sì, la prendo.'],
          ],
        },
        {
          title: 'I quattro pronomi diretti',
          table: [
            ['Genere e numero', 'Pronome', 'Esempio', 'English · Punjabi'],
            ['maschile singolare', 'LO', 'Prendi lo sciroppo? → Sì, lo prendo.', 'it · is nu'],
            ['femminile singolare', 'LA', 'Metti la pomata? → Sì, la metto.', 'it · is nu'],
            ['maschile plurale', 'LI', 'Compri i biglietti? → Sì, li compro.', 'them · inhaan nu'],
            ['femminile plurale', 'LE', 'Prendi le compresse? → Sì, le prendo.', 'them · inhaan nu'],
          ],
        },
        {
          title: 'Dove si mette il pronome?',
          table: [
            ['Frase', 'Regola', 'Esempio'],
            ['affermativa', 'pronoun + verb', 'Sì, li compro.'],
            ['negativa', 'NON + pronoun + verb', 'No, non li compro.'],
            ['con l\'infinito', 'joined to the end', 'Devo prenderla.'],
          ],
        },
        { tip: 'Before **ho, hai, ha, hanno** (avere) *lo* and *la* become **l\'**: *Hai la ricetta? — Sì, **l\'ho** (ce l\'ho) nella borsa.*' },
      ],
      exercises: [
        fb('Prendi lo sciroppo la sera? — Sì, ___ prendo prima di dormire.', ['lo', 'la', 'li', 'le']),
        fb('Porti la ricetta medica? — Sì, ___ porto nella borsa.', ['la', 'lo', 'li', 'le']),
        fb('Compri i farmaci oggi? — Sì, ___ compro subito.', ['li', 'le', 'lo', 'la']),
        fb('Prendi le compresse con acqua? — Sì, ___ prendo con un bicchiere d\'acqua.', ['le', 'li', 'la', 'lo']),
        fb('Metti la pomata sul braccio? — No, non ___ metto adesso.', ['la', 'lo', 'le', 'li']),
        fb('Bevi il tè? — Sì, ___ bevo.', ['lo', 'la', 'li', 'le']),
        fb('No, ___ compro.', ['non lo', 'lo non', 'non', 'lo']),
        mc('"Devo prenderla" — where is the pronoun?', 'joined to the end of the infinitive', 'before devo', 'after devo, separate', 'there is no pronoun'),
      ],
      homework: 'Write 5 questions and answers with lo, la, li, le about your shopping or medicines.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Dialogue: before leaving on a trip',
      titleIt: 'Lezione 3 · Dialogo: prima di partire per il viaggio',
      summary: 'Simran and Gurpreet pack: li compro online, la preparo stasera, lo metto nello zaino.',
      minutes: 10,
      sections: [
        {
          title: 'Usare lo, la, li, le con le medicine',
          practice: false,
          words: [
            wp('Porti la ricetta? — Sì, la porto nella borsa.', 'Are you bringing the prescription? — Yes, I\'m bringing it in my bag.', 'Ki tusi parchi lai ke ja rahe ho? — Haan, bag vich lai ke ja riha haan.'),
            wp('Prendi lo sciroppo? — Sì, lo prendo dopo cena.', 'Do you take the syrup? — Yes, I take it after dinner.', 'Ki tusi sharbat lainde ho? — Haan, raat de khaane ton baad lainda haan.'),
            wp('Compri i farmaci oggi? — Sì, li compro in farmacia.', 'Are you buying the medicines today? — Yes, I\'m buying them at the pharmacy.', 'Ajj davaiaan kharidoge? — Haan, faramesi ton kharidanga.'),
            wp('Prendi le pastiglie? — No, non le prendo a stomaco vuoto.', 'Do you take the pills? — No, I don\'t take them on an empty stomach.', 'Goliaan lainde ho? — Nahi, khaali pet nahi lainda.'),
          ],
        },
        {
          title: 'Dialogo: prima di partire per il viaggio',
          situation: 'Simran and Gurpreet are getting ready for a trip.',
          dialogue: [
            lp('Simran', 'Ciao Gurpreet! Compri tu i biglietti del treno?', 'Hi Gurpreet! Are you buying the train tickets?', 'Sat Sri Akal Gurpreet! Tren diaan tikataan tusi kharidoge?'),
            lp('Gurpreet', 'Sì, li compro subito online! E tu prepari la valigia?', 'Yes, I\'m buying them online right now! And are you packing the suitcase?', 'Haan, main hune onlain kharid lainda haan! Ate tusi sutkes tiar karoge?'),
            lp('Simran', 'Sì, la preparo stasera. Porti anche il passaporto?', 'Yes, I\'ll pack it tonight. Are you bringing the passport too?', 'Haan, ajj raat tiar karangi. Paasport vi lai ke chaloge?'),
            lp('Gurpreet', 'Certo, lo metto nello zaino insieme alle medicine: le porto sempre!', 'Of course, I\'ll put it in my backpack with the medicines: I always bring them!', 'Zarur, main is nu davaiaan naal bag vich rakh lainda haan: main inhaan nu hamesha lai ke jaanda haan!'),
          ],
        },
        {
          title: 'Botta e risposta veloce',
          text: '- **Studente A chiede:** «Bevi il tè? Compri il pane? Prendi lo sciroppo? Guardi i film?»\n- **Studente B risponde subito con il pronome:** «Sì, lo bevo! / No, non lo compro! / Sì, li guardo!»',
        },
      ],
      exercises: [
        tf('Gurpreet compra i biglietti online.', true, 'Gurpreet buys the tickets online.'),
        tf('Simran prepara la valigia domani.', false, 'Simran packs the suitcase tomorrow.'),
        tf('Gurpreet porta sempre le medicine.', true, 'Gurpreet always brings the medicines.'),
        fb('Il passaporto? ___ metto nello zaino.', ['Lo', 'La', 'Li', 'Le']),
        fb('La valigia? ___ preparo stasera.', ['La', 'Lo', 'Le', 'Li']),
        wo('Li compro subito online', 'I\'m buying them online right now'),
      ],
      homework: 'Play "botta e risposta" with a partner: 10 quick questions, answers with lo, la, li, le.',
    },
    {
      kind: 'Review',
      title: 'Lesson 3 · Practice: lo, la, li, le',
      titleIt: 'Lezione 3 · Esercizi sui pronomi diretti',
      summary: 'Quick practice with food, shopping and everyday things.',
      minutes: 8,
      sections: [
        {
          title: 'Ricorda',
          text: '- **lo** = il / lo (masc. sing.) · **la** = la (fem. sing.)\n- **li** = i / gli (masc. pl.) · **le** = le (fem. pl.)\n- Always **before** the verb: *lo prendo, non lo prendo* · joined to an infinitive: *devo prenderlo*',
        },
      ],
      exercises: [
        fb('Compri il pane? — Sì, ___ compro.', ['lo', 'la', 'li', 'le']),
        fb('Mangi la pizza? — Sì, ___ mangio.', ['la', 'lo', 'le', 'li']),
        fb('Guardi i film italiani? — Sì, ___ guardo.', ['li', 'le', 'lo', 'la']),
        fb('Chiami le tue sorelle? — Sì, ___ chiamo ogni domenica.', ['le', 'li', 'la', 'lo']),
        fb('Leggi il giornale? — No, non ___ leggo.', ['lo', 'la', 'li', 'le']),
        fb('Devo comprare le uova. Devo ___ oggi.', ['comprarle', 'comprarli', 'le comprare', 'comprarla']),
        fb('Il caffè? ___ bevo senza zucchero.', ['Lo', 'La', 'Li', 'Le']),
      ],
      homework: 'Write 5 sentences about what you buy every week with lo, la, li, le.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — L'imperativo
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 4 · The imperative: tu and Lei',
      titleIt: 'Lezione 4 · L\'imperativo informale (tu) e formale (Lei)',
      summary: 'Prendi! / Prenda! · Riposa! / Riposi! · Non uscire!',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 10 · Lezione 4 (PDF)' },
        {
          title: 'Chi parla e a chi?',
          table: [
            ['Con un amico / familiare (TU)', 'Dal medico / in ufficio (LEI)'],
            ['Prendi questa compressa!', 'Prenda due compresse al giorno.'],
            ['Bevi molta acqua calda!', 'Aspetti un momento in sala.'],
            ['Riposa a casa oggi!', 'Firmi qui sul modulo, per favore.'],
          ],
        },
        {
          title: 'Come si forma l\'imperativo',
          table: [
            ['Infinito', 'TU', 'LEI', 'English · Punjabi'],
            ['riposare (-ARE)', 'riposa!', 'riposi!', 'Rest! · Aaram karo!'],
            ['aspettare (-ARE)', 'aspetta!', 'aspetti!', 'Wait! · Intazar karo!'],
            ['firmare (-ARE)', 'firma!', 'firmi!', 'Sign! · Dastakhat karo!'],
            ['prendere (-ERE)', 'prendi!', 'prenda!', 'Take! · Lao!'],
            ['bere (irregolare)', 'bevi!', 'beva!', 'Drink! · Pio!'],
            ['aprire / sentire (-IRE)', 'apri! / senti!', 'apra! / senta!', 'Open / Listen! · Kholo / Suno!'],
          ],
        },
        { tip: 'The endings **swap**: -ARE verbs → tu **-a**, Lei **-i** · -ERE / -IRE verbs → tu **-i**, Lei **-a**.' },
        {
          title: 'Imperativo negativo (tu): NON + infinito',
          table: [
            ['Situazione', 'Consiglio'],
            ['con la febbre', 'Non uscire! (never ~~non esci~~)'],
            ['con mal di pancia', 'Non mangiare troppo!'],
            ['con mal di schiena', 'Non alzare pesi!'],
          ],
        },
      ],
      exercises: [
        fb('(tu) Hai la febbre! ___ a casa.', ['Riposa', 'Riposi', 'Riposare', 'Riposo']),
        fb('(tu) ___ oggi!', ['Non uscire', 'Non esci', 'Non esca', 'Non uscite']),
        fb('(Lei) Signor Singh, ___ un attimo, per favore.', ['aspetti', 'aspetta', 'aspettare', 'aspetto']),
        fb('(Lei) ___ qui, per favore.', ['Firmi', 'Firma', 'Firmare', 'Firmo']),
        fb('(Lei) ___ questa medicina due volte al giorno.', ['Prenda', 'Prendi', 'Prende', 'Prendere']),
        fb('(tu) ___ molta acqua calda con miele!', ['Bevi', 'Beva', 'Bere', 'Bevo']),
        fb('(tu) Hai mal di pancia: ___ troppo!', ['non mangiare', 'non mangi', 'non mangia', 'mangia non']),
        mc('The doctor (Lei) says: "___ la bocca, per favore."', 'Apra', 'Apri', 'Aprire', 'Apre'),
      ],
      homework: 'Write 3 pieces of advice for a friend with the flu (2 positive, 1 negative).',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 4 · Formal instructions: at the doctor\'s and at the office',
      titleIt: 'Lezione 4 · Istruzioni formali dal medico e allo sportello',
      summary: 'Prenda, beva, riposi, metta, torni · Entri, si accomodi, compili il modulo, vada allo sportello.',
      minutes: 10,
      sections: [
        {
          title: 'Le prescrizioni del medico (Lei)',
          practice: false,
          words: [
            wp('Prenda questo sciroppo tre volte al giorno dopo i pasti.', 'Take this syrup three times a day after meals.', 'Eh sharbat din vich tinn vaar khaane ton baad lao.'),
            wp('Beva molta acqua e riposi almeno tre giorni a casa.', 'Drink a lot of water and rest at home for at least three days.', 'Bahut paani pio ate ghatto ghatt tinn din ghar aaram karo.'),
            wp('Metta questa pomata sulla schiena mattina e sera.', 'Put this ointment on your back morning and evening.', 'Eh maraham savere ate shaam pitth te lao.'),
            wp('Se ha ancora febbre lunedì, torni in ambulatorio!', 'If you still have a temperature on Monday, come back to the surgery!', 'Je somvar nu vi bukhar hove, taan klinik vaapas ao!'),
          ],
        },
        {
          title: 'Allo sportello: Comune, posta, banca, questura',
          words: [
            wp('Entri pure e si accomodi!', 'Please come in and have a seat!', 'Andar ao ate baitho ji!', { img: 'Chair' }),
            wp('Aspetti un attimo.', 'Wait a moment.', 'Ikk pal intazar karo.', { img: 'Hourglass done', ex: 'Aspetti un attimo, controllo i documenti.', exEn: 'Wait a moment, I\'m checking the documents.' }),
            wp('Compili questo modulo.', 'Fill in this form.', 'Eh faaram bharo.', { img: 'Clipboard' }),
            wp('Firmi qui in basso.', 'Sign here at the bottom.', 'Ithe hethaan dastakhat karo.', { img: 'Writing hand' }),
            wp('Vada allo sportello numero 4.', 'Go to window number 4.', 'Khidki nambar 4 te jao.', { img: 'Keycap 4' }),
          ],
        },
        { tip: 'You hear these forms at the **Comune**, the **posta**, the **banca** and the **questura** (for the *permesso di soggiorno*). They are polite orders: just follow them!' },
      ],
      exercises: [
        mc('"Compili questo modulo" means…', 'Fill in this form', 'Sign this form', 'Read this form', 'Buy this form'),
        mc('"Si accomodi" means…', 'Have a seat', 'Go away', 'Wait outside', 'Stand up'),
        mc('"Vada allo sportello numero 4" means…', 'Go to window number 4', 'Wait for 4 minutes', 'Take ticket number 4', 'Come back on the 4th'),
        fb('Beva molta acqua e ___ almeno tre giorni.', ['riposi', 'riposa', 'riposare', 'riposo']),
        fb('Se ha ancora febbre lunedì, ___ in ambulatorio!', ['torni', 'torna', 'tornare', 'torno']),
      ],
      homework: 'Write the instructions you heard the last time you were at an office in Italy.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 4 · Practice: advice for a friend, instructions for a patient',
      titleIt: 'Lezione 4 · Consigli e istruzioni in coppia',
      summary: 'Situation A: tu (a sick friend) · Situation B: Lei (doctor or office).',
      minutes: 10,
      sections: [
        {
          title: 'Dai 3 consigli o istruzioni al tuo compagno',
          text: '- **Situazione A · tu (amico malato):** il compagno ha mal di gola e febbre — usa 2 imperativi positivi e 1 negativo (*Bevi…, riposa…, non uscire!*).\n- **Situazione B · Lei (medico o ufficio):** usa *prenda, beva, aspetti, firmi qui, compili il modulo*.',
        },
        {
          title: 'Esempio: un amico con l\'influenza',
          situation: 'Ravi phones his friend Karan, who is ill.',
          dialogue: [
            lp('Ravi', 'Ciao Karan, come stai?', 'Hi Karan, how are you?', 'Sat Sri Akal Karan, ki haal hai?'),
            lp('Karan', 'Male: ho la febbre e mi fa male la gola.', 'Bad: I have a temperature and a sore throat.', 'Theek nahi: bukhar hai ate gala kharab hai.'),
            lp('Ravi', 'Allora riposa, bevi molta acqua calda e non uscire oggi!', 'Then rest, drink a lot of hot water and don\'t go out today!', 'Phir aaram kar, bahut garam paani pi ate ajj baahar na jai!'),
            lp('Karan', 'Grazie, domani vado dal medico.', 'Thanks, tomorrow I\'ll go to the doctor.', 'Dhanvad, kall main daaktar kol jaavanga.'),
          ],
        },
      ],
      exercises: [
        tf('Karan ha la febbre.', true, 'Karan has a temperature.'),
        tf('Ravi dice a Karan di uscire.', false, 'Ravi tells Karan to go out.'),
        mc('Which sentence is informal (tu)?', 'Bevi molta acqua!', 'Beva molta acqua!', 'Firmi qui!', 'Prenda la compressa!'),
        mc('Which sentence is formal (Lei)?', 'Aspetti un attimo.', 'Aspetta un attimo.', 'Non uscire!', 'Riposa!'),
        wo('Riposa e non uscire oggi', 'Rest and don\'t go out today'),
        wo('Prenda queste compresse dopo cena', 'Take these tablets after dinner'),
      ],
      homework: 'Do situations A and B with a partner, then change roles.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 10
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 10 test',
      titleIt: 'Test Capitolo 10',
      summary: 'Listening, body and health words, mi fa / fanno male, lo / la / li / le and the imperative. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 10',
          text: 'This test checks everything in Chapter 10 (Lessons 1–4).\n\n- **1. Listening** — sentences and a dialogue at the doctor\'s\n- **2. Vocabulary** — body, symptoms, doctor and pharmacy\n- **3. Grammar** — mi fa / mi fanno male, lo / la / li / le, the imperative (tu and Lei)\n- **4. Writing** — symptoms and health advice\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (symptoms and advice) and the **oral interview** (where it hurts, at the pharmacy with lo / la / li / le, advice with the imperative — 2 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A
        lis('1A · Listen and choose the sentence you hear', 'Oggi non vado al lavoro perché ho mal di testa e ho la febbre.', 'HO MAL DI TESTA E HO LA FEBBRE', 'MI FANNO MALE I DENTI DA IERI'),
        lis('1A · Listen and choose the sentence you hear', 'Buongiorno, ho una ricetta medica del dottore per queste medicine.', 'HO UNA RICETTA MEDICA DEL DOTTORE', 'VORREI UNO SCIROPPO PER LA TOSSE'),
        lis('1A · Listen and choose the sentence you hear', 'Prendi la medicina? Sì, la prendo due volte al giorno.', 'LA PRENDO DUE VOLTE AL GIORNO', 'LI COMPRO SUBITO IN FARMACIA'),
        lis('1A · Listen and choose the sentence you hear', 'Allo sportello l\'impiegato dice: aspetti un attimo e firmi qui.', 'ASPETTI UN ATTIMO E FIRMI QUI', 'PRENDA QUESTE COMPRESSE DOPO CENA'),
        lis('1A · Listen and choose the sentence you hear', 'Hai il raffreddore: non uscire oggi, riposa a casa!', 'NON USCIRE OGGI, RIPOSA A CASA!', 'BEVI MOLTA ACQUA CALDA CON MIELE!'),
        // 1B
        lis('1B · Listen: what hurts? (1)', 'Buongiorno dottore, non sto bene: mi fa male la gola.', 'GOLA', 'GAMBA', 'MANO', 'PANCIA'),
        lis('1B · Listen again: "ho un forte…" (2)', 'Ho un forte raffreddore.', 'RAFFREDDORE', 'MAL DI DENTI', 'FEBBRE', 'SCIROPPO'),
        lis('1B · Listen again: what does the doctor write? (3)', 'Va bene, Le scrivo una ricetta per una medicina.', 'RICETTA', 'LETTERA', 'POMATA', 'VISITA'),
        lis('1B · Listen again: "___ questa medicina ogni sera" (4)', 'Prenda questa medicina ogni sera per cinque giorni.', 'PRENDA', 'PRENDI', 'PRENDO', 'PRENDE'),
        lis('1B · Listen again: "e ___ molto durante il giorno" (5)', 'E beva molto durante il giorno.', 'BEVA', 'BEVI', 'BEVO', 'BERE'),
        // 2A
        pairs('2A · Match each word with its meaning', [
          ['Schiena', 'Back · Pitth'],
          ['Ricetta medica', 'Prescription · Daaktar di parchi'],
          ['Sciroppo', 'Syrup · Sharbat'],
          ['Pomata', 'Ointment · Maraham'],
          ['Allergia', 'Allergy · Alarji'],
        ]),
        // 2B
        mc('2B · Ho mangiato troppo, ho mal di ?', 'pancia', 'febbre', 'farmacia', 'visita'),
        mc('2B · Ho 38.5 di temperatura: ho la ?', 'febbre', 'pancia', 'visita', 'compresse'),
        mc('2B · Compro le medicine in ?', 'farmacia', 'febbre', 'pancia', 'visita'),
        mc('2B · Prendo due ? dopo pranzo.', 'compresse', 'visite', 'farmacie', 'febbri'),
        mc('2B · Oggi pomeriggio ho una ? medica.', 'visita', 'pancia', 'compresse', 'febbre'),
        // 3A
        fb('3A · Ho camminato molto oggi e ___ le gambe.', ['mi fanno male', 'mi fa male']),
        fb('3A · Ho lavorato tanto e adesso ___ il braccio destro.', ['mi fa male', 'mi fanno male']),
        fb('3A · Non posso mangiare cose fredde perché ___ i denti.', ['mi fanno male', 'mi fa male']),
        // 3B
        fb('3B · Prendi lo sciroppo la sera? — Sì, ___ prendo prima di dormire.', ['lo', 'la', 'li', 'le']),
        fb('3B · Porti la ricetta medica? — Sì, ___ porto nella borsa.', ['la', 'lo', 'li', 'le']),
        fb('3B · Compri i farmaci oggi? — Sì, ___ compro subito.', ['li', 'le', 'lo', 'la']),
        fb('3B · Prendi le compresse con acqua? — Sì, ___ prendo con un bicchiere d\'acqua.', ['le', 'li', 'la', 'lo']),
        fb('3B · Metti la pomata sul braccio? — No, non ___ metto adesso.', ['la', 'lo', 'le', 'li']),
        // 3C
        fb('3C · (Lei) «Signor Singh, ___ un attimo…»', ['aspetti', 'aspetta']),
        fb('3C · (Lei) «… e ___ qui, per favore.»', ['firmi', 'firma']),
        fb('3C · (tu) «Hai la febbre! ___ a casa…»', ['Riposa', 'Riposi']),
        fb('3C · (tu) «… e ___ oggi!»', ['non uscire', 'non esci']),
        fb('3C · (Lei) «___ questa medicina due volte al giorno dopo i pasti.»', ['Prenda', 'Prendi']),
        // 4
        wo('Oggi non sto bene ho mal di testa', 'Today I don\'t feel well, I have a headache'),
        wo('Bevi molta acqua e non prendere freddo', 'Drink a lot of water and don\'t catch cold'),
        wo('Prenda queste compresse e riposi a casa', 'Take these tablets and rest at home'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences about a health problem and give advice with the imperative. Then prepare the 2-minute oral interview.',
    },
  ],
};
