# Course content

Source for the built-in courses. The API loads the generated seed on startup and
creates a course the first time it doesn't find its slug (it never overwrites a
course that already exists, so changes made in the admin panel are kept).

```bash
cd content
npm install
npm run build
```

`npm run build`:

1. downloads the illustrations used by the course into
   `backend/SSeuropa.Api/wwwroot/content/img/` (Microsoft Fluent Emoji 3D, MIT licence),
2. generates the practice exercises (listen & pick, picture → word, translation, match pairs)
   from each lesson's words and merges them with the hand-written ones,
3. writes `backend/SSeuropa.Infrastructure/Seed/<slug>.json`,
4. renders a printable PDF study sheet per unit into `backend/SSeuropa.Api/wwwroot/content/pdf/`.

## Writing content

Courses live in `courses/<course>/`. A unit has 4 lessons — **Vocabulary → Dialogue →
Grammar → Review** — plus can-do goals and the focus of its weekly live class.
Helpers in `lib.mjs`:

| Helper | Meaning |
|---|---|
| `w(it, en, pronunciation, illustration, example, exampleEn)` | a word or phrase |
| `line(speaker, it, en)` | a dialogue line |
| `fb(sentence with ___, options)` | fill the gap — **first option is correct** |
| `wo(italian, english)` | put the words in order |
| `tf(statement, isTrue, english)` | true / false |
| `tr(italian, ...options)` / `mc(question, ...options)` | multiple choice — **first option is correct** |

Illustration names are folder names from https://github.com/microsoft/fluentui-emoji/tree/main/assets
(e.g. `'Red apple'`). The build fails if a name doesn't exist.
