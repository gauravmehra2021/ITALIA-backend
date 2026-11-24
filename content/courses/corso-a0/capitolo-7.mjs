import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 7 of the school syllabus (Corso di Italiano, livello A0 → A1) — the last A1 chapter.
// Source: syllabus/Capitolo 7 Lezione 1–4 (.pptx) and Capitolo 7 Test Finale Livello A1 (.docx).
// Corrections to the client material:
//   - Lezione 3 dialogue: "Sembra che piove" → "Forse piove" (the subjunctive is not A1).
//   - Test finale 3B.2: the key says "Compro un chilo pomodori", which is wrong without "di";
//     the app asks "Compro ___ pomodori" with "dei" as the answer.
//   - Test finale 3B.4: "vicino a la stazione" → "vicino alla stazione".

const slides = (lez) => `/content/slides/corso-a0-cap7-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-7',
  title: 'Chapter 7: Clothes, colours and the weather — A1 review',
  titleIt: 'Capitolo 7: Vestiti, colori e meteo — ripasso A1',
  cover: 'T-shirt',
  description: 'Clothes and colours with gender and number agreement, shopping for clothes (sizes, prices, sales, paying), questo and quello, the weather and the four seasons, then a complete review of Chapters 1–7 and the final A1 test.',
  canDo: [
    'I can name clothes and colours and make them agree: una camicia bianca, i pantaloni neri',
    'I can buy clothes: ask for my size, try them on, ask the price and pay',
    'I can point at things with questo and quello',
    'I can say what the weather is like and name the four seasons',
    'I can say what I wear in each season',
    'I can use everything from Chapters 1–7 in a real conversation (level A1)',
  ],
  liveClassFocus: 'Shop role-play: buy two clothes in the right size and colour, ask the price and pay. Then the 4 review stations (introduce yourself, buy, give directions, your day and the weather) and the 3-minute A1 oral exam.',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — Abbigliamento e colori
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · Clothes',
      titleIt: 'Lezione 1 · I capi di vestiario',
      summary: 'La maglietta, la camicia, i pantaloni, la gonna, la giacca, il cappotto, le scarpe.',
      minutes: 10,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 7 · Lezione 1 (PDF)' },
        {
          title: 'I 7 capi di vestiario principali',
          words: [
            wp('la maglietta — le magliette', 'T-shirt', 'Ṭī-sharṭ', { pron: 'lah mah-LYET-tah', img: 'T-shirt' }),
            wp('la camicia — le camicie', 'shirt', 'Kamīz', { pron: 'lah kah-MEE-chah', img: 'Necktie' }),
            wp('i pantaloni', 'trousers', 'Paiṇṭ / Patlūn', { pron: 'ee pahn-tah-LOH-nee', img: 'Jeans' }),
            wp('la gonna — le gonne', 'skirt', 'Sakarṭ', { pron: 'lah GON-nah', img: 'Dress' }),
            wp('la giacca — le giacche', 'jacket', 'Jaikiṭ', { pron: 'lah JAHK-kah', img: 'Lab coat' }),
            wp('il cappotto — i cappotti', 'coat', 'Koṭ / Ovarkoṭ', { pron: 'eel kahp-POT-toh', img: 'Coat' }),
            wp('le scarpe', 'shoes', 'Juttīāṁ', { pron: 'leh SKAR-peh', img: 'Running shoe' }),
          ],
        },
        { tip: '**I pantaloni** and **le scarpe** are almost always plural: *Questi pantaloni sono comodi. Le scarpe sono nuove.* "A pair of trousers" = *un paio di pantaloni*.' },
      ],
      exercises: [
        mc('What is the plural of "la giacca"?', 'le giacche', 'le giacca', 'le giacci', 'i giacchi'),
        mc('What is the plural of "la camicia"?', 'le camicie', 'le camicia', 'i camici', 'le camiche'),
        mc('You wear it over everything when it\'s very cold:', 'il cappotto', 'la maglietta', 'la gonna', 'la camicia'),
        tr('le scarpe', 'shoes', 'trousers', 'skirt', 'jacket'),
      ],
      homework: 'Write what you are wearing today (Oggi porto…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 1 · Colours: bianco, bianca, bianchi, bianche',
      titleIt: 'Lezione 1 · I colori e l\'accordo',
      summary: 'The 8 basic colours and how they change: 4 forms, 2 forms or 1 form (blu).',
      minutes: 12,
      sections: [
        {
          title: 'Gli 8 colori fondamentali',
          words: [
            wp('bianco', 'white', 'Chiṭṭā / Safed', { pron: 'BYAHN-koh', img: 'White circle' }),
            wp('nero', 'black', 'Kāḷā', { pron: 'NEH-roh', img: 'Black circle' }),
            wp('rosso', 'red', 'Lāl', { pron: 'ROS-soh', img: 'Red circle' }),
            wp('blu', 'blue', 'Nīlā', { pron: 'BLOO', img: 'Blue circle' }),
            wp('verde', 'green', 'Harā', { pron: 'VEHR-deh', img: 'Green circle' }),
            wp('giallo', 'yellow', 'Pīḷā', { pron: 'JAHL-loh', img: 'Yellow circle' }),
            wp('marrone', 'brown', 'Bhūrā', { pron: 'mar-ROH-neh', img: 'Brown circle' }),
            wp('grigio', 'grey', 'Saleṭī', { pron: 'GREE-joh', img: 'Cloud' }),
          ],
        },
        {
          title: 'Come cambiano i colori: 3 gruppi',
          table: [
            ['Gruppo', 'Colori', 'Forme'],
            ['4 forme', 'bianco, nero, rosso, giallo, grigio', '-o (m. sing.) · -a (f. sing.) · -i (m. pl.) · -e (f. pl.)'],
            ['2 forme', 'verde, marrone', '-e (singolare) · -i (plurale)'],
            ['1 forma', 'blu', 'never changes (invariabile)'],
          ],
        },
        {
          title: 'Accordo: maschile, femminile e plurale',
          table: [
            ['Colore', 'Masch. sing.', 'Femm. sing.', 'Masch. plur.', 'Femm. plur.'],
            ['bianco', 'cappotto bianco', 'camicia bianca', 'pantaloni bianchi', 'scarpe bianche'],
            ['nero', 'cappotto nero', 'giacca nera', 'pantaloni neri', 'scarpe nere'],
            ['rosso', 'cappotto rosso', 'maglietta rossa', 'pantaloni rossi', 'scarpe rosse'],
            ['grigio', 'cappotto grigio', 'gonna grigia', 'pantaloni grigi', 'scarpe grigie'],
            ['verde', 'cappotto verde', 'maglietta verde', 'pantaloni verdi', 'scarpe verdi'],
            ['marrone', 'cappotto marrone', 'giacca marrone', 'pantaloni marroni', 'scarpe marroni'],
            ['blu', 'cappotto blu', 'camicia blu', 'pantaloni blu', 'scarpe blu'],
          ],
        },
        { tip: 'In Italian the colour goes **after** the noun: *una camicia **bianca***, *i pantaloni **neri***. Watch the spelling: bian**co** → bian**chi** / bian**che** (the h keeps the hard "k" sound).' },
      ],
      exercises: [
        fb('Porto una maglietta ___.', ['bianca', 'bianco', 'bianchi', 'bianche']),
        fb('Ho due pantaloni ___.', ['neri', 'nero', 'nera', 'nere']),
        fb('Ho una gonna ___.', ['verde', 'verdi', 'verda', 'verdo']),
        fb('Ho una sciarpa ___.', ['blu', 'blua', 'blue', 'bli']),
        fb('Queste scarpe sono ___.', ['marroni', 'marrone', 'marrona', 'marrono']),
        fb('Il cappotto è ___.', ['grigio', 'grigia', 'grigi', 'grigie']),
        fb('Le magliette sono ___.', ['gialle', 'gialli', 'gialla', 'giallo']),
        mc('Which colour never changes?', 'blu', 'verde', 'rosso', 'marrone'),
      ],
      homework: 'Write 5 clothes you have at home with their colour (Ho una giacca nera, due magliette bianche…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Dialogue: buying a shirt and trousers',
      titleIt: 'Lezione 1 · Dialogo: comprare camicia e pantaloni',
      summary: 'Cerco una giacca nera · Che taglia porta? · Posso provarla?',
      minutes: 10,
      sections: [
        {
          title: 'Frasi utili in negozio di abbigliamento',
          practice: false,
          words: [
            wp('Cerco una giacca nera.', 'I\'m looking for a black jacket.', 'Mainūṁ kāḷī jaikiṭ chāhīdī hai.'),
            wp('Che taglia porta? — Porto la M / la 42.', 'What size do you wear? — I wear M / 42.', 'Tuhāḍā sāīz kī hai? — Merā sāīz M / 42 hai.'),
            wp('Posso provarla? Dove sono i camerini?', 'Can I try it on? Where are the fitting rooms?', 'Kī maiṁ pā ke vekh sakdā hāṁ? Ṭrāīal rūm kitthe han?'),
          ],
        },
        {
          title: 'Dialogo: comprare camicia e pantaloni',
          situation: 'Navdeep is in a clothes shop.',
          dialogue: [
            lp('Commessa', 'Buongiorno! Posso aiutarla?', 'Good morning! May I help you?', 'Sat Srī Akāl! Kī maiṁ tuhāḍī madad kar sakdī hāṁ?'),
            lp('Navdeep', 'Sì, grazie. Vorrei una camicia bianca e un paio di pantaloni blu.', 'Yes, thank you. I\'d like a white shirt and a pair of blue trousers.', 'Hāṁ jī, dhanvād. Mainūṁ chiṭṭī kamīz ate nīlī paiṇṭ chāhīdī hai.'),
            lp('Commessa', 'Ecco a Lei. Abbiamo anche queste scarpe marroni in offerta.', 'Here you are. We also have these brown shoes on offer.', 'Eh lao jī. Sāḍe kol eh bhūrīāṁ juttīāṁ vī sel te han.'),
            lp('Navdeep', 'Grazie mille! Posso provare la camicia e i pantaloni?', 'Thank you very much! Can I try on the shirt and the trousers?', 'Bahut dhanvād! Kī maiṁ kamīz ate paiṇṭ pā ke vekh sakdā hāṁ?'),
          ],
        },
        {
          title: 'Role-play: cliente e commesso in negozio',
          text: '1. **Chiedi due capi:** scegli due vestiti e i colori giusti.\n2. **Controlla l\'accordo:** singolare, plurale, maschile e femminile.\n3. **Chiedi taglia e prezzo:** usa *Posso provarla?* e *Quanto costa?*',
        },
      ],
      exercises: [
        tf('Navdeep vuole una camicia bianca.', true, 'Navdeep wants a white shirt.'),
        tf('Navdeep vuole i pantaloni neri.', false, 'Navdeep wants black trousers.'),
        tf('Le scarpe marroni sono in offerta.', true, 'The brown shoes are on offer.'),
        wo('Vorrei una camicia bianca', 'I would like a white shirt'),
        wo('Posso provare i pantaloni', 'Can I try on the trousers'),
      ],
      homework: 'Role-play with a partner: buy two clothes with the right colours, then change roles.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — In negozio: taglie, prezzi e dimostrativi
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 2 · This and that: questo and quello',
      titleIt: 'Lezione 2 · I dimostrativi: questo e quello',
      summary: 'Questo / questa / questi / queste (here) and quel / quello / quella / quei / quegli / quelle (there).',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 7 · Lezione 2 (PDF)' },
        {
          title: 'QUESTO: vicino a chi parla (qui)',
          table: [
            ['Forma', 'Esempio', 'English · Punjabi'],
            ['QUESTO (quest\' + vocale)', 'Questo cappotto è molto caldo.', 'this coat · eh koṭ'],
            ['QUESTA (quest\' + vocale)', 'Questa giacca costa poco.', 'this jacket · eh jaikiṭ'],
            ['QUESTI', 'Questi pantaloni sono comodi.', 'these trousers · eh paiṇṭ'],
            ['QUESTE', 'Queste scarpe sono nere.', 'these shoes · eh juttīāṁ'],
          ],
        },
        {
          title: 'QUELLO: lontano da chi parla (lì)',
          table: [
            ['Genere / numero', 'Aggettivo + nome', 'Pronome (senza nome)'],
            ['m. sing. (consonante)', 'quel cappotto blu', 'Preferisco quello blu.'],
            ['m. sing. (s + consonante, z…)', 'quello zaino, quello stivale', 'Prendo quello lì.'],
            ['m. sing. (vocale)', 'quell\'abito elegante', 'Guarda quello in vetrina!'],
            ['f. sing.', 'quella camicia, quell\'offerta', 'Mi piace quella rossa.'],
            ['m. plur.', 'quei pantaloni, quegli stivali', 'Quanto costano quelli?'],
            ['f. plur.', 'quelle scarpe marroni', 'Vorrei provare quelle.'],
          ],
        },
        { tip: '**Quello** before a noun changes like the article: *il → quel*, *lo → quello*, *l\' → quell\'*, *i → quei*, *gli → quegli*, *le → quelle*. Without a noun use **quello, quella, quelli, quelle**: *Quel cappotto è caro. — Sì, quello costa troppo!*' },
      ],
      exercises: [
        fb('___ giacca costa poco.', ['Questa', 'Questo', 'Questi', 'Queste']),
        fb('___ pantaloni sono comodi.', ['Questi', 'Queste', 'Questo', 'Questa']),
        fb('Quanto costa ___ cappotto?', ['questo', 'questa', 'questi', 'queste']),
        fb('Preferisco ___ scarpe marroni lì sul tavolo.', ['quelle', 'quei', 'quegli', 'quella']),
        fb('Guarda ___ cappotto blu in vetrina!', ['quel', 'quello', 'quella', 'quei']),
        fb('Mi piacciono ___ stivali.', ['quegli', 'quei', 'quelle', 'quel']),
        fb('Mi piace ___ camicia rossa.', ['quella', 'quello', 'quel', 'quelle']),
        mc('Quel cappotto in vetrina è caro. — Sì, ___ costa troppo!', 'quello', 'quel', 'quella', 'quegli'),
      ],
      homework: 'Look at the clothes in a shop window (or at home) and write 4 sentences with questo and quello.',
    },
    {
      kind: 'Vocabulary',
      title: 'Lesson 2 · Size, fitting room and price',
      titleIt: 'Lezione 2 · Taglia, camerino e prezzo',
      summary: 'Che taglia porta? Posso provarlo? Quanto costa? Caro o economico? Pagare alla cassa.',
      minutes: 12,
      sections: [
        {
          title: 'Provare i vestiti: taglie, numeri e camerini',
          words: [
            wp('Che taglia porta? — Porto la M / la 44.', 'What size do you wear? — I wear M / 44.', 'Tuhāḍā sāīz kī hai?', { img: 'T-shirt' }),
            wp('Che numero porta? — Porto il 41.', 'What shoe size do you wear? — I wear 41.', 'Juttī dā nambar kī hai?', { img: 'Running shoe' }),
            wp('Posso provarlo / provarla?', 'Can I try it on?', 'Kī maiṁ pā ke vekh sakdā hāṁ?', { img: 'Coat' }),
            wp('i camerini', 'fitting rooms', 'Ṭrāīal rūm', { pron: 'ee kah-meh-REE-nee', img: 'Door' }),
            wp('È troppo stretta / larga.', 'It\'s too tight / loose.', 'Bahut taṅg / khullhī hai.', { img: 'Person tipping hand' }),
            wp('Mi sta bene!', 'It fits me well!', 'Mainūṁ ṭhīk ābdī hai!', { img: 'Thumbs up' }),
          ],
        },
        {
          title: 'Prezzi, saldi e cassa',
          words: [
            wp('il prezzo', 'price', 'Kīmat', { pron: 'eel PRET-tsoh', img: 'Label', ex: 'Qual è il prezzo?', exEn: 'What is the price?' }),
            wp('i saldi / lo sconto', 'sales / discount', 'Sel / Chhoṭ', { pron: 'ee SAHL-dee', img: 'Shopping bags', ex: 'È in saldo con il 30% di sconto.', exEn: 'It\'s on sale with 30% off.' }),
            wp('caro / economico', 'expensive / cheap', 'Mahiṅgā / Sastā', { pron: 'KAH-roh / eh-koh-NOH-mee-koh', img: 'Money bag', ex: 'Non è caro, è molto economico!', exEn: 'It\'s not expensive, it\'s very cheap!' }),
            wp('la cassa', 'checkout / till', 'Kaish kāūṇṭar', { pron: 'lah KAHS-sah', img: 'Shopping cart', ex: 'Può pagare alla cassa.', exEn: 'You can pay at the till.' }),
            wp('in contanti', 'in cash', 'Nakad', { pron: 'een kon-TAHN-tee', img: 'Euro banknote', ex: 'Pago in contanti.', exEn: 'I pay in cash.' }),
            wp('con la carta / il bancomat', 'by card / debit card', 'Kāraḍ nāḷ', { pron: 'BAHN-koh-maht', img: 'Credit card', ex: 'Posso pagare con la carta?', exEn: 'Can I pay by card?' }),
            wp('lo scontrino', 'receipt', 'Rasīd / Bill', { pron: 'loh skon-TREE-noh', img: 'Receipt', ex: 'Ecco a Lei il resto e lo scontrino.', exEn: 'Here is your change and the receipt.' }),
          ],
        },
        {
          title: 'Quanto costa? Quanto costano?',
          table: [
            ['Domanda', 'Quando', 'Risposta'],
            ['QUANTO COSTA?', 'singular (1 item)', 'Costa venticinque euro.'],
            ['QUANTO COSTANO?', 'plural (pantaloni, scarpe)', 'Costano quaranta euro.'],
            ['LO / LA / LI / LE PRENDO!', 'you decide to buy', 'Lo prendo (il cappotto). Le prendo (le scarpe).'],
          ],
        },
        { tip: 'Try it on: *Posso provar**lo**?* (il cappotto) · *provar**la**?* (la giacca) · *provar**li**?* (i pantaloni) · *provar**le**?* (le scarpe).' },
      ],
      exercises: [
        fb('Quanto ___ questi pantaloni?', ['costano', 'costa', 'costi', 'costo']),
        fb('Quanto ___ questa giacca?', ['costa', 'costano', 'costi', 'costiamo']),
        fb('Il cappotto mi piace: ___ prendo!', ['lo', 'la', 'li', 'le']),
        fb('Le scarpe mi stanno bene: ___ prendo!', ['le', 'li', 'la', 'lo']),
        fb('Posso ___? (la giacca)', ['provarla', 'provarlo', 'provarli', 'provarle']),
        mc('For shoes the shop assistant asks…', 'Che numero porta?', 'Che taglia porta?', 'Quanti anni ha?', 'Che lavoro fa?'),
        mc('"Economico" means…', 'cheap', 'expensive', 'on sale', 'receipt'),
        tr('lo scontrino', 'receipt', 'discount', 'till', 'price'),
      ],
      homework: 'Write a short dialogue in a shop: size, fitting room, price and payment.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · Dialogues: in the shop and at the sales',
      titleIt: 'Lezione 2 · Dialoghi: in negozio e ai saldi',
      summary: 'Amrit compares two coats; Gurpreet changes size and pays with a 30% discount.',
      minutes: 12,
      sections: [
        {
          title: 'Dialogo 1: commesso e cliente in negozio',
          situation: 'Amrit looks at the coats in a shop window.',
          dialogue: [
            lp('Commesso', 'Buongiorno! Posso aiutarla? Che taglia porta?', 'Good morning! May I help you? What size do you wear?', 'Sat Srī Akāl! Kī maiṁ madad kar sakdā hāṁ? Tuhāḍā sāīz kī hai?'),
            lp('Amrit', 'Porto la L. Quanto costa quel cappotto blu in vetrina?', 'I wear L. How much is that blue coat in the window?', 'Merā sāīz L hai. Shīshe vich oh nīlā koṭ kinne dā hai?'),
            lp('Commesso', 'Quello costa 120 euro, ma questo giubbotto nero è economico: 45 euro.', 'That one is 120 euros, but this black jacket is cheap: 45 euros.', 'Oh 120 yūro dā hai, par eh kāḷī jaikiṭ sastī hai: 45 yūro.'),
            lp('Amrit', 'Quello blu è troppo caro! Posso provare questo nero, per favore?', 'The blue one is too expensive! Can I try on this black one, please?', 'Oh nīlā bahut mahiṅgā hai! Kī maiṁ eh kāḷā pā ke vekh sakdā hāṁ?'),
          ],
        },
        {
          title: 'Dialogo 2: taglia, sconto e pagamento',
          situation: 'Gurpreet is trying on a jacket during the sales.',
          dialogue: [
            lp('Gurpreet', 'Scusi, questa giacca verde è troppo stretta. Avete una XL?', 'Excuse me, this green jacket is too tight. Do you have an XL?', 'Māf karnā, eh harī jaikiṭ bahut taṅg hai. Kī tuhāḍe kol XL hai?'),
            lp('Commesso', 'Sì, eccola! Ed è anche in saldo al 30% di sconto.', 'Yes, here it is! And it\'s also on sale with 30% off.', 'Hāṁ jī, eh lao! Ate is te 30% chhoṭ vī hai.'),
            lp('Gurpreet', 'Perfetto, mi sta benissimo! Quanto costa adesso?', 'Perfect, it fits me really well! How much is it now?', 'Bilkul ṭhīk, mainūṁ bahut vadhīā ābdī hai! Huṇ kinne dī hai?'),
            lp('Commesso', 'Costa 35 euro. Paga in contanti o con la carta alla cassa?', 'It\'s 35 euros. Are you paying in cash or by card at the till?', 'Eh 35 yūro dī hai. Nakad deṇe ne jāṁ kāraḍ nāḷ?'),
          ],
        },
        {
          title: 'Cartelli utili nei negozi italiani',
          text: '- **SALDI -30% / -50%** — January and July are the sales months: prices are lower.\n- **CAMERINI / PROVA** — maximum 3 or 4 items at a time · *libero* (free) / *occupato* (busy).\n- **CAMBIO MERCE** — always keep the receipt (lo scontrino)! You can change the size within 15–30 days.',
        },
        {
          title: '4 passi per comprare vestiti in Italia',
          text: '1. **Chiedere la taglia:** *Avete questa giacca nella taglia L?*\n2. **Provare in camerino:** *Posso provarla? Mi sta benissimo.*\n3. **Controllare il prezzo:** *Quanto viene con lo sconto?*\n4. **Pagare alla cassa:** *Pago con la carta, grazie mille.*',
        },
      ],
      exercises: [
        tf('Il cappotto blu costa 120 euro.', true, 'The blue coat costs 120 euros.'),
        tf('Amrit compra il cappotto blu.', false, 'Amrit buys the blue coat.'),
        tf('La giacca verde è troppo larga.', false, 'The green jacket is too loose.'),
        tf('La giacca XL costa 35 euro.', true, 'The XL jacket costs 35 euros.'),
        mc('In Italy the sales (i saldi) are in…', 'January and July', 'March and October', 'only in December', 'every month'),
        wo('Quanto costa quel cappotto blu', 'How much is that blue coat'),
        wo('Pago con la carta alla cassa', 'I pay by card at the till'),
      ],
      homework: 'Role-play at the sales: ask for your size, say it\'s too tight or too loose, ask the discount and pay.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — Il tempo atmosferico e le stagioni
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · The four seasons',
      titleIt: 'Lezione 3 · Le quattro stagioni',
      summary: 'La primavera, l\'estate, l\'autunno, l\'inverno — and their months.',
      minutes: 8,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 7 · Lezione 3 (PDF)' },
        {
          title: 'Le 4 stagioni dell\'anno',
          words: [
            wp('la primavera (in primavera)', 'spring', 'Basant rutt', { pron: 'lah pree-mah-VEH-rah', img: 'Cherry blossom', ex: 'Marzo, aprile, maggio.', exEn: 'March, April, May.' }),
            wp('l\'estate (in estate / d\'estate)', 'summer', 'Garmī dī rutt', { pron: 'les-TAH-teh', img: 'Sun', ex: 'Giugno, luglio, agosto.', exEn: 'June, July, August.' }),
            wp('l\'autunno (in autunno)', 'autumn', 'Patjhaṛ', { pron: 'low-TOON-noh', img: 'Fallen leaf', ex: 'Settembre, ottobre, novembre.', exEn: 'September, October, November.' }),
            wp('l\'inverno (in inverno / d\'inverno)', 'winter', 'Sardī dī rutt', { pron: 'leen-VEHR-noh', img: 'Snowman', ex: 'Dicembre, gennaio, febbraio.', exEn: 'December, January, February.' }),
          ],
        },
        { tip: 'To say *in* a season use **in**: *in primavera, in estate, in autunno, in inverno*. For summer and winter you can also say **d\'estate, d\'inverno**.' },
      ],
      exercises: [
        mc('Luglio is in…', 'l\'estate', 'l\'inverno', 'l\'autunno', 'la primavera'),
        mc('Gennaio is in…', 'l\'inverno', 'l\'estate', 'la primavera', 'l\'autunno'),
        mc('Ottobre is in…', 'l\'autunno', 'la primavera', 'l\'estate', 'l\'inverno'),
        mc('Aprile is in…', 'la primavera', 'l\'autunno', 'l\'inverno', 'l\'estate'),
      ],
      homework: 'Write your favourite season and its three months.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 3 · What\'s the weather like? Fa, c\'è, è',
      titleIt: 'Lezione 3 · Che tempo fa? FA, C\'È, È',
      summary: 'Fa caldo, fa freddo, piove, nevica, c\'è il sole, c\'è vento, è nuvoloso.',
      minutes: 12,
      sections: [
        {
          title: 'Le 7 espressioni meteo principali',
          words: [
            wp('Fa caldo.', 'It\'s hot.', 'Garmī hai.', { pron: 'fah KAHL-doh', img: 'Hot face' }),
            wp('Fa freddo.', 'It\'s cold.', 'Ṭhaṇḍ hai.', { pron: 'fah FRED-doh', img: 'Cold face' }),
            wp('Piove.', 'It\'s raining.', 'Mīṁh pai rihā hai.', { pron: 'PYOH-veh', img: 'Cloud with rain' }),
            wp('Nevica.', 'It\'s snowing.', 'Baraf pai rahī hai.', { pron: 'NEH-vee-kah', img: 'Cloud with snow' }),
            wp('C\'è il sole.', 'It\'s sunny.', 'Dhupp niklī hoī hai.', { pron: 'cheh eel SOH-leh', img: 'Sun' }),
            wp('C\'è vento.', 'It\'s windy.', 'Tez havā chal rahī hai.', { pron: 'cheh VEN-toh', img: 'Wind face' }),
            wp('È nuvoloso.', 'It\'s cloudy.', 'Baddalvāī hai.', { pron: 'eh noo-voh-LOH-soh', img: 'Cloud' }),
          ],
        },
        {
          title: '3 modi per dire il tempo',
          table: [
            ['Struttura', 'Espressioni', 'Esempio'],
            ['FA + aggettivo', 'fa caldo, fa freddo, fa bel tempo', 'Oggi fa molto caldo (35 gradi)!'],
            ['C\'È + nome', 'c\'è il sole, c\'è vento, c\'è nebbia', 'In Pianura Padana c\'è spesso nebbia.'],
            ['È + aggettivo / verbo', 'è nuvoloso · piove · nevica', 'Prendi l\'ombrello perché piove!'],
          ],
        },
        { tip: 'Key question: **«Che tempo fa oggi?»** — **«C\'è il sole e fa caldo!»** Around Mantova, in the Po Valley (Pianura Padana), autumn and winter often bring **la nebbia** (fog).' },
      ],
      exercises: [
        fb('Oggi ___ caldo.', ['fa', 'c\'è', 'è', 'ha']),
        fb('Oggi ___ il sole.', ['c\'è', 'fa', 'è', 'ci sono']),
        fb('Oggi ___ nuvoloso.', ['è', 'fa', 'c\'è', 'ha']),
        fb('In inverno ___ freddo.', ['fa', 'è', 'c\'è', 'ci sono']),
        fb('In autunno c\'è spesso ___.', ['nebbia', 'caldo', 'nuvoloso', 'freddo']),
        mc('"Piove" means…', 'It\'s raining', 'It\'s snowing', 'It\'s windy', 'It\'s sunny'),
        mc('"Nevica" means…', 'It\'s snowing', 'It\'s raining', 'It\'s cloudy', 'It\'s cold'),
        tr('C\'è vento.', 'It\'s windy.', 'It\'s sunny.', 'It\'s foggy.', 'It\'s hot.'),
      ],
      homework: 'Write the weather today and for the next 3 days (Oggi c\'è il sole, domani piove…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · What do you wear in each season?',
      titleIt: 'Lezione 3 · Cosa metti in ogni stagione?',
      summary: 'Mettere / indossare, l\'ombrello, la sciarpa, i guanti — with a dialogue about the weather.',
      minutes: 12,
      sections: [
        {
          title: 'Cosa metti in ogni stagione?',
          table: [
            ['Stagione e meteo', 'Cosa metto'],
            ['IN ESTATE (fa caldo, c\'è il sole)', 'Metto una maglietta leggera e i sandali.'],
            ['IN INVERNO (fa freddo, nevica)', 'Indosso il cappotto, il maglione e gli stivali.'],
            ['IN PRIMAVERA (è nuvoloso, c\'è vento)', 'Porto una camicia e una giacca leggera.'],
            ['IN AUTUNNO (piove spesso)', 'Metto l\'impermeabile e prendo l\'ombrello.'],
          ],
        },
        {
          title: 'Accessori per il meteo',
          words: [
            wp('l\'ombrello', 'umbrella', 'Chhatarī', { pron: 'lom-BREL-loh', img: 'Umbrella', ex: 'Porto l\'ombrello in borsa.', exEn: 'I carry the umbrella in my bag.' }),
            wp('la sciarpa', 'scarf', 'Galobandh / Mafalar', { pron: 'lah SHAR-pah', img: 'Scarf', ex: 'Quando fa freddo metto la sciarpa.', exEn: 'When it\'s cold I put on a scarf.' }),
            wp('i guanti', 'gloves', 'Dastāne', { pron: 'ee GWAHN-tee', img: 'Gloves' }),
            wp('il cappello', 'hat', 'Ṭopī', { pron: 'eel kahp-PEL-loh', img: 'Billed cap' }),
            wp('il maglione', 'jumper / sweater', 'Svaiṭar', { pron: 'eel mah-LYOH-neh', img: 'Coat' }),
            wp('gli stivali', 'boots', 'Lambe būṭ', { pron: 'lyee stee-VAH-lee', img: 'Womans boot' }),
            wp('i sandali', 'sandals', 'Saiṇḍal / Chappal', { pron: 'ee SAHN-dah-lee', img: 'Womans sandal' }),
          ],
        },
        {
          title: 'Dialogo: che tempo fa fuori?',
          situation: 'Chiara phones her friend Karam on a winter morning.',
          dialogue: [
            lp('Chiara', 'Ciao Karam! Che tempo fa oggi a Mantova?', 'Hi Karam! What\'s the weather like in Mantova today?', 'Sat Srī Akāl Karam! Ajj Mantova vich mausam kiven dā hai?'),
            lp('Karam', 'È nuvoloso, c\'è vento e fa molto freddo! Forse piove.', 'It\'s cloudy, it\'s windy and it\'s very cold! Maybe it will rain.', 'Baddalvāī hai, havā chal rahī hai ate bahut ṭhaṇḍ hai! Shāid mīṁh pavegā.'),
            lp('Chiara', 'Allora metto il cappotto nero e prendo l\'ombrello.', 'Then I\'ll put on my black coat and take the umbrella.', 'Phir maiṁ kāḷā koṭ pā laindī hāṁ ate chhatarī lai laindī hāṁ.'),
            lp('Karam', 'Sì, copriti bene! In estate invece c\'è il sole e fa sempre caldo.', 'Yes, wrap up well! In summer, instead, it\'s sunny and always hot.', 'Hāṁ, chaṅgī tarhāṁ ḍhakk ke niklo! Garmīāṁ vich dhupp hundī hai ate hameshā garmī hundī hai.'),
          ],
        },
        {
          title: 'Role-play: previsioni meteo e valigia',
          text: '1. **Scegli una stagione:** descrivi il meteo di oggi o in vacanza.\n2. **Scegli 3 capi adatti:** indica vestiti e colori giusti.\n3. **Dai un consiglio:** usa *Devi mettere…* o *Prendi l\'ombrello!*',
        },
      ],
      exercises: [
        tf('A Mantova oggi c\'è il sole.', false, 'In Mantova today it\'s sunny.'),
        tf('Oggi fa molto freddo.', true, 'Today it\'s very cold.'),
        tf('Chiara prende l\'ombrello.', true, 'Chiara takes the umbrella.'),
        mc('In summer you wear…', 'una maglietta leggera e i sandali', 'il cappotto e gli stivali', 'la sciarpa e i guanti', 'il maglione e il cappello'),
        mc('It\'s raining. You take…', 'l\'ombrello', 'i sandali', 'la maglietta leggera', 'gli occhiali da sole'),
        fb('In inverno ___ il cappotto e il maglione.', ['indosso', 'piove', 'nevica', 'fa']),
        wo('Metto il cappotto e prendo l\'ombrello', 'I put on my coat and take the umbrella'),
      ],
      homework: 'Prepare a suitcase for a trip in winter and in summer: write 4 clothes for each.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — Ripasso generale completo A1 (Capitoli 1–7)
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Review',
      title: 'Lesson 4 · A1 review: who you are, free time, food',
      titleIt: 'Lezione 4 · Ripasso A1: identità, tempo libero e cibo',
      summary: 'Chapters 1–4: presentarsi, famiglia, numeri, giorni e mesi, il bar e la spesa.',
      minutes: 15,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 7 · Lezione 4 (PDF)' },
        {
          title: 'Tutto il programma (Capitoli 1–7)',
          table: [
            ['Capitoli', 'Temi', 'Cosa sai fare'],
            ['1–2', 'Identità e famiglia', 'saluti, essere, avere, chiamarsi, numeri, possessivi'],
            ['3–4', 'Tempo libero e cibo', 'giorni, mesi, presente (-are / -ere / -ire), bar e spesa'],
            ['5–6', 'Città, routine e lavoro', 'strada, andare / venire, modali, riflessivi, mestieri'],
            ['7', 'Vestiti, colori e meteo', 'accordo aggettivi, questo / quello, taglie e stagioni'],
          ],
        },
        {
          title: 'Blocco 1 · Capitoli 1 e 2: presentarsi e famiglia',
          practice: false,
          words: [
            wp('Mi chiamo Amarjit, sono indiano.', 'My name is Amarjit, I\'m Indian.', 'Merā nāṁ Amarjit hai, maiṁ Bhāratī hāṁ.'),
            wp('Ho 32 anni e abito a Mantova.', 'I\'m 32 and I live in Mantova.', 'Maiṁ 32 sāl dā hāṁ ate Mantova vich rahindā hāṁ.'),
            wp('Tu di dove sei? Come si chiama Lei?', 'Where are you from? What is your name? (polite)', 'Tusīṁ kitthoṁ ho? Tuhāḍā nāṁ kī hai jī?'),
            wp('Mio padre, mia madre, i miei genitori.', 'My father, my mother, my parents.', 'Mere pitā jī, merī mātā jī, mere māpe.'),
            wp('Qual è il tuo numero di telefono?', 'What is your phone number?', 'Tuhāḍā fon nambar kī hai?'),
          ],
        },
        { tip: 'Possessives with family: **no article** with one relative (*mio padre, mia sorella*), but **with the article** in the plural (*i miei genitori, le mie sorelle*).' },
        {
          title: 'Blocco 2 · Capitoli 3 e 4: tempo libero, bar e spesa',
          practice: false,
          words: [
            wp('Il sabato leggo, cucino e gioco a calcio.', 'On Saturdays I read, cook and play football.', 'Shanivār nūṁ maiṁ paṛhdā hāṁ, khāṇā banāundā hāṁ ate fuṭbāl kheḍdā hāṁ.'),
            wp('Vorrei un caffè, un cornetto e il conto.', 'I\'d like a coffee, a croissant and the bill.', 'Mainūṁ ikk kauffī, ikk kroissāṇṭ ate bill chāhīdā hai.'),
            wp('Un chilo di mele, mezzo litro di latte.', 'A kilo of apples, half a litre of milk.', 'Ikk kilo seb, addhā liṭar duddh.'),
            wp('In frigo c\'è il latte, ma non ci sono uova.', 'There\'s milk in the fridge, but there are no eggs.', 'Frij vich duddh hai, par āṇḍe nahīṁ han.'),
          ],
        },
      ],
      exercises: [
        fb('Io ___ venticinque anni.', ['ho', 'sono', 'ha', 'hai']),
        fb('Io ___ indiano.', ['sono', 'ho', 'è', 'sei']),
        fb('___ padre fa l\'operaio.', ['Mio', 'Il mio', 'Mia', 'I miei']),
        fb('___ genitori abitano in India.', ['I miei', 'Mie', 'Miei', 'Le mie']),
        fb('Vorrei un chilo ___ mele.', ['di', 'del', 'da', 'a']),
        fb('In frigo non ___ uova.', ['ci sono', 'c\'è', 'è', 'sono']),
        mc('At the bar, to get the bill you ask for…', 'il conto', 'lo scontrino del treno', 'la cassa', 'il prezzo'),
        mc('Which day comes after giovedì?', 'venerdì', 'mercoledì', 'sabato', 'martedì'),
      ],
      homework: 'Introduce yourself in 5 sentences: name, age, nationality, city and family.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · A1 review: the key verbs',
      titleIt: 'Lezione 4 · Ripasso A1: i verbi chiave al presente',
      summary: 'Essere, avere, andare, venire, fare, dovere, potere, volere and the regular verbs.',
      minutes: 15,
      sections: [
        {
          title: 'Tabella dei verbi chiave (Capitoli 1–7)',
          table: [
            ['Verbo', 'io', 'tu', 'lui / lei / Lei', 'noi', 'voi / loro'],
            ['ESSERE', 'sono', 'sei', 'è', 'siamo', 'siete / sono'],
            ['AVERE', 'ho', 'hai', 'ha', 'abbiamo', 'avete / hanno'],
            ['ANDARE', 'vado', 'vai', 'va', 'andiamo', 'andate / vanno'],
            ['VENIRE', 'vengo', 'vieni', 'viene', 'veniamo', 'venite / vengono'],
            ['FARE', 'faccio', 'fai', 'fa', 'facciamo', 'fate / fanno'],
            ['DOVERE', 'devo', 'devi', 'deve', 'dobbiamo', 'dovete / devono'],
            ['POTERE', 'posso', 'puoi', 'può', 'possiamo', 'potete / possono'],
            ['VOLERE', 'voglio', 'vuoi', 'vuole', 'vogliamo', 'volete / vogliono'],
            ['regolari -are / -ere / -ire', '-o', '-i', '-a / -e / -e', '-iamo', '-ate, -ete, -ite / -ano, -ono'],
          ],
        },
        {
          title: 'Blocco 3 · Capitolo 5: casa e città',
          practice: false,
          words: [
            wp('Abito in affitto al secondo piano.', 'I rent a flat on the second floor.', 'Maiṁ dūjī manzil te kirāe te rahindā hāṁ.'),
            wp('Vada dritto, giri a destra.', 'Go straight on, turn right. (polite)', 'Sidhe jāo, sajje muṛo.'),
            wp('La farmacia è vicino alla banca.', 'The pharmacy is near the bank.', 'Davāīāṁ dī dukān baiṁk de neṛe hai.'),
          ],
        },
        {
          title: 'Blocco 4 · Capitolo 6: routine e lavoro',
          practice: false,
          words: [
            wp('Mi sveglio alle sei e mezza e mi preparo.', 'I wake up at half past six and get ready.', 'Maiṁ sāḍhe chhe vaje jāgdā hāṁ ate tiār hundā hāṁ.'),
            wp('Faccio l\'operaio in fabbrica.', 'I\'m a factory worker.', 'Maiṁ faikṭarī vich mazdūr hāṁ.'),
            wp('Vorrei fissare un appuntamento.', 'I would like to book an appointment.', 'Maiṁ mulākāt taiā karnā chāhundā hāṁ.'),
          ],
        },
      ],
      exercises: [
        fb('Noi ___ al mercato e poi prendiamo un caffè.', ['andiamo', 'andate', 'vanno', 'vado']),
        fb('Noi andiamo al mercato e poi ___ un caffè.', ['prendiamo', 'prendete', 'prendono', 'prendo']),
        fb('Tu ___ venire domani?', ['puoi', 'posso', 'può', 'potete']),
        fb('Tu ___ fare il colloquio?', ['vuoi', 'voglio', 'vuole', 'volete']),
        fb('Loro ___ da Amritsar.', ['vengono', 'venite', 'viene', 'veniamo']),
        fb('Che lavoro ___ voi?', ['fate', 'fanno', 'facciamo', 'fai']),
        fb('La mattina io ___ alle sette.', ['mi sveglio', 'sveglio', 'si sveglia', 'ti svegli']),
        fb('Il libro è ___ divano.', ['sul', 'su', 'nel', 'al']),
        fb('Abito vicino ___ stazione.', ['alla', 'a', 'la', 'della']),
      ],
      homework: 'Write the 8 key verbs (essere … volere) from memory for io, tu and noi.',
    },
    {
      kind: 'Review',
      title: 'Lesson 4 · A1 review: a typical day in Italy',
      titleIt: 'Lezione 4 · Ripasso A1: una giornata tipo in Italia',
      summary: 'A dialogue with everything from Chapters 1–7, and the 4 practice stations.',
      minutes: 15,
      sections: [
        {
          title: 'Blocco 5 · Capitolo 7: vestiti, colori e meteo',
          text: '**Esempio completo:** «In inverno fa freddo e piove: indosso questo cappotto nero e prendo l\'ombrello!»\n\n- **Accordo colori:** bianco / bianca / bianchi / bianche · verde, marrone (-e / -i) · blu (invariabile)\n- **Questo / quello:** Che taglia porta? Posso provarlo? Quanto costa? È caro / è economico!\n- **Stagioni e meteo:** fa caldo / freddo · piove · nevica · c\'è il sole · c\'è vento · è nuvoloso',
        },
        {
          title: 'Dialogo: una giornata tipo in Italia (Cap. 1–7)',
          situation: 'Navjot meets Sara in the street on a winter morning.',
          dialogue: [
            lp('Navjot', 'Ciao Sara! Mi sono alzato alle sette: oggi c\'è il sole ma fa freddo!', 'Hi Sara! I got up at seven: today it\'s sunny but cold!', 'Sat Srī Akāl Sara! Maiṁ satt vaje uṭhiā: ajj dhupp hai par ṭhaṇḍ hai!'),
            lp('Sara', 'Hai ragione, anch\'io ho messo questo cappotto grigio. Dove vai adesso?', 'You\'re right, I\'ve put on this grey coat too. Where are you going now?', 'Tūṁ sahī kihā, maiṁ vī eh saleṭī koṭ pāiā hai. Huṇ kitthe jā rihā haiṁ?'),
            lp('Navjot', 'Devo andare in centro: alle dieci ho un appuntamento per un lavoro part-time.', 'I have to go to the centre: at ten I have an appointment for a part-time job.', 'Mainūṁ shahir de vichkār jāṇā hai: das vaje pārṭ-ṭāīm kamm laī mulākāt hai.'),
            lp('Sara', 'In bocca al lupo! Dopo ci vediamo al bar vicino alla piazza per un caffè?', 'Good luck! Shall we meet later at the café near the square for a coffee?', 'Shubh kāmnāvāṁ! Bāad vich chaunk de neṛe kaife te kauffī laī milīe?'),
          ],
        },
        { tip: '**In bocca al lupo!** (literally "into the wolf\'s mouth") is how Italians say *good luck* before an exam or an interview. The answer is **Crepi!**' },
        {
          title: '4 stazioni pratiche: metti tutto in gioco!',
          text: '1. **Presentarsi:** chi sei, famiglia, città e lavoro.\n2. **Comprare:** al bar o in negozio, con colori e prezzi.\n3. **Orientarsi:** chiedi e spiega la strada in città.\n4. **La giornata:** orari, routine riflessiva e meteo.',
        },
      ],
      exercises: [
        tf('Oggi c\'è il sole ma fa freddo.', true, 'Today it\'s sunny but cold.'),
        tf('Sara ha un cappotto nero.', false, 'Sara has a black coat.'),
        tf('Navjot ha un appuntamento per un lavoro part-time.', true, 'Navjot has an appointment for a part-time job.'),
        tf('L\'appuntamento è alle otto.', false, 'The appointment is at eight.'),
        mc('Before an interview a friend says "In bocca al lupo!". You answer…', 'Crepi!', 'Prego!', 'Salute!', 'Buon appetito!'),
        fb('Porto una maglietta ___ e due pantaloni neri.', ['bianca', 'bianco', 'bianchi', 'bianche']),
        fb('Quanto costa ___ cappotto? È molto economico.', ['questo', 'questa', 'questi', 'queste']),
        wo('Dopo ci vediamo al bar per un caffè', 'Afterwards shall we meet at the café for a coffee'),
      ],
      homework: 'Prepare the 4 stations for the oral exam: say 3 sentences for each one.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST FINALE LIVELLO A1 (Capitoli 1–7)
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Final A1 test (Chapters 1–7)',
      titleIt: 'Test finale livello A1 (Capitoli 1–7)',
      summary: 'The final A1 test: listening, vocabulary and grammar from all 7 chapters. Pass mark: 60%.',
      minutes: 30,
      practice: false,
      sections: [
        {
          title: 'Test finale livello A1',
          text: 'This is the final test of level A1. It checks everything in Chapters 1–7.\n\n- **1. Listening** — sentences and a short story\n- **2. Vocabulary** — family, time, city, work, home and weather\n- **3. Grammar** — verbs, articles, prepositions, colours and questo / quello\n- **4. Writing** — your complete A1 profile\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (your A1 profile) and the **final oral exam** (introduce yourself, your day and job · order at the bar, shop or buy clothes · give directions, describe your home and the weather — 3 minutes) are done **with your teacher** in the live class.' },
        { phrasebook: true },
      ],
      exercises: [
        // 1A — Ascolta e scegli
        lis('1A · Listen and choose the sentence you hear', 'Ciao, mi chiamo Harpreet, ho ventotto anni e ho due figli.', 'HO VENTOTTO ANNI E HO DUE FIGLI', 'ABITO A MANTOVA E PARLO ITALIANO'),
        lis('1A · Listen and choose the sentence you hear', 'Dobbiamo fare la spesa perché in frigo non ci sono uova.', 'IN FRIGO NON CI SONO UOVA', 'VORREI UN CAFFÈ E UN CHILO DI MELE'),
        lis('1A · Listen and choose the sentence you hear', 'Il divano è in soggiorno, vicino al tavolo.', 'IL DIVANO È IN SOGGIORNO, VICINO AL TAVOLO', 'VADO IN STAZIONE A PIEDI, POI GIRO A DESTRA'),
        lis('1A · Listen and choose the sentence you hear', 'Buongiorno, posso fissare un appuntamento per il lavoro full-time?', 'POSSO FISSARE UN APPUNTAMENTO PER IL LAVORO FULL-TIME?', 'MI ALZO ALLE SETTE E FACCIO L\'OPERAIO'),
        lis('1A · Listen and choose the sentence you hear', 'In negozio dico: compro questa giacca nera e quelle scarpe.', 'COMPRO QUESTA GIACCA NERA E QUELLE SCARPE', 'IN INVERNO FA FREDDO E NEVICA SPESSO'),
        // 1B — Il racconto
        lis('1B · Listen to the story. What day is it? (1)', 'Oggi è sabato, c\'è il sole e fa caldo.', 'SABATO', 'LUNEDÌ', 'MERCOLEDÌ', 'GIOVEDÌ'),
        lis('1B · Listen again: how does Tariq go to work? (2)', 'Tariq esce di casa alle otto e prende l\'autobus per andare al lavoro.', 'L\'AUTOBUS', 'LA BICI', 'IL TAXI', 'L\'AEREO'),
        lis('1B · Listen again: what job does Tariq do? (3)', 'Tariq va al lavoro come operaio.', 'OPERAIO', 'INSEGNANTE', 'MEDICO', 'COMMESSO'),
        lis('1B · Listen again: how much bread does he buy? (4)', 'Poi passa al mercato per comprare un chilo di pane.', 'UN CHILO', 'UN LITRO', 'UNA BOTTIGLIA', 'UNA SCATOLA'),
        lis('1B · Listen again: where does he make dinner? (5)', 'La sera torna a casa, prepara la cena in cucina e guarda un film.', 'CUCINA', 'BAGNO', 'GIARDINO', 'UFFICIO'),
        // 2A — Collega
        pairs('2A · Match each word with its meaning', [
          ['Cognato / Cognata', 'Brother- / sister-in-law · Sālā / Jījā / Nanāṇ'],
          ['Negozio', 'Shop · Dukān'],
          ['Stipendio / Contratto', 'Salary / Work contract · Tankhāh / Ikrārnāmā'],
          ['Piove', 'It\'s raining · Mīṁh pai rihā hai'],
          ['Armadio e lavatrice', 'Wardrobe and washing machine · Almārī ate mashīn'],
        ]),
        // 2B — Completa
        mc('2B · Stagione calda (giugno–agosto): ?', 'l\'estate', 'l\'inverno', 'l\'autunno', 'la primavera'),
        mc('2B · Contenitore per l\'acqua: una ?', 'bottiglia', 'scatola', 'fermata', 'giacca'),
        mc('2B · Chiedere il totale al ristorante: il ?', 'conto', 'prezzo', 'contratto', 'cappotto'),
        mc('2B · Dove si aspetta il bus: la ?', 'fermata dell\'autobus', 'stazione dei treni', 'cassa', 'piazza'),
        mc('2B · Chi guida un veicolo per lavoro: l\'?', 'autista', 'operaio', 'impiegato', 'insegnante'),
        // 3A — Verbi
        fb('3A · Io ___ (avere) venticinque anni e sono indiano.', ['ho', 'sono', 'ha', 'hai']),
        fb('3A · Io ho venticinque anni e ___ (essere) indiano.', ['sono', 'ho', 'è', 'sei']),
        fb('3A · Noi ___ (andare) al mercato e poi prendiamo un caffè.', ['andiamo', 'andate', 'vanno', 'vado']),
        fb('3A · Noi andiamo al mercato e poi ___ (prendere) un caffè.', ['prendiamo', 'prendete', 'prendono', 'prendo']),
        fb('3A · Tu ___ (potere) venire domani e vuoi fare il colloquio?', ['puoi', 'posso', 'può', 'potete']),
        fb('3A · Tu puoi venire domani e ___ (volere) fare il colloquio?', ['vuoi', 'voglio', 'vuole', 'volete']),
        // 3B — Forme grammaticali
        fb('3B · In cucina ___ due sedie e un tavolo.', ['ci sono', 'c\'è']),
        fb('3B · … ma non ___ pane.', ['c\'è', 'ci sono']),
        fb('3B · Compro ___ pomodori.', ['dei', 'un chilo']),
        fb('3B · … e poi vado ___ medico.', ['dal', 'al']),
        fb('3B · La mattina io ___ (svegliarsi) alle sette.', ['mi sveglio', 'sveglio', 'si sveglia', 'ti svegli']),
        fb('3B · … e ___ (prepararsi) in fretta.', ['mi preparo', 'preparo', 'si prepara', 'ti prepari']),
        fb('3B · Il libro è ___ divano.', ['sul', 'su']),
        fb('3B · Io abito ___ alla stazione.', ['vicino', 'vicino a']),
        // 3C — Colori e dimostrativi
        fb('3C · Porto una maglietta ___.', ['bianca', 'bianco']),
        fb('3C · … e due pantaloni ___.', ['neri', 'nera']),
        fb('3C · Ho una gonna ___.', ['verde', 'verdi']),
        fb('3C · … e una sciarpa ___.', ['blu', 'blua']),
        fb('3C · Quanto costa ___ cappotto?', ['questo', 'questa']),
        fb('3C · È molto ___.', ['economico', 'economica']),
        fb('3C · Preferisco ___ scarpe marroni lì sul tavolo.', ['quelle', 'quei']),
        // 4 — Il mio profilo A1
        wo('Mi chiamo Ravi e vivo in Italia', 'My name is Ravi and I live in Italy'),
        wo('La mattina mi sveglio alle sette', 'In the morning I wake up at seven'),
        wo('Vicino a casa c\'è una farmacia', 'Near my home there is a pharmacy'),
        wo('Oggi fa freddo e indosso il cappotto', 'Today it\'s cold and I\'m wearing my coat'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences of your A1 profile (who you are, your morning, your job, your home, the weather and your clothes). Then prepare the 3-minute final oral exam.',
    },
  ],
};
