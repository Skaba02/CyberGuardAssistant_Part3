# CyberGuard Assistant — Part 3 / POE

## How to Run

1. Make sure **MySQL** is installed and running on your computer
2. Open `CyberGuardAssistant.csproj` in **Visual Studio 2022**
3. Press **Ctrl + Shift + B** to build (this downloads the MySQL package automatically)
4. Copy `VoiceGreeting.wav` into the project folder
5. Press **F5** to run

> The database and table are created automatically on first run.
> Default MySQL credentials used: username `root`, password `root`.
> If yours are different, update the connection string in `Data/DatabaseService.cs`.

---

## What Was Added in Part 3 (on top of Parts 1 and 2)

| Task | Feature | How to use |
|---|---|---|
| Task 1 | Task Assistant + MySQL | `add task - enable 2FA` |
| Task 1 | Set reminders | Reply with `remind me in 3 days` |
| Task 1 | View tasks | `show tasks` |
| Task 1 | Complete a task | `complete task 1` |
| Task 1 | Delete a task | `delete task 2` |
| Task 2 | Cybersecurity Quiz | `quiz` or `test me` |
| Task 3 | NLP — varied phrasing | `Can you remind me to update my password?` |
| Task 4 | Activity Log | `show activity log` |

---

## File Structure

```
CyberGuardAssistant/
├── Program.cs                      ← Entry point
├── MainForm.cs                     ← GUI event handling
├── MainForm.Designer.cs            ← GUI layout
├── Models/
│   ├── UserProfile.cs              ← User memory (Part 1+2)
│   ├── SecurityTopic.cs            ← Topic model (Part 1)
│   ├── CyberTask.cs                ← Task model (Part 3 Task 1)
│   ├── QuizQuestion.cs             ← Quiz model (Part 3 Task 2)
│   └── ActivityEntry.cs            ← Log entry model (Part 3 Task 4)
├── Data/
│   └── DatabaseService.cs          ← MySQL database access (Part 3 Task 1)
├── Services/
│   ├── ResponseEngine.cs           ← Main chatbot brain (all parts)
│   ├── TaskService.cs              ← Task assistant logic (Part 3 Task 1)
│   ├── QuizService.cs              ← Quiz game logic (Part 3 Task 2)
│   ├── NLPService.cs               ← NLP keyword detection (Part 3 Task 3)
│   └── ActivityLogService.cs       ← Activity logging (Part 3 Task 4)
└── VoiceGreeting.wav               ← Voice greeting audio
```

---

## Example Conversations

```
You:  add task - review my privacy settings
Bot:  Task added: 'Review my privacy settings'... Would you like a reminder?
You:  remind me in 3 days
Bot:  Got it! Reminder set for [date].

You:  quiz
Bot:  Question 1 of 10 — What should you do if you receive an email asking for your password?

You:  C
Bot:  ✅ Correct! Reporting phishing emails helps protect others.

You:  show activity log
Bot:  Here is a summary of recent actions:
      1. [14:02] Task added: Review my privacy settings
      2. [14:03] Quiz started
      3. [14:04] Quiz answer submitted
```

## Author
Created for CyberGuard Assistant — Part 3/POE — 2026
