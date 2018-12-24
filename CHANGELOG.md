## 2.4.3 (24-12-2018)
  * Allow longer console input than the default 254 character limitation
## 2.4.2 (08-11-2017)
  * Allow TimeSpan arguments
## 2.4.1 (26-10-2017)
  * Navigatable commands in command line mode now get executed
## 2.4.0 (06-09-2017)
  * Update to netstandard20
## 2.3.2 (15-08-2017)
  * Add inner exception stack trace when executing command
## 2.3.1 (15-08-2017)
  * Ignore get/set methods on public type scanning
## 2.3.0 (15-08-2017)
  * Methods with similar call structure are now fixed (#2)
  * Auto up after commands is now available (#3)
  * Async void commands will show an error message and won't execute
  * Ability to scan public types, allowing modes where dependant libraries won't need a reference to commanr runner to still offer commands (#7)