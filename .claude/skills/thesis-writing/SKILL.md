---
name: thesis-writing
description: Uzbek BMI (Bitiruv Malakaviy Ishi) writing style — sentence structure, paragraph patterns, anti-plagiarism techniques, and formatting rules. Open before writing any Uzbek thesis chapter.
---

# BMI Writing Style — Alfraganus Universiteti Standard

Same formatting requirements as TDIU. Based on a validated BMI sample ("EduWay.Platform") that passed all checks.

---

## 1. Formatting Requirements

| Parameter | Value |
|---|---|
| Font | Times New Roman 14pt |
| Line spacing | 1.5 |
| Left margin | 3 cm |
| Right margin | 1.5 cm |
| Top/bottom margins | 2 cm |
| Paragraph indent | 1.25 cm |
| Total pages | 55–70 pages |
| Figures | 15–20 |
| References | 14–20 sources |

### Numbering system
- Chapters: **I BOB**, **II BOB** (Roman numerals, uppercase)
- Sections: **1.1.**, **1.2.**, **2.1.**
- Figures: **1.1.1-rasm.**, **2.1.3-rasm.** (chapter.section.order — ends with period)
- Tables: **2.1-jadval.** (ends with period)

---

## 2. Writing Style Rules

### 2.1 First person ("Men") — REQUIRED in practical chapters

```
✅ "Men bu tizimni Blazor Server texnologiyasi asosida ishlab chiqdim..."
✅ "Men PostgreSQL ma'lumotlar bazasini tanladim, chunki..."
✅ "Men quyidagi algoritmni loyihalashtirdim..."
❌ "Tizim ishlab chiqildi" (passive — wrong for BMI)
❌ "Biz tanladik" (plural — thesis is individual work)
```

### 2.2 Long, complex sentences (40–80 words per sentence)

Structure: `[Context/reason] + [main idea] + [result/addition]`

```
✅ "Zamonaviy farmatsevtika sohasida axborot texnologiyalarining keng qo'llanilishi 
    dorixonalar boshqaruvini tubdan o'zgartirib, dori vositalarini qidirish va 
    mavjudligini tekshirish jarayonlarini avtomatlashtirishga imkon berayotgan bo'lsa-da, 
    O'zbekistonda bu sohadagi raqamli yechimlar hali etarli darajada rivojlanmagan."
```

### 2.3 Transition phrases (use frequently)

- "Shu bilan birga, ..."
- "Bundan tashqari, ..."
- "Biroq, ..."
- "Shuningdek, ..."
- "Natijada, ..."
- "Yuqorida keltirilganlardan kelib chiqib, ..."
- "Ushbu jihatdan, ..."
- "Xususan, ..."

### 2.4 "Universal connectors" (anti-plagiarism safe phrases)

Use these frequently — they are organic to Uzbek academic writing:
- "muhim ahamiyatga ega"
- "sezilarli darajada"
- "samarali tashkil etish"
- "alohida e'tibor qaratildi"
- "muhim rol o'ynaydi"
- "optimallashtirish imkonini beradi"
- "jarayonni tezlashtiradi"
- "yondashuvning asosiy afzalliklari"
- "imkoniyat yaratadi"

### 2.5 Two-sided analysis pattern

Every technology or approach: present **advantages + problems**. This demonstrates professional understanding to the committee.

```
"[Texnologiya] bir qator afzalliklarga ega: [список]. Biroq, [muammo] ham mavjud 
bo'lib, bu [yechim] orqali bartaraf etiladi."
```

---

## 3. Paragraph Structure (3-part pattern)

1. **Topic sentence** — main idea (1 sentence)
2. **Details** — examples, definitions, numbers, technology names (3–5 sentences)
3. **Conclusion** — "Natijada", "Bu esa", "Shunday qilib" (1 sentence)

Average paragraph: **150–250 words**. Theoretical chapters allow 500–700 word paragraphs.

---

## 4. Mandatory O'zbekiston Context (every section)

**Every** section must have at least one paragraph referencing Uzbekistan. Use:
- "Raqamli O'zbekiston – 2030" strategiyasi
- Prezident Shavkat Mirziyoyevning farmoni
- O'zbekiston Respublikasining tegishli qonuni
- Local examples: apteka.uz, local pharmacy chains
- O'zbekistonda farmatsevtika bozori holati

This is a **non-negotiable** requirement from the committee.

---

## 5. Section Opening Phrases

```
"Ushbu bo'limda ... keng yoritiladi."
"Mazkur bob ... ga bag'ishlangan."
"Quyida ... batafsil ko'rib chiqiladi."
```

---

## 6. Anti-Plagiarism Techniques

| Technique | Why it works |
|---|---|
| O'zbekiston qonunlari (direct quotes) | Not counted as plagiarism |
| Prezident farmonlari | Not counted as plagiarism |
| Long personal sentences with "men" | Unique, AI-safe |
| Technology brand names (PostgreSQL, etc.) | Technical terms excluded from check |
| Uzbek-specific market context | Not in international databases |

---

## 7. Chapter Conclusion Pattern

End every chapter section with:
```
"Shunday qilib, [main conclusion]. Yuqoridagi tahlillar shuni ko'rsatadiki, [insight]. 
Keyingi bo'limda [what follows] ko'rib chiqiladi."
```

---

## 8. Things to NEVER do

```
❌ Short AI-style sentences: "Blazor yaxshi. U tez ishlaydi."
❌ Bullet lists in theoretical sections (use prose)
❌ "Keyinchalik qo'shiladi" — thesis describes FINISHED work
❌ English words without Uzbek equivalent or explanation
❌ "biz" (plural) — use "men" throughout
```

---

## 9. Working Process (Claude)

1. Get chapter outline from `thesis-structure` skill
2. Write one section at a time — user reads and approves
3. Save to `docs/thesis/XX-name.md`
4. Show `[RASM: X.X.X-rasm.png — caption]` where screenshots should go
5. Never claim a section is complete until user confirms
6. At the end: `pandoc docs/thesis/*.md -o DrugstoreSystem-BMI.docx --reference-doc=template.docx`
