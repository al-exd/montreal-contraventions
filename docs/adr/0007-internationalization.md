# 0007. Internationalization: Angular translations and resx

Status: Accepted · Date: 2026-10-14

## Context and problem statement

The dataset and likely audience are French-speaking; the documentation and code are English. The UI should be usable in both.

## Decision outcome

Documentation, code and API in English; domain terms follow the [ubiquitous language](../02-ubiquitous-language.md). The Angular UI uses translation files for English and French with a language switch; the .NET side uses `.resx` resources for emails and validation messages, selected by the request or subscriber locale. Delivered as a late, optional increment (17); screens are written with translation keys from the start to avoid rework.

## Consequences

- Good: bilingual reach, clean separation of text from code.
- Bad: every user-facing string needs a key and two translations.
