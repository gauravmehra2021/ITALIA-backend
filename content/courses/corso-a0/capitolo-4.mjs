import { wp, lp, fb, wo, tf, tr, mc, lis, pairs } from '../../lib.mjs';

// Capitolo 4 of the school syllabus (Corso di Italiano, livello A0 → A1).
// Source: syllabus/Capitolo 4 Lezione 1–4 (.pptx) and Test Capitolo 4 (.docx).

const slides = (lez) => `/content/slides/corso-a0-cap4-lez${lez}-slides.pdf`;

export default {
  key: 'capitolo-4',
  title: 'Chapter 4: At the café, the restaurant and the supermarket',
  titleIt: 'Capitolo 4: Al bar, al ristorante e al supermercato',
  cover: 'Hot beverage',
  description: 'Order at the bar with vorrei and prendo, read a restaurant menu and ask for the bill, explain allergies and food preferences, buy food with the right quantities and containers, count over 100, and use c\'è / ci sono and the partitives (del, della, dei…).',
  canDo: [
    'I can order food and drinks at the bar and ask how much it costs',
    'I can read an Italian menu, order a meal and ask for the bill',
    'I can say if I am vegetarian, allergic or don\'t eat something',
    'I can buy food with kilos, etti, bottles and packets',
    'I can read prices and numbers over 100',
    'I can say what there is and isn\'t with c\'è / ci sono and the partitives',
  ],
  liveClassFocus: 'Role-plays: barista and customer at the bar, waiter and customer at the restaurant (with the bill), and a full shopping trip with a list, quantities and the total in euro.',
  lessons: [
    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 1 — Al bar
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 1 · At the café: drinks, food and meals',
      titleIt: 'Lezione 1 · Al bar: bevande, cibo e pasti',
      summary: 'Caffè, cappuccino, cornetto, acqua frizzante… and the meals of the day.',
      minutes: 12,
      sections: [
        { slides: slides(1), title: 'Class slides: Capitolo 4 · Lezione 1 (PDF)' },
        {
          title: 'Al bar: bevande calde e fredde',
          words: [
            wp('un caffè', 'an espresso', 'Kaafi', { pron: 'oon kahf-FEH', img: 'Hot beverage' }),
            wp('un cappuccino', 'a cappuccino', 'Kapuchino', { pron: 'oon kahp-poot-CHEE-noh', img: 'Teacup without handle' }),
            wp('un tè', 'a tea', 'Chaah', { pron: 'oon TEH', img: 'Teapot' }),
            wp('un cornetto / una brioche', 'a croissant', 'Mitha kroisaan', { pron: 'oon kor-NET-toh', img: 'Croissant' }),
            wp('l\'acqua naturale', 'still water', 'Saada paani', { pron: 'LAHK-kwah nah-too-RAH-leh', img: 'Droplet' }),
            wp('l\'acqua frizzante', 'sparkling water', 'Gais vaala paani', { pron: 'LAHK-kwah freed-DZAHN-teh', img: 'Bottle with popping cork' }),
            wp('un succo di frutta', 'a fruit juice', 'Phalaan da joos', { pron: 'oon SOOK-koh dee FROOT-tah', img: 'Beverage box' }),
            wp('una spremuta', 'a fresh orange juice', 'Taaza santre da joos', { pron: 'OO-nah spreh-MOO-tah', img: 'Tangerine' }),
          ],
        },
        {
          title: 'I pasti della giornata',
          words: [
            wp('la colazione', 'breakfast', 'Naashta', { pron: 'lah koh-lah-tsee-OH-neh', img: 'Sunrise', ex: 'La mattina faccio colazione al bar.', exEn: 'In the morning I have breakfast at the café.' }),
            wp('il pranzo', 'lunch', 'Dupahar da khaana', { pron: 'eel PRAHN-tsoh', img: 'Fork and knife with plate', ex: 'In pausa pranzo prendo un panino al bar.', exEn: 'On my lunch break I have a sandwich at the café.' }),
            wp('la merenda', 'afternoon snack', 'Shaam da naashta', { pron: 'lah meh-REN-dah', img: 'Soft ice cream', ex: 'Alle quattro faccio merenda con un succo.', exEn: 'At four I have a snack with a juice.' }),
            wp('la cena', 'dinner', 'Raat da khaana', { pron: 'lah CHEH-nah', img: 'Spaghetti', ex: 'La sera ceno a casa con la mia famiglia.', exEn: 'In the evening I have dinner at home with my family.' }),
          ],
        },
        {
          title: 'Cibo al bar',
          practice: false,
          words: [
            wp('un panino / un tramezzino', 'a sandwich / a triangle sandwich', 'Saindvich', { img: 'Sandwich' }),
            wp('una pizza / un\'insalata', 'a pizza / a salad', 'Piza / Salad', { img: 'Pizza' }),
            wp('un toast / una focaccia', 'a toasted sandwich / focaccia bread', 'Tost / Fokacha', { img: 'Bread' }),
            wp('i biscotti', 'biscuits', 'Biskut', { img: 'Cookie' }),
          ],
        },
        { tip: 'In Italy **un caffè** is always a small, strong espresso. If you want milk, ask for **un cappuccino** or **un caffè macchiato**. Italians drink cappuccino only at breakfast!' },
      ],
      exercises: [
        tr('l\'acqua frizzante', 'sparkling water', 'still water', 'fruit juice', 'hot tea'),
        tr('la merenda', 'afternoon snack', 'breakfast', 'lunch', 'dinner'),
        mc('What is "la colazione"?', 'breakfast', 'lunch', 'dinner', 'a snack'),
      ],
      homework: 'Write what you eat and drink for colazione, pranzo and cena.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 1 · Ordering: vorrei, prendo, quanto costa?',
      titleIt: 'Lezione 1 · Ordinare al bar e i prezzi',
      summary: 'Polite phrases for ordering, the receipt and the change, and reading prices in euro.',
      minutes: 12,
      sections: [
        {
          title: 'Ordinare al bar: VORREI e PRENDO',
          table: [
            ['Funzione', 'Espressione', 'Esempio', 'English / Punjabi'],
            ['ask politely', 'Vorrei… (per favore)', 'Buongiorno, vorrei un caffè.', 'I would like… · Mainu chaahida hai'],
            ['choose', 'Io prendo…', 'Io prendo un panino e un\'acqua.', 'I\'ll have… · Main lainda haan'],
            ['ask the barista', 'Avete…? / C\'è…?', 'Scusi, avete cornetti alla crema?', 'Do you have…? · Ki tuhade kol hai?'],
            ['ask the price', 'Quanto costa? / Quanto viene?', 'Scusi, quanto viene in tutto?', 'How much is it? · Kinne paise hoe?'],
            ['barista\'s answer', 'Subito! / Ecco a lei!', 'Ecco il cappuccino. Sono 1,50 €.', 'Right away / Here you are · Aah lao ji'],
          ],
        },
        {
          title: '3 parole importanti al bar italiano',
          words: [
            wp('lo scontrino', 'the receipt', 'Rasid', { pron: 'loh skon-TREE-noh', img: 'Receipt', ex: 'Prima paghi alla cassa e prendi lo scontrino.', exEn: 'First you pay at the till and take the receipt.' }),
            wp('il resto', 'the change (money)', 'Baaki paise', { pron: 'eel RES-toh', img: 'Coin', ex: 'Ecco 5 euro. — Grazie, ecco il resto!', exEn: 'Here are 5 euros. — Thank you, here\'s your change!' }),
            wp('al banco / al tavolo', 'at the counter / at the table', 'Kauntar te / Tebal te', { pron: 'ahl BAHN-koh / ahl TAH-voh-loh', img: 'Chair', ex: 'Al banco costa meno rispetto al tavolo!', exEn: 'At the counter it costs less than at the table!' }),
          ],
        },
        {
          title: 'Quanto costa? Leggere i prezzi',
          table: [
            ['Prezzo', 'Prodotto', 'Si legge'],
            ['1,20 €', 'un caffè espresso', 'un euro e venti'],
            ['1,50 €', 'un cappuccino', 'un euro e cinquanta'],
            ['4,50 €', 'un panino imbottito', 'quattro euro e cinquanta'],
          ],
        },
        { tip: 'One thing: **Quanto costa il caffè? — Costa 1,20 €.** More things: **Quanto costano due panini? — Costano 9,00 €.** The comma is the decimal point: 1,20 € = *un euro e venti*.' },
      ],
      exercises: [
        mc('How do you ask politely for a coffee?', 'Vorrei un caffè, per favore.', 'Dammi un caffè.', 'Caffè!', 'Voglio caffè subito.'),
        mc('You want to know the total. You say…', 'Quanto viene in tutto?', 'Che ore sono?', 'Come si chiama?', 'Dov\'è il bar?'),
        mc('"Lo scontrino" is…', 'the receipt', 'the change', 'the counter', 'the menu'),
        lis('Listen: how much is it?', 'Un euro e venti.', '1,20 €', '1,02 €', '2,10 €', '12,00 €'),
        lis('Listen: how much is it?', 'Quattro euro e cinquanta.', '4,50 €', '4,15 €', '5,40 €', '14,50 €'),
        fb('Quanto ___ due panini?', ['costano', 'costa', 'costi', 'costiamo']),
        fb('Quanto ___ il cappuccino?', ['costa', 'costano', 'costi', 'costo']),
        wo('Vorrei un caffè per favore', 'I would like a coffee please'),
      ],
      homework: 'Write 3 orders you could make at the bar with vorrei or prendo, and ask the price.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 1 · Dialogue: Good morning, what would you like?',
      titleIt: 'Lezione 1 · Dialogo: Buongiorno, cosa desidera?',
      summary: 'A customer orders breakfast at the bar.',
      minutes: 8,
      sections: [
        {
          title: 'Dialogo: Buongiorno, cosa desidera?',
          situation: 'A customer walks into a café in the morning.',
          dialogue: [
            lp('Barista', 'Buongiorno! Prego, cosa desidera?', 'Good morning! What would you like?', 'Sat Sri Akal! Ki chaahida hai?'),
            lp('Cliente', 'Buongiorno! Vorrei un cappuccino e un cornetto alla marmellata.', 'Good morning! I would like a cappuccino and a jam croissant.', 'Sat Sri Akal! Mainu ikk kapuchino ate jaim vaala kroisaan chaahida hai.'),
            lp('Barista', 'Subito! Da bere prende qualcos\'altro?', 'Right away! Anything else to drink?', 'Hune! Peen lai hor kujh?'),
            lp('Cliente', 'Sì, prendo anche una bottiglia d\'acqua naturale. Quanto viene?', 'Yes, I\'ll also have a bottle of still water. How much is it?', 'Haan, ikk botal saada paani vi. Kinne paise hoe?'),
          ],
        },
        {
          title: 'Gioco di ruolo: barista e cliente',
          text: '1. **Studente A, il barista:** saluta, chiedi «Cosa desidera?» e di\' il prezzo.\n2. **Studente B, il cliente:** ordina con «Vorrei… / Prendo…» e chiedi «Quanto viene?».\n3. **Poi cambiate ruolo!** Provate con la colazione (mattina) e il pranzo (panino e acqua).',
        },
      ],
      exercises: [
        tf('Il cliente vuole un caffè.', false, 'The customer wants an espresso.'),
        tf('Il cliente prende un cornetto alla marmellata.', true, 'The customer has a jam croissant.'),
        tf('Il cliente prende anche l\'acqua frizzante.', false, 'The customer also has sparkling water.'),
        tr('Cosa desidera?', 'What would you like?', 'How much is it?', 'Where do you live?', 'Anything else?'),
        wo('Prendo anche una bottiglia d\'acqua', 'I\'ll also have a bottle of water'),
      ],
      homework: 'Role-play at home: order your breakfast in Italian from start to finish, out loud.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 2 — Al ristorante
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 2 · The Italian menu',
      titleIt: 'Lezione 2 · Le portate del menù',
      summary: 'Antipasto, primo, secondo, contorno, dolce — and the coperto and the conto.',
      minutes: 12,
      sections: [
        { slides: slides(2), title: 'Class slides: Capitolo 4 · Lezione 2 (PDF)' },
        {
          title: 'Le portate del menù italiano',
          words: [
            wp('l\'antipasto', 'starter', 'Shuruati khaana', { pron: 'lahn-tee-PAHS-toh', img: 'Olive', ex: 'Bruschette, olive, formaggi.', exEn: 'Bruschetta, olives, cheeses.' }),
            wp('il primo', 'first course (pasta, rice, soup)', 'Pahila khaana', { pron: 'eel PREE-moh', img: 'Spaghetti', ex: 'Pasta al pomodoro, risotto, lasagne.', exEn: 'Pasta with tomato, risotto, lasagne.' }),
            wp('il secondo', 'main course (meat, fish, eggs)', 'Mukkh khaana', { pron: 'eel seh-KON-doh', img: 'Poultry leg', ex: 'Pollo arrosto, pesce, frittata.', exEn: 'Roast chicken, fish, omelette.' }),
            wp('il contorno', 'side dish', 'Said dish', { pron: 'eel kon-TOR-noh', img: 'Green salad', ex: 'Insalata mista, patate al forno.', exEn: 'Mixed salad, roast potatoes.' }),
            wp('il dolce', 'dessert', 'Mithai', { pron: 'eel DOL-cheh', img: 'Shortcake', ex: 'Tiramisù, gelato, panna cotta.', exEn: 'Tiramisu, ice cream, panna cotta.' }),
            wp('le bevande', 'drinks', 'Peen lai', { pron: 'leh beh-VAHN-deh', img: 'Cup with straw' }),
            wp('il coperto', 'cover charge (bread and service)', 'Tebal chaarj', { pron: 'eel koh-PEHR-toh', img: 'Fork and knife with plate' }),
            wp('il conto', 'the bill', 'Bil', { pron: 'eel KON-toh', img: 'Receipt' }),
          ],
        },
        { tip: 'Italians don\'t eat every course! A normal meal is **un primo** or **un secondo con contorno**. The **coperto** (e.g. 2,00 € per person) is a normal charge for bread and table service, not a tip.' },
      ],
      exercises: [
        mc('Pasta and risotto are…', 'il primo', 'il secondo', 'il contorno', 'il dolce'),
        mc('Roast chicken is…', 'il secondo', 'il primo', 'l\'antipasto', 'il dolce'),
        mc('Tiramisù is…', 'il dolce', 'il contorno', 'il primo', 'l\'antipasto'),
        mc('An insalata mista with your main course is…', 'il contorno', 'il coperto', 'il conto', 'il primo'),
      ],
      homework: 'Write your perfect Italian menu: antipasto, primo, secondo, contorno and dolce.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 2 · Prendere, mangiare, pagare and food needs',
      titleIt: 'Lezione 2 · Prendere, mangiare, pagare e le esigenze alimentari',
      summary: 'The three restaurant verbs, allergies, "I\'m vegetarian" and "without…".',
      minutes: 12,
      sections: [
        {
          title: 'Presente: PRENDERE, MANGIARE, PAGARE',
          table: [
            ['Soggetto', 'PRENDERE (order / take)', 'MANGIARE (eat · khaana)', 'PAGARE (pay · paise dene)'],
            ['io', 'prendo', 'mangio', 'pago'],
            ['tu', 'prendi', 'mangi (one i)', 'paghi (+ h)'],
            ['lui / lei', 'prende', 'mangia', 'paga'],
            ['noi', 'prendiamo', 'mangiamo (one i)', 'paghiamo (+ h)'],
            ['voi', 'prendete', 'mangiate', 'pagate'],
            ['loro', 'prendono', 'mangiano', 'pagano'],
          ],
        },
        {
          title: 'Allergie, intolleranze e preferenze',
          words: [
            wp('Sono allergico / allergica a…', 'I am allergic to…', 'Mainu … ton alarji hai', { img: 'Peanuts', pron: 'SOH-noh ahl-LEHR-jee-koh' }),
            wp('Sono intollerante al lattosio.', 'I am lactose intolerant.', 'Mainu dudh ton intolerans hai.', { img: 'Glass of milk' }),
            wp('Sono vegetariano / vegetariana.', 'I am vegetarian.', 'Main shaakahari haan.', { pron: 'SOH-noh veh-jeh-tah-ree-AH-noh', img: 'Leafy green' }),
            wp('Non mangio carne / maiale / pesce.', 'I don\'t eat meat / pork / fish.', 'Main maas / suar / machhi nahi khaanda.', { img: 'Cut of meat' }),
            wp('un panino senza prosciutto', 'a sandwich without ham', '… ton bina', { img: 'Sandwich', pron: 'SEN-tsah proh-SHOOT-toh' }),
            wp('Questo piatto ha carne o uova?', 'Does this dish have meat or eggs?', 'Ki is dish vich maas jaan ande han?', { img: 'Egg' }),
          ],
        },
        { tip: '**Mangiare** keeps one **i**: *tu mangi, noi mangiamo*. **Pagare** adds an **h** to keep the hard g sound: *tu paghi, noi paghiamo*.' },
      ],
      exercises: [
        fb('Al ristorante noi ___ un primo piatto.', ['prendiamo', 'prendete', 'prendono', 'prendo']),
        fb('Tu ___ la carne o sei vegetariano?', ['mangi', 'mangii', 'mangia', 'mangiate']),
        fb('Alla cassa tu ___ con la carta?', ['paghi', 'pagi', 'paga', 'pagate']),
        fb('I clienti ___ il conto.', ['pagano', 'paghano', 'paga', 'paghiamo']),
        fb('Noi ___ insieme.', ['paghiamo', 'pagiamo', 'pagano', 'paga']),
        mc('You don\'t eat pork. You say…', 'Non mangio maiale.', 'Non mangio pesce.', 'Sono allergico al latte.', 'Mangio solo maiale.'),
        tr('Sono vegetariano.', 'I am vegetarian.', 'I am hungry.', 'I am allergic.', 'I am full.'),
        wo('Vorrei un panino senza prosciutto', 'I would like a sandwich without ham'),
      ],
      homework: 'Write 3 sentences about what you eat and don\'t eat (Sono…, Non mangio…, Sono allergico a…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 2 · Dialogue: ordering and asking for the bill',
      titleIt: 'Lezione 2 · Dialogo: ordinare e chiedere il conto',
      summary: 'Two friends order dinner and pay by card.',
      minutes: 10,
      sections: [
        {
          title: 'Chiedere il conto e pagare',
          words: [
            wp('Scusi, il conto, per favore!', 'Excuse me, the bill, please!', 'Maaf karna, bil le ao ji!', { img: 'Receipt' }),
            wp('Ci porta il conto?', 'Could you bring us the bill?', 'Saanu bil de devo?', { img: 'Receipt' }),
            wp('Pago in contanti.', 'I pay in cash.', 'Main nakad paise denda haan.', { img: 'Euro banknote' }),
            wp('Pago con la carta.', 'I pay by card.', 'Main kaard naal paise denda haan.', { img: 'Credit card' }),
            wp('Paghiamo insieme / separato.', 'We pay together / separately.', 'Asi ikatthe / vakhre paise devange.', { img: 'Credit card' }),
          ],
        },
        {
          title: 'Dialogo: ordinare e chiedere il conto',
          situation: 'Two friends at a restaurant in the evening. One is vegetarian, the other doesn\'t eat pork.',
          dialogue: [
            lp('Cameriere', 'Buonasera! Cosa prendete da mangiare?', 'Good evening! What are you having to eat?', 'Shubh shaam! Tusi ki khaoge?'),
            lp('Cliente 1', 'Io sono vegetariano: prendo un primo di pasta al pomodoro.', 'I am vegetarian: I\'ll have pasta with tomato sauce.', 'Main shaakahari haan: main tamatar vaala paasta lainda haan.'),
            lp('Cliente 2', 'Io non mangio maiale: prendo pollo arrosto con contorno di patate.', 'I don\'t eat pork: I\'ll have roast chicken with potatoes.', 'Main suar da maas nahi khaanda: murga ate aalu lainda haan.'),
            lp('Cliente 1', 'Scusi, ci porta il conto? Paghiamo con la carta.', 'Excuse me, could you bring us the bill? We\'ll pay by card.', 'Maaf karna, bil le ao ji? Asi kaard naal paise devange.'),
          ],
        },
        {
          title: 'Gioco di ruolo: cameriere e cliente',
          text: '1. **Ordina le portate:** antipasto, primo, secondo, contorno o dolce con «Prendo…».\n2. **Esprimi preferenze:** «Non mangio… / Sono vegetariano / Senza…».\n3. **Chiedi il conto:** «Il conto, per favore!» e scegli contanti o carta.',
        },
      ],
      exercises: [
        tf('Il cliente 1 è vegetariano.', true, 'Customer 1 is vegetarian.'),
        tf('Il cliente 2 mangia maiale.', false, 'Customer 2 eats pork.'),
        tf('Pagano in contanti.', false, 'They pay in cash.'),
        mc('At the end of the meal you say…', 'Il conto, per favore!', 'Buongiorno!', 'Vorrei un antipasto.', 'Sono vegetariano.'),
        tr('Paghiamo separato.', 'We pay separately.', 'We pay together.', 'We pay by card.', 'We pay in cash.'),
        wo('Scusi ci porta il conto', 'Excuse me, could you bring us the bill'),
      ],
      homework: 'Role-play with a partner: order a full meal, say one food preference and ask for the bill.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 3 — Al supermercato e al mercato
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Vocabulary',
      title: 'Lesson 3 · Food at the supermarket and the market',
      titleIt: 'Lezione 3 · Frutta, verdura, carne, pesce e latticini',
      summary: 'Mele, pomodori, pollo, pesce, latte, formaggio, pane… and where to buy them.',
      minutes: 12,
      sections: [
        { slides: slides(3), title: 'Class slides: Capitolo 4 · Lezione 3 (PDF)' },
        {
          title: 'La frutta e la verdura',
          words: [
            wp('le mele', 'apples', 'Seb', { pron: 'leh MEH-leh', img: 'Red apple' }),
            wp('le banane', 'bananas', 'Kele', { pron: 'leh bah-NAH-neh', img: 'Banana' }),
            wp('le arance', 'oranges', 'Santre', { pron: 'leh ah-RAHN-cheh', img: 'Tangerine' }),
            wp('le fragole', 'strawberries', 'Straabariaan', { pron: 'leh FRAH-goh-leh', img: 'Strawberry' }),
            wp('i pomodori', 'tomatoes', 'Tamatar', { pron: 'ee poh-moh-DOH-ree', img: 'Tomato' }),
            wp('le patate', 'potatoes', 'Aalu', { pron: 'leh pah-TAH-teh', img: 'Potato' }),
            wp('le cipolle', 'onions', 'Gande / Piaz', { pron: 'leh chee-POL-leh', img: 'Onion' }),
            wp('le carote', 'carrots', 'Gaajaraan', { pron: 'leh kah-ROH-teh', img: 'Carrot' }),
          ],
        },
        {
          title: 'Carne, pesce, latticini e pane',
          words: [
            wp('il pollo', 'chicken', 'Murga', { pron: 'eel POL-loh', img: 'Poultry leg' }),
            wp('la carne', 'meat', 'Maas', { pron: 'lah KAHR-neh', img: 'Cut of meat' }),
            wp('il pesce', 'fish', 'Machhi', { pron: 'eel PEH-sheh', img: 'Fish' }),
            wp('i gamberi', 'prawns', 'Jhinge', { pron: 'ee GAHM-beh-ree', img: 'Shrimp' }),
            wp('il latte', 'milk', 'Dudh', { pron: 'eel LAHT-teh', img: 'Glass of milk' }),
            wp('il formaggio', 'cheese', 'Panir', { pron: 'eel for-MAHD-joh', img: 'Cheese wedge' }),
            wp('il burro', 'butter', 'Makkhan', { pron: 'eel BOOR-roh', img: 'Butter' }),
            wp('il pane', 'bread', 'Roti', { pron: 'eel PAH-neh', img: 'Bread' }),
            wp('le uova', 'eggs', 'Ande', { pron: 'leh WOH-vah', img: 'Egg' }),
          ],
        },
        {
          title: 'Dove facciamo la spesa?',
          table: [
            ['Dove', 'Cosa', 'Esempio'],
            ['al mercato (open-air market · mandi)', 'frutta e verdura fresca di stagione', 'Compro un chilo di pomodori al mercato.'],
            ['al supermercato', 'latticini, pane, carne, pesce, prodotti confezionati', 'Prendo pasta, latte e pane al supermercato.'],
            ['in panetteria / al reparto forno', 'pane fresco, focaccia, biscotti', 'Compro il pane in panetteria.'],
            ['al banco frigo', 'latte, formaggio, yogurt, burro', 'Lo yogurt è al banco frigo.'],
          ],
        },
        { tip: '**Le uova** (eggs) is irregular: one egg is **un uovo**, two eggs are **due uova** — and it\'s feminine in the plural!' },
      ],
      exercises: [
        mc('Milk, cheese and yogurt are…', 'i latticini', 'la verdura', 'la frutta', 'la carne'),
        mc('Where do you buy fresh seasonal fruit and vegetables cheaply?', 'al mercato', 'in farmacia', 'in banca', 'al bar'),
        tr('le cipolle', 'onions', 'potatoes', 'carrots', 'tomatoes'),
      ],
      homework: 'Write your shopping list for this week in Italian, with the article (le mele, il latte…).',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 3 · Quantities and containers',
      titleIt: 'Lezione 3 · Unità di misura e contenitori',
      summary: 'Un chilo di, un etto di, una bottiglia di, un pacco di, una scatola di…',
      minutes: 12,
      sections: [
        {
          title: 'Chilo, etto, bottiglia, pacco',
          table: [
            ['Unità / contenitore', 'Misura', 'Esempio', 'English / Punjabi'],
            ['un chilo di / mezzo chilo di', '1.000 g / 500 g', 'Vorrei un chilo di mele e mezzo chilo di carne.', '1 kg / half a kg · ikk kilo / addha kilo'],
            ['un etto di / due etti di', '100 g / 200 g', 'Prendo due etti di formaggio fresco.', '100 g / 200 g · sau / do sau graam'],
            ['una bottiglia di / un litro di', 'liquids', 'Compro una bottiglia d\'acqua e un litro di latte.', 'a bottle / litre of · ikk botal / litar'],
            ['un pacco di', 'dry packets', 'Prendo un pacco di pasta, di riso o di biscotti.', 'a pack of · ikk paikat'],
            ['una scatola di / un vasetto di', 'tins / jars', 'Vorrei una scatola di tonno e un vasetto di yogurt.', 'a tin / jar of · ikk dabba / dabbi'],
          ],
        },
        {
          title: 'Examples',
          words: [
            wp('un chilo di mele', '1 kg of apples', 'Ikk kilo seb', { img: 'Red apple' }),
            wp('due etti di formaggio', '200 g of cheese', 'Do sau graam panir', { img: 'Cheese wedge' }),
            wp('una bottiglia d\'acqua', 'a bottle of water', 'Ikk botal paani', { img: 'Droplet' }),
            wp('un litro di latte', 'a litre of milk', 'Ikk litar dudh', { img: 'Glass of milk' }),
            wp('un pacco di pasta', 'a packet of pasta', 'Ikk paikat paasta', { img: 'Package' }),
            wp('una scatola di tonno', 'a tin of tuna', 'Ikk dabba tuna machhi', { img: 'Canned food' }),
          ],
        },
        { tip: 'Italians buy cheese and ham by the **etto** (100 grams): *un etto di prosciutto*, *due etti di formaggio*. After a quantity always use **di**: *un chilo **di** pomodori*.' },
      ],
      exercises: [
        mc('"Un etto" is…', '100 grams', '1 kilo', '500 grams', '10 grams'),
        mc('"Mezzo chilo" is…', '500 grams', '100 grams', '1.000 grams', '50 grams'),
        fb('Vorrei un chilo ___ mele.', ['di', 'a', 'in', 'con']),
        fb('Prendo un ___ di pasta.', ['pacco', 'litro', 'etto', 'vasetto']),
        fb('Compro una ___ d\'acqua.', ['bottiglia', 'scatola', 'pacco', 'chilo']),
        fb('Vorrei due ___ di formaggio.', ['etti', 'etto', 'litri', 'bottiglie']),
        wo('Vorrei un chilo di pomodori', 'I would like a kilo of tomatoes'),
      ],
      homework: 'Write 5 things you buy with the right quantity or container (un litro di latte…).',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 3 · Numbers over 100 and prices',
      titleIt: 'Lezione 3 · I numeri oltre 100 e i prezzi',
      summary: 'Cento, duecento, mille, duemila — read a receipt and pay at the supermarket.',
      minutes: 12,
      sections: [
        {
          title: 'Contare oltre 100',
          table: [
            ['100 – 199', '200 – 900', '1.000 +'],
            ['100 = cento', '200 = duecento', '1.000 = mille'],
            ['120 = centoventi', '300 = trecento', '1.500 = millecinquecento'],
            ['150 = centocinquanta', '500 = cinquecento', '2.000 = duemila'],
            ['185 = centottantacinque', '800 = ottocento', '10.000 = diecimila'],
          ],
        },
        {
          title: 'Numbers',
          practice: false,
          words: [
            wp('cento', '100', 'Sau', { pron: 'CHEN-toh' }),
            wp('duecento', '200', 'Do sau', { pron: 'doo-eh-CHEN-toh' }),
            wp('cinquecento', '500', 'Panj sau', { pron: 'cheen-kweh-CHEN-toh' }),
            wp('mille', '1.000', 'Ikk hazar', { pron: 'MEEL-leh' }),
            wp('duemila', '2.000', 'Do hazar', { pron: 'doo-eh-MEE-lah' }),
          ],
        },
        {
          title: 'Prezzi e pesi',
          table: [
            ['Prezzo / peso', 'Si legge'],
            ['1,80 € (un etto di formaggio)', 'un euro e ottanta'],
            ['14,50 € (un chilo di pesce)', 'quattordici euro e cinquanta'],
            ['125,90 € (spesa settimanale)', 'centoventicinque euro e novanta'],
            ['250 g', 'duecentocinquanta grammi'],
          ],
        },
        {
          title: 'Dialogo: al banco e alla cassa del supermercato',
          situation: 'A customer buys food for the week at the supermarket.',
          dialogue: [
            lp('Commesso', 'Buongiorno! Dica pure, cosa desidera?', 'Good morning! Tell me, what would you like?', 'Dassi ji, ki chaahida hai?'),
            lp('Cliente', 'Vorrei un chilo di pesce fresco, due etti di formaggio e mezzo chilo di pane.', 'I would like 1 kg of fresh fish, 200 g of cheese and half a kilo of bread.', 'Mainu ikk kilo taazi machhi, do sau graam panir ate addha kilo roti chaahidi hai.'),
            lp('Cliente', 'Ho anche due pacchi di pasta e una bottiglia d\'olio. Quanto viene in tutto?', 'I also have two packets of pasta and a bottle of oil. How much is it altogether?', 'Mere kol do paikat paasta ate ikk botal tel vi hai. Kinne paise hoe?'),
            lp('Cassiera', 'Sono centodieci euro e cinquanta. Paga con la carta?', 'That\'s 110.50 €. Are you paying by card?', '110,50 € hoe ji. Kaard naal paise devoge?'),
          ],
        },
        { tip: '**Cento** and **mille** don\'t take "un": 100 = *cento* (not "un cento"), 1.000 = *mille*. The plural of mille is **-mila**: *duemila, tremila*.' },
      ],
      exercises: [
        lis('Listen and choose the number', 'centoventi', '120', '102', '112', '220'),
        lis('Listen and choose the number', 'duecentocinquanta', '250', '205', '2.500', '150'),
        lis('Listen and choose the number', 'millecinquecento', '1.500', '1.050', '150', '5.100'),
        mc('How do you write 130?', 'centotrenta', 'centotredici', 'trecento', 'cento e trenta'),
        mc('How do you write 1.000?', 'mille', 'un mille', 'mila', 'cento cento'),
        tf('La cliente paga 110,50 €.', true, 'The customer pays 110.50 €.'),
        tf('La cliente compra due etti di pesce.', false, 'The customer buys 200 g of fish.'),
      ],
      homework: 'Role-play: make a shopping list over 100 € (e.g. 112,80 €) and read the total out loud.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // LEZIONE 4 — C'è / ci sono, partitivi e fare la spesa
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Grammar',
      title: 'Lesson 4 · There is, there are: c\'è / ci sono',
      titleIt: 'Lezione 4 · C\'è e ci sono',
      summary: 'Say what there is (and isn\'t) in the fridge, the kitchen and the shop.',
      minutes: 12,
      sections: [
        { slides: slides(4), title: 'Class slides: Capitolo 4 · Lezione 4 (PDF)' },
        {
          title: 'C\'è / ci sono: presenza e assenza',
          table: [
            ['Forma', 'Uso', 'Esempio', 'English / Punjabi'],
            ['C\'È + singolare', 'there is', 'In frigo c\'è una bottiglia di latte.', 'There is · ikk cheez hai'],
            ['CI SONO + plurale', 'there are', 'Sul tavolo ci sono quattro mele.', 'There are · bahut chizaan han'],
            ['NON C\'È / NON CI SONO', 'absence', 'In cucina non c\'è pane.', 'There isn\'t · nahi hai'],
            ['C\'È…? / CI SONO…?', 'question', 'Scusi, c\'è il pesce fresco oggi?', 'Is there…? · ki … hai?'],
          ],
        },
        {
          title: 'In cucina e nel negozio: cosa c\'è?',
          words: [
            wp('in frigo', 'in the fridge', 'Frij vich', { ex: 'C\'è lo yogurt e ci sono le carote, ma non c\'è il burro.', exEn: 'There\'s yogurt and there are carrots, but there\'s no butter.' }),
            wp('in dispensa', 'in the pantry', 'Rasoi di almari vich', { ex: 'Ci sono due pacchi di riso, ma non c\'è l\'olio.', exEn: 'There are two packets of rice, but there\'s no oil.' }),
            wp('al mercato', 'at the market', 'Mandi vich', { ex: 'Ci sono pomodori maturi? — Sì, eccoli!', exEn: 'Are there ripe tomatoes? — Yes, here they are!' }),
            wp('al supermercato', 'at the supermarket', 'Dukan vich', { ex: 'Scusi, c\'è il pane integrale? — Sì, al reparto forno.', exEn: 'Excuse me, is there wholemeal bread? — Yes, in the bakery section.' }),
          ],
          practice: false,
        },
        { tip: '**C\'è** = ci + è (singular). **Ci sono** = plural. Count the things: *c\'è **un** pezzo di formaggio* · *ci sono **sei** uova*.' },
      ],
      exercises: [
        fb('In frigo ___ una bottiglia di latte fresco.', ['c\'è', 'ci sono', 'non ci sono']),
        fb('Sul tavolo ___ tre bicchieri e due piatti.', ['ci sono', 'c\'è', 'è']),
        fb('Scusi, ___ del pane integrale oggi?', ['c\'è', 'ci sono', 'sono']),
        fb('In dispensa ___ uova per fare la torta.', ['non ci sono', 'non c\'è', 'c\'è']),
        fb('In cucina ___ pane.', ['non c\'è', 'non ci sono', 'ci sono']),
        mc('Choose the correct sentence:', 'Ci sono quattro mele.', 'C\'è quattro mele.', 'Ci è quattro mele.', 'Sono quattro mele ci.'),
        wo('In frigo non c\'è il burro', 'There is no butter in the fridge'),
      ],
      homework: 'Open your fridge and write 3 things there are and 2 things there aren\'t.',
    },
    {
      kind: 'Grammar',
      title: 'Lesson 4 · Some: del, della, dei, un po\' di',
      titleIt: 'Lezione 4 · Gli articoli partitivi',
      summary: 'Del, della, dell\', dei, degli, delle and un po\' di for amounts that aren\'t exact.',
      minutes: 12,
      sections: [
        {
          title: 'I partitivi: DI + articolo',
          table: [
            ['Genere e numero', 'di + articolo', 'Partitivo', 'Esempio'],
            ['maschile singolare (il)', 'di + il', 'DEL', 'Vorrei del pane, del latte e del riso.'],
            ['femminile singolare (la)', 'di + la', 'DELLA', 'Compro della carne, della pasta e della frutta.'],
            ['singolare + vocale (l\')', 'di + l\'', 'DELL\'', 'Prendo dell\'acqua, dell\'olio e dell\'insalata.'],
            ['maschile plurale (i / gli)', 'di + i / gli', 'DEI / DEGLI', 'Ci sono dei pomodori e degli spinaci.'],
            ['femminile plurale (le)', 'di + le', 'DELLE', 'Vorrei delle mele, delle patate e delle uova.'],
          ],
        },
        {
          title: '3 modi per chiedere una quantità non precisa',
          table: [
            ['Forma', 'Uso', 'Esempio'],
            ['UN PO\' DI', 'a small amount (a little · thodha jiha)', 'Vorrei un po\' di formaggio, un po\' di sale.'],
            ['DEL / DELLA / DELL\'', 'some (singular: weighed food, liquids)', 'Prendo del pesce, della mozzarella e dell\'acqua.'],
            ['DEI / DEGLI / DELLE', 'some (plural = alcuni / alcune)', 'Compro dei biscotti, degli yogurt e delle banane.'],
          ],
        },
        { tip: 'In a **negative** sentence don\'t use the partitive: *C\'è **del** latte? — No, non c\'è latte!*' },
      ],
      exercises: [
        fb('Al mercato compro ___ pomodori rossi.', ['dei', 'delle', 'del', 'della']),
        fb('Compro anche ___ mele dolci.', ['delle', 'dei', 'del', 'degli']),
        fb('Vorrei ___ pane fresco.', ['un po\' di', 'degli', 'delle', 'della']),
        fb('Vorrei ___ acqua naturale.', ['dell\'', 'dei', 'del', 'delle']),
        fb('In frigo c\'è ___ frutta fresca.', ['della', 'del', 'dei', 'degli']),
        fb('Ci sono ___ spinaci.', ['degli', 'delle', 'del', 'dei']),
        fb('Prendo ___ pesce.', ['del', 'della', 'delle', 'dell\'']),
        mc('— C\'è del latte? — No, …', 'non c\'è latte.', 'non c\'è del latte.', 'c\'è della latte.', 'non ci sono latte.'),
      ],
      homework: 'Write 4 things you want to buy with del / della / dei / delle / un po\' di.',
    },
    {
      kind: 'Dialogue',
      title: 'Lesson 4 · Dialogue: doing the shopping',
      titleIt: 'Lezione 4 · Dialogo: fare la spesa al negozio',
      summary: 'A full shopping conversation with c\'è, un po\' di and the partitives.',
      minutes: 10,
      sections: [
        {
          title: 'Dialogo: fare la spesa al negozio',
          situation: 'A customer at a small food shop.',
          dialogue: [
            lp('Commesso', 'Buongiorno! Dica pure, cosa desidera?', 'Good morning! What would you like?', 'Sat Sri Akal! Ki chaahida hai?'),
            lp('Cliente', 'Buongiorno! C\'è del pesce fresco? Vorrei anche un po\' di formaggio.', 'Good morning! Is there any fresh fish? I\'d also like a little cheese.', 'Sat Sri Akal! Ki taazi machhi hai? Mainu thodha panir vi chaahida hai.'),
            lp('Commesso', 'Sì, c\'è del salmone ottimo! E poi ci sono delle fragole molto dolci.', 'Yes, there\'s some excellent salmon! And there are some very sweet strawberries.', 'Haan ji, vadhia saaman machhi hai! Ate mitthiaan straabariaan vi han.'),
            lp('Cliente', 'Prendo mezzo chilo di salmone, delle fragole e un pacco di pasta. Quanto viene?', 'I\'ll take half a kilo of salmon, some strawberries and a packet of pasta. How much is it?', 'Addha kilo machhi, straabariaan ate ikk paikat paasta lainda haan. Kinne paise hoe?'),
          ],
        },
        {
          title: 'Role-play completo: fare la spesa',
          text: '1. **Controlla cosa manca:** in cucina non c\'è pane e non ci sono uova — scrivi la tua lista di 4 prodotti.\n2. **Chiedi e ordina al banco:** usa «C\'è / Ci sono…?», «un po\' di», i partitivi e chilo / etto.\n3. **Chiedi il prezzo e paga:** «Quanto viene in tutto?», leggi il totale in euro, poi cambiate ruolo!',
        },
      ],
      exercises: [
        tf('Il commesso ha del salmone.', true, 'The shop assistant has some salmon.'),
        tf('La cliente prende un chilo di salmone.', false, 'The customer takes a kilo of salmon.'),
        tf('La cliente compra anche delle fragole.', true, 'The customer also buys some strawberries.'),
        tr('Dica pure!', 'Go ahead, tell me!', 'Goodbye!', 'Pay here!', 'Wait a moment!'),
        wo('C\'è del pesce fresco', 'Is there any fresh fish'),
      ],
      homework: 'Full role-play with a partner: check what is missing, order with c\'è / partitives / quantities, ask the total and pay.',
    },

    // ═════════════════════════════════════════════════════════════════════
    // TEST CAPITOLO 4
    // ═════════════════════════════════════════════════════════════════════
    {
      kind: 'Exam',
      title: 'Chapter 4 test',
      titleIt: 'Test Capitolo 4',
      summary: 'Listening, menu and food, numbers over 100, prendere / mangiare / pagare, c\'è / ci sono and partitives. Pass mark: 60%.',
      minutes: 25,
      practice: false,
      sections: [
        {
          title: 'Test Capitolo 4',
          text: 'This test checks everything in Chapter 4 (Lessons 1–4).\n\n- **1. Listening** — orders, preferences, quantities, prices and weights\n- **2. Vocabulary** — menu, food, containers and numbers over 100\n- **3. Grammar** — prendere, mangiare, pagare, c\'è / ci sono and partitives\n- **4. Writing** — sentences about ordering and shopping\n\nYou need **60%** to pass. You can repeat the test as many times as you like.',
        },
        { tip: 'The **written part** (at the restaurant and the shopping list) and the **oral interview** (bar and restaurant, shopping and prices, c\'è / ci sono — 2 minutes) are done **with your teacher** in the live class.' },
      ],
      exercises: [
        // 1A — Ascolta e scegli
        lis('1A · Listen and choose the sentence you hear', 'Buongiorno, vorrei un caffè e un cornetto, per favore.', 'VORREI UN CAFFÈ E UN CORNETTO', 'VORREI UN TÈ E UN PANINO'),
        lis('1A · Listen and choose the sentence you hear', 'Io sono pronto: come secondo prendo il pesce con insalata.', 'COME SECONDO PRENDO IL PESCE', 'COME PRIMO PRENDO LA PASTA'),
        lis('1A · Listen and choose the sentence you hear', 'Scusi, non posso bere il cappuccino perché sono allergico al latte.', 'SONO ALLERGICO AL LATTE', 'SONO VEGETARIANO, NON MANGIO CARNE'),
        lis('1A · Listen and choose the sentence you hear', 'Al mercato compro un chilo di mele rosse.', 'COMPRO UN CHILO DI MELE', 'COMPRO UN ETTO DI FORMAGGIO'),
        lis('1A · Listen and choose the sentence you hear', 'Dobbiamo fare la spesa perché in frigo non ci sono uova.', 'IN FRIGO NON CI SONO UOVA', 'IN FRIGO C\'È DEL LATTE'),
        // 1B — Prezzi e pesi
        lis('1B · Listen and choose the price or weight', 'Un euro e quaranta.', '1,40 €', '1,14 €', '4,10 €', '14,00 €'),
        lis('1B · Listen and choose the price or weight', 'Diciotto euro e cinquanta.', '18,50 €', '80,50 €', '18,15 €', '8,50 €'),
        lis('1B · Listen and choose the price or weight', 'Centoquindici euro.', '115,00 €', '150,00 €', '105,00 €', '15,00 €'),
        lis('1B · Listen and choose the price or weight', 'Duecentocinquanta grammi.', '250 g', '205 g', '2.500 g', '150 g'),
        lis('1B · Listen and choose the price or weight', 'Cinquecento grammi.', '500 g', '50 g', '5.000 g', '105 g'),
        // 2A — Collega
        pairs('2A · Match each word with its meaning', [
          ['Antipasto', 'Starter · Shuruati khaana'],
          ['Contorno', 'Side dish · Said dish'],
          ['Latticini', 'Dairy products · Dudh de utpad'],
          ['Una bottiglia d\'acqua', 'A bottle of water · Paani di botal'],
          ['Il conto', 'The bill · Bil'],
        ]),
        // 2B — Numeri oltre 100
        mc('2B · Write the number in Italian: 100', 'cento', 'un cento', 'mille', 'centi'),
        mc('2B · Write the number in Italian: 130', 'centotrenta', 'centotredici', 'trecento', 'trentacento'),
        mc('2B · Write the number in Italian: 250', 'duecentocinquanta', 'duecinquanta', 'cinquecentodue', 'duecentoquindici'),
        mc('2B · Write the number in Italian: 500', 'cinquecento', 'cinquanta', 'cinquemila', 'cinquecenti'),
        mc('2B · Write the number in Italian: 1.000', 'mille', 'un mille', 'mila', 'diecicento'),
        // 3A — Prendere, mangiare, pagare
        fb('3A · Al ristorante noi ___ (prendere) un primo piatto.', ['prendiamo', 'prendete', 'prendono', 'prendo']),
        fb('3A · Tu ___ (mangiare) la carne o sei vegetariano?', ['mangi', 'mangii', 'mangia', 'mangiate']),
        fb('3A · Alla cassa tu ___ (pagare) con la carta o in contanti?', ['paghi', 'pagi', 'paga', 'pagate']),
        fb('3A · I clienti ___ (pagare) il conto al barista.', ['pagano', 'paghano', 'pagate', 'paga']),
        // 3B — C'è / ci sono
        fb('3B · In frigo ___ una bottiglia di latte fresco.', ['c\'è', 'ci sono', 'non ci sono', 'sono']),
        fb('3B · Sul tavolo ___ tre bicchieri e due piatti.', ['ci sono', 'c\'è', 'non c\'è', 'è']),
        fb('3B · Scusi, ___ del pane integrale oggi?', ['c\'è', 'ci sono', 'non ci sono', 'sono']),
        fb('3B · In dispensa ___ uova per fare la torta.', ['non ci sono', 'non c\'è', 'c\'è', 'ci sono']),
        // 3C — Partitivi
        fb('3C · Al mercato compro ___ pomodori rossi.', ['dei', 'delle']),
        fb('3C · … e ___ mele dolci.', ['delle', 'dei']),
        fb('3C · Vorrei ___ pane fresco.', ['un po\' di', 'degli']),
        fb('3C · … e ___ acqua naturale.', ['dell\'', 'dei']),
        fb('3C · In frigo c\'è ___ frutta fresca.', ['della', 'del']),
        fb('3C · … e ci sono ___ spinaci.', ['degli', 'delle']),
        // 4 — Produzione
        wo('Al bar la mattina prendo un cappuccino', 'At the café in the morning I have a cappuccino'),
        wo('Al supermercato compro un chilo di mele', 'At the supermarket I buy a kilo of apples'),
        wo('Nella mia cucina ci sono delle patate', 'In my kitchen there are some potatoes'),
      ],
      homework: 'Written part for your teacher: complete the 5 sentences about the bar, the restaurant, what you don\'t eat, your shopping and your kitchen. Then prepare the 2-minute oral interview.',
    },
  ],
};
