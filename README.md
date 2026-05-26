# ks-kit-sdd

[![code size](https://img.shields.io/github/repo-size/k-s-studio/ks-kit-sdd?style=for-the-badge&color=%237E9CD8)](https://github.com/k-s-studio/ks-kit-sdd)
[![agent](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2Fk-s-studio%2Fks-kit-sdd%2Fmain%2Fmetrics.json&query=%24.agents&label=agents&style=for-the-badge&color=%2392B3A4&cacheSeconds=300)](https://github.com/k-s-studio/ks-kit-sdd/tree/main/.github/agents)
[![skill](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2Fk-s-studio%2Fks-kit-sdd%2Fmain%2Fmetrics.json&query=%24.skills&label=skills&style=for-the-badge&color=%23C4A46B&cacheSeconds=300)](https://github.com/k-s-studio/ks-kit-sdd/tree/main/.github/skills)
[![prompt](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2Fk-s-studio%2Fks-kit-sdd%2Fmain%2Fmetrics.json&query=%24.prompts&label=prompts&style=for-the-badge&color=%23A97C73&cacheSeconds=300)](https://github.com/k-s-studio/ks-kit-sdd/tree/main/.github/prompts)
[![latest](https://img.shields.io/github/last-commit/k-s-studio/ks-kit-sdd/main?style=for-the-badge&label=latest&color=%236F8FAF)](https://github.com/k-s-studio/ks-kit-sdd/commits/main)

## Description

Work with GitHub Copilot (currently) and OpenSpec. Everything I need in feature-branch workflow.

## Quick Start

Clone the repo:

```bash
git clone https://github.com/k-s-studio/ks-kit-sdd.git
```

Add `ks-kit-sdd` into your project workspace as an extra folder, so the repo-local agents, skills, prompts, and notes are available alongside your working project.

If you use a multi-root VS Code workspace, open your main project first and then add this repository as another workspace folder.

## Problem-shooting
Make sure there are configured in vs code settings (ctrl+,):
```
Chat: Agent Files Locations
Chat: Agent Skills Locations
Chat: Instructions Files Locations
Chat: Use Agent Skills
```