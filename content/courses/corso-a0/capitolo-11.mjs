import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 11 of the school syllabus (Corso di Italiano, livello A2).
// Source: syllabus/Capitolo 11 Lezione 1–4 (.pptx) and Test Capitolo 11 (.docx).
// Corrections to the client material:
//   - Test 3C.1 "L'aereo è più veloce (di / che) treno" needs the article: "del treno".
//   - Test 2B.3 calls "migliore" the superlative of buono; it is the comparative
//     (superlatives: il migliore / buonissimo). The app asks for the comparative.
//   - Test 2B.5 "Mentre dormivo, è suonato il bus" → "Mentre dormivo, ha suonato la sveglia".

const slides = (lez) => `/content/slides/corso-a0-cap11-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-11',
  title: 'Chapter 11: The imperfect, comparisons — then and now',
  titleIt: 'Capitolo 11: L\'imperfetto, i paragoni, ieri e oggi',
  cover: 'Framed picture',
  description: 'The imperfetto (parlavo, prendevo, dormivo; ero, avevo, facevo) for habits, descriptions and memories, imperfetto or passato prossimo (mentre dormivo, ha suonato il telefono), comparatives and superlatives (più … di / che, meno, come, il più grande, bellissimo, migliore), and comparing life in your home country and in Italy, city and countryside.',
  canDo: [
    'I can form the imperfetto, including ero, avevo, facevo, dicevo, bevevo',
    'I can talk about childhood habits and describe places and people in the past',
    'I can choose imperfetto (background, habit) or passato prossimo (a single event)',
    'I can compare two things: più … di / che, meno … di, come',
    'I can use superlatives: il più grande, bellissimo, migliore, peggiore',
    'I can compare my life before and now, my country and Italy, the city and the countryside',
  ],
  liveClassFocus: 'Childhood interview ("Com\'era la tua infanzia?"), then the "Mentre…" game (background + sudden event) and a group discussion comparing your home country and Italy.',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — L'imperfetto
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 1 · The imperfetto: regular and irregular forms',
      titleIt: 'Lezione 1 · L\'imperfetto: forme regolari e irregolari',
      summary: 'Parlavo, prendevo, dormivo — and ero, avevo, facevo, dicevo, bevevo.',
      minutes: 12,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 11 · Lezione 1 (PDF)' },
        {
          title: 'Verbi regolari: sempre la stessa musica (-V-)',
          table: [
            ['Persona', 'PARLARE', 'PRENDERE', 'DORMIRE', 'English', 'Punjabi'],
            ['io', 'parlavo', 'prendevo', 'dormivo', 'I used to speak', 'main bolda si'],
            ['tu', 'parlavi', 'prendevi', 'dormivi', 'you used to speak', 'tu bolda si'],
            ['lui / lei', 'parlava', 'prendeva', 'dormiva', 'he / she used to speak', 'uh bolda / boldi si'],
            ['noi', 'parlavamo', 'prendevamo', 'dormivamo', 'we used to speak', 'asi bolde si'],
            ['voi', 'parlavate', 'prendevate', 'dormivate', 'you (pl.) used to speak', 'tusi bolde si'],
            ['loro', 'parlavano', 'prendevano', 'dormivano', 'they used to speak', 'uh bolde san'],
          ],
        },
        {
          title: 'Forme irregolari: solo 5 radici',
          table: [
            ['Infinito', 'io', 'tu', 'lui / lei', 'noi / voi / loro'],
            ['ESSERE', 'ero', 'eri', 'era', 'eravamo / eravate / erano'],
            ['AVERE (regolare)', 'avevo', 'avevi', 'aveva', 'avevamo / avevate / avevano'],
            ['FARE', 'facevo', 'facevi', 'faceva', 'facevamo / facevate / facevano'],
            ['DIRE', 'dicevo', 'dicevi', 'diceva', 'dicevamo / dicevate / dicevano'],
            ['BERE', 'bevevo', 'bevevi', 'beveva', 'bevevamo / bevevate / bevevano'],
          ],
        },
        { tip: 'The imperfetto is "used to / was …ing": *parlavo* = I used to speak / I was speaking. In Punjabi it is like **bolda si**.' },
      ],
      exercises: [
        fb('Quando io ___ piccolo, avevo una bicicletta rossa.', ['ero', 'era', 'sono stato', 'eri']),
        fb('Quando ero piccolo, ___ una bicicletta rossa.', ['avevo', 'aveva', 'ho avuto', 'avevi']),
        fb('Da bambini noi ___ sempre in giardino.', ['giocavamo', 'giocavate', 'giocavano', 'giocavo']),
        fb('Da bambini noi ___ succo di frutta.', ['bevevamo', 'bevavamo', 'beviamo', 'bevevano']),
        fb('Mio nonno ___ il panettiere.', ['faceva', 'fava', 'facevo', 'ha fatto']),
        fb('Mio nonno ___ alle quattro ogni mattina.', ['si alzava', 'si alzavo', 'alzava', 'si è alzato']),
        fb('Tu ___ sempre la verità.', ['dicevi', 'dicevo', 'divi', 'diceva']),
        fb('Loro ___ in una casa grande.', ['abitavano', 'abitavamo', 'abitava', 'abitavate']),
      ],
      homework: 'Write the imperfetto of essere, avere and fare for all persons from memory.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · When to use the imperfetto',
      titleIt: 'Lezione 1 · Le 3 funzioni dell\'imperfetto',
      summary: 'Past habits, descriptions, time / age / feelings — «Quando ero piccolo…».',
      minutes: 10,
      sections: [
        {
          title: '3 funzioni fondamentali',
          table: [
            ['Funzione', 'Esempio'],
            ['1. Abitudini nel passato (puraniaan aadataan)', 'Da bambino giocavo sempre fuori.'],
            ['2. Descrizioni di persone e luoghi', 'La mia casa aveva un giardino grande.'],
            ['3. Tempo, età ed emozioni', 'Era estate, faceva caldo ed ero felice.'],
          ],
        },
        {
          title: '«Quando ero piccolo / piccola…»',
          practice: false,
          words: [
            wp('Quando ero piccolo, abitavo in un piccolo villaggio.', 'When I was little, I lived in a small village.', 'Jadon main chhota si, main ikk chhote pind vich rahinda si.'),
            wp('Ogni mattina andavo a scuola a piedi e bevevo latte caldo.', 'Every morning I walked to school and drank hot milk.', 'Har savere main paidal sakul jaanda si ate garam duddh pinda si.'),
            wp('Il pomeriggio facevo i compiti e poi giocavo con gli amici.', 'In the afternoon I did my homework and then played with friends.', 'Dupahir nu main ghar da kamm karda si ate phir dostaan naal khedda si.'),
            wp('Non avevamo il cellulare, ma stavamo sempre insieme!', 'We didn\'t have mobile phones, but we were always together!', 'Saade kol mobail nahi si, par asi hamesha ikatthe rahinde si!'),
          ],
        },
        {
          title: 'Parole segnale',
          words: [
            wp('sempre', 'always', 'Hamesha', { img: 'Repeat button' }),
            wp('ogni giorno', 'every day', 'Har roz', { img: 'Calendar' }),
            wp('di solito', 'usually', 'Aamtaur te', { img: 'Spiral calendar' }),
            wp('da bambino / da bambina', 'as a child', 'Bachpan vich', { img: 'Child' }),
            wp('quando ero giovane', 'when I was young', 'Jadon main javan si', { img: 'Boy' }),
          ],
        },
      ],
      exercises: [
        mc('"Da bambino giocavo sempre fuori" is…', 'a past habit', 'a single event', 'the future', 'an order'),
        mc('"Era estate e faceva caldo" describes…', 'the weather and the time', 'a sudden event', 'a habit today', 'a plan'),
        fb('Ogni mattina ___ a scuola a piedi.', ['andavo', 'sono andato', 'vado', 'andrò']),
        fb('Non ___ il cellulare.', ['avevamo', 'abbiamo avuto', 'avevate', 'avevano']),
        tr('di solito', 'usually', 'never', 'suddenly', 'yesterday'),
      ],
      homework: 'Write 5 sentences beginning with «Quando ero piccolo / piccola…».',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Reading: summers at my grandparents\'',
      titleIt: 'Lezione 1 · Lettura: le estati dai miei nonni',
      summary: 'A childhood memory in the imperfetto — then interview your partner.',
      minutes: 10,
      sections: [
        {
          title: 'C\'era una volta…',
          reading: {
            title: 'Le estati dai miei nonni',
            it: 'Quando avevo dieci anni, d\'estate andavo sempre in campagna dai miei nonni. La loro casa era grande e aveva tanti alberi da frutto.\n\nLa nonna preparava sempre dolci buoni e diceva: «Mangia, che devi crescere!».\n\nLa sera faceva fresco, guardavamo le stelle e dormivamo con le finestre aperte. Erano giorni bellissimi!',
            en: 'When I was ten, in summer I always went to the countryside to my grandparents\'. Their house was big and had lots of fruit trees.\n\nGrandma always made good sweets and said: "Eat, you need to grow!".\n\nIn the evening it was cool, we watched the stars and slept with the windows open. They were wonderful days!',
          },
        },
        {
          title: 'Com\'era la tua infanzia? Intervista il compagno',
          text: '- **Chiedi abitudini e gusti:** «Dove abitavi? Che sport facevi? Cosa mangiavi sempre?»\n- **Rispondi all\'imperfetto:** inizia con «Quando ero…» e usa *ero, avevo, abitavo, giocavo, andavo, facevo*.',
        },
      ],
      exercises: [
        tf('D\'estate andava in campagna dai nonni.', true, 'In summer he / she went to the grandparents\' in the countryside.'),
        tf('La casa dei nonni era piccola.', false, 'The grandparents\' house was small.'),
        tf('La nonna preparava dolci.', true, 'Grandma made sweets.'),
        tf('Dormivano con le finestre chiuse.', false, 'They slept with the windows closed.'),
        wo('Quando avevo dieci anni andavo dai nonni', 'When I was ten I used to go to my grandparents\''),
        wo('La sera guardavamo le stelle', 'In the evening we watched the stars'),
      ],
      homework: 'Interview a partner about their childhood and write 5 sentences (Quando era piccolo, lui…).',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — Passato prossimo vs imperfetto
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Imperfetto (the film) vs passato prossimo (the photo)',
      titleIt: 'Lezione 2 · Imperfetto o passato prossimo?',
      summary: 'Background and habit vs a single, finished event — and the signal words.',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 11 · Lezione 2 (PDF)' },
        {
          title: 'Due tempi del passato a confronto',
          table: [
            ['IMPERFETTO (sfondo / abitudine)', 'PASSATO PROSSIMO (evento puntuale)'],
            ['long, repeated action or description', 'single, finished or sudden action'],
            ['Quando ero giovane, correvo ogni giorno.', 'Ieri mattina ho corso dieci chilometri.'],
            ['Punjabi: aadat jaan maahaul', 'Punjabi: ikk vaar hoi ghatna'],
          ],
        },
        {
          title: 'Le parole segnale del tempo',
          table: [
            ['Con l\'IMPERFETTO', 'Esempio', 'Con il PASSATO PROSSIMO', 'Esempio'],
            ['sempre / di solito', 'Andavo sempre in bici.', 'ieri / stamattina', 'Ieri sono andato in auto.'],
            ['ogni giorno / tutti i lunedì', 'Leggeva ogni sera.', 'una volta / due volte', 'Ho letto quel libro una volta.'],
            ['da bambino / da giovane', 'Da piccolo ero timido.', 'l\'anno scorso / nel 2024', 'Nel 2024 sono venuto qui.'],
            ['mentre', 'Mentre dormivo…', 'all\'improvviso / subito', '…ha suonato il telefono!'],
          ],
        },
        {
          title: '3 regole d\'oro',
          table: [
            ['Regola', 'Esempio'],
            ['1. Time and weather → always imperfetto', 'Erano le nove e c\'era il sole.'],
            ['2. The action that interrupts → passato prossimo', 'Leggevo quando è entrata Sara.'],
            ['3. An exact duration → passato prossimo', 'Ho studiato due ore ieri.'],
          ],
        },
      ],
      exercises: [
        fb('Da bambina mia sorella ___ sempre latte e biscotti.', ['beveva', 'ha bevuto', 'bevuto', 'beve']),
        fb('Ieri sera ___ alle otto.', ['siamo usciti', 'uscivamo', 'usciamo', 'siamo uscito']),
        fb('Sabato scorso io ___ una giacca nuova in centro.', ['ho comprato', 'compravo', 'comprato', 'ero comprato']),
        fb('Prima di venire in Italia, noi ___ vicino a Jalandhar.', ['abitavamo', 'abbiamo abitato una volta', 'abitiamo', 'abitavano']),
        fb('L\'anno scorso ___ a Roma per la prima volta.', ['sono andato', 'andavo', 'vado', 'ho andato']),
        fb('Ieri ___ due ore.', ['ho studiato', 'studiavo', 'studio', 'studiavamo']),
        fb('___ le nove e c\'era il sole.', ['Erano', 'Sono state', 'Sono', 'Era']),
      ],
      homework: 'Write 3 sentences about your habits as a child and 3 about things you did last week.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Mentre + imperfetto, passato prossimo',
      titleIt: 'Lezione 2 · Un\'azione interrompe un\'altra',
      summary: 'Mentre dormivo, ha suonato il telefono — and a rainy morning in Mantova.',
      minutes: 12,
      sections: [
        {
          title: 'Mentre + imperfetto, passato prossimo',
          practice: false,
          words: [
            wp('Mentre dormivo, ha suonato il telefono.', 'While I was sleeping, the phone rang.', 'Jadon main suta pia si, fon vajjia.'),
            wp('Mentre cucinavo la cena, è arrivato mio fratello.', 'While I was cooking dinner, my brother arrived.', 'Jadon main raat da khaana bana riha si, mera bhara aa gia.'),
            wp('Mentre aspettavamo il treno, abbiamo preso un caffè.', 'While we were waiting for the train, we had a coffee.', 'Jadon asi tren di udik kar rahe si, asi kauffi piti.'),
          ],
        },
        {
          title: 'Una mattina di pioggia a Mantova',
          reading: {
            title: 'Una mattina di pioggia a Mantova',
            it: 'Ieri mattina era lunedì. Fuori pioveva forte, faceva freddo e il cielo era grigio.\n\nIo ho preso l\'ombrello e sono uscito di casa alle otto.\n\nMentre camminavo verso la stazione, ho incontrato il mio amico Harpreet e siamo andati al bar insieme.',
            en: 'Yesterday morning it was Monday. Outside it was raining hard, it was cold and the sky was grey.\n\nI took my umbrella and left home at eight.\n\nWhile I was walking to the station, I met my friend Harpreet and we went to the café together.',
          },
        },
        {
          title: 'Gioco: completa la frase con un evento improvviso',
          text: '- **Studente A — lo sfondo (imperfetto):** «Mentre guidavo… / Mentre studiavo… / Mentre facevo la spesa…»\n- **Studente B — l\'evento (passato prossimo):** «…ho visto un vecchio amico! / …è andata via la luce!»',
        },
      ],
      exercises: [
        fb('Mentre ___ la TV, è arrivato mio cugino.', ['guardavo', 'ho guardato', 'guardo', 'guarderò']),
        fb('Mentre guardavo la TV, ___ mio cugino.', ['è arrivato', 'arrivava', 'arriva', 'ha arrivato']),
        fb('Mentre io guardavo la TV, mia madre ___.', ['ha chiamato', 'chiamava', 'chiama', 'è chiamata']),
        fb('Ieri sera ___ forte quando siamo usciti dal lavoro.', ['pioveva', 'è piovuto', 'piove', 'piovevano']),
        fb('Quando sono partito per il lavoro, ___ molto.', ['nevicava', 'ha nevicato', 'nevica', 'nevicavo']),
        tf('Nella lettura pioveva forte.', true, 'In the reading it was raining hard.'),
        tf('Ha incontrato Harpreet in stazione.', false, 'He met Harpreet at the station.'),
      ],
      homework: 'Play the "Mentre…" game with a partner and write your 5 best sentences.',
    },
    {
      kind: 'Review',
      title: 'Lesson 2 · Practice: imperfetto or passato prossimo?',
      titleIt: 'Lezione 2 · Esercizi: imperfetto o passato prossimo?',
      summary: 'The class exercise from the slides — choose the right form.',
      minutes: 8,
      sections: [
        {
          title: 'Ricorda',
          text: '- **Imperfetto:** background, habit, description, time, weather, age, feelings — *sempre, di solito, ogni giorno, mentre*\n- **Passato prossimo:** a single finished event, a sudden action, an exact duration — *ieri, una volta, all\'improvviso, due ore*',
        },
      ],
      exercises: [
        fb('Mentre ___ la TV, è arrivato mio cugino.', ['guardavo', 'ho guardato']),
        fb('Da bambina mia sorella ___ sempre latte e biscotti.', ['beveva', 'ha bevuto']),
        fb('Ieri sera siamo usciti alle otto e ___ in pizzeria.', ['siamo andati', 'andavamo']),
        fb('Quando sono partito per il lavoro, ___ molto.', ['nevicava', 'ha nevicato']),
        fb('Sabato scorso io ___ una giacca nuova in centro.', ['ho comprato', 'compravo']),
        fb('Prima di venire in Italia, noi ___ vicino a Jalandhar.', ['abitavamo', 'abbiamo abitato']),
        fb('Prima di venire in Italia, tu ___ tutti i giorni in un negozio?', ['lavoravi', 'hai lavorato']),
        fb('Leggevo quando ___ Sara.', ['è entrata', 'entrava']),
      ],
      homework: 'Tell a partner about a day when something unexpected happened (Mentre…, all\'improvviso…).',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — Comparativi e superlativi
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 3 · Comparatives: più, meno, come',
      titleIt: 'Lezione 3 · I comparativi: più … di / che, meno, come',
      summary: 'Milano è più grande di Mantova · più … di or più … che?',
      minutes: 12,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 11 · Lezione 3 (PDF)' },
        {
          title: 'I tre tipi di comparativo',
          table: [
            ['Tipo', 'Forma', 'Esempio'],
            ['maggioranza (+)', 'più … di / che', 'Milano è più grande di Mantova.'],
            ['minoranza (−)', 'meno … di / che', 'Il treno è meno veloce dell\'aereo.'],
            ['uguaglianza (=)', 'come / quanto', 'Sara è alta come sua sorella.'],
          ],
        },
        {
          title: 'Più … DI o più … CHE?',
          table: [
            ['«DI» — two nouns or pronouns', '«CHE» — two adjectives, verbs or quantities'],
            ['Roma è più grande di Torino.', 'Questo lavoro è più faticoso che difficile.'],
            ['Marco è più alto di me.', 'Leggere è più rilassante che guardare la TV.'],
            ['L\'oro costa più dell\'argento.', 'Ci sono più bici che macchine.'],
          ],
        },
        { tip: '**Di** joins the article: più veloce **del** treno (di + il), meno caro **dell\'**albergo, più alta **della** sorella.' },
        {
          title: 'Esempi reali',
          practice: false,
          words: [
            wp('Il mio telefono nuovo è più leggero del tuo.', 'My new phone is lighter than yours.', 'Mera navaan fon tuhade naalon halka hai.'),
            wp('Questo ristorante è meno caro di quello in centro.', 'This restaurant is cheaper than the one in the centre.', 'Eh restoraint shahir vaale naalon ghatt mahinga hai.'),
            wp('Il caffè italiano è buono come il tè speziato!', 'Italian coffee is as good as spiced tea!', 'Italvi kauffi masale vaali chaah jinni hi vadhia hai!'),
          ],
        },
      ],
      exercises: [
        fb('L\'aereo è molto più veloce ___ treno.', ['del', 'che', 'di', 'come']),
        fb('Vivere in campagna è meno caro ___ vivere in centro città.', ['che', 'di', 'del', 'come']),
        fb('Il mio paese d\'origine è caldo ___ il sud Italia in estate.', ['come', 'di', 'che', 'più']),
        fb('Marco è più alto ___ me.', ['di', 'che', 'del', 'come']),
        fb('Ci sono più bici ___ macchine.', ['che', 'di', 'del', 'come']),
        fb('Milano è più grande ___ Mantova.', ['di', 'che', 'del', 'della']),
        fb('Il treno è meno veloce ___ aereo.', ['dell\'', 'che', 'di', 'del']),
      ],
      homework: 'Write 5 comparisons between two cities, two people or two things you know.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · Superlatives and special forms',
      titleIt: 'Lezione 3 · Superlativi e forme speciali',
      summary: 'Il più grande, bellissimo, buonissimo · migliore, peggiore, maggiore, minore.',
      minutes: 12,
      sections: [
        {
          title: 'Superlativo relativo e assoluto',
          table: [
            ['Aggettivo', 'Relativo (in un gruppo)', 'Assoluto (-issimo)', 'English · Punjabi'],
            ['grande', 'il più grande (d\'Italia)', 'grandissimo', 'the biggest / very big · sab ton vadda'],
            ['bello / bella', 'la più bella (della città)', 'bellissima', 'the prettiest / very pretty · sab ton sohni'],
            ['buono / buona', 'il più buono (del menu)', 'buonissimo', 'the tastiest / delicious · bahut savad'],
            ['caldo / fredda', 'il più caldo (dell\'anno)', 'caldissimo / freddissima', 'hottest / freezing · sab ton garam / bahut thanda'],
            ['simpatico / a', 'i più simpatici (della classe)', 'simpaticissimi', 'the nicest / super nice · sab ton changa / bahut changa'],
          ],
        },
        {
          title: 'Forme speciali molto usate',
          words: [
            wp('buono → migliore / ottimo', 'good → better / excellent', 'Vadhia / Uttam', { img: 'Thumbs up', ex: 'Questa pizza è migliore di quella.', exEn: 'This pizza is better than that one.' }),
            wp('cattivo → peggiore / pessimo', 'bad → worse / terrible', 'Hor kharab', { img: 'Thumbs down', ex: 'Il tempo oggi è peggiore di ieri.', exEn: 'The weather today is worse than yesterday.' }),
            wp('grande / piccolo → maggiore / minore', 'older / younger (brother, sister)', 'Vadda / Chhota', { img: 'People hugging', ex: 'Mio fratello maggiore ha 30 anni.', exEn: 'My older brother is 30.' }),
          ],
        },
        { tip: 'Never say ~~più buonissimo~~ or ~~più migliore~~: *-issimo* and *migliore* already mean "very / more".' },
        {
          title: 'Mantova, Verona e Milano a confronto',
          reading: {
            title: 'Tre città a confronto',
            it: 'Milano è la città più grande della Lombardia ed è più moderna di Mantova.\n\nPerò Mantova è meno caotica di Milano e la vita costa meno.\n\nIn estate fa caldissimo, ma il centro storico con i suoi laghi è bellissimo e tranquillo come un quadro!',
            en: 'Milan is the biggest city in Lombardy and it is more modern than Mantova.\n\nBut Mantova is less chaotic than Milan and life costs less.\n\nIn summer it\'s very hot, but the historic centre with its lakes is beautiful and as peaceful as a painting!',
          },
        },
      ],
      exercises: [
        fb('Questa pizza è ___ ed è la più economica del menu!', ['buonissima', 'più buonissima', 'la più buonissima', 'migliorissima']),
        fb('Questa pizza è buonissima ed è ___ del menu!', ['la più economica', 'più economica', 'economicissima di', 'la più economicissima']),
        fb('Milano è la città più grande ___ Lombardia.', ['della', 'di', 'che', 'nella']),
        fb('Questa pizza è ___ di quella.', ['migliore', 'più buonissima', 'più migliore', 'ottima di']),
        fb('Mio fratello ___ ha 30 anni.', ['maggiore', 'migliore', 'più grande di', 'grandissimo']),
        mc('Comparative of "buono"?', 'migliore', 'buonissimo', 'il più buono', 'ottimo'),
        mc('"Pessimo" means…', 'terrible', 'excellent', 'older', 'cheap'),
        tf('Mantova è più caotica di Milano.', false, 'Mantova is more chaotic than Milan.'),
      ],
      homework: 'Class records: write 4 sentences with the superlative (Chi è il più alto? Chi abita più lontano?).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Compare your country and Italy',
      titleIt: 'Lezione 3 · Confronta il tuo Paese e l\'Italia',
      summary: 'Climate, food and cities: in India fa più caldo che in Italia…',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: Punjab o Lombardia?',
          situation: 'Giulia and Jaspreet compare their countries.',
          dialogue: [
            lp('Giulia', 'Jaspreet, in Punjab fa più caldo che in Italia?', 'Jaspreet, is it hotter in Punjab than in Italy?', 'Jaspreet, ki Panjab vich Itali naalon ziada garmi hundi hai?'),
            lp('Jaspreet', 'Sì, in estate fa caldissimo! Ma l\'inverno italiano è più freddo.', 'Yes, in summer it\'s very hot! But the Italian winter is colder.', 'Haan, garmiaan vich bahut garmi hundi hai! Par Itali di sardi ziada thandi hai.'),
            lp('Giulia', 'E il cibo? Qual è il piatto più buono del tuo Paese?', 'And the food? What\'s the tastiest dish in your country?', 'Ate khaana? Tuhade desh da sab ton savadi khaana kihada hai?'),
            lp('Jaspreet', 'Secondo me è il sarson da saag: è buonissimo, ma è meno leggero della pasta!', 'I think it\'s sarson da saag: it\'s delicious, but it\'s less light than pasta!', 'Mere khial vich sarhon da saag: bahut savadi hai, par paasta naalon bhaara hai!'),
          ],
        },
        {
          title: 'Conversazione a coppie',
          text: '- **Fase 1 · Clima, cibo e città:** fai 3 paragoni («In India fa più caldo che in Italia… / Il cibo è buonissimo!»).\n- **Fase 2 · I record della classe:** usa il superlativo relativo («Chi è il più alto? Chi abita più lontano dalla scuola?»).',
        },
      ],
      exercises: [
        tf('In estate in Punjab fa caldissimo.', true, 'In summer it\'s very hot in Punjab.'),
        tf('L\'inverno italiano è meno freddo.', false, 'The Italian winter is less cold.'),
        tf('Per Jaspreet il sarson da saag è buonissimo.', true, 'For Jaspreet sarson da saag is delicious.'),
        wo('In India fa più caldo che in Italia', 'In India it is hotter than in Italy'),
        wo('Qual è il piatto più buono del tuo Paese', 'What is the tastiest dish in your country'),
      ],
      homework: 'Write 3 comparisons between your country and Italy and 2 class records.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Ieri e oggi: confrontare luoghi e stili di vita
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 4 · City and countryside',
      titleIt: 'Lezione 4 · In città e in campagna',
      summary: 'Caotica / tranquilla, traffico / aria pulita, vita frenetica / rilassata, affitto caro…',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 11 · Lezione 4 (PDF)' },
        {
          title: 'Parole per confrontare i luoghi',
          words: [
            wp('caotico / rumoroso', 'chaotic / noisy', 'Bheed-bhadakke vaala / Raula', { img: 'Cityscape' }),
            wp('tranquillo / silenzioso', 'quiet / peaceful', 'Shaant', { img: 'Deciduous tree' }),
            wp('il traffico / lo smog', 'traffic / smog', 'Traifik / Dhuaan', { img: 'Automobile' }),
            wp('l\'aria pulita', 'clean air', 'Saaf hava', { img: 'Leafy green' }),
            wp('la vita frenetica', 'hectic life', 'Tez zindagi', { img: 'Person running' }),
            wp('la vita rilassata', 'relaxed life', 'Aaram vaali zindagi', { img: 'Relieved face' }),
            wp('l\'affitto caro', 'expensive rent', 'Mahinga kiraia', { img: 'Money bag' }),
            wp('la campagna / il villaggio', 'countryside / village', 'Pind / Kheti vaala ilaka', { img: 'House with garden' }),
            wp('all\'improvviso', 'suddenly', 'Achanak', { img: 'Collision' }),
          ],
        },
        {
          title: 'Vivere in città vs vivere in campagna',
          table: [
            ['La città (es. Milano, Brescia)', 'La campagna / il paese'],
            ['Ci sono più opportunità di lavoro e studio.', 'La vita è più tranquilla e la gente si conosce.'],
            ['I trasporti sono più comodi e frequenti.', 'L\'aria è freschissima e il cibo è buonissimo.'],
            ['Però l\'aria è meno pulita e le case costano di più.', 'Però senza macchina è più difficile spostarsi.'],
          ],
        },
      ],
      exercises: [
        mc('Opposite of "caotica"?', 'tranquilla', 'frenetica', 'cara', 'rumorosa'),
        mc('Opposite of "rilassata"?', 'frenetica', 'tranquilla', 'pulita', 'economica'),
        mc('Opposite of "caro"?', 'economico', 'pulito', 'frenetico', 'rumoroso'),
        mc('Where is it harder to move around without a car?', 'in campagna', 'in città', 'a Milano', 'in centro'),
        tr('all\'improvviso', 'suddenly', 'usually', 'slowly', 'always'),
      ],
      homework: 'Write 3 good things and 1 bad thing about where you live now.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 4 · Harpreet\'s story: India before, Italy today',
      titleIt: 'Lezione 4 · La storia di Harpreet: prima in India, oggi in Italia',
      summary: 'Imperfetto for before, presente and comparatives for today.',
      minutes: 12,
      sections: [
        {
          title: 'Testimonianza: ieri e oggi',
          reading: {
            title: 'La storia di Harpreet',
            it: '«Quando abitavo in India, vivevo con tutta la mia grande famiglia. La nostra casa era sempre piena di parenti e vicini.»\n\n«Oggi vivo in un piccolo paese vicino a Mantova. Qui la vita è più silenziosa e più organizzata, ma in India la gente aveva più tempo per stare insieme.»\n\n«Il clima in estate è caldissimo come nel Punjab, però l\'inverno italiano è molto più freddo!»',
            en: '"When I lived in India, I lived with my whole big family. Our house was always full of relatives and neighbours."\n\n"Today I live in a small town near Mantova. Here life is quieter and more organised, but in India people had more time to be together."\n\n"The summer weather is as hot as in Punjab, but the Italian winter is much colder!"',
          },
        },
        {
          title: '3 espressioni utili per la discussione',
          words: [
            wp('Secondo me…', 'In my opinion…', 'Mere khial vich…', { img: 'Thinking face', ex: 'Secondo me la campagna è più rilassante della città.', exEn: 'In my opinion the countryside is more relaxing than the city.' }),
            wp('Preferisco… perché…', 'I prefer… because…', 'Mainu … ziada pasand hai kiunki…', { img: 'Thumbs up', ex: 'Preferisco la campagna perché c\'è più aria pulita.', exEn: 'I prefer the countryside because there is cleaner air.' }),
            wp('Prima… invece oggi…', 'Before… but today…', 'Pahilaan… par ajj…', { img: 'Counterclockwise arrows button', ex: 'Prima andavo a piedi, oggi uso l\'auto.', exEn: 'Before I walked, today I use the car.' }),
          ],
        },
      ],
      exercises: [
        tf('In India Harpreet viveva da solo.', false, 'In India Harpreet lived alone.'),
        tf('Oggi abita vicino a Mantova.', true, 'Today he lives near Mantova.'),
        tf('Secondo Harpreet la vita in Italia è più organizzata.', true, 'According to Harpreet life in Italy is more organised.'),
        tf('L\'inverno italiano è meno freddo del Punjab.', false, 'The Italian winter is less cold than Punjab.'),
        fb('Quando abitavo in India, ___ con tutta la mia famiglia.', ['vivevo', 'ho vissuto', 'vivo', 'vivrò']),
        wo('Prima andavo a piedi oggi uso l\'auto', 'Before I walked, today I use the car'),
      ],
      homework: 'Write Harpreet\'s story about yourself: 2 sentences about before, 2 about today.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · Discussion: then and now',
      titleIt: 'Lezione 4 · Parliamo insieme: ieri e oggi',
      summary: 'Guide questions for the group discussion and your 4-sentence comparison.',
      minutes: 10,
      sections: [
        {
          title: '4 aspetti da confrontare',
          table: [
            ['Aspetto', 'Idee'],
            ['Il clima', 'stagioni, caldo, pioggia, neve'],
            ['Il cibo e gli orari', 'colazione, pranzo, cena, spezie'],
            ['La famiglia', 'case grandi, vicini, feste, tradizioni'],
            ['Lavoro e tempo libero', 'trasporti, domeniche, sport'],
          ],
        },
        {
          title: 'Domande guida per la discussione',
          text: '1. Nel tuo Paese abitavi in una grande città o in un piccolo villaggio? Com\'era?\n2. Che cosa è più bello o più comodo qui in Italia rispetto a prima?\n3. Che cosa ti manca di più della tua vita nel Paese d\'origine?\n4. Preferisci vivere in una grande città o in campagna? Perché?',
        },
        { tip: 'Final activity: write **4 sentences** with the imperfetto and comparatives — *Prima abitavo…, oggi vivo…, qui è più … che …*' },
      ],
      exercises: [
        fb('Prima ___ in un villaggio, oggi vivo in città.', ['abitavo', 'abito', 'ho abitato una volta', 'abiterò']),
        fb('Oggi la vita è ___ frenetica di prima.', ['più', 'come', 'che', 'meno di']),
        fb('Qui il lavoro è ___ rispetto a prima.', ['migliore', 'più migliore', 'buonissimo di', 'ottimo di']),
        fb('Ogni pomeriggio ___ i miei amici al parco.', ['incontravo', 'ho incontrato una volta', 'incontro ieri', 'incontrerò']),
        mc('"Che cosa ti manca di più?" means…', 'What do you miss most?', 'What is missing in the house?', 'What do you want more?', 'What did you lose?'),
      ],
      homework: 'Write your 4-sentence comparison "Ieri e oggi" and read it to the class.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 11
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 11 test',
      titleIt: 'Test Capitolo 11',
      summary: 'Listening, places and lifestyles, imperfetto, imperfetto vs passato prossimo, comparatives. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 11',
          text: 'This test checks everything in Chapter 11 (Lessons 1–4).\n\n- **1. Listening** — sentences and a then-and-now comparison\n- **2. Vocabulary** — places, lifestyles and descriptions\n- **3. Grammar** — imperfetto, imperfetto vs passato prossimo, comparatives and superlatives\n- **4. Writing** — then and now: my country and Italy\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (my country and Italy) and the **oral interview** (childhood memories, «Mentre tornavo a casa ieri…», comparing places — 2 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A
        lis('1A · Listen and choose the sentence you hear', 'Da bambino abitavo in un piccolo villaggio con i miei nonni.', 'DA BAMBINO ABITAVO IN UN PICCOLO VILLAGGIO', 'QUANDO ERO PICCOLO GIOCAVO SEMPRE IN STRADA'),
        lis('1A · Listen and choose the sentence you hear', 'Ieri notte mentre dormivo, ha suonato il telefono alle tre.', 'MENTRE DORMIVO, HA SUONATO IL TELEFONO', 'MENTRE CAMMINAVO, È INIZIATO A PIOVERE'),
        lis('1A · Listen and choose the sentence you hear', 'Secondo me Milano è più grande e più caotica di Mantova.', 'MILANO È PIÙ GRANDE E PIÙ CAOTICA DI MANTOVA', 'IL TRENO È VELOCE COME L\'AUTOBUS'),
        lis('1A · Listen and choose the sentence you hear', 'Tutti dicono che questo ristorante è il migliore della città.', 'QUESTO RISTORANTE È IL MIGLIORE DELLA CITTÀ', 'LA CAMPAGNA È TRANQUILLISSIMA E L\'ARIA È PULITA'),
        lis('1A · Listen and choose the sentence you hear', 'Prima facevo il contadino, oggi lavoro in fabbrica vicino a casa.', 'PRIMA FACEVO IL CONTADINO, OGGI LAVORO IN FABBRICA', 'IN INVERNO FA MOLTO PIÙ FREDDO CHE IN ESTATE'),
        // 1B
        lis('1B · Listen: "Quando ___ nel mio Paese" (1)', 'Quando abitavo nel mio Paese, la vita era più tranquilla.', 'ABITAVO', 'ABITO', 'HO ABITATO', 'ABITERÒ'),
        lis('1B · Listen again: "la vita ___ più tranquilla" (2)', 'La vita era più tranquilla e avevo molto tempo libero.', 'ERA', 'È', 'È STATA', 'SARÀ'),
        lis('1B · Listen again: what did the person do every afternoon? (3)', 'Ogni pomeriggio incontravo i miei amici al parco e bevevamo il tè.', 'INCONTRAVO', 'CHIAMAVO', 'CERCAVO', 'LASCIAVO'),
        lis('1B · Listen again: life in the city is ___ frenetica (4)', 'Oggi vivo in città in Italia: la vita è più frenetica.', 'PIÙ', 'MENO', 'COME', 'TANTO'),
        lis('1B · Listen again: the job is ___ than before (5)', 'Ci sono molti servizi e il lavoro è migliore rispetto a prima.', 'MIGLIORE', 'PEGGIORE', 'MAGGIORE', 'MINORE'),
        // 2A
        pairs('2A · Match each word with its meaning', [
          ['Campagna / Villaggio', 'Countryside / village · Pind'],
          ['Caotico / Rumoroso', 'Chaotic / noisy · Raula'],
          ['Aria pulita', 'Clean air · Saaf hava'],
          ['Affitto caro', 'Expensive rent · Mahinga kiraia'],
          ['All\'improvviso', 'Suddenly · Achanak'],
        ]),
        // 2B
        mc('2B · Contrario di caotica: ?', 'tranquilla', 'frenetica', 'migliore', 'piccoli'),
        mc('2B · Contrario di rilassata: ?', 'frenetica', 'tranquilla', 'migliore', 'mentre'),
        mc('2B · Comparativo di buono: ?', 'migliore', 'tranquilla', 'frenetica', 'piccoli'),
        mc('2B · Quando eravamo ___ (bambini)', 'piccoli', 'migliore', 'mentre', 'frenetica'),
        mc('2B · ___ dormivo, ha suonato la sveglia.', 'Mentre', 'Migliore', 'Piccoli', 'Tranquilla'),
        // 3A
        fb('3A · Quando io ___ (essere) piccolo, avevo una bicicletta rossa.', ['ero', 'sono stato', 'era', 'eri']),
        fb('3A · Quando ero piccolo, ___ (avere) una bicicletta rossa.', ['avevo', 'ho avuto', 'aveva', 'avevi']),
        fb('3A · Da bambini noi ___ (giocare) sempre in giardino.', ['giocavamo', 'abbiamo giocato', 'giocavano', 'giocavate']),
        fb('3A · … e ___ (bere) succo di frutta.', ['bevevamo', 'bevavamo', 'abbiamo bevuto', 'bevevano']),
        fb('3A · Mio nonno ___ (fare) il panettiere…', ['faceva', 'fava', 'ha fatto', 'facevo']),
        fb('3A · … e ___ (alzarsi) alle quattro ogni mattina.', ['si alzava', 'alzava', 'si è alzato', 'mi alzavo']),
        // 3B
        fb('3B · Mentre io ___ la TV, mia madre ha chiamato.', ['guardavo', 'ho guardato']),
        fb('3B · Mentre io guardavo la TV, mia madre ___.', ['ha chiamato', 'chiamava']),
        fb('3B · Ieri sera ___ forte quando noi siamo usciti dal lavoro.', ['pioveva', 'è piovuto']),
        fb('3B · Ieri sera pioveva forte quando noi ___ dal lavoro.', ['siamo usciti', 'uscivamo']),
        fb('3B · L\'anno scorso ___ a Roma per la prima volta.', ['sono andato', 'andavo']),
        fb('3B · Prima di venire in Italia, tu ___ tutti i giorni in un negozio?', ['lavoravi', 'hai lavorato']),
        // 3C
        fb('3C · L\'aereo è molto più veloce ___ treno.', ['del', 'che']),
        fb('3C · Vivere in campagna è meno caro ___ vivere in centro città.', ['che', 'di']),
        fb('3C · Il mio paese d\'origine è caldo ___ il sud Italia in estate.', ['come', 'di']),
        fb('3C · Questa pizza è ___', ['buonissima', 'più buonissima']),
        fb('3C · … ed è ___ del menu!', ['la più economica', 'più economica']),
        // 4
        wo('Quando abitavo in India vivevo in un villaggio', 'When I lived in India I lived in a village'),
        wo('Secondo me la vita qui è più organizzata', 'In my opinion life here is more organised'),
        wo('Il clima in Italia è più freddo', 'The climate in Italy is colder'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences comparing your life before (imperfetto) and today (present and comparatives). Then prepare the 2-minute oral interview.',
    },
  ],
};
