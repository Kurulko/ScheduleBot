# ScheduleBot!

## Telegram bot for university subject schedule

To use this bot, you should have the schedule's token<br/>
About the token:<br/>
1) Firsty you need to create the file in google sheets<br/>
2) Your token:<br/>
![token](https://user-images.githubusercontent.com/95112563/225616052-ff120be0-b1c7-49cf-bb7d-6bc19cbb17b7.jpg)<br/><br/>
3) There are two sheets: 'Lessons' and 'Event'<br/>
4) Sheet 'Lessons':<br/>
  a) Column 'Time':<br/>
  ![sheet_for_lessons](https://user-images.githubusercontent.com/95112563/225613736-ebf3f694-3fb5-406e-b90d-2e3d83542594.jpg)<br/><br/>
  b) Column 'Lectors':<br/>
  The colum can be empty<br/>
  ![teachers](https://user-images.githubusercontent.com/95112563/225613883-015bf15c-f0fd-4148-a2bc-251bba92baa9.png)<br/><br/>
  c) Column 'Links':<br/>
    The link for lessons or empty<br/>
  d) Column 'Subjects':<br/>
    Just the subject's name<br/>
  e) Columns 'Numberator', 'Denominator', 'Always':<br/>
      If this subject is always, all these three colums must have value 'true'<br/>
    If this subject is only numberator, only this colum must have value 'true', others 'false'<br/>
    If this subject is only denominator, only this colum must have value 'true', others 'false'<br/>
  f) Column 'Weekday':<br/>
    The day of week<br/>
5) Sheet 'Events':<br/>
  ![events](https://user-images.githubusercontent.com/95112563/225615246-07c14a86-0a4b-4d07-9aba-f2859c18aeb4.png)<br/><br/>
  
  a) Column 'Dealine':<br/>
    The day when it will be<br/>
  b) Column 'Lector', 'Subject':<br/>
    The same like for sheet 'Lessons'<br/>
  c) Column 'Type':<br/>
    The type of event<br/>
  d) Column 'Description':<br/>
    Just description, not necessary<br/>
