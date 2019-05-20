For the git version the ftp login file has been left out, since it contains sensitive information. For proper running of the program add a file called: 'ftplogin.json' to the RftpUI project with the following inside:

{
  "host" :  "<hostname>",
  "username": "<username>",
  "password": "<password>"
} 

with the text between <> (also replace <>) changed for the credentials for an ftp connection where <hostname> is the server address, for example: rafaeltab.tk